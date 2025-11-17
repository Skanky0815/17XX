using System.Collections.Generic;
using Core.States;
using Map.Serializables;
using UnityEngine;

namespace Map.Objects
{
    public class Faction
    {
        public int Gold { get; private set; }
        public int Food { get; private set; }
        public int Material { get; private set; }
        public int Population { get; private set; }

        public readonly Dictionary<Region.Id, Region> Regions = new();

        public readonly Id FactionId;

        public readonly FactionInfo FactionInfo;

        public readonly Texture2D Icon;

        public Faction(Id factionId, FactionInfo factionInfo, Texture2D icon)
        {
            FactionId = factionId;
            FactionInfo = factionInfo;
            Icon = icon;
        }

        public void AddResources(int gold, int food, int material, int population)
        {
            Gold += gold;
            Food += food;
            Material += material;
            Population += population;
        }

        public FactionState Save()
        {
            return new FactionState
            {
                factionId = FactionId,
                gold = Gold,
                food = Food,
                material = Material,
                population = Population
            };
        }

        public enum Id
        {
            NEUTRAL,
            DWARF,
            ORC
        } 
    }
}