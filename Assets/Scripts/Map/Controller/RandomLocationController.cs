using System.Collections.Generic;
using Core;
using Map.Objects;
using UI.Elements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Map.Controller
{
    public class RandomLocationController : MonoBehaviour
    {
        public Region region;
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

        private void OnTriggerExit(Collider other)
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggert) return;
            
            var factionUnit = other.gameObject.GetComponentInParent<Player.Player>();
            if (!other.gameObject.GetComponentInParent<Player.Player>()) return;

            _isTriggert = true;
            player.playerMovement.StopMoving();

            var enterButton = new Button(() => EnterLocation(factionUnit)) { text = _localizationManager.GetText("location.enter") };
            var skipButton = new Button(SkipLocation) { text = _localizationManager.GetText("location.skip") };
            _eventPanel.Show(
                location.locationName,
                string.Format(location.welcomeText, location.locationName, location.locationName, region.name),
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
            _eventPanel.Hide();
            //GameManager.Instance.SwitchToScene("Region01_Location_Town");
        }
        
        private void SkipLocation()
        {
            _eventPanel.Hide();
        }
    }
}