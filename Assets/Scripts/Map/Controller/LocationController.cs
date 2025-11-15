using Assets.Scripts.Map.Objects;
using Map.Objects;
using UnityEngine;

namespace Map.Controller
{
    public class LocationController : MonoBehaviour, IInteractable
    {
        public Region Region;
        public string KnotId;
        public Player.Player Player;

        public Location Location;

        public void Interact(Vector2 position)
        {
            Player.MoveTo(KnotId);
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.gameObject.GetComponentInParent<Player.Player>();
            if (!other.gameObject.GetComponentInParent<Player.Player>()) return;

            Debug.Log($"Objekt {player.name} ist in {Location.Name} in der {Region.RegionInfo.Name} Region eingetroffen.");

            Region.ChangeOwner(player.Faction);
        }
    }
}
