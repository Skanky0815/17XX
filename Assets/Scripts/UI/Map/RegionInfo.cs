using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Assets.Scripts.Map.Objects;
using Map;

namespace Ui.Map
{
    public class RegionInfo : MonoBehaviour
    {
        private VisualElement _regionInfo;
        private Label _nameLabel;
        private Label _descriptionLabel;
        private Button _closeButton;
        private VisualElement _ambientImage;
        private VisualElement _factionIcon;

        private Dictionary<string, Texture2D> _ambientImageChach = new();

        private void Start()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            _regionInfo = root.Q<VisualElement>("RegionInfo");
            _nameLabel = _regionInfo.Q<Label>("Name");
            _closeButton = _regionInfo.Q<Button>("CloseButton");
            _descriptionLabel = _regionInfo.Q<Label>("Description");
            _ambientImage = _regionInfo.Q<VisualElement>("AmbientImage");
            _factionIcon = _regionInfo.Q<VisualElement>("FactionIcon");

            _closeButton.clicked += Hide;

            Hide();
        }

        public void Show(Region.Id regionId)
        {
            var region = RegionManager.GetRegion(regionId);

            _nameLabel.text = region.RegionInfo.Name;
            _descriptionLabel.text = region.RegionInfo.Description;
            _ambientImage.style.backgroundImage = new StyleBackground(GetAmbientImage($"Map/Regions/{region.RegionInfo.AmbientImage}"));

            if (region.Owner != null)
            {
                _factionIcon.style.backgroundImage = new StyleBackground(region.Owner.Icon);
            }
            else
            {
                _factionIcon.style.backgroundImage = null;
            }

            _regionInfo.style.visibility = Visibility.Visible;
        }

        public void Hide()
        {
            if (_regionInfo == null) return;

            _regionInfo.style.visibility = Visibility.Hidden;
        }

        private Texture2D GetAmbientImage(string imageName)
        {
            if (_ambientImageChach.TryGetValue(imageName, out var chahedImage)) return chahedImage;

            var ambientImage = Resources.Load<Texture2D>(imageName);
            _ambientImageChach[imageName] = ambientImage;

            return ambientImage;
        }
    }
}
