using UnityEngine;

namespace Map.Objects.Events
{
    [CreateAssetMenu(menuName = "Game/Events/LevelOption")]
    public class LevelOption : RandomEventOption
    {
        public string scene;
    }
}