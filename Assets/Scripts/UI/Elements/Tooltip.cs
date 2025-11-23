using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Elements
{
    [UxmlElement]
    public partial class Tooltip : VisualElement
    {
        private readonly Label _text;
        
        public Tooltip()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("UI/Elements/Tooltip"));
            AddToClassList("tooltip");
            AddToClassList("panel");
            
            name = "Tooltip";
            
            _text = new Label();
            
            Add(_text);
        }

        public void Show(string text, Vector2 position)
        {
            style.top = position.y;
            style.left = position.x;
            _text.text = text;
            style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            style.display = DisplayStyle.None;
        }
    }
}