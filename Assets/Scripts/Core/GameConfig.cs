using Map.Objects;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public Faction.Id selectedFaction;
    }
}