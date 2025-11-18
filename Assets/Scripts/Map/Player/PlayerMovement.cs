using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        
        private string _currentKnotId;

        public string targetKnotId;
        private string _targetKnotId;

        private Queue<string> _pathQueue;

        public float moveSpeed = 1f;
        public float arrivalThreshold = 0.05f;
        private float _arrivalThresholdSq;

        private float _t;
        private float _targetKnotT;
        private MoveDirection _moveDirection;
        private Animator _animator;

        private const float DebugInterval = 0.5f;
        private float _debugTimer;
        private Vector3 _lastPosition;
        private float _stuckTimer;
        private const float StuckThreshold = 1.0f;
        private int _stuckRecoverAttempts;
        private const float MovedThresholdSq = 0.000001f;

        private IEnumerable _cachedNodeIds;
        private bool _nodeIdsCacheAttempted;

        private void Awake()
        {
            _animator = gameObject.GetComponentInChildren<Animator>();
            _lastPosition = transform.position;
            _arrivalThresholdSq = arrivalThreshold * arrivalThreshold;
        }

        private void Start()
        {
            _currentKnotId = "0:0";
            _t = GetClosestTOnSpline(_currentKnotId, transform.position);

#if UNITY_EDITOR
            Debug.Log($"PlayerMovement.Start current={_currentKnotId ?? "null"} t={_t:F2}");
#endif
        }

        public void StopMoving()
        {
            CancelPath();
        }
        
        public void RequestPath(string newTargetKnotId)
        {
            if (newTargetKnotId == null)
            {
                Debug.LogWarning("RequestPath: targetKnots ist null.");
                return;
            }

            EnsureCurrentKnotInitialized();

#if UNITY_EDITOR
            Debug.Log($"RequestPath: selected goal {newTargetKnotId} from current {_currentKnotId ?? "null"}");
#endif

            var pathIds = SplinePathfinder.FindPath(_currentKnotId, newTargetKnotId, PathManager.KnotGraph);
            if (pathIds == null || pathIds.Count == 0)
            {
                Debug.LogWarning($"RequestPath: kein Pfad gefunden von {_currentKnotId} nach {newTargetKnotId}");
                return;
            }

            targetKnotId = newTargetKnotId;
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
                if (_pathQueue is { Count: > 0 })
                    MoveToNextInPath();
                else
                    return;
            }

            var splineLength = Mathf.Max(0.0001f, PathManager.GetSpineLenght(_targetKnotId));
            var step = Time.deltaTime * moveSpeed / splineLength;

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

            var tangentMagnitudeSq = tangent.sqrMagnitude;
            if (tangentMagnitudeSq > 0.0001f)
                transform.rotation = Quaternion.LookRotation(tangent * (1f / Mathf.Sqrt(tangentMagnitudeSq)));

            if (_animator)
                _animator.SetBool(IsMoving, true);

            var targetPos = PathManager.GetKnotWoldPosition(_targetKnotId);
            var distToTargetSq = (transform.position - targetPos).sqrMagnitude;

            _debugTimer += Time.deltaTime;
            if (_debugTimer >= DebugInterval)
            {
                _debugTimer = 0f;
#if UNITY_EDITOR
                Debug.Log($"Player.Update curr={_currentKnotId ?? "null"} target={_targetKnotId ?? "null"} t={_t:F2} dir={_moveDirection} distToTarget={Mathf.Sqrt(distToTargetSq):F3} queue={_pathQueue?.Count ?? 0}");
#endif
            }

            var movedSq = (transform.position - _lastPosition).sqrMagnitude;
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

                if (_animator)
                    _animator.SetBool(IsMoving, false);

                MoveToNextInPath();
            }
        }

        private static float GetClosestTOnSpline(string knotId, Vector3 worldPos)
        {
            const int coarse = 24;
            var bestT = 0f;
            var bestDist = float.MaxValue;

            for (var i = 0; i <= coarse; i++)
            {
                var t = i / (float)coarse;
                var p = PathManager.EvaluateSplinePosition(knotId, t);
                var d = (p - worldPos).sqrMagnitude;
                if (!(d < bestDist)) continue;
                
                bestDist = d; 
                bestT = t;
            }

            const int refineSteps = 8;
            const float span = 1f / coarse;
            var start = Mathf.Max(0f, bestT - span);
            var end = Mathf.Min(1f, bestT + span);

            for (var i = 0; i <= refineSteps; i++)
            {
                var t = Mathf.Lerp(start, end, i / (float)refineSteps);
                var p = PathManager.EvaluateSplinePosition(knotId, t);
                var d = (p - worldPos).sqrMagnitude;
                
                if (!(d < bestDist)) continue;
                
                bestDist = d;
                bestT = t;
            }

            return Mathf.Clamp01(bestT);
        }

        private void EnsureCurrentKnotInitialized()
        {
            if (_currentKnotId != null) return;

            if (!_nodeIdsCacheAttempted)
            {
                _nodeIdsCacheAttempted = true;
                var kgType = PathManager.KnotGraph.GetType();
                var nodesProp = kgType.GetProperty("Nodes") ?? kgType.GetProperty("AllNodes") ?? kgType.GetProperty("NodeIds");
                if (nodesProp != null)
                {
                    _cachedNodeIds = nodesProp.GetValue(PathManager.KnotGraph) as IEnumerable;
                }
                else
                {
                    var getNodesMethod = kgType.GetMethod("GetNodes") ?? kgType.GetMethod("GetAllNodes");
                    if (getNodesMethod != null)
                    {
                        _cachedNodeIds = getNodesMethod.Invoke(PathManager.KnotGraph, null) as IEnumerable;
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
            if (bestKnotId == null) return;
            
            _currentKnotId = bestKnotId;

            _t = GetClosestTOnSpline(bestKnotId, transform.position);
#if UNITY_EDITOR
            Debug.Log($"EnsureCurrentKnotInitialized fallback: chose {bestKnotId} t={_t:F2}");
#endif
        }

        private bool TryFindClosestFromCache(out string result)
        {
            result = null;
            if (_cachedNodeIds == null) return false;

            var bestDist = float.MaxValue;
            var currentPos = transform.position;

            foreach (var obj in _cachedNodeIds)
            {
                if (obj == null) continue;
                var knotId = obj.ToString();
                try
                {
                    var pos = PathManager.GetKnotWoldPosition(knotId);
                    var d = (pos - currentPos).sqrMagnitude;
                    if (d < bestDist)
                    {
                        bestDist = d;
                        result = knotId;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return result != null;
        }

        private void TryBruteForceFindClosest(out string result)
        {
            result = null;
            var bestDist = float.MaxValue;
            var currentPos = transform.position;

            for (var si = 0; si < 32; si++)
            {
                for (var ki = 0; ki < 32; ki++)
                {
                    var knotId = $"{si}:{ki}";
                    try
                    {
                        var p = PathManager.GetKnotWoldPosition(knotId);
                        var d = (p - currentPos).sqrMagnitude;
                        if (d < bestDist)
                        {
                            bestDist = d;
                            result = knotId;
                        }
                    }
                    catch { break; }
                }
            } 
        }

        private enum MoveDirection
        {
            FORWARD,
            BACKWARD
        }
    }
}
