using Assets.Scripts.Map.Objects;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement PlayerMovement;

    public bool IsSelected;

    public Faction Faction { get; internal set; }

    public void MoveTo(KnotCollection knot)
    {
        if (!IsSelected) return;

        PlayerMovement.RequestPath(knot);
    }
}
