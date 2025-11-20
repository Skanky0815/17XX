using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace UI.Elements
{
    [UxmlElement]
    public partial class EventPanel : VisualElement
    {
        private readonly PanelHeader _panelHeader;
        private readonly Image _image;
        private readonly Label _textLabel;
        private readonly VisualElement _actionArea;

        public EventPanel()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("UI/Elements/EventPanel"));
            AddToClassList("panel");
            AddToClassList("eventPanel");

            name = "EventPanel";

            _panelHeader = new PanelHeader();
            _image = new Image { name = "EventImage" };
            _textLabel = new Label("Event Text") { name = "EventText" };
            _actionArea = new VisualElement { name = "EventArea" };
            
            Add(_panelHeader);
            Add(_image);
            Add(_textLabel);
            Add(_actionArea);
        }

        public void Show(string title, string text, Texture2D image, List<Button> buttons, bool disableCloseButton = false)
        {
            _textLabel.text = text;
            if (image != null)
            {
                _image.image = image;
                _image.style.display = DisplayStyle.Flex;
            }
            else
            {
                _image.style.display = DisplayStyle.None;
            }

            if (disableCloseButton)
            {
                _panelHeader.SetContent(title, "");
            }
            else
            {
                _panelHeader.SetContent(title, "X");
                _panelHeader.RegisterButtonCallback(Hide);
            }

            _actionArea.Clear();
            foreach (var button in buttons)
            {
                _actionArea.Add(button);
            }

            style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
           style.display = DisplayStyle.None;
        }
    }
}
