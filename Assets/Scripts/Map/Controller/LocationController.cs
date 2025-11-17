using System.Collections.Generic;
using Assets.Scripts.Map.Objects;
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
        public string KnotId;
        public Player.Player Player;

        public Location Location;

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
            Player.MoveTo(KnotId);
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.gameObject.GetComponentInParent<Player.Player>();
            if (!other.gameObject.GetComponentInParent<Player.Player>()) return;

            var enterButton = new Button(EnterLocation) { text = _localizationManager.GetText("location.enter") };
            var skipButton = new Button(SkipLocation) { text = _localizationManager.GetText("location.skip") };
            _eventPanel.Show(
                Location.Name,
                $"Willkommen in {Location.Name}!\n\n {Location.Name} ist die Hauptsiedlung in {Region.RegionInfo.Name}.",
                null,
                new List<Button>
                    {
                        enterButton,
                        skipButton,
                    }
            );

            Region.ChangeOwner(player.Faction);
        }

        private void EnterLocation()
        {
            GameManager.Instance.SwitchToScene("Region01_Location_Town");
        }
        
        private void SkipLocation()
        {
            _eventPanel.Hide();
        }
    }
}
