using System.Collections.Generic;
using Map.Objects;
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
        private readonly Tooltip _tooltip;

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
            _tooltip = new Tooltip();
            
            Add(_panelHeader);
            Add(_image);
            Add(_textLabel);
            Add(_actionArea);
            Add(_tooltip);
        }

        public void Show(Location location, List<Button> buttons)
        {
            _textLabel.text = location.welcomeText;
            SetAbientImage(location.ambientImage);
            
            _panelHeader.SetContent(location.locationName, "X");
            _panelHeader.RegisterButtonCallback(Hide);

            _actionArea.Clear();
            for (var i = 0; i < buttons.Count; i++)
            {
                _actionArea.Add(buttons[i]);
            }

            style.display = DisplayStyle.Flex;
        }

        public void Show(RandomEvent randomEvent, List<Button> buttons)
        {
            _textLabel.text = randomEvent.description;
            SetAbientImage(randomEvent.ambientImage);

            if (randomEvent.isSkipable)
            {
                _panelHeader.SetContent(randomEvent.eventName, "X");
                _panelHeader.RegisterButtonCallback(Hide);
            }
            else
            {
                _panelHeader.SetContent(randomEvent.eventName, "");
            }

            _actionArea.Clear();
            for (var i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                if (button.tooltip != "")
                {
                    button.RegisterCallback<MouseEnterEvent>(evt =>
                    {
                        var position = this.WorldToLocal(evt.mousePosition);
                        _tooltip.Show(button.tooltip, position);
                    });
                    button.RegisterCallback<MouseLeaveEvent>(_ => _tooltip.Hide());
                }
                _actionArea.Add(button);
            }

            style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
           style.display = DisplayStyle.None;
        }

        private void SetAbientImage(Texture2D ambientImage)
        {
            if (ambientImage != null)
            {
                _image.image = ambientImage;
                _image.style.display = DisplayStyle.Flex;
            }
            else
            {
                _image.style.display = DisplayStyle.None;
            }
        }
    }
}
