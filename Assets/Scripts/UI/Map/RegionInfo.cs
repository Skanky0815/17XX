using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Map;
using Map.Objects;
using UI.Elements;

namespace UI.Map
{
    public class RegionInfo : MonoBehaviour
    {
        private VisualElement _regionInfo;
        private PanelHeader _panelHeader;
        private Label _descriptionLabel;
        private VisualElement _ambientImage;

        private readonly Dictionary<string, Texture2D> _ambientImageChach = new();

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

        public void Show(Region region)
        {
            _panelHeader.SetContent(region.name, "X", region.owner?.icon);
            _descriptionLabel.text = region.description;
            _ambientImage.style.backgroundImage = new StyleBackground(region.ambientImage);

            _regionInfo.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            if (_regionInfo == null) return;

            _regionInfo.style.display = DisplayStyle.None;
        }
    }
}
