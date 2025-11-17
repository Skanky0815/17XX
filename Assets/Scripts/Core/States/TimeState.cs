namespace Core.States
{
    [System.Serializable]
    public class TimeState
    {
        public int day;
        public int hour;
        public int minute;

        public bool IsNotSaved()
        {
            return 0 == day || 0 == hour || 0 == minute; 
        }
    }
}