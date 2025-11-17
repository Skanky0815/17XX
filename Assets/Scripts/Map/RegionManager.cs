using System.Collections.Generic;
using Core.States;
using Map.Controller;
using Map.Objects;
using Map.Serializable;
using Newtonsoft.Json;
using UnityEngine;

namespace Map
{
    public static class RegionManager
    {
        private static GameTimeController _gameTimeController;

        public static Dictionary<Region.Id, Region> Regions = new();
        public static Dictionary<Color32, Region.Id> RegionColorMapping = new();

        public static void Initialize(GameTimeController gameTimeControllerRef)
        {
            _gameTimeController = gameTimeControllerRef;
        }

        public static void Load(MapWorldState worldState)
        {
            _gameTimeController.CurrentTime.OnNewDay += OnNewDay;

            return;
            if (worldState.HasNoRegions()) return;

            Regions.Clear();

            var textAsset = Resources.Load<TextAsset>("Map/Regions/RegionData");
            var regionInfos = JsonConvert.DeserializeObject<Dictionary<Region.Id, RegionInfo>>(textAsset.text);
            foreach (var regionState in worldState.regions)
            {
                var faction = FactionManager.GetFaction(regionState.factionId);
                var region = new Region(regionState.regionId, regionInfos[regionState.regionId], faction);

                Regions.Add(regionState.regionId, region);
            }
        }

        public static void Save(MapWorldState worldState)
        {
            worldState.regions.Clear();
            foreach (var (regionId, region) in Regions)
            {
                var regionState = new RegionState
                {
                    regionId = regionId,
                    factionId = region.Owner.FactionId,
                };
                worldState.regions.Add(regionState);
            }

            if (_gameTimeController == null) return;

            _gameTimeController.CurrentTime.OnNewDay -= OnNewDay;
        }

        public static void OnNewDay(int day)
        {
            foreach (var region in Regions.Values)
            {
                region.AggregateDailyResources();
            }
        }

        public static Region GetRegion(Region.Id regionId)
        {
            return !Regions.TryGetValue(regionId, out var region) ? throw new KeyNotFoundException($"Region with Id '{regionId}' not found.") : region;
        }
    }
}
