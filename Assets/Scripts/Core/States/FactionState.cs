using Assets.Scripts.Map.Objects;

namespace Assets.Scripts.Core.States
{
    [System.Serializable]
    public class FactionState
    {
        public Faction.Id FactionId;
        public int Gold;
        public int Food;
        public int Material;
        public int Population;
    }
}