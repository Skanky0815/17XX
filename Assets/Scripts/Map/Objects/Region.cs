using System.Collections.Generic;
using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/Region")]
    public class Region : ScriptableObject
    {
        public Id id;
        public string idMapColor;
        public Location town;
        public List<RandomEvent> randomEvents = new();
        public Faction owner;
        [Header("Lore")]
        public string regionName;
        [TextArea(5, 10)]
        public string description;
        public Texture2D ambientImage;
        
        [Header("Resources")]
        public int gold;
        public int food;
        public int material;
        public int population;
        
        public readonly List<KnotCollection.Knot> Knots = new();
        public RandomEvent currentEvent;
        
        public void ChangeOwner(Faction faction)
        {
            owner?.regions.Remove(this);

            faction.regions.Add(this);
            owner = faction;
        }

        public void AggregateDailyResources()
        {
            owner?.AddResources(gold, food, material, population);
        }
        
        public enum Id
        {
            REGION_01,
            REGION_02,
            REGION_03,
            REGION_04,
            REGION_05,
            REGION_06,
            REGION_07,
            REGION_08,
            REGION_09,
            REGION_10,
            REGION_11,
            REGION_12,
            REGION_13,
            REGION_14
        }
    }
}
