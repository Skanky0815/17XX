using UnityEngine.UIElements;
using UnityEngine;
using Map.Objects;
using UI.Elements;
using UI.Map.Elements;
using Resources = UnityEngine.Resources;

namespace UI.Map
{
    public class RegionMenu : MonoBehaviour
    {
        private VisualElement _regionMenu;
        private PanelHeader _panelHeader;
        private RegionInfo _regionInfo;
        private Region _currentRegion;
        private VisualElement _buildSlots;

        private Label _goldLabel;
        private Label _foodLabel;
        private Label _materialLabel;
        private Label _populationLabel;

        private void Start()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            _regionInfo = gameObject.GetComponent<RegionInfo>();
            _regionMenu = root.Q<VisualElement>("RegionMenu");
            _panelHeader = _regionMenu.Q<PanelHeader>("PanelHeader");
            _goldLabel = _regionMenu.Q<Label>("Gold");
            _foodLabel = _regionMenu.Q<Label>("Food");
            _materialLabel = _regionMenu.Q<Label>("Material");
            _populationLabel = _regionMenu.Q<Label>("Population");
            _buildSlots = _regionMenu.Q<VisualElement>("BuildSlots");

            _panelHeader.RegisterButtonCallback(ShowRegionInfo);

            Hide();
        }

        public void Show(Region region)
        {
            _regionInfo.Hide();
            _currentRegion = region;
            _buildSlots.Clear();

            _panelHeader.SetContent(region.regionName, "?", region.owner?.icon);
            _goldLabel.text = region.income.gold.ToString();
            _foodLabel.text = region.income.food.ToString();
            _materialLabel.text = region.income.material.ToString();
            _populationLabel.text = region.income.population.ToString();
            
            if (region.owner)
            {
                _regionMenu.style.bottom = 0;
                var accordion = new Accordion();
                for (var slotIndex = 0; slotIndex < _currentRegion.buildingSlots; slotIndex++)
                {
                    var buildingSlotButton = new Button
                    {
                        text = "Bebauen",
                        style = {
                            backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Map/Buildings/slot_icon"))
                        }
                    };
                    var content = new VisualElement
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Row
                        }
                    };
                    for (var buildingIndex = 0; buildingIndex < region.buildings.Count; buildingIndex++)
                    {
                        var building = region.buildings[buildingIndex];

                        var index = slotIndex;
                        content.Add(new BuildMenuButton(building, _ => Build(building, index)));
                    }
                    var section = new AccordionSection(buildingSlotButton, content);
                    accordion.AddSection(section);
                }
                _buildSlots.Add(accordion);
            }
            else
            {
                _regionMenu.style.bottom = -150;
            }
            
            _regionMenu.style.visibility = Visibility.Visible;
        }

        private void Build(Building building, int slotIndex)
        {
            Debug.Log("Building " + building.buildingName + " on slot " + slotIndex);
        }

        private void ShowRegionInfo()
        {
            _regionInfo.Show(_currentRegion);
        }

        public void Hide()
        {
            _regionMenu.style.visibility = Visibility.Hidden;
            _regionInfo.Hide();
        }
    }
}
