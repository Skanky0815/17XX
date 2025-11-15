using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class KnotCollection : Collection<KnotCollection.Knot>
{
    public Knot GetMatchingWithSplineIndex(KnotCollection otherKnots)
    {
        foreach (var knot in this)
        {
            if (otherKnots.Any(b => knot.SplineIndex == b.SplineIndex)) return knot;
        }

        throw new Exception("No Knot found");
    }

    public Knot GetBySplineIndex(int splineIndex)
    {
        foreach (var knot in this)
        {
            if (knot.SplineIndex == splineIndex) return knot;
        }

        throw new Exception("No Knot found");
    }

    public class Knot
    {
        public readonly int SplineIndex;
        public readonly int KnotIndex;
        public string Id => $"{SplineIndex}:{KnotIndex}";
        public readonly Vector3 WorldPosition;
        public KnotCollection ConnectedKnots = new();
        
        public Knot(int splineIndex, int knotIndex, Vector3 worldPosition)
        {
            SplineIndex = splineIndex;
            KnotIndex = knotIndex;
            WorldPosition = worldPosition;
        }

        public Knot(int splineIndex, int knotIndex)
        {
            SplineIndex = splineIndex;
            KnotIndex = knotIndex;
        }

        public bool IsSameSpline(Knot otherKnot)
        {
            return SplineIndex == otherKnot.SplineIndex;
        }
    }
}
