using UnityEngine;

namespace Map.Controller
{
    class PlayerController : MonoBehaviour, ISelectable
    {
        public Player.Player player;

        public void Select(Vector2 position)
        {
            Debug.Log("Controller hier vom PLayer");
            player.isSelected = true;
        }
        
        public void Deselect()
        {
            player.isSelected = false;
        }
    }
}