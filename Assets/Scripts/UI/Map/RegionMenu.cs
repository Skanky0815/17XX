using UnityEngine.UIElements;
using UnityEngine;
using Assets.Scripts.Map.Objects;
using Map;

namespace Ui.Map
{
    public class RegionMenu : MonoBehaviour
    {
        private VisualElement _regionMenu;
        private Label _nameLabel;
        private Button _infoButton;
        private RegionInfo _regionInfo;
        private Region.Id _currentRegionId;

        private Label _goldLabel;
        private Label _foodLabel;
        private Label _materialLabel;
        private Label _populationLabel;

        private VisualElement _factionIcon;

        private void Start()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            _regionInfo = gameObject.GetComponent<RegionInfo>();
            _regionMenu = root.Q<VisualElement>("RegionMenu");
            _nameLabel = _regionMenu.Q<Label>("Name");
            _infoButton = _regionMenu.Q<Button>("InfoButton");
            _goldLabel = _regionMenu.Q<Label>("Gold");
            _foodLabel = _regionMenu.Q<Label>("Food");
            _materialLabel = _regionMenu.Q<Label>("Material");
            _populationLabel = _regionMenu.Q<Label>("Population");

            _factionIcon = _regionMenu.Q<VisualElement>("FactionIcon");

            _infoButton.clicked += ShowRegionInfo;

            Hide();
        }

        public void Show(Region.Id regionId)
        {
            _regionInfo.Hide();
            
            _currentRegionId = regionId;

            var region = RegionManager.GetRegion(regionId);

            _nameLabel.text = region.RegionInfo.Name;
            _goldLabel.text = region.RegionInfo.Gold.ToString();
            _foodLabel.text = region.RegionInfo.Food.ToString();
            _materialLabel.text = region.RegionInfo.Material.ToString();
            _populationLabel.text = region.RegionInfo.Population.ToString();

            if (region.Owner != null)
            {
                _factionIcon.style.backgroundImage = new StyleBackground(region.Owner.Icon);
            } else
            {
                _factionIcon.style.backgroundImage = null;
            }
            _regionMenu.style.visibility = Visibility.Visible;
        }

        public void ShowRegionInfo()
        {
            _regionInfo.Show(_currentRegionId);
        }

        public void Hide()
        {
            _regionMenu.style.visibility = Visibility.Hidden;
            _regionInfo.Hide();
        }
    }
}
