using Assets.Scripts.Map.Objects;
using UnityEngine;

namespace Assets.Scripts.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public Faction.Id SelectedFaction;
    }
}