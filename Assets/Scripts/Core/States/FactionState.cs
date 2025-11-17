using Map.Objects;

namespace Core.States
{
    [System.Serializable]
    public class FactionState
    {
        public Faction.Id factionId;
        public int gold;
        public int food;
        public int material;
        public int population;
    }
}