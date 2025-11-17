using System.Collections.Generic;
using Core.States;
using Map.Controller;
using Map.Objects;
using UnityEngine;

namespace Map.Loader
{
    public class LocationSpawner : MonoBehaviour
    {
        public void Spawn(MapWorldState worldState, Player.Player player)
        {
            foreach (var region in worldState.regions)
            {
                foreach (var location in region.locations)
                {
                    try
                    {
                        InstantiateLocation(region, location, player);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error spawning location {location.locationName} in region {region.regionName}: {e.Message}");
                    }
                }
            }
        }

        private void InstantiateLocation(Region region, Location location, Player.Player player)
        {
            var locationObject = Instantiate(location.prefab, region.Knots[0].WorldPosition, Quaternion.identity, transform);
            locationObject.name = $"{location.locationName} in {region.regionName}";

            var locationController = locationObject.GetComponent<LocationController>();
            locationController.player = player;
            locationController.region = region;
            locationController.knotId = region.Knots[0].Id;
            locationController.location = location;

            locationObject.SetActive(true);
        }
    }
}