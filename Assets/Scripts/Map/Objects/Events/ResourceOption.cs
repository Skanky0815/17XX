using UnityEngine;

namespace Map.Objects.Events
{
    [CreateAssetMenu(menuName = "Game/Events/ResourceOption")]
    public class ResourceOption : RandomEventOption
    {
        [Header("Rewads/Costs")]
        public int gold;
        public int food;
        public int material;
        public int population;

        public int hour;

        public Costs ToCosts()
        {
            return new Costs()
            {
                gold = gold,
                material = material,
                food = food,
                population = population,
            };
        }
    }
}