using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/World/Location")]
    public class Location : ScriptableObject
    {
        public GameObject prefab;
        [Header("Lore")]
        public string locationName;
        [TextArea(5, 10)]
        public string welcomeText;

        public Texture2D ambientImage;
    }
}
