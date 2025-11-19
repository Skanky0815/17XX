using System.Linq;
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
        public RandomEvent randomEvent;

        private EventPanel _eventPanel;
        private LocalizationManager _localizationManager;

        private bool _isTriggert;
        private bool _isHandled;

        private void Start()
        {
            var uiDocument = FindFirstObjectByType<UIDocument>();
            _eventPanel = uiDocument.rootVisualElement.Q<EventPanel>();
            _localizationManager = LocalizationManager.Instance;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_isHandled) return;
            
            gameObject.SetActive(false);
            _isHandled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggert) return;
            
            var factionUnit = other.gameObject.GetComponentInParent<Player.Player>();
            if (!other.gameObject.GetComponentInParent<Player.Player>()) return;

            _isTriggert = true;
            player.playerMovement.StopMoving();

            // TODO: Check if Faction can pay the costs if not buttons should be disabled
            var buttons = randomEvent.options.Select(option => new Button(() => HandleOption(factionUnit, option)) { text = option.optionName, }).ToList();

            buttons.Add(new Button(SkipLocation) { text = _localizationManager.GetText("location.skip") });
            _eventPanel.Show(
                randomEvent.eventName,
                randomEvent.description,
                randomEvent.ambientImage,
                buttons
            );
        }

        private void HandleOption(Player.Player factionUnit,  RandomEventOption option)
        {
            factionUnit.worldState.playerFaction.AddResources(option.gold, option.food,option.material, option.population);
            factionUnit.worldState.gameTime.hour += option.hour;
            
            _isHandled = true;
            
            _eventPanel.Hide();
        }
        
        private void SkipLocation()
        {
            _eventPanel.Hide();
        }
    }
}