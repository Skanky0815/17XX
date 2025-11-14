using System.Threading.Tasks;
using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ui.Map
{
    public class Faction : MonoBehaviour
    {
        private Image _factionImage;

        private void Awake()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            var topBar = root.Q<VisualElement>("TopBar");
            var factionPanelElement = new FactionPanelElement();

            topBar.Insert(0, factionPanelElement);

            _factionImage = factionPanelElement.Q<Image>("FactionImage");
        }

        private void Start()
        {
            var factionId = GameManager.Instance.mapWorldState.playerFactionId;
            _factionImage.image = Resources.Load<Texture2D>($"{factionId.ToString().ToLowerInvariant()}_flag");
        }
    }
}