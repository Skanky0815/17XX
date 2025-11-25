using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UI.Elements
{
    public class Accordion : VisualElement
    {
        private readonly List<AccordionSection> _sections = new();

        public void AddSection(AccordionSection section)
        {
            style.flexDirection = FlexDirection.Row;
            
            _sections.Add(section);
            Add(section);

            section.header.clicked += () =>
            {
                for (var sectionIndex = 0; sectionIndex < _sections.Count; sectionIndex++)
                {
                    _sections[sectionIndex].SetActive(false);
                    
                    section.SetActive(true);
                }
            };
        }
    }

    public class AccordionSection : VisualElement
    {
        public readonly Button header;
        private readonly VisualElement _content;

        public AccordionSection(Button header, VisualElement content)
        {
            style.flexDirection = FlexDirection.Row;
            this.header = header;
            _content = content;
            _content.style.display = DisplayStyle.None;
            
            Add(header);
            Add(_content);
        }
        
        public void SetActive(bool active)
        {
            _content.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}