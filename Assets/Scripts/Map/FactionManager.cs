using System.Collections.Generic;
using System.Linq;
using Core;
using Core.States;
using Map.Objects;
using Map.Serializables;
using Newtonsoft.Json;
using UnityEngine;

namespace Map
{
    public static class FactionManager
    {
        private static Player.Player _player;

        public static Faction PlayerFaction;

        public static Dictionary<Faction.Id, Faction> Factions = new();

        public static Faction GetFaction(Faction.Id factionId)
        {
            return Factions[factionId];
        }

        public static void Initialize(Player.Player playerRef, MapWorldState worldState)
        {
            _player = playerRef;
            return;
            if (Factions.Count == 0)
            {
                var (factionInfos, factionIcons) = LoadAssets();
                foreach ((var factionId, var factionInfo) in factionInfos)
                {
                    Texture2D texture = null;
                    if (factionId != Faction.Id.NEUTRAL)
                    {
                        var icon = factionIcons.FirstOrDefault(s => s.name == factionInfo.icon);
                        texture = SpriteConverter.ToTexture(icon);
                    }

                    var faction = new Faction(factionId, factionInfo, texture);

                    Factions[factionId] = faction;
                }
            }

            _player.Faction = Factions[worldState.playerFactionId];
            PlayerFaction = Factions[worldState.playerFactionId];
        }

        public static void Save(MapWorldState worldState)
        {
            worldState.factions.Clear();
            foreach (var faction in Factions.Values)
            {
                worldState.factions.Add(faction.Save());
            }
        }

        public static void Load(MapWorldState worldState)
        {
            return;
            if (worldState.HasNoFactions()) return;

            Factions.Clear();

            var (factionInfos, factionIcons) = LoadAssets();
            foreach (var factionState in worldState.factions)
            {
                var factionInfo = factionInfos[factionState.factionId];
                Texture2D texture = null;
                if (factionState.factionId != Faction.Id.NEUTRAL)
                {
                    var icon = factionIcons.FirstOrDefault(s => s.name == factionInfo.icon);
                    texture = SpriteConverter.ToTexture(icon);
                }

                var faction = new Faction(factionState.factionId, factionInfo, texture);
                faction.AddResources(factionState.gold, factionState.food, factionState.material, factionState.population);

                Factions[faction.FactionId] = faction;
            }

            if (Factions.TryGetValue(worldState.playerFactionId, out var playerFaction))
            {
                PlayerFaction = playerFaction;
                _player.Faction = playerFaction;
            }
        }

        private static (Dictionary<Faction.Id, FactionInfo>, Sprite[]) LoadAssets()
        {
            var textAsset = Resources.Load<TextAsset>("FactionData");
            var factionInfos = JsonConvert.DeserializeObject<Dictionary<Faction.Id, FactionInfo>>(textAsset.text);
            var factionIcons = Resources.LoadAll<Sprite>("faction_icons");

            return (factionInfos, factionIcons);
        }
    }
}