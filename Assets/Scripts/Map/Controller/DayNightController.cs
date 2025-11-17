using Map.Objects;
using UnityEngine;

namespace Map.Controller
{
    public class DayNightController : MonoBehaviour
    {
        public GameTime gameTime;

        private Light _light;

        private void Start()
        {
            _light = gameObject.GetComponent<Light>();
            
            gameTime.OnNewMinute += OnNewMinute;
        }

        private void OnDestroy()
        {
            gameTime.OnNewMinute -= OnNewMinute;
        }

        private void OnNewMinute(int hour, int minute)
        {
            var timeInHours = hour + minute / 60f;

            UpdateLighting(timeInHours);
            UpdateLightColor(timeInHours);
        }

        private void UpdateLighting(float timeInHours)
        {
            var sunAngle = Mathf.Lerp(-90f, 270f, timeInHours / 24f);
            gameObject.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
            
            var normalizedX = sunAngle > 180f ? sunAngle - 360f : sunAngle;
            const float fadeStart = 10f;
            const float fadeEnd = 0.5f;

            var shadowStrength = Mathf.InverseLerp(fadeEnd, fadeStart, Mathf.Abs(normalizedX));
            _light.shadowStrength = Mathf.Clamp01(shadowStrength);
        }

        private void UpdateLightColor(float timeInHours)
        {
            // Farbverlauf: Morgen → Tag → Abend → Nacht
            Color morning = new(1f, 0.8f, 0.6f);
            var day = Color.white;
            Color evening = new(1f, 0.6f, 0.4f);
            Color night = new(0.2f, 0.3f, 0.5f);

            Color targetColor;
            float intensity;


            float t;
            switch (timeInHours)
            {
                case >= 3f and < 6f:
                    t = Mathf.InverseLerp(3f, 6f, timeInHours);
                    targetColor = Color.Lerp(morning, day, t);
                    intensity = Mathf.Lerp(0.5f, 1f, t);
                    break;
                case >= 6f and < 18f:
                    t = Mathf.InverseLerp(6f, 18f, timeInHours);
                    targetColor = Color.Lerp(day, evening, t);
                    intensity = 1f;
                    break;
                case >= 18f and < 21f:
                    t = Mathf.InverseLerp(18f, 21f, timeInHours);
                    targetColor = Color.Lerp(evening, night, t);
                    intensity = Mathf.Lerp(1f, 0.5f, t);
                    break;
                case >= 21f and <= 24f:
                    t = Mathf.InverseLerp(21f, 24f, timeInHours);
                    targetColor = Color.Lerp(night, night, t);
                    intensity = Mathf.Lerp(0.5f, 0.0f, t);
                    break;
                default:
                    t = Mathf.InverseLerp(0f, 3f, timeInHours);
                    targetColor = Color.Lerp(night, morning, t);
                    intensity = Mathf.Lerp(0.0f, 0.5f, t);
                    break;
            }

            _light.color = targetColor;
            _light.intensity = intensity;
            _light.shadowStrength = Mathf.Clamp01(intensity);
        }
    }
}
