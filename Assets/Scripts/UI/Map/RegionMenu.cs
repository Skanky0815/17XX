using UnityEngine.UIElements;
using UnityEngine;
using Map;
using Map.Objects;
using UI.Elements;

namespace UI.Map
{
    public class RegionMenu : MonoBehaviour
    {
        private VisualElement _regionMenu;
        private PanelHeader _panelHeader;
        private RegionInfo _regionInfo;
        private Region _currentRegion;

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

            _panelHeader.RegisterButtonCallback(ShowRegionInfo);

            Hide();
        }

        public void Show(Region region)
        {
            _regionInfo.Hide();
            
            _currentRegion = region;

            _panelHeader.SetContent(region.name, "?", region.owner?.icon);
            _goldLabel.text = region.gold.ToString();
            _foodLabel.text = region.food.ToString();
            _materialLabel.text = region.material.ToString();
            _populationLabel.text = region.population.ToString();

            _regionMenu.style.visibility = Visibility.Visible;
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
