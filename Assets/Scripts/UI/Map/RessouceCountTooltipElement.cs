using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Map
{
    public class RessourceCountTooltipElement : VisualElement
    {
        private readonly Label _descriptionLabel;
        private readonly Label _summeryLabel;
        private readonly Label _summeryValueLabel;
        private readonly VisualElement _infoList;

        public RessourceCountTooltipElement()
        {
            var asset = Resources.Load<VisualTreeAsset>("UI/RessourceCountTooltipElement");
            asset.CloneTree(this);
            style.display = DisplayStyle.None;

            _descriptionLabel = this.Q<Label>("Description");
            _summeryLabel = this.Q<Label>("Summery");
            _summeryValueLabel = this.Q<Label>("SummeryValue");
            _infoList = this.Q<VisualElement>("InfoList");
        }

        public void Show(Vector2 position, string description, string summery, int summeryValue, List<(string, int)> infos)
        {
            _descriptionLabel.text = description;

            foreach (var (regionName, value) in infos)
            {
                var visualElement = new VisualElement();
                visualElement.AddToClassList("infoline");
                var nameLabel = new Label
                {
                    text = regionName
                };
                var valueLabel = new Label
                {
                    text = value.ToString()
                };

                visualElement.Add(nameLabel);
                visualElement.Add(valueLabel);

                _infoList.Add(visualElement);
            }
            _summeryLabel.text = summery;
            _summeryValueLabel.text = summeryValue.ToString();
            _summeryValueLabel.style.color = 0 < summeryValue ? Color.darkGreen : Color.darkRed;

            style.top = position.y + 5;
            style.left = position.x + 5;
            style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            style.display = DisplayStyle.None;
            _infoList.Clear();
        }
    }
}