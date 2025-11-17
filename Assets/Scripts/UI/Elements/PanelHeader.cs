
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Elements
{
    [UxmlElement]
    public partial class PanelHeader : VisualElement
    {
        private readonly VisualElement _leftField;
        private readonly Label _textLabel;
        private readonly Button _rightButton;

        public PanelHeader()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("UI/PanelHeader"));
            AddToClassList("panelHeader");
            name = "PanelHeader";

            _leftField = new VisualElement { name = "LeftField" };
            _textLabel = new Label("Label") { name = "Text" };
            _rightButton = new Button { name = "RightButton", text = "Button" };

            Add(_leftField);
            Add(_textLabel);
            Add(_rightButton);
        }

        public void RegisterButtonCallback(System.Action callback)
        {
            _rightButton.clicked += callback;
        }

        public void SetContent(string text, string buttonLabel, Texture2D icon)
        {
            _textLabel.text = text;
            if (icon != null)
            {
                _leftField.style.backgroundImage = new StyleBackground(icon);
            }
            _rightButton.text = buttonLabel;
        }

        public void SetContent(string text, string buttonLabel)
        {
            SetContent(text, buttonLabel, null);
        }
    }
}