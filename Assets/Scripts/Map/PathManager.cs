using System.Linq;
using Map.Objects;
using UnityEngine;
using UnityEngine.Splines;

namespace Map
{
    public static class PathManager
    {
        public static SplineContainer SplineContainer;

        public static KnotGraph KnotGraph = new();

        public static float GetSpineLenght(string knotId)
        {
            var (splineIndex, _) = ParseKnotId(knotId);
            var spline = SplineContainer.Splines[splineIndex];
            
            return spline.GetLength();
        }

        public static Vector3 GetKnotWoldPosition(string knotId)
        {
            var (splineIndex, knotIndex) = ParseKnotId(knotId);

            var spline = SplineContainer.Splines[splineIndex];
            var localPos = spline.Knots.ElementAt(knotIndex).Position;

            return SplineContainer.transform.TransformPoint(localPos);
        }

        public static Vector3 EvaluateSplinePosition(string knotId, float t)
        {
            var (splineIndex, _) = ParseKnotId(knotId);
            var spline = SplineContainer.Splines[splineIndex];

            return spline.EvaluatePosition(t);
        }

        public static Vector3 EvaluateSplineTangent(string knotId, float t)
        {
            var (splineIndex, _) = ParseKnotId(knotId);
            var spline = SplineContainer.Splines[splineIndex];

            return spline.EvaluateTangent(t);
        }

        private static (int splineIndex, int knotIndex) ParseKnotId(string knotId)
        {
            var parts = knotId.Split(':');
            var splineIndex = int.Parse(parts[0]);
            var knotIndex = int.Parse(parts[1]);
            return (splineIndex, knotIndex);
        }
    }
}
