using Map.Player;
using UnityEngine;

public class WayPoint : MonoBehaviour, IInteractable
{
    public KnotCollection ConnectecdKnots;
    public Player Player;

    public void Interact(Vector2 position)
    {
        Player.MoveTo(ConnectecdKnots);
    }
}
