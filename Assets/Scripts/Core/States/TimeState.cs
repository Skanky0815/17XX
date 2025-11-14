namespace Assets.Scripts.Core.States
{
    [System.Serializable]
    public class TimeState
    {
        public int Day;
        public int Houre;
        public int Minute;

        public bool IsNotSaved()
        {
            return 0 == Day || 0 == Houre || 0 == Minute; 
        }
    }
}