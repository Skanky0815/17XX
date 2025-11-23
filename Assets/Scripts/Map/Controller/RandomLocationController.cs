using System.Linq;
using Core;
using Map.Objects;
using Map.Objects.Events;
using UI.Elements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Map.Controller
{
    public class RandomLocationController : MonoBehaviour
    {
        private Region _region;
        private Player.Player _player;
        private RandomEvent _randomEvent;

        private EventPanel _eventPanel;
        private LocalizationManager _localizationManager;

        private bool _isTriggert;
        private bool _isHandled;

        public void SetData(Region region, Player.Player player, RandomEvent randomEvent)
        {
            _region = region;
            region.currentEvent = randomEvent;
            _player = player;
            _randomEvent = randomEvent;
        }

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
            _region.currentEvent = null;
            _isHandled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggert) return;
            
            var factionUnit = other.gameObject.GetComponentInParent<Player.Player>();
            if (!other.gameObject.GetComponentInParent<Player.Player>()) return;

            _isTriggert = true;
            _player.playerMovement.StopMoving();

            var buttons = _randomEvent.options.Select(option => CreateButton(option, factionUnit)).ToList();
            if (_randomEvent.isSkipable)
            {
                buttons.Add(new Button(SkipLocation) { text = _localizationManager.GetText("location.skip") });
            }

            _eventPanel.Show(_randomEvent, buttons
            );
        }

        private void HandleOption(Player.Player factionUnit,  RandomEventOption option)
        {
            switch (option)
            {
                case ResourceOption resourceOption:
                    factionUnit.worldState.playerFaction.AddResources(resourceOption.costs);
                    factionUnit.worldState.gameTime.hour += resourceOption.hour;
                    break;
                case LevelOption levelOption:
                    GameManager.Instance.SwitchToScene(levelOption.scene);
                    break;
            }
            
            _isHandled = true;
            _eventPanel.Hide();
        }

        private Button CreateButton(RandomEventOption option, Player.Player factionUnit)
        {
            var button = new Button(() => HandleOption(factionUnit, option))
            {
                text = option.optionName,
                tooltip = option.tooltipText
            };

            if (option is not ResourceOption resourceOption) return button;
            
            if (!factionUnit.worldState.playerFaction.CanPay(resourceOption.costs))
            {
                button.SetEnabled(false);
            }

            return  button;
        }
        
        private void SkipLocation()
        {
            _eventPanel.Hide();
        }
    }
}