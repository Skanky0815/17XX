using Map.Objects;
using UnityEngine;

namespace Map.Player
{
    public class Player : MonoBehaviour
    {
        public PlayerMovement playerMovement;

        public bool isSelected;

        public Faction Faction { get; internal set; }

        public void MoveTo(string knotId)
        {
            if (!isSelected) return;

            playerMovement.RequestPath(knotId);
        }
    }
}