using System;
using System.Collections.Generic;
using Core;
using Map;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Map
{
    public class ResourcesInfo : MonoBehaviour
    {
        private Label _goldLabel;
        private Label _foodLabel;
        private Label _materialLabel;
        private Label _populationLabel;

        private RessourceCountTooltipElement _tooltipElement;

        private LocalizationManager _localizationManager;

        private void Awake()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            _localizationManager = LocalizationManager.Instance;

            var factionStatsPanel = root.Q<VisualElement>("FactionStats");
            _goldLabel = factionStatsPanel.Q<Label>("Gold");
            _foodLabel = factionStatsPanel.Q<Label>("Food");
            _materialLabel = factionStatsPanel.Q<Label>("Material");
            _populationLabel = factionStatsPanel.Q<Label>("Population");

            _tooltipElement = new RessourceCountTooltipElement();
            root.Add(_tooltipElement);
        }

        private void Start()
        {
            _goldLabel.parent.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var totalValue = 0;
                List<(string, int)> infoList = new();
                foreach (var region in FactionManager.PlayerFaction.Regions.Values)
                {
                    infoList.Add((region.RegionInfo.name, region.RegionInfo.gold));
                    totalValue += region.RegionInfo.gold;
                }

                _tooltipElement.Show(
                    evt.mousePosition,
                    _localizationManager.GetText("tooltip.resources.gold_description"),
                    _localizationManager.GetText("tooltip.resources.gold_value"),
                    totalValue,
                    infoList
                );
            });
            _goldLabel.parent.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                _tooltipElement.Hide();
            });
            _materialLabel.parent.RegisterCallback<MouseEnterEvent>(evt =>
            {
                if (evt == null) throw new ArgumentNullException(nameof(evt));
                var totalValue = 0;
                List<(string, int)> infoList = new();
                foreach (var region in FactionManager.PlayerFaction.Regions.Values)
                {
                    infoList.Add((region.RegionInfo.name, region.RegionInfo.material));
                    totalValue += region.RegionInfo.material;
                }

                _tooltipElement.Show(
                    evt.mousePosition,
                    _localizationManager.GetText("tooltip.resources.material_description"),
                    _localizationManager.GetText("tooltip.resources.material_value"),
                    totalValue,
                    infoList
                );
            });
            _materialLabel.parent.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                _tooltipElement.Hide();
            });
            _foodLabel.parent.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var totalValue = 0;
                List<(string, int)> infoList = new();
                foreach (var region in FactionManager.PlayerFaction.Regions.Values)
                {
                    infoList.Add((region.RegionInfo.name, region.RegionInfo.food));
                    totalValue += region.RegionInfo.food;
                }

                _tooltipElement.Show(
                    evt.mousePosition,
                    _localizationManager.GetText("tooltip.resources.food_description"),
                    _localizationManager.GetText("tooltip.resources.food_value"),
                    totalValue,
                    infoList
                );
            });
            _foodLabel.parent.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                _tooltipElement.Hide();
            });
            _populationLabel.parent.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var totalValue = 0;
                List<(string, int)> infoList = new();
                foreach (var region in FactionManager.PlayerFaction.Regions.Values)
                {
                    infoList.Add((region.RegionInfo.name, region.RegionInfo.population));
                    totalValue += region.RegionInfo.population;
                }

                _tooltipElement.Show(
                    evt.mousePosition,
                    _localizationManager.GetText("tooltip.resources.population_description"),
                    _localizationManager.GetText("tooltip.resources.population_Value"),
                    totalValue,
                    infoList
                );
            });
            _populationLabel.parent.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                _tooltipElement.Hide();
            });
        }

        private void Update()
        {
            if (FactionManager.PlayerFaction == null) return;

            UpdateResourceCounter(_goldLabel, FactionManager.PlayerFaction.Gold);
            UpdateResourceCounter(_foodLabel, FactionManager.PlayerFaction.Food);
            UpdateResourceCounter(_materialLabel, FactionManager.PlayerFaction.Material);
            UpdateResourceCounter(_populationLabel, FactionManager.PlayerFaction.Population);
        }

        private static void UpdateResourceCounter(Label label, int value)
        {
            label.text = value.ToString();
            label.style.color = value > 0 ? Color.darkGreen : Color.darkRed;
        }
    }
}