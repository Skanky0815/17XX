using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private string _currentKnotId;

    private string _targetKnotId;

    private Queue<string> _pathQueue;

    public float MoveSpeed = 1f;
    public float ArrivalThreshold = 0.05f;
    private float _arrivalThresholdSq;

    private float _t = 0f;
    private float _targetKnotT = 0f;
    private MoveDirection _moveDirection;
    private Animator _animator;

    private float _debugInterval = 0.5f;
    private float _debugTimer = 0f;
    private Vector3 _lastPosition;
    private float _stuckTimer = 0f;
    private const float StuckThreshold = 1.0f;
    private int _stuckRecoverAttempts = 0;
    private const float MovedThresholdSq = 0.000001f;

    private IEnumerable _cachedNodeIds = null;
    private bool _nodeIdsCacheAttempted = false;

    private void Awake()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();
        _lastPosition = transform.position;
        _arrivalThresholdSq = ArrivalThreshold * ArrivalThreshold;
    }

    private void Start()
    {
        _currentKnotId = "0:0";
        _t = GetClosestTOnSpline(_currentKnotId, transform.position);

#if UNITY_EDITOR
        Debug.Log($"PlayerMovement.Start current={_currentKnotId ?? "null"} t={_t:F2}");
#endif
    }

    public void RequestPath(string targetKnotId)
    {
        if (targetKnotId == null)
        {
            Debug.LogWarning("RequestPath: targetKnots ist null.");
            return;
        }

        EnsureCurrentKnotInitialized();

#if UNITY_EDITOR
        Debug.Log($"RequestPath: selected goal {targetKnotId} from current {_currentKnotId ?? "null"}");
#endif

        var pathIds = SplinePathfinder.FindPath(_currentKnotId, targetKnotId, PathManager.KnotGrapth);
        if (pathIds == null || pathIds.Count == 0)
        {
            Debug.LogWarning($"RequestPath: kein Pfad gefunden von {_currentKnotId} nach {targetKnotId}");
            return;
        }

        StartFollowingPath(pathIds);
    }

    private void StartFollowingPath(List<string> path)
    {
        if (path == null || path.Count == 0) return;

        _pathQueue = new Queue<string>(path);
#if UNITY_EDITOR
        Debug.Log($"StartFollowingPath: enqueued {_pathQueue.Count} knots. first={_pathQueue.Peek()}");
#endif
        if (_currentKnotId == null)
        {
            EnsureCurrentKnotInitialized();
            if (_currentKnotId == null)
            {
                _currentKnotId = path.First();
                _t = GetClosestTOnSpline(_currentKnotId, transform.position);
            }
        }

        while (_pathQueue.Count > 0 && _currentKnotId != null && _pathQueue.Peek() == _currentKnotId)
        {
            _pathQueue.Dequeue();
        }

        MoveToNextInPath();
    }

    private void MoveToNextInPath()
    {
        if (_pathQueue == null || _pathQueue.Count == 0)
        {
            _targetKnotId = null;
            _targetKnotT = 0f;
#if UNITY_EDITOR
            Debug.Log("MoveToNextInPath: queue empty");
#endif
            return;
        }

        string nextId = null;
        while (_pathQueue.Count > 0)
        {
            nextId = _pathQueue.Dequeue();
            if (_currentKnotId == null || nextId != _currentKnotId)
                break;
            nextId = null;
        }

        if (nextId == null)
        {
            _targetKnotId = null;
            _targetKnotT = 0f;
#if UNITY_EDITOR
            Debug.Log("MoveToNextInPath: no non-duplicate knot found");
#endif
            return;
        }

        _targetKnotId = nextId;

        _t = GetClosestTOnSpline(_targetKnotId, transform.position);


        var knotWorld = PathManager.GetKnotWoldPosition(_targetKnotId);
        _targetKnotT = GetClosestTOnSpline(_targetKnotId, knotWorld);

        _moveDirection = (_t <= _targetKnotT) ? MoveDirection.FORWARD : MoveDirection.BACKWARD;
#if UNITY_EDITOR
        Debug.Log($"MoveToNextInPath: next={_targetKnotId} t={_t:F2} knotT={_targetKnotT:F2} dir={_moveDirection} remainingQueue={_pathQueue?.Count ?? 0}");
#endif
        _stuckTimer = 0f;
        _stuckRecoverAttempts = 0;
    }

    public void CancelPath()
    {
        _pathQueue = null;
        _targetKnotId = null;
#if UNITY_EDITOR
        Debug.Log("CancelPath called");
#endif
    }

    private void Update()
    {
        if (_targetKnotId == null)
        {
            if (_pathQueue != null && _pathQueue.Count > 0)
                MoveToNextInPath();
            else
                return;
        }

        float splineLength = Mathf.Max(0.0001f, PathManager.GetSpineLenght(_targetKnotId));
        var step = Time.deltaTime * MoveSpeed / splineLength;

        var tangent = PathManager.EvaluateSplineTangent(_targetKnotId, _t);

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

        var pos = PathManager.EvaluateSplinePosition(_targetKnotId, _t);
        transform.position = pos;

        float tangentMagnitudeSq = tangent.sqrMagnitude;
        if (tangentMagnitudeSq > 0.0001f)
            transform.rotation = Quaternion.LookRotation(tangent * (1f / Mathf.Sqrt(tangentMagnitudeSq)));

        if (_animator != null)
            _animator.SetBool("isMoving", true);

        var targetPos = PathManager.GetKnotWoldPosition(_targetKnotId);
        float distToTargetSq = (transform.position - targetPos).sqrMagnitude;

        _debugTimer += Time.deltaTime;
        if (_debugTimer >= _debugInterval)
        {
            _debugTimer = 0f;
#if UNITY_EDITOR
            Debug.Log($"Player.Update curr={_currentKnotId ?? "null"} target={_targetKnotId ?? "null"} t={_t:F2} dir={_moveDirection} distToTarget={Mathf.Sqrt(distToTargetSq):F3} queue={_pathQueue?.Count ?? 0}");
#endif
        }

        float movedSq = (transform.position - _lastPosition).sqrMagnitude;
        if (movedSq < MovedThresholdSq)
        {
            _stuckTimer += Time.deltaTime;
        }
        else
        {
            _stuckTimer = 0f;
        }
        _lastPosition = transform.position;

        if (_stuckTimer > StuckThreshold && _targetKnotId != null)
        {
            _stuckRecoverAttempts++;
            Debug.LogWarning($"Player seems stuck for {_stuckTimer:F2}s on target {_targetKnotId}. Attempting recovery #{_stuckRecoverAttempts}");
            _moveDirection = (_t <= _targetKnotT) ? MoveDirection.FORWARD : MoveDirection.BACKWARD;

            if (_stuckRecoverAttempts >= 3)
            {
                Debug.LogWarning("Recovery attempts exhausted - skipping to next knot.");
                MoveToNextInPath();
            }
            _stuckTimer = 0f;
        }

        if (distToTargetSq < _arrivalThresholdSq)
        {
            transform.position = targetPos;
#if UNITY_EDITOR
            Debug.Log($"Arrived at {_targetKnotId}");
#endif
            _currentKnotId = _targetKnotId;

            _targetKnotId = null;
            _targetKnotT = 0f;

            if (_animator != null)
                _animator.SetBool("isMoving", false);

            MoveToNextInPath();
        }
    }

    private float GetClosestTOnSpline(string knotId, Vector3 worldPos)
    {
        const int Coarse = 24;
        float bestT = 0f;
        float bestDist = float.MaxValue;

        for (int i = 0; i <= Coarse; i++)
        {
            float t = i / (float)Coarse;
            var p = PathManager.EvaluateSplinePosition(knotId, t);
            float d = (p - worldPos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; bestT = t; }
        }

        const int RefineSteps = 8;
        float span = 1f / Coarse;
        float start = Mathf.Max(0f, bestT - span);
        float end = Mathf.Min(1f, bestT + span);

        for (int i = 0; i <= RefineSteps; i++)
        {
            float t = Mathf.Lerp(start, end, i / (float)RefineSteps);
            var p = PathManager.EvaluateSplinePosition(knotId, t);
            float d = (p - worldPos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; bestT = t; }
        }

        return Mathf.Clamp01(bestT);
    }

    private void EnsureCurrentKnotInitialized()
    {
        if (_currentKnotId != null) return;

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

        if (_cachedNodeIds != null && TryFindClosestFromCache(out var knotId))
        {
            _currentKnotId = knotId;
            _t = GetClosestTOnSpline(knotId, transform.position);
#if UNITY_EDITOR
            Debug.Log($"EnsureCurrentKnotInitialized: chose {knotId} t={_t:F2}");
#endif
            return;
        }

        TryBruteForceFindClosest(out var bestKnotId);
        if (bestKnotId != null)
        {
            _currentKnotId = bestKnotId;

            _t = GetClosestTOnSpline(bestKnotId, transform.position);
#if UNITY_EDITOR
            Debug.Log($"EnsureCurrentKnotInitialized fallback: chose {bestKnotId} t={_t:F2}");
#endif
        }
    }

    private bool TryFindClosestFromCache(out string result)
    {
        result = null;
        if (_cachedNodeIds == null) return false;

        float bestDist = float.MaxValue;
        Vector3 currentPos = transform.position;

        foreach (var obj in _cachedNodeIds)
        {
            if (obj == null) continue;
            string knotId = obj.ToString();
            try
            {
                var pos = PathManager.GetKnotWoldPosition(knotId);
                float d = (pos - currentPos).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    result = knotId;
                }
            }
            catch { continue; }
        }

        return result != null;
    }

    private bool TryBruteForceFindClosest(out string result)
    {
        result = null;
        float bestDist = float.MaxValue;
        Vector3 currentPos = transform.position;

        for (int si = 0; si < 32; si++)
        {
            for (int ki = 0; ki < 32; ki++)
            {
                var knotId = $"{si}:{ki}";
                try
                {
                    var p = PathManager.GetKnotWoldPosition(knotId);
                    float d = (p - currentPos).sqrMagnitude;
                    if (d < bestDist)
                    {
                        bestDist = d;
                        result = knotId;
                    }
                }
                catch { break; }
            }
        }

        return result != null;
    }

    private enum MoveDirection
    {
        FORWARD,
        BACKWARD
    }
}
