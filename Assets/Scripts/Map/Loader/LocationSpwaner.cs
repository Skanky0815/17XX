using System.Collections.Generic;
using System.Linq;
using Map.Controller;
using Map.Objects;
using UnityEngine;

namespace Map.Loader
{
    public class LocationSpawner : MonoBehaviour
    {
        public void Spawn(List<Location> locations, Dictionary<Region.Id, Region> regions, Player.Player player)
        {
            foreach (var location in locations)
            {
                foreach (var region in location.allowedRegions.Select(allowedRegion => regions[allowedRegion]))
                {
                    try
                    {
                        InstantiateLocation(region, location, player);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error spawning location {location.locationName} in region {region.RegionId}: {e.Message}");
                    }
                }
            }
        }

        private void InstantiateLocation(Region region, Location location, Player.Player player)
        {
            var locationObject = Instantiate(location.prefab, region.Knots[0].WorldPosition, Quaternion.identity, transform);
            locationObject.name = $"{location.locationName} in {region.RegionInfo.name}";

            var locationController = locationObject.GetComponent<LocationController>();
            locationController.player = player;
            locationController.Region = region;
            locationController.knotId = region.Knots[0].Id;
            locationController.location = location;

            locationObject.SetActive(true);
        }
    }
}