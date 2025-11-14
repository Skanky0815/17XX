using Assets.Scripts.Core.States;
using Assets.Scripts.Map.Objects;
using Core;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    public MapWorldState WorldState;
    private VisualElement _dwarfButton;
    private VisualElement _orcButton;

    private void Awake()
    {
        var root = gameObject.GetComponent<UIDocument>().rootVisualElement;

        _dwarfButton = root.Q<VisualElement>("Dwarf");
        _orcButton = root.Q<VisualElement>("Orc");
    }

    private void Start()
    {
        _dwarfButton.RegisterCallback<MouseDownEvent>(evt => StartGame(Faction.Id.DWARF));
        _orcButton.RegisterCallback<MouseDownEvent>(evt => StartGame(Faction.Id.ORC));
    }

    private void StartGame(Faction.Id factionId)
    {
        WorldState.playerFactionId = factionId;
        GameManager.Instance.SwitchToScene("MapScene");
    }
}
