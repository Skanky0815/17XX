using System;

public class GameTime
{
    public int Day = 1;
    public int Hour = 12;
    public int Minute;

    public float RealSecondsPerIngameDay = 60f;
    public float TimeScale => 1440f / RealSecondsPerIngameDay;

    private float _minuteBuffer = 0f;

    public GameTime(int day = 1, int houre = 12, int minute = 1)
    {
        Day = day;
        Hour = houre;
        Minute = minute;

        OnNewMinute?.Invoke(Hour, Minute);
        OnNewHoure?.Invoke(Hour);
        OnNewDay?.Invoke(Day);
    }

    public void Advance(float deltaTime)
    {
        _minuteBuffer += deltaTime * TimeScale;

        while (_minuteBuffer >= 1f)
        {
            _minuteBuffer -= 1f;
            Minute++;

            OnNewMinute?.Invoke(Hour, Minute);

            if (Minute >= 60)
            {
                Minute = 0;
                Hour++;
                OnNewHoure?.Invoke(Hour);

                if (Hour >= 24)
                {
                    Hour = 0;
                    Day++;
                    OnNewDay?.Invoke(Day);
                }
            }
        }
    }

    public event Action<int> OnNewDay;

    public event Action<int> OnNewHoure;

    public event Action<int, int> OnNewMinute;
}
