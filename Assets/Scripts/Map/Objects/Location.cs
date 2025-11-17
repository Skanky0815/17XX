using System.Collections.Generic;
using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/Location")]
    public class Location : ScriptableObject
    {
        public string locationName;
        public LocationType locationType;
        public GameObject prefab;

        public List<Region.Id> allowedRegions = new();
    }
}

public enum LocationType { FIXED, RANDOM }
