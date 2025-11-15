using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.States;
using Assets.Scripts.Map.Objects;
using Map.Controller;
using Map.Objects;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Splines;

namespace Map.Loader
{
    public class MapLoader : MonoBehaviour
    {
        public TextAsset RegionData;
        public TextAsset FactionData;
        public Sprite[] FactionIcons;
        public SplineContainer PathSplines;
        public Texture2D IdMap;
        public Player.Player Player;
        public MapWorldState WorldState;
        public List<Location> Locations = new();
        public Renderer MapRenderer;

        private Dictionary<Faction.Id, Faction> _factions = new();
        private Dictionary<Region.Id, Region> _regions = new();
        private Dictionary<Color32, Region.Id> _regionColorMapping = new();
        private List<KnotCollection> _knots = new();
        private KnotGraph _knotGrapth = new();

        private void Start()
        {
            LoadFactions();
            LoadRegions();
            InitializeKnots();
            InitializeKnotGraph();
            AssginKnotsToRegions();
            SpwanLocations();
        }

        private void LoadFactions()
        {
            var factionInfos = JsonConvert.DeserializeObject<Dictionary<Faction.Id, FactionInfo>>(FactionData.text);

            foreach ((var factionId, var factionInfo) in factionInfos)
            {
                Texture2D texture = null;
                if (factionId != Faction.Id.NEUTRAL)
                {
                    var icon = FactionIcons.FirstOrDefault(s => s.name == factionInfo.Icon);
                    texture = SpriteConverter.ToTexture(icon);
                }

                _factions[factionId] = new Faction(factionId, factionInfo, texture);
            }

            Player.Faction = _factions[WorldState.playerFactionId];
            FactionManager.PlayerFaction = _factions[WorldState.playerFactionId];
            FactionManager.Factions = _factions;
        }

        private void LoadRegions()
        {
            var regionInfos = JsonConvert.DeserializeObject<Dictionary<Region.Id, RegionInfo>>(RegionData.text);
            var neutralFaction = _factions[Faction.Id.NEUTRAL];
            foreach ((var regionId, var regionInfo) in regionInfos)
            {
                var region = new Region(regionId, regionInfo, neutralFaction);

                _regions.Add(regionId, region);
                _regionColorMapping[Hex.ToColor32(regionInfo.IdMapColor)] = regionId;
            }

            RegionManager.RegionColorMapping = _regionColorMapping;
            RegionManager.Regions = _regions;
        }

        private void InitializeKnots()
        {
            for (var splineIndex = 0; splineIndex < PathSplines.Splines.Count; splineIndex++)
            {
                var knots = PathSplines.Splines[splineIndex].Knots;
                for (var knotIndex = 0; knotIndex < knots.Count(); knotIndex++)
                {
                    var knot = knots.ElementAt(knotIndex);
                    var worldPosition = PathSplines.transform.TransformPoint(knot.Position);

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

            PathManager.SplineContainer = PathSplines;
        }

        private void InitializeKnotGraph()
        {
            foreach (var spline in PathSplines.Splines.Select((spline, splineIndex) => (spline, splineIndex)))
            {
                for (int i = 0; i < spline.spline.Knots.Count() - 1; i++)
                {
                    var knotA = $"{spline.splineIndex}:{i}";
                    var knotB = $"{spline.splineIndex}:{i + 1}";
                    _knotGrapth.AddEdge(knotA, knotB);
                }
            }

            foreach (var knot in _knots)
            {
                var knots = knot[0].ConnectedKnots;
                for (int i = 0; i < knots.Count; i++)
                {
                    for (int j = i + 1; j < knots.Count; j++)
                    {
                        _knotGrapth.AddEdge(knots[i].Id, knots[j].Id);
                    }
                }
            }

            PathManager.KnotGrapth = _knotGrapth;
        }

        private void AssginKnotsToRegions()
        {
            foreach (var knotCollection in _knots)
            {
                var knot = knotCollection[0];
                var localPos = MapRenderer.transform.InverseTransformPoint(knot.WorldPosition);
                var bounds = MapRenderer.localBounds;
                var u = Mathf.InverseLerp(bounds.min.x, bounds.max.x, localPos.x);
                var v = Mathf.InverseLerp(bounds.min.z, bounds.max.z, localPos.z);
                var uv = new Vector2(u, v);

                var pixelColor = IdMap.GetPixelBilinear(uv.x, uv.y);
                if (_regionColorMapping.TryGetValue(pixelColor, out var regionId))
                {
                    _regions[regionId].Knots.AddRange(knotCollection);
                }
                else
                {
                    Debug.LogWarning($"No region found for knot at position {knot.WorldPosition} with color {pixelColor}");
                }
            }
        }

        private void SpwanLocations()
        {
            var layer = LayerMask.NameToLayer("Interactable");
            foreach (var location in Locations)
            {
                foreach (var allowedRegion in location.AllowedRegions)
                {
                    var region = _regions[allowedRegion];
                    try
                    {
                        var locationObject = Instantiate(location.Prefab, region.Knots[0].WorldPosition, Quaternion.identity, transform);
                        locationObject.name = $"{location.Name} in {region.RegionInfo.Name}";
                        var locationController = locationObject.GetComponent<LocationController>();
                        locationController.Player = Player;
                        locationController.Region = region;
                        locationController.KnotId = region.Knots[0].Id;
                        locationController.Location = location;

                        locationObject.SetActive(true);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error spawning location {location.Name} in region {region.RegionId}: {e.Message}");
                    }
                }
            }
        }
    }
}
