using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Map
{
    public class Faction : MonoBehaviour
    {
        private FactionPanel _factionPanel;

        private void Awake()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            _factionPanel = root.Q<FactionPanel>("FactionPanel");
        }

        private void Start()
        {
            var factionId = GameManager.Instance.mapWorldState.playerFactionId;
            var factionFlag = Resources.Load<Texture2D>($"{factionId.ToString().ToLowerInvariant()}_flag");
            _factionPanel.SetFaction(factionFlag);
        }
    }
}