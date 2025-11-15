using System.Collections.Generic;
using Assets.Scripts.Map.Objects;
using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/Location")]
    public class Location : ScriptableObject
    {
        public string Name;
        public LocationType LocationType;
        public GameObject Prefab;

        public List<Region.Id> AllowedRegions = new();
    }
}

public enum LocationType { Fixed, Random }
