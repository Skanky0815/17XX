namespace Map.Objects
{
    [System.Serializable]
    public class Resources
    {
        public int gold;
        public int material;
        public int food;
        public int population;

        public static Resources operator +(Resources a, Resources b)
        {
            a.gold += b.gold;
            a.material += b.material;
            a.food += b.food;
            a.population += b.population;
            
            return a;
        }
    }
}