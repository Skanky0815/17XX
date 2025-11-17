using System;

namespace Map.Objects
{
    public class GameTime
    {
        public int Day;
        public int Hour;
        public int Minute;

        private const float RealSecondsPerIngameDay = 60f;
        private static float TimeScale => 1440f / RealSecondsPerIngameDay;

        private float _minuteBuffer;

        public GameTime(int day = 1, int hour = 12, int minute = 1)
        {
            Day = day;
            Hour = hour;
            Minute = minute;
        }

        public void Advance(float deltaTime)
        {
            _minuteBuffer += deltaTime * TimeScale;

            while (_minuteBuffer >= 1f)
            {
                _minuteBuffer -= 1f;
                Minute++;

                OnNewMinute?.Invoke(Hour, Minute);

                if (Minute < 60) continue;
                Minute = 0;
                Hour++;
                OnNewHour?.Invoke(Hour);

                if (Hour < 24) continue;
                Hour = 0;
                Day++;
                OnNewDay?.Invoke(Day);
            }
        }

        public event Action<int> OnNewDay;

        public event Action<int> OnNewHour;

        public event Action<int, int> OnNewMinute;
    }
}