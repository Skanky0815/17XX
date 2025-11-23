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
        public Region region;
        public string knotId;
        public Player.Player player;
        public Location location;

        private EventPanel _eventPanel;
        private LocalizationManager _localizationManager;

        private bool _isTriggert;

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

        private void OnTriggerExit(Collider other)
        {
            _isTriggert = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggert) return;
            
            var factionUnit = other.gameObject.GetComponentInParent<Player.Player>();
            if (!factionUnit || factionUnit.playerMovement.targetKnotId != knotId) return;

            _isTriggert = true;
            player.playerMovement.StopMoving();

            var enterButton = new Button(() => EnterLocation(factionUnit)) { text = _localizationManager.GetText("location.enter") };
            var skipButton = new Button(SkipLocation) { text = _localizationManager.GetText("location.skip") };
            _eventPanel.Show(
                location,
                new List<Button>
                    {
                        enterButton,
                        skipButton,
                    }
            );
        }

        private void EnterLocation(Player.Player factionUnit)
        {
            region.ChangeOwner(factionUnit.worldState.playerFaction);
            _eventPanel.Hide();
            //GameManager.Instance.SwitchToScene("Region01_Location_Town");
        }
        
        private void SkipLocation()
        {
            _eventPanel.Hide();
        }
    }
}
