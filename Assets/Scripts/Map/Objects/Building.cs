using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/Building")]
    public class Building : ScriptableObject
    {
        public Texture2D icon;
        public GameObject prefab;
        public string buildingName;
        public string description;
        
        public Resources resources;
        public int constructionTimeInDays;
        public Resources income;
    }
}