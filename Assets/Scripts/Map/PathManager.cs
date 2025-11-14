using System;
using System.Collections.Generic;
using System.Linq;
using Map.Objects;
using UnityEngine;
using UnityEngine.Splines;

namespace Map
{
    public class PathManager : MonoBehaviour
    {
        public SplineContainer _splineContainer;

        public GameObject Prefab;

        public readonly KnotGraph KnotGrapth = new();

        private void Start()
        {
            var knotGroups = new List<List<(int splineIndex, int knotIndex, Vector3 worldPosition)>>();

            for (var splineIndex = 0; splineIndex < _splineContainer.Splines.Count; splineIndex++)
            {
                var knots = _splineContainer.Splines[splineIndex].Knots;
                for (var knotIndex = 0; knotIndex < knots.Count(); knotIndex++)
                {
                    var knot = knots.ElementAt(knotIndex);
                    var worldPosition = _splineContainer.transform.TransformPoint(knot.Position);

                    var isAdded = false;
                    foreach (var knotGroup in knotGroups)
                    {
                        if (Vector3.Distance(knotGroup[0].worldPosition, worldPosition) < .05f)
                        {
                            knotGroup.Add((splineIndex, knotIndex, worldPosition));
                            isAdded = true;
                            break;
                        }
                    }

                    if (isAdded) continue;

                    knotGroups.Add(new List<(int, int, Vector3)> { (splineIndex, knotIndex, worldPosition) });
                }
            }

            foreach (var knotGroup in knotGroups)
            {
                var cube = Instantiate(Prefab, knotGroup[0].worldPosition, Quaternion.identity, transform);
                cube.name = $"WayPoint {(knotGroup.Count > 1 ? "Intersection" : "Path")}";
                cube.SetActive(true);
                cube.layer = 10;
                var wayPoint = cube.GetComponent<WayPoint>();
                KnotCollection knots = new();
                foreach ((int splineIndex, int knotIndex, Vector3 worldPosition) in knotGroup)
                {
                    knots.Add(new(splineIndex, knotIndex));
                }

                wayPoint.ConnectecdKnots = knots;
            }

            foreach (var spline in _splineContainer.Splines.Select((spline, splineIndex) => (spline, splineIndex)))
            {
                for (int i = 0; i < spline.spline.Knots.Count() - 1; i++)
                {
                    var knotA = new KnotCollection.Knot(spline.splineIndex, i);
                    var knotB = new KnotCollection.Knot(spline.splineIndex, i + 1);
                    KnotGrapth.AddEdge(knotA, knotB);
                }
            }

            foreach (var wayPoint in FindObjectsByType<WayPoint>(FindObjectsSortMode.None))
            {
                var knots = wayPoint.ConnectecdKnots;
                for (int i = 0; i < knots.Count; i++)
                {
                    for (int j = i + 1; j < knots.Count; j++)
                    {
                        KnotGrapth.AddEdge(knots[i], knots[j]);
                    }
                }
            }

        }

        public float GetSpineLenght(int splineIndex)
        {
            var spline = _splineContainer.Splines[splineIndex];
            return spline.GetLength();
        }

        public Vector3 GetKnotWoldPosition(int splineInderx, int knotIndex)
        {
            var spline = _splineContainer.Splines[splineInderx];
            var localPos = spline.Knots.ElementAt(knotIndex).Position;

            return _splineContainer.transform.TransformPoint(localPos);
        }

        public Vector3 EvaluateSplinePosition(int splineIndex, float t)
        {
            var spline = _splineContainer.Splines[splineIndex];
            return SplineUtility.EvaluatePosition(spline, t);
        }

        public Vector3 EvaluateSplineTangent(int splineIndex, float t)
        {
            var spline = _splineContainer.Splines[splineIndex];
            return SplineUtility.EvaluateTangent(spline, t);
        }

        internal KnotCollection GetCollectionBySplineIndex(int splineIndex)
        {
            var knots = new KnotCollection();
            foreach (var wayPoint in FindObjectsByType<WayPoint>(FindObjectsSortMode.None))
            {
                foreach (var knot in wayPoint.ConnectecdKnots)
                {
                    if (knot.SplineIndex == splineIndex)
                    {
                        knots.Add(knot);
                    }
                }
            }

            return knots;
        }
    }
}
