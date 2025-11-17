using System;
using UnityEngine;

namespace Map.Objects
{
    [CreateAssetMenu(menuName = "Game/GameTime")]
    public class GameTime : ScriptableObject
    {
        public int day = 1;
        public int hour = 12;
        public int minute = 1;

        private const float RealSecondsPerIngameDay = 60f;
        private static float TimeScale => 1440f / RealSecondsPerIngameDay;

        private float _minuteBuffer;

        public void Advance(float deltaTime)
        {
            _minuteBuffer += deltaTime * TimeScale;

            while (_minuteBuffer >= 1f)
            {
                _minuteBuffer -= 1f;
                minute++;

                OnNewMinute?.Invoke(hour, minute);

                if (minute < 60) continue;
                minute = 0;
                hour++;
                OnNewHour?.Invoke(hour);

                if (hour < 24) continue;
                hour = 0;
                day++;
                OnNewDay?.Invoke(day);
            }
        }

        public event Action<int> OnNewDay;

        public event Action<int> OnNewHour;

        public event Action<int, int> OnNewMinute;
    }
}