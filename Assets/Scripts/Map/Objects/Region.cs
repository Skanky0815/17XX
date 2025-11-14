namespace Assets.Scripts.Map.Objects
{
    public class Region
    {
        public readonly Id RegionId;

        public readonly RegionInfo RegionInfo;

        public Faction Owner { get; private set; }

        public Region(Id regionId, RegionInfo regionInfo, Faction owner)
        {
            RegionId = regionId;
            RegionInfo = regionInfo;
            Owner = owner;
            
            owner.Regions.Add(RegionId, this);
        }

        public void ChangeOwner(Faction faction)
        {
            Owner?.Regions.Remove(RegionId);

            faction.Regions.Add(RegionId, this);
            Owner = faction;
        }

        public void AggregateDailyResources()
        {
            Owner?.AddResources(RegionInfo.Gold, RegionInfo.Food, RegionInfo.Population, RegionInfo.Population);
        }

        public enum Id
        {
            REGION_01,
            REGION_02,
            REGION_03,
            REGION_04,
            REGION_05,
            REGION_06,
            REGION_07,
            REGION_08,
            REGION_09,
            REGION_10,
            REGION_11,
            REGION_12,
            REGION_13,
            REGION_14
        }
    }
}
