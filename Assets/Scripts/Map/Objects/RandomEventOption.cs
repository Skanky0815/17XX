using UnityEngine;

namespace Map.Objects
{
    public class RandomEventOption : ScriptableObject
    {
        public string optionName;
        [TextArea(2, 5)]
        public string tooltipText;
    }
}