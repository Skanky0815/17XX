using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PathManager PathManager;

    private KnotCollection.Knot _currentKnot;
    private KnotCollection _currentKnotCollection;

    private KnotCollection.Knot _targetKnot;
    private KnotCollection _targetKnotCollection;

    private Queue<KnotCollection.Knot> _pathQueue;

    public float MoveSpeed = 1f;
    public float ArrivalThreshold = 0.05f;

    private float _t = 0f;
    private float _targetKnotT = 0f;
    private MoveDirection _moveDirection;
    private Animator _animator;

    // Debug / stuck detection
    private float _debugInterval = 0.5f;
    private float _debugTimer = 0f;
    private Vector3 _lastPosition;
    private float _stuckTimer = 0f;
    private float _stuckThreshold = 1.0f;
    private int _stuckRecoverAttempts = 0;

    private IEnumerable _cachedNodeIds = null;
    private bool _nodeIdsCacheAttempted = false;

    private void Awake()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();
        _lastPosition = transform.position;
    }

    private void Start()
    {
        _currentKnot = new KnotCollection.Knot(0, 0);
        _currentKnotCollection = new KnotCollection { _currentKnot };

        if (PathManager != null)
        {
            _t = GetClosestTOnSpline(_currentKnot.SplineIndex, transform.position);
        }
#if UNITY_EDITOR
        Debug.Log($"PlayerMovement.Start current={_currentKnot?.Id ?? "null"} t={_t:F2}");
#endif
    }

    public void RequestPath(KnotCollection targetKnots)
    {
        if (targetKnots == null)
        {
            Debug.LogWarning("RequestPath: targetKnots ist null.");
            return;
        }

        if (PathManager == null || PathManager.KnotGrapth == null)
        {
            Debug.LogWarning("RequestPath: PathManager oder KnotGrapth ist null.");
            return;
        }

        EnsureCurrentKnotInitialized();

        KnotCollection.Knot goalKnot = null;

        try
        {
            goalKnot = targetKnots.GetMatchingWithSplineIndex(_currentKnotCollection);
        }
        catch (Exception)
        {
        }

        if (goalKnot == null)
        {
            float bestDist = float.MaxValue;
            foreach (var k in targetKnots)
            {
                Vector3 knotWorld;
                try
                {
                    knotWorld = PathManager.GetKnotWoldPosition(k.SplineIndex, k.KnotIndex);
                }
                catch
                {
                    continue;
                }

                float d = (knotWorld - transform.position).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    goalKnot = k;
                }
            }
        }

        if (goalKnot == null)
        {
            Debug.LogWarning("RequestPath: kein gültiger Zielknoten gefunden.");
            return;
        }

#if UNITY_EDITOR
        Debug.Log($"RequestPath: selected goal {goalKnot.Id} from current {_currentKnot?.Id ?? "null"}");
#endif

        var pathIds = SplinePathfinder.FindPath(_currentKnot.Id, goalKnot.Id, PathManager.KnotGrapth);
        if (pathIds == null || pathIds.Count == 0)
        {
            Debug.LogWarning($"RequestPath: kein Pfad gefunden von {_currentKnot.Id} nach {goalKnot.Id}");
            return;
        }

        var path = pathIds.Select(ParseIdToKnot).ToList();
        StartFollowingPath(path);
    }

    private void StartFollowingPath(List<KnotCollection.Knot> path)
    {
        if (path == null || path.Count == 0) return;

        _pathQueue = new Queue<KnotCollection.Knot>(path);
#if UNITY_EDITOR
        Debug.Log($"StartFollowingPath: enqueued {_pathQueue.Count} knots. first={_pathQueue.Peek().Id}");
#endif
        if (_currentKnot == null)
        {
            EnsureCurrentKnotInitialized();
            if (_currentKnot == null)
            {
                _currentKnot = path.First();
                try { _currentKnotCollection = PathManager.GetCollectionBySplineIndex(_currentKnot.SplineIndex); } catch { _currentKnotCollection = new KnotCollection { _currentKnot }; }
                _t = GetClosestTOnSpline(_currentKnot.SplineIndex, transform.position);
            }
        }

        while (_pathQueue != null && _pathQueue.Count > 0 && _currentKnot != null && _pathQueue.Peek().Id == _currentKnot.Id)
        {
            _pathQueue.Dequeue();
        }

        MoveToNextInPath();
    }

    private void MoveToNextInPath()
    {
        if (_pathQueue == null || _pathQueue.Count == 0)
        {
            _targetKnot = null;
            _targetKnotCollection = null;
            _targetKnotT = 0f;
#if UNITY_EDITOR
            Debug.Log("MoveToNextInPath: queue empty");
#endif
            return;
        }

        KnotCollection.Knot next = null;
        while (_pathQueue.Count > 0)
        {
            next = _pathQueue.Dequeue();
            if (_currentKnot == null || next.Id != _currentKnot.Id)
                break;
            next = null;
        }

        if (next == null)
        {
            _targetKnot = null;
            _targetKnotCollection = null;
            _targetKnotT = 0f;
#if UNITY_EDITOR
            Debug.Log("MoveToNextInPath: no non-duplicate knot found");
#endif
            return;
        }

        _targetKnot = next;
        try
        {
            _targetKnotCollection = PathManager.GetCollectionBySplineIndex(next.SplineIndex);
        }
        catch
        {
            _targetKnotCollection = null;
        }

        _t = GetClosestTOnSpline(_targetKnot.SplineIndex, transform.position);

        try
        {
            var knotWorld = PathManager.GetKnotWoldPosition(_targetKnot.SplineIndex, _targetKnot.KnotIndex);
            _targetKnotT = GetClosestTOnSpline(_targetKnot.SplineIndex, knotWorld);
        }
        catch
        {
            _targetKnotT = (_targetKnot.KnotIndex >= 0.5f ? 1f : 0f);
        }

        _moveDirection = (_t <= _targetKnotT) ? MoveDirection.FORWARD : MoveDirection.BACKWARD;
#if UNITY_EDITOR
        Debug.Log($"MoveToNextInPath: next={_targetKnot.Id} t={_t:F2} knotT={_targetKnotT:F2} dir={_moveDirection} remainingQueue={_pathQueue?.Count ?? 0}");
#endif
        _stuckTimer = 0f;
        _stuckRecoverAttempts = 0;
    }

    public void CancelPath()
    {
        _pathQueue = null;
        _targetKnot = null;
        _targetKnotCollection = null;
#if UNITY_EDITOR
        Debug.Log("CancelPath called");
#endif
    }

    private MoveDirection DetermineMoveDirection(KnotCollection.Knot from, KnotCollection.Knot to, KnotCollection fromCollection)
    {
        if (from == null || fromCollection == null || to == null) return MoveDirection.FORWARD;

        if (to.IsSameSpline(from))
        {
            return from.KnotIndex <= to.KnotIndex ? MoveDirection.FORWARD : MoveDirection.BACKWARD;
        }

        try
        {
            var entry = fromCollection.GetBySplineIndex(to.SplineIndex);
            return entry.KnotIndex <= to.KnotIndex ? MoveDirection.FORWARD : MoveDirection.BACKWARD;
        }
        catch
        {
            var tClosest = GetClosestTOnSpline(to.SplineIndex, transform.position);
            return tClosest <= 0.5f ? MoveDirection.FORWARD : MoveDirection.BACKWARD;
        }
    }

    private void Update()
    {
        if (_targetKnot == null && _pathQueue != null && _pathQueue.Count > 0)
        {
            MoveToNextInPath();
        }

        if (_targetKnot == null || PathManager == null) return;

        float splineLength = Mathf.Max(0.0001f, PathManager.GetSpineLenght(_targetKnot.SplineIndex));
        var step = Time.deltaTime * MoveSpeed / splineLength;

        var tangent = PathManager.EvaluateSplineTangent(_targetKnot.SplineIndex, _t);
        if (_moveDirection == MoveDirection.FORWARD)
        {
            _t += step;
        }
        else
        {
            _t -= step;
            tangent = -tangent;
        }

        _t = Mathf.Clamp01(_t);

        var pos = PathManager.EvaluateSplinePosition(_targetKnot.SplineIndex, _t);
        transform.position = pos;

        if (tangent.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(tangent.normalized);

        if (_animator != null) _animator.SetBool("isMoving", true);

        var targetPos = PathManager.GetKnotWoldPosition(_targetKnot.SplineIndex, _targetKnot.KnotIndex);
        float distToTarget = Vector3.Distance(transform.position, targetPos);

        _debugTimer += Time.deltaTime;
        if (_debugTimer >= _debugInterval)
        {
            _debugTimer = 0f;
#if UNITY_EDITOR
            Debug.Log($"Player.Update curr={_currentKnot?.Id ?? "null"} target={_targetKnot?.Id ?? "null"} t={_t:F2} dir={_moveDirection} distToTarget={distToTarget:F3} queue={_pathQueue?.Count ?? 0}");
#endif
        }

        float moved = (transform.position - _lastPosition).sqrMagnitude;
        if (moved < 0.000001f)
        {
            _stuckTimer += Time.deltaTime;
        }
        else
        {
            _stuckTimer = 0f;
        }
        _lastPosition = transform.position;

        if (_stuckTimer > _stuckThreshold && _targetKnot != null)
        {
            _stuckRecoverAttempts++;
            Debug.LogWarning($"Player seems stuck for {_stuckTimer:F2}s on target {_targetKnot.Id}. Attempting recovery #{_stuckRecoverAttempts}");
            _moveDirection = (_t <= _targetKnotT) ? MoveDirection.FORWARD : MoveDirection.BACKWARD;

            if (_stuckRecoverAttempts >= 3)
            {
                Debug.LogWarning("Recovery attempts exhausted - skipping to next knot.");
                MoveToNextInPath();
            }
            _stuckTimer = 0f;
        }

        if (distToTarget < ArrivalThreshold)
        {
            transform.position = targetPos;
#if UNITY_EDITOR
            Debug.Log($"Arrived at {_targetKnot.Id}");
#endif
            _currentKnot = _targetKnot;

            if (_targetKnotCollection != null)
                _currentKnotCollection = _targetKnotCollection;
            else
            {
                try { _currentKnotCollection = PathManager.GetCollectionBySplineIndex(_currentKnot.SplineIndex); } catch { /* leave as is */ }
            }

            _targetKnot = null;
            _targetKnotCollection = null;
            _targetKnotT = 0f;

            if (_animator != null) _animator.SetBool("isMoving", false);

            MoveToNextInPath();
        }
    }

    #region Hilfsfunktionen

    private static KnotCollection.Knot ParseIdToKnot(string id)
    {
        var parts = id.Split(':');
        return new KnotCollection.Knot(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private float GetClosestTOnSpline(int splineIndex, Vector3 worldPos)
    {
        if (PathManager == null) return 0f;

        try
        {
            var method = PathManager.GetType().GetMethod("GetClosestTOnSpline");
            if (method != null)
            {
                var res = method.Invoke(PathManager, new object[] { splineIndex, worldPos });
                if (res is float f) return Mathf.Clamp01(f);
            }
        }
        catch { }

        int coarse = 24;
        float bestT = 0f;
        float bestDist = float.MaxValue;
        for (int i = 0; i <= coarse; i++)
        {
            float t = i / (float)coarse;
            var p = PathManager.EvaluateSplinePosition(splineIndex, t);
            var d = (p - worldPos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; bestT = t; }
        }

        int refineSteps = 8;
        float span = 1f / coarse;
        float start = Mathf.Max(0f, bestT - span);
        float end = Mathf.Min(1f, bestT + span);
        for (int i = 0; i <= refineSteps; i++)
        {
            float t = Mathf.Lerp(start, end, i / (float)refineSteps);
            var p = PathManager.EvaluateSplinePosition(splineIndex, t);
            var d = (p - worldPos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; bestT = t; }
        }

        return Mathf.Clamp01(bestT);
    }

    private void EnsureCurrentKnotInitialized()
    {
        if (PathManager == null || PathManager.KnotGrapth == null) return;
        if (_currentKnot != null && _currentKnotCollection != null) return;

        if (!_nodeIdsCacheAttempted)
        {
            _nodeIdsCacheAttempted = true;
            var kgType = PathManager.KnotGrapth.GetType();
            var nodesProp = kgType.GetProperty("Nodes") ?? kgType.GetProperty("AllNodes") ?? kgType.GetProperty("NodeIds");
            if (nodesProp != null)
            {
                _cachedNodeIds = nodesProp.GetValue(PathManager.KnotGrapth) as IEnumerable;
            }
            else
            {
                var getNodesMethod = kgType.GetMethod("GetNodes") ?? kgType.GetMethod("GetAllNodes");
                if (getNodesMethod != null)
                {
                    _cachedNodeIds = getNodesMethod.Invoke(PathManager.KnotGrapth, null) as IEnumerable;
                }
            }
        }

        if (_cachedNodeIds != null)
        {
            float bestDist = float.MaxValue;
            string bestId = null;
            foreach (var obj in _cachedNodeIds)
            {
                if (obj == null) continue;
                string id = obj.ToString();
                try
                {
                    var parts = id.Split(':');
                    int si = int.Parse(parts[0]);
                    int ki = int.Parse(parts[1]);
                    var pos = PathManager.GetKnotWoldPosition(si, ki);
                    var d = (pos - transform.position).sqrMagnitude;
                    if (d < bestDist) { bestDist = d; bestId = id; }
                }
                catch { continue; }
            }
            if (bestId != null)
            {
                var knot = ParseIdToKnot(bestId);
                _currentKnot = knot;
                try { _currentKnotCollection = PathManager.GetCollectionBySplineIndex(knot.SplineIndex); } catch { _currentKnotCollection = new KnotCollection { knot }; }
                _t = GetClosestTOnSpline(knot.SplineIndex, transform.position);
#if UNITY_EDITOR
                Debug.Log($"EnsureCurrentKnotInitialized: chose {knot.Id} t={_t:F2}");
#endif
                return;
            }
        }

        float bestDist2 = float.MaxValue;
        KnotCollection.Knot bestK = null;
        for (int si = 0; si < 32; si++)
        {
            for (int ki = 0; ki < 32; ki++)
            {
                try
                {
                    var p = PathManager.GetKnotWoldPosition(si, ki);
                    var d = (p - transform.position).sqrMagnitude;
                    if (d < bestDist2) { bestDist2 = d; bestK = new KnotCollection.Knot(si, ki); }
                }
                catch { break; }
            }
        }

        if (bestK != null)
        {
            _currentKnot = bestK;
            try { _currentKnotCollection = PathManager.GetCollectionBySplineIndex(bestK.SplineIndex); } catch { _currentKnotCollection = new KnotCollection { bestK }; }
            _t = GetClosestTOnSpline(bestK.SplineIndex, transform.position);
#if UNITY_EDITOR
            Debug.Log($"EnsureCurrentKnotInitialized fallback: chose {bestK.Id} t={_t:F2}");
#endif
        }
    }

    #endregion

    private enum MoveDirection
    {
        FORWARD,
        BACKWARD
    }
}
