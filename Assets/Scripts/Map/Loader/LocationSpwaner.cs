using System.Collections.Generic;
using Core.States;
using Map.Controller;
using Map.Objects;
using UnityEngine;

namespace Map.Loader
{
    public class LocationSpawner : MonoBehaviour
    {
        private Dictionary<string, GameObject> _randomEventPool = new();
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

        public void SpawnRandom(MapWorldState worldState, Player.Player player)
        {
            for (var i = 0; i < worldState.regions.Count; i++)
            {
                var region  = worldState.regions[i];
                if (region.randomEvents.Count == 0 || region.currentEvent) continue;
                
                var randomEvent = region.randomEvents[_random.Next(0, region.randomEvents.Count - 1)];
                if (_randomEventPool.TryGetValue($"{region.name}+{randomEvent.name}", out var randomEventGameObjekt))
                {
                    ReactivateRandomEvent(region, randomEventGameObjekt, randomEvent);
                }
                else
                {
                    SpawnRandom(region, randomEvent, player);    
                }
            }
        }

        private void ReactivateRandomEvent(Region region, GameObject randomEventGameObjekt, RandomEvent randomEvent)
        {
            var knot = region.Knots[_random.Next(1, region.Knots.Count - 1)];
            randomEventGameObjekt.transform.position = knot.worldPosition;
            randomEventGameObjekt.SetActive(true);
            region.currentEvent = randomEvent;
        }

        private void SpawnRandom(Region region, RandomEvent randomEvent, Player.Player player)
        {
            var knot = region.Knots[_random.Next(1, region.Knots.Count - 1)];
            var locationObject = Instantiate(randomEvent.prefab, knot.worldPosition, Quaternion.identity, transform);
            locationObject.name = $"Random: {randomEvent.name} in {region.name}";

            var randomLocationController = locationObject.GetComponent<RandomLocationController>();
            randomLocationController.SetData(region, player, randomEvent);

            locationObject.SetActive(true);   
            _randomEventPool.Add($"{region.name}+{randomEvent.name}", locationObject);
        }
        
        private void SpawnTown(Region region, Player.Player player)
        {
            var town = region.town;
            
            if (town == null) return;
            
            var locationObject = Instantiate(town.prefab, region.Knots[1].worldPosition, Quaternion.identity, transform);
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