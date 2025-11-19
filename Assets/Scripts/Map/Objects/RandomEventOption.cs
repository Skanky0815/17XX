using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/RandomEventOption")]
    public class RandomEventOption : ScriptableObject
    {
        public string optionName;
        [TextArea(2, 5)]
        public string tooltipText;
        
        [Header("Rewads/Costs")]
        public int gold;
        public int food;
        public int material;
        public int population;

        public int hour;
    }
}