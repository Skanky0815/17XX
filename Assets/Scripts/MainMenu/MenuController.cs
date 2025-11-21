using Core;
using Core.States;
using Map.Objects;
using UnityEngine;
using UnityEngine.UIElements;

namespace MainMenu
{
    public class MenuController : MonoBehaviour
    {
        public MapWorldState worldState;
        private VisualElement _dwarfButton;
        private VisualElement _orcButton;
        private Button _quitButton;

        private void Awake()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            
            _dwarfButton = root.Q<VisualElement>("Dwarf");
            _orcButton = root.Q<VisualElement>("Orc");
            _quitButton = root.Q<Button>("QuitButton");
        }

        private void Start()
        {
            _dwarfButton.RegisterCallback<MouseDownEvent>(evt => StartGame(worldState.factions[0]));
            _orcButton.RegisterCallback<MouseDownEvent>(evt => StartGame(worldState.factions[1]));
            _quitButton.clicked += QuitGame;
        }

        private void StartGame(Faction faction)
        {
            worldState.playerFaction = faction;
            GameManager.Instance.SwitchToScene("MapScene");
        }

        private void QuitGame()
        {
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
