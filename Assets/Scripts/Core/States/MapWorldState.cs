using System.Collections.Generic;
using Map.Objects;
using UnityEngine;

namespace Core.States
{
    [CreateAssetMenu(menuName = "Game/WorldState")]
    public class MapWorldState : ScriptableObject
    {
        public Vector3 cameraPosition;
        public List<Faction> factions;
        public Faction playerFaction;
        public List<Region> regions;
        public GameTime gameTime;

        private readonly Dictionary<Color32, Region> _regionColorMapping = new();
        
        public Dictionary<Color32, Region> RegionColorMapping()
        {
            if (_regionColorMapping.Count != 0) return _regionColorMapping;
            
            foreach (var region in regions)
            { 
                _regionColorMapping[Hex.ToColor32(region.idMapColor)] = region;
            }
            
            return _regionColorMapping;
        }
    }
}