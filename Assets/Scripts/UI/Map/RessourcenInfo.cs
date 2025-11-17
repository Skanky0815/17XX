using System.Collections.Generic;
using Core;
using Map;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

namespace UI.Map
{
    public class ResourcenInfo : MonoBehaviour
    {
        private Label _goldLabel;
        private Label _foodLabel;
        private Label _materialLabel;
        private Label _populationLabel;

        private RessouceCountTooltipElement _tooltipElement;

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

            _tooltipElement = new RessouceCountTooltipElement();
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
                    infoList.Add((region.RegionInfo.Name, region.RegionInfo.Gold));
                    totalValue += region.RegionInfo.Gold;
                }

                _tooltipElement.Show(
                    evt.mousePosition,
                    _localizationManager.GetText("tooltip.resources.gold_description"),
                    _localizationManager.GetText("tooltip.resources.gold_value"),
                    totalValue,
                    infoList
                );
            });
            _goldLabel.parent.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                _tooltipElement.Hide();
            });
            _materialLabel.parent.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var totalValue = 0;
                List<(string, int)> infoList = new();
                foreach (var region in FactionManager.PlayerFaction.Regions.Values)
                {
                    infoList.Add((region.RegionInfo.Name, region.RegionInfo.Material));
                    totalValue += region.RegionInfo.Material;
                }

                _tooltipElement.Show(
                    evt.mousePosition,
                    _localizationManager.GetText("tooltip.resources.material_description"),
                    _localizationManager.GetText("tooltip.resources.material_value"),
                    totalValue,
                    infoList
                );
            });
            _materialLabel.parent.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                _tooltipElement.Hide();
            });
            _foodLabel.parent.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var totalValue = 0;
                List<(string, int)> infoList = new();
                foreach (var region in FactionManager.PlayerFaction.Regions.Values)
                {
                    infoList.Add((region.RegionInfo.Name, region.RegionInfo.Food));
                    totalValue += region.RegionInfo.Food;
                }

                _tooltipElement.Show(
                    evt.mousePosition,
                    _localizationManager.GetText("tooltip.resources.food_description"),
                    _localizationManager.GetText("tooltip.resources.food_value"),
                    totalValue,
                    infoList
                );
            });
            _foodLabel.parent.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                _tooltipElement.Hide();
            });
            _populationLabel.parent.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var totalValue = 0;
                List<(string, int)> infoList = new();
                foreach (var region in FactionManager.PlayerFaction.Regions.Values)
                {
                    infoList.Add((region.RegionInfo.Name, region.RegionInfo.Population));
                    totalValue += region.RegionInfo.Population;
                }

                _tooltipElement.Show(
                    evt.mousePosition,
                    _localizationManager.GetText("tooltip.resources.population_description"),
                    _localizationManager.GetText("tooltip.resources.population_Value"),
                    totalValue,
                    infoList
                );
            });
            _populationLabel.parent.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                _tooltipElement.Hide();
            });
        }

        private void Update()
        {
            if (FactionManager.PlayerFaction == null) return;

            UpdateResouceCounter(_goldLabel, FactionManager.PlayerFaction.Gold);
            UpdateResouceCounter(_foodLabel, FactionManager.PlayerFaction.Food);
            UpdateResouceCounter(_materialLabel, FactionManager.PlayerFaction.Material);
            UpdateResouceCounter(_populationLabel, FactionManager.PlayerFaction.Population);
        }

        private void UpdateResouceCounter(Label label, int value)
        {
            label.text = value.ToString();
            label.style.color = value > 0 ? Color.darkGreen : Color.darkRed;
        }
    }
}