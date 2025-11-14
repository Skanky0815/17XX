using UnityEngine;
using UnityEngine.UIElements;

namespace Ui.Map
{
    public class FactionPanelElement : VisualElement
    {
        public FactionPanelElement()
        {
            var asset = Resources.Load<VisualTreeAsset>("UI/Map/FactionPanelElement");
            asset.CloneTree(this);
        }

        public void ShowFactionPanel(Texture2D factionLogo, string factionName)
        {
            style.backgroundImage = factionLogo;
        }
    }
}