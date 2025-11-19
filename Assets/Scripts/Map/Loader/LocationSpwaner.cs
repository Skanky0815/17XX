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
                
                if (region.randomEvents.Count == 0) continue;
                
                var regionRandomEvent = region.randomEvents[_random.Next(0, region.randomEvents.Count - 1)];
                SpawnRandom(region, regionRandomEvent, player);
            }
        }

        private void SpawnRandom(Region region, RandomEvent randomEvent, Player.Player player)
        {
            var knot = region.Knots[_random.Next(1, region.Knots.Count - 1)];
            var locationObject = Instantiate(randomEvent.prefab, knot.WorldPosition, Quaternion.identity, transform);
            locationObject.name = $"Random: {randomEvent.name} in {region.name}";

            var randomLocationController = locationObject.GetComponent<RandomLocationController>();
            randomLocationController.player = player;
            randomLocationController.region = region;
            randomLocationController.randomEvent = randomEvent;

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