using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Assets.Scripts.Map.Objects;
using Map;
using UI.Elements;

namespace UI.Map
{
    public class RegionInfo : MonoBehaviour
    {
        private VisualElement _regionInfo;
        private PanelHeader _panelHeader;
        private Label _descriptionLabel;
        private VisualElement _ambientImage;

        private Dictionary<string, Texture2D> _ambientImageChach = new();

        private void Start()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            _regionInfo = root.Q<VisualElement>("RegionInfo");
            _panelHeader = _regionInfo.Q<PanelHeader>("PanelHeader");
            _descriptionLabel = _regionInfo.Q<Label>("Description");
            _ambientImage = _regionInfo.Q<VisualElement>("AmbientImage");

            _panelHeader.RegisterButtonCallback(Hide);

            Hide();
        }

        public void Show(Region.Id regionId)
        {
            var region = RegionManager.GetRegion(regionId);

            _panelHeader.SetContent(region.RegionInfo.Name, "X", region.Owner?.Icon);
            _descriptionLabel.text = region.RegionInfo.Description;
            _ambientImage.style.backgroundImage = new StyleBackground(GetAmbientImage($"Map/Regions/{region.RegionInfo.AmbientImage}"));

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
