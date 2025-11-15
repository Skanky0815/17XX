using System.Collections.Generic;
using Assets.Scripts.Map.Objects;
using Map.Controller;
using Map.Objects;
using UnityEngine;

namespace Map.Loader
{
    public class LocationSpwaner : MonoBehaviour
    {
        public void Spwan(List<Location> locations, Dictionary<Region.Id, Region> regions, Player.Player player)
        {
            foreach (var location in locations)
            {
                foreach (var allowedRegion in location.AllowedRegions)
                {
                    var region = regions[allowedRegion];
                    try
                    {
                        InstantiateLocation(region, location, player);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error spawning location {location.Name} in region {region.RegionId}: {e.Message}");
                    }
                }
            }
        }

        private void InstantiateLocation(Region region, Location location, Player.Player player)
        {
            var locationObject = Instantiate(location.Prefab, region.Knots[0].WorldPosition, Quaternion.identity, transform);
            locationObject.name = $"{location.Name} in {region.RegionInfo.Name}";

            var locationController = locationObject.GetComponent<LocationController>();
            locationController.Player = player;
            locationController.Region = region;
            locationController.KnotId = region.Knots[0].Id;
            locationController.Location = location;

            locationObject.SetActive(true);
        }
    }
}