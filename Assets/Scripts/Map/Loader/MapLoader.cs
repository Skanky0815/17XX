using System.Collections.Generic;
using System.Linq;
using Core.States;
using Map.Objects;
using UnityEngine;
using UnityEngine.Splines;

namespace Map.Loader
{
    public class MapLoader : MonoBehaviour
    {
        public SplineContainer pathSplines;
        public Texture2D idMap;
        public Player.Player player;
        public MapWorldState worldState;
        public Renderer mapRenderer;
        
        private readonly List<KnotCollection> _knots = new();
        private readonly KnotGraph _knotGraph = new();

        private LocationSpawner _locationSpawner;

        private void Awake()
        {
            _locationSpawner = gameObject.AddComponent<LocationSpawner>();
        }

        private void Start()
        {
            InitializeKnots();
            InitializeKnotGraph();
            AssignKnotsToRegions();
            
            _locationSpawner.Spawn(worldState, player);
            worldState.gameTime.OnNewDay += OnNewDay;
        }

        private void OnDestroy()
        {
            worldState.gameTime.OnNewDay -= OnNewDay;
        }

        private void OnNewDay(int day)
        {
            _locationSpawner.SpawnRandom(worldState, player);
        }
        
        private void InitializeKnots()
        {
            for (var splineIndex = 0; splineIndex < pathSplines.Splines.Count; splineIndex++)
            {
                var knots = pathSplines.Splines[splineIndex].Knots;
                for (var knotIndex = 0; knotIndex < knots.Count(); knotIndex++)
                {
                    var knot = knots.ElementAt(knotIndex);
                    var worldPosition = pathSplines.transform.TransformPoint(knot.Position);

                    var isAdded = false;
                    foreach (var knotGroup in _knots)
                    {
                        if (!(Vector3.Distance(knotGroup[0].worldPosition, worldPosition) < .05f)) continue;
                        
                        var newKnot = new KnotCollection.Knot(splineIndex, knotIndex, worldPosition);
                        knotGroup.Add(newKnot);
                        isAdded = true;
                        break;
                    }

                    if (isAdded) continue;

                    _knots.Add(new KnotCollection { new KnotCollection.Knot(splineIndex, knotIndex, worldPosition) });
                }
            }

            foreach (var knotGroup in _knots)
            {
                KnotCollection knots = new();
                foreach (var knot in knotGroup)
                {
                    knots.Add(knot);
                }

                knotGroup[0].connectedKnots = knots;
            }

            PathManager.SplineContainer = pathSplines;
        }

        private void InitializeKnotGraph()
        {
            foreach (var spline in pathSplines.Splines.Select((spline, splineIndex) => (spline, splineIndex)))
            {
                for (var i = 0; i < spline.spline.Knots.Count() - 1; i++)
                {
                    var knotA = $"{spline.splineIndex}:{i}";
                    var knotB = $"{spline.splineIndex}:{i + 1}";
                    _knotGraph.AddEdge(knotA, knotB);
                }
            }

            foreach (var knots in _knots.Select(knot => knot[0].connectedKnots))
            {
                for (var i = 0; i < knots.Count; i++)
                {
                    for (var j = i + 1; j < knots.Count; j++)
                    {
                        _knotGraph.AddEdge(knots[i].Id, knots[j].Id);
                    }
                }
            }

            PathManager.KnotGraph = _knotGraph;
        }

        private void AssignKnotsToRegions()
        {
            foreach (var knotCollection in _knots)
            {
                var knot = knotCollection[0];
                var localPos = mapRenderer.transform.InverseTransformPoint(knot.worldPosition);
                var bounds = mapRenderer.localBounds;
                var u = 1f - Mathf.InverseLerp(bounds.min.x, bounds.max.x, localPos.x);
                var v = 1f - Mathf.InverseLerp(bounds.min.z, bounds.max.z, localPos.z);
                var uv = new Vector2(u, v);

                var pixelColor = idMap.GetPixelBilinear(uv.x, uv.y);
                if (worldState.RegionColorMapping().TryGetValue(pixelColor, out var region))
                {
                    region.Knots.AddRange(knotCollection);
                }
            }
        }
    }
}
