using UnityEngine;
using UnityEngine.UIElements;
using Resources = UnityEngine.Resources;

namespace UI.Map
{
    [UxmlElement]
    public partial class FactionPanel : VisualElement
    {
        public FactionPanel()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("UI/Map/FactionPanelElement"));
            AddToClassList("factionPanel");
            name = "FactionPanel";
        }

        public void SetFaction(Texture2D factionLogo)
        {
            style.backgroundImage = new StyleBackground(factionLogo);
        }
    }
}