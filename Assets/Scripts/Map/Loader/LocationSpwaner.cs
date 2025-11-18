using Core.States;
using Map.Controller;
using Map.Objects;
using UnityEngine;

namespace Map.Loader
{
    public class LocationSpawner : MonoBehaviour
    {
        private readonly System.Random _random = new(); 
        
        public void Spawn(MapWorldState worldState, Player.Player player)
        {
            foreach (var region in worldState.regions)
            {
                SpawnTown(region, player);
                
                if (region.locations.Count == 0) continue;
                
                var randomLocation = region.locations[_random.Next(0, region.locations.Count - 1)];
                SpawnRandom(region, randomLocation, player);
            }
        }

        private void SpawnRandom(Region region, Location location, Player.Player player)
        {
            var knot = region.Knots[_random.Next(1, region.Knots.Count - 1)];
            var locationObject = Instantiate(location.prefab, knot.WorldPosition, Quaternion.identity, transform);
            locationObject.name = $"Random: {location.locationName} in {region.regionName}";

            var locationController = locationObject.GetComponent<RandomLocationController>();
            locationController.player = player;
            locationController.region = region;
            locationController.location = location;

            locationObject.SetActive(true);   
        }
        
        private void SpawnTown(Region region, Player.Player player)
        {
            var town = region.town;
            
            if (town == null) return;
            
            var locationObject = Instantiate(town.prefab, region.Knots[1].WorldPosition, Quaternion.identity, transform);
            locationObject.name = $"{town.locationName} in {region.regionName}";

            var locationController = locationObject.GetComponent<LocationController>();
            locationController.player = player;
            locationController.region = region;
            locationController.knotId = region.Knots[1].Id;
            locationController.location = town;

            locationObject.SetActive(true);
        }
    }
}