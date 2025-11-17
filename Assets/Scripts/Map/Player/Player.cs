using Core.States;

using UnityEngine;

namespace Map.Player
{
    public class Player : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public bool isSelected;
        public MapWorldState worldState;

        public void MoveTo(string knotId)
        {
            if (!isSelected) return;

            playerMovement.RequestPath(knotId);
        }
    }
}