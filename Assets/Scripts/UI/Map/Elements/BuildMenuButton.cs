using System;
using Map.Objects;
using UnityEngine.UIElements;

namespace UI.Map.Elements
{
    public class BuildMenuButton : Button
    {
        private readonly Building _building;
        private readonly Action<Building> _callback;
        
        public BuildMenuButton(Building building, Action<Building> callback)
        {
            _building = building;
            _callback = callback;
            
            style.backgroundImage = new StyleBackground(building.icon);
            text = building.buildingName;
           
            clicked += OnButtonClick;
        }

        public sealed override string text
        {
            get { return base.text; }
            set { base.text = value; }
        }

        private void OnButtonClick()
        {
            _callback?.Invoke(_building);
        }
    }
}