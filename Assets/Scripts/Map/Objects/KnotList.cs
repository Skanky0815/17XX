using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Map.Objects
{
    public class KnotCollection : Collection<KnotCollection.Knot>
    {
      public class Knot
        {
            private readonly int _splineIndex;
            private readonly int _knotIndex;
            public string Id => $"{_splineIndex}:{_knotIndex}";
            public readonly Vector3 WorldPosition;
            public KnotCollection ConnectedKnots = new();
        
            public Knot(int splineIndex, int knotIndex, Vector3 worldPosition)
            {
                _splineIndex = splineIndex;
                _knotIndex = knotIndex;
                WorldPosition = worldPosition;
            }
        }
    }
}
