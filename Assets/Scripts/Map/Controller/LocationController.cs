using System.Collections.Generic;
using Core;
using Map.Objects;
using UI.Elements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Map.Controller
{
    public class LocationController : MonoBehaviour, IInteractable
    {
        public Region Region;
        public string knotId;
        public Player.Player player;
        public Location location;

        private EventPanel _eventPanel;
        private LocalizationManager _localizationManager;

        private void Start()
        {
            var uiDocument = FindFirstObjectByType<UIDocument>();
            _eventPanel = uiDocument.rootVisualElement.Q<EventPanel>();
            _localizationManager = LocalizationManager.Instance;
        }

        public void Interact(Vector2 position)
        {
            player.MoveTo(knotId);
        }

        private void OnTriggerEnter(Collider other)
        {
            var factionUnit = other.gameObject.GetComponentInParent<Player.Player>();
            if (!other.gameObject.GetComponentInParent<Player.Player>()) return;

            var enterButton = new Button(() => EnterLocation(factionUnit)) { text = _localizationManager.GetText("location.enter") };
            var skipButton = new Button(SkipLocation) { text = _localizationManager.GetText("location.skip") };
            _eventPanel.Show(
                location.locationName,
                $"Willkommen in {location.locationName}!\n\n {location.locationName} ist die Hauptsiedlung in {Region.RegionInfo.name}.",
                null,
                new List<Button>
                    {
                        enterButton,
                        skipButton,
                    }
            );
        }

        private void EnterLocation(Player.Player factionUnit)
        {
            Region.ChangeOwner(factionUnit.Faction);
            GameManager.Instance.SwitchToScene("Region01_Location_Town");
        }
        
        private void SkipLocation()
        {
            _eventPanel.Hide();
        }
    }
}
