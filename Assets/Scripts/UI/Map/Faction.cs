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
            var factionFlag = GameManager.Instance.mapWorldState.playerFaction.flag;
            _factionPanel.SetFaction(factionFlag);
        }
    }
}