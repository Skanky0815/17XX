using System.Collections.Generic;
using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/RandomEvent")]
    public class RandomEvent : ScriptableObject
    {
        // TODO: factor for respawn in region (eg.: one per week)
        public GameObject prefab;
        public List<RandomEventOption> options;
        public bool isSkipable;
        public int daysToRespawn;
        
        [Header("Lore")]
        public string eventName;
        public Texture2D ambientImage;
        [TextArea(5, 10)]
        public string description;
    }
}