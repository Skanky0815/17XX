using System.Collections.Generic;
using Assets.Scripts.Map.Objects;
using UnityEngine;

namespace Assets.Scripts.Core.States
{
    [CreateAssetMenu(menuName = "Game/WorldState")]
    public class MapWorldState : ScriptableObject
    {
        public Vector3 cameraPosition;
        public List<FactionState> factions;
        public Faction.Id playerFactionId;
        public List<RegionState> regions;
        public TimeState timeState;

        public bool HasNoFactions()
        {
            return factions.Count == 0;
        }

        public bool HasNoRegions()
        {
            return regions.Count == 0;
        }
    }
}