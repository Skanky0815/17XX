using System.Collections.Generic;
using System.Linq;
using Core;
using Core.States;
using Map.Objects;
using Map.Serializable;
using Map.Serializables;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Splines;

namespace Map.Loader
{
    public class MapLoader : MonoBehaviour
    {
        public TextAsset regionData;
        public TextAsset factionData;
        public Sprite[] factionIcons;
        public SplineContainer pathSplines;
        public Texture2D idMap;
        public Player.Player player;
        public MapWorldState worldState;
        public List<Location> locations = new();
        public Renderer mapRenderer;

        private readonly Dictionary<Faction.Id, Faction> _factions = new();
        private readonly Dictionary<Region.Id, Region> _regions = new();
        private readonly Dictionary<Color32, Region.Id> _regionColorMapping = new();
        private readonly List<KnotCollection> _knots = new();
        private readonly KnotGraph _knotGraph = new();

        private LocationSpawner _locationSpawner;

        private void Awake()
        {
            _locationSpawner = gameObject.AddComponent<LocationSpawner>();
        }

        private void Start()
        {
            LoadFactions();
            LoadRegions();
            InitializeKnots();
            InitializeKnotGraph();
            AssignKnotsToRegions();
            
            _locationSpawner.Spawn(locations, _regions, player);
        }

        private void LoadFactions()
        {
            var factionInfos = JsonConvert.DeserializeObject<Dictionary<Faction.Id, FactionInfo>>(factionData.text);

            foreach (var (factionId, factionInfo) in factionInfos)
            {
                Texture2D texture = null;
                if (factionId != Faction.Id.NEUTRAL)
                {
                    var icon = factionIcons.FirstOrDefault(s => s.name == factionInfo.icon);
                    texture = SpriteConverter.ToTexture(icon);
                }

                _factions[factionId] = new Faction(factionId, factionInfo, texture);
            }

            player.Faction = _factions[worldState.playerFactionId];
            FactionManager.PlayerFaction = _factions[worldState.playerFactionId];
            FactionManager.Factions = _factions;
        }

        private void LoadRegions()
        {
            var regionInfos = JsonConvert.DeserializeObject<Dictionary<Region.Id, RegionInfo>>(regionData.text);
            var neutralFaction = _factions[Faction.Id.NEUTRAL];
            foreach (var (regionId, regionInfo) in regionInfos)
            {
                var region = new Region(regionId, regionInfo, neutralFaction);

                _regions.Add(regionId, region);
                _regionColorMapping[Hex.ToColor32(regionInfo.idMapColor)] = regionId;
            }

            RegionManager.RegionColorMapping = _regionColorMapping;
            RegionManager.Regions = _regions;
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
                        if (Vector3.Distance(knotGroup[0].WorldPosition, worldPosition) < .05f)
                        {
                            var newKnot = new KnotCollection.Knot(splineIndex, knotIndex, worldPosition);
                            knotGroup.Add(newKnot);
                            isAdded = true;
                            break;
                        }
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

                knotGroup[0].ConnectedKnots = knots;
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

            foreach (var knots in _knots.Select(knot => knot[0].ConnectedKnots))
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
                var localPos = mapRenderer.transform.InverseTransformPoint(knot.WorldPosition);
                var bounds = mapRenderer.localBounds;
                var u = 1f - Mathf.InverseLerp(bounds.min.x, bounds.max.x, localPos.x);
                var v = 1f - Mathf.InverseLerp(bounds.min.z, bounds.max.z, localPos.z);
                var uv = new Vector2(u, v);

                var pixelColor = idMap.GetPixelBilinear(uv.x, uv.y);
                if (_regionColorMapping.TryGetValue(pixelColor, out var regionId))
                {
                    _regions[regionId].Knots.AddRange(knotCollection);
                }
            }
        }
    }
}
