using Assets.Scripts.Map.Objects;
using UnityEngine;

namespace Map.Player
{
    public class Player : MonoBehaviour
    {
        public PlayerMovement PlayerMovement;

        public bool IsSelected;

        public Faction Faction { get; internal set; }

        public void MoveTo(string knotId)
        {
            if (!IsSelected) return;

            PlayerMovement.RequestPath(knotId);
        }
    }
}