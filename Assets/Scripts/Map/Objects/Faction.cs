using System.Collections.Generic;
using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/Faction")]
    public class Faction : ScriptableObject
    {
        public Id id;
        public Resources resources;
        public List<Region> regions = new();
        public string factionName;
        public Texture2D icon;
        public Texture2D flag;

        public void AddResources(Resources income)
        {
            resources += income;
        }

        public enum Id
        {
            DWARF,
            ORC
        }

        public bool CanPay(Resources costs)
        {
            var canPayGold = true;
            var canPayFood = true;
            var canPayMaterial = true;
            var canPayPopulation = true;
            if (resources.gold < 0)
            {
                canPayGold = resources.gold + resources.gold >= 0;
            }

            if (resources.food < 0)
            {
                canPayFood = resources.food + resources.food >= 0;
            }

            if (resources.material < 0)
            {
                canPayMaterial = resources.material + resources.material >= 0;
            }

            if (resources.population < 0)
            {
                canPayPopulation = resources.population + resources.population >= 0;
            }
            
            return canPayGold && canPayFood && canPayMaterial && canPayPopulation;
        }
    }
}