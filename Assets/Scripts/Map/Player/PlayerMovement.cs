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

    private float _t = 0f;

    private MoveDirection _moveDirection;

    private Animator _animator;

    private void Awake()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();
    }

    public void RequestPath(KnotCollection knots)
    {
        if (_currentKnot == null) return;
        var goalKnot = knots.GetMatchingWithSplineIndex(_currentKnotCollection);
        var goalId = goalKnot.Id;

        var pathIds = SplinePathfinder.FindPath(_currentKnot.Id, goalId, PathManager.KnotGrapth);
        if (pathIds == null || pathIds.Count == 0) return;

        var path = pathIds.Select(id =>
        {
            var parts = id.Split(':');
            return new KnotCollection.Knot(int.Parse(parts[0]), int.Parse(parts[1]));
        }).ToList();

        StartFollowingPath(path);
    }

    public void StartFollowingPath(List<KnotCollection.Knot> path)
    {
        if (path == null || path.Count == 0) return;
        _pathQueue = new Queue<KnotCollection.Knot>(path);
        MoveToNextInPath();
    }

     private void MoveToNextInPath()
    {
        if (_pathQueue == null || _pathQueue.Count == 0)
        {
            _targetKnot = null;
            _targetKnotCollection = null;
            return;
        }

        var next = _pathQueue.Dequeue();
        _targetKnot = next;

        _targetKnotCollection = PathManager.GetCollectionBySplineIndex(next.SplineIndex);

        _moveDirection = HandleMoveDirection(_targetKnot);
        _t = HandleSplinePositon(_targetKnot);
    }

    public void SetTargetKnot(KnotCollection targetKnotCollection)
    {
        var targetKnot = targetKnotCollection.GetMatchingWithSplineIndex(_currentKnotCollection);

        _moveDirection = HandleMoveDirection(targetKnot);
        _targetKnot = targetKnot;
        _targetKnotCollection = targetKnotCollection;
        _t = HandleSplinePositon(targetKnot);
    }


    private MoveDirection HandleMoveDirection(KnotCollection.Knot targetKnot)
    {
        if (targetKnot.IsSameSpline(_currentKnot) && _currentKnot.KnotIndex < targetKnot.KnotIndex) return MoveDirection.FORWARD;

        if (_currentKnotCollection.GetBySplineIndex(targetKnot.SplineIndex).KnotIndex < targetKnot.KnotIndex) return MoveDirection.FORWARD;

        return MoveDirection.BACKWARD;
    }

    private float HandleSplinePositon(KnotCollection.Knot targetKnot)
    {
        if (targetKnot.IsSameSpline(_currentKnot)) return _t;

        return _currentKnotCollection.GetBySplineIndex(targetKnot.SplineIndex).KnotIndex < targetKnot.KnotIndex ? 0 : 1;
    }

    private void Start()
    {
        _currentKnot = new(0, 0);
        _currentKnotCollection = new() { _currentKnot };
    }

    private void Update()
    {
        if (_targetKnot == null) return;

        var newT = Time.deltaTime * MoveSpeed / PathManager.GetSpineLenght(_targetKnot.SplineIndex);
        var tangent = PathManager.EvaluateSplineTangent(_targetKnot.SplineIndex, _t);
        if (_moveDirection == MoveDirection.FORWARD)
        {
            _t += newT;
        }
        else
        {
            _t -= newT;
            tangent = -tangent;
        }

        _t = Mathf.Clamp01(_t);

        var pos = PathManager.EvaluateSplinePosition(_targetKnot.SplineIndex, _t);
        transform.position = pos;


        var targetRotation = Quaternion.LookRotation(tangent.normalized);
        transform.rotation = targetRotation;


        var targetPos = PathManager.GetKnotWoldPosition(_targetKnot.SplineIndex, _targetKnot.KnotIndex);
        _animator.SetBool("isMoving", true);
        if (Vector3.Distance(transform.position, targetPos) < .05f)
        {
            transform.position = targetPos;
            _currentKnot = _targetKnot;

            if (_targetKnotCollection != null)
                _currentKnotCollection = _targetKnotCollection;
            else
            {
                _currentKnotCollection = PathManager.GetCollectionBySplineIndex(_currentKnot.SplineIndex);
            }

            _targetKnot = null;
            _targetKnotCollection = null;
            _animator.SetBool("isMoving", false);

            MoveToNextInPath();
        }
    }

    private enum MoveDirection
    {
        FORWARD,
        BACKWARD
    }
}
