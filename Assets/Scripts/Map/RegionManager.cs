using System.Collections.Generic;
using Assets.Scripts.Core.States;
using Assets.Scripts.Map.Objects;
using Map.Controller;
using Newtonsoft.Json;
using UnityEngine;

namespace Map
{
    public static class RegionManager
    {
        private static GameTimeController gameTimeController;

        public static Dictionary<Region.Id, Region> Regions = new();
        public static Dictionary<Color32, Region.Id> RegionColorMapping = new();

        public static void Initialize(GameTimeController gameTimeControllerRef)
        {
            gameTimeController = gameTimeControllerRef;
        }

        public static void Load(MapWorldState worldState)
        {
            gameTimeController.CurrentTime.OnNewDay += OnNewDay;

            return;
            if (worldState.HasNoRegions()) return;

            Regions.Clear();

            var textAsset = Resources.Load<TextAsset>("Map/Regions/RegionData");
            var regionInfos = JsonConvert.DeserializeObject<Dictionary<Region.Id, RegionInfo>>(textAsset.text);
            foreach (var regionState in worldState.regions)
            {
                var faction = FactionManager.GetFaction(regionState.FactionId);
                var region = new Region(regionState.RegionId, regionInfos[regionState.RegionId], faction);

                Regions.Add(regionState.RegionId, region);
            }
        }

        public static void Save(MapWorldState worldState)
        {
            worldState.regions.Clear();
            foreach ((var regionId, var region) in Regions)
            {
                var regionState = new RegionState
                {
                    RegionId = regionId,
                    FactionId = region.Owner.FactionId,
                };
                worldState.regions.Add(regionState);
            }

            if (gameTimeController == null) return;

            gameTimeController.CurrentTime.OnNewDay -= OnNewDay;
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
            if (!Regions.TryGetValue(regionId, out var region)) throw new KeyNotFoundException($"Region with Id '{regionId}' not found.");

            return region;
        }
    }
}
