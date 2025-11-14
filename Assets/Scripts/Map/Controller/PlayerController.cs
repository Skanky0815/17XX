using UnityEngine;

namespace Map.Controller
{
    class PlayerController : MonoBehaviour, ISelectable
    {
        public Player.Player Player;

        public void Select(Vector2 position)
        {
            Debug.Log("Controller hier vom PLayer");
            Player.IsSelected = true;
        }
        
        public void Deselect()
        {
            Player.IsSelected = false;
        }
    }
}