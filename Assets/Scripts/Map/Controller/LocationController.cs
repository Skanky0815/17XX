using Assets.Scripts.Map.Objects;
using UnityEngine;

namespace Map.Controller
{
    public class LocationController : MonoBehaviour
    {
        public Region.Id RegionId;

        private Region _region;

        private void Start()
        {
            _region = RegionManager.GetRegion(RegionId);
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.gameObject.GetComponentInParent<Player.Player>();
            if (!other.gameObject.GetComponentInParent<Player.Player>()) return;

            Debug.Log($"Objekt {player.name} ist in {_region.RegionInfo.Name} eingetreten.");

            _region.ChangeOwner(player.Faction);
        }
    }

}
