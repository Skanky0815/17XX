using UnityEngine;

namespace Map.Controller
{
    class PlayerController : MonoBehaviour, IInteractable
    {
        public Player player;

        public void Interact(Vector2 position)
        {
            Debug.Log("Controller hier vom PLayer");
            player.IsSelected = true;
        }
    }
}