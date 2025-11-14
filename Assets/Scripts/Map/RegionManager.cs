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

        private static readonly Dictionary<Region.Id, Region> Regions = new();

        public static void Initialize(GameTimeController gameTimeControllerRef)
        {
            gameTimeController = gameTimeControllerRef;

            if (Regions.Count == 0)
            {
                var textAsset = Resources.Load<TextAsset>("Map/Regions/RegionData");
                var regionInfos = JsonConvert.DeserializeObject<Dictionary<Region.Id, RegionInfo>>(textAsset.text);
                var neutralFaction = FactionManager.GetFaction(Faction.Id.NEUTRAL);
                foreach ((var regionId, var regionInfo) in regionInfos)
                {
                    var region = new Region(regionId, regionInfo, neutralFaction);

                    Regions.Add(regionId, region);
                }
            }
        }

        public static void Load(MapWorldState worldState)
        {
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

            gameTimeController.CurrentTime.OnNewDay += OnNewDay;
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

        public static Dictionary<Color32, Region.Id> GetIdMap()
        {
            var idMap = new Dictionary<Color32, Region.Id>();

            foreach ((var regionId, var region) in Regions)
            {
                var color = Hex.ToColor32(region.RegionInfo.IdMapColor);
                idMap.Add(color, regionId);
            }

            return idMap;
        }

        public static Region GetRegion(Region.Id regionId)
        {
            if (!Regions.TryGetValue(regionId, out var region)) throw new KeyNotFoundException($"Region with Id '{regionId}' not found.");

            return region;
        }
    }
}
