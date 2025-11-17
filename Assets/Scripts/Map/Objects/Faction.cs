using System.Collections.Generic;
using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/Faction")]
    public class Faction : ScriptableObject
    {
        public Id id;
        public int gold;
        public int food;
        public int material;
        public int population;
        public List<Region> regions = new();
        public string factionName;
        public Texture2D icon;
        public Texture2D flag;

        public void AddResources(int gold, int food, int material, int population)
        {
            this.gold += gold;
            this.food += food;
            this.material += material;
            this.population += population;
        }

        public enum Id
        {
            DWARF,
            ORC
        } 
    }
}