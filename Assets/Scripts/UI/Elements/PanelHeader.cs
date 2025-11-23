
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
            styleSheets.Add(Resources.Load<StyleSheet>("UI/Elements/PanelHeader"));
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

        public void SetContent(string text, string buttonLabel, Texture2D icon = null)
        {
            _textLabel.text = text;
            _leftField.style.backgroundImage = icon != null ? new StyleBackground(icon) : new StyleBackground();

            if (buttonLabel == null)
            {
                _rightButton.SetEnabled(false);
            }
            else
            {
                _rightButton.text = buttonLabel;
                _rightButton.SetEnabled(true);
            }
        }
    }
}