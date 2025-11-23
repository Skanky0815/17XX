using UnityEngine;

namespace Map.Objects.Events
{
    [CreateAssetMenu(menuName = "Game/Events/ResourceOption")]
    public class ResourceOption : RandomEventOption
    {
        [Header("Rewads/Costs")]
        public Resources costs;

        public int hour;
    }
}