using UnityEngine;

namespace Map.Controller
{
    public class DayNightController : MonoBehaviour
    {
        public GameTimeController GameTimeManager;

        private Light _light;

        private void Start()
        {
            _light = gameObject.GetComponent<Light>();


            GameTimeManager.CurrentTime.OnNewMinute += OnNewMinute;
        }

        private void OnNewMinute(int hour, int minute)
        {
            float timeInHours = hour + minute / 60f;

            UpdateLighting(timeInHours);
            UpdateLightColor(timeInHours);
        }

        private void UpdateLighting(float timeInHours)
        {
            // Sonne: 0 Uhr = unter dem Horizont, 12 Uhr = Zenit
            float sunAngle = Mathf.Lerp(-90f, 270f, timeInHours / 24f);
            gameObject.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);


            // Normalisiere X-Rotation in sinnvollen Bereich (z. B. 0° bis 90°)
            float normalizedX = sunAngle > 180f ? sunAngle - 360f : sunAngle; // z. B. -90° bis +90°

            float fadeStart = 10f;   // Schatten beginnen zu schwächen
            float fadeEnd = 0.5f;    // Schatten sind komplett weg

            float shadowStrength = Mathf.InverseLerp(fadeEnd, fadeStart, Mathf.Abs(normalizedX));
            _light.shadowStrength = Mathf.Clamp01(shadowStrength);



            // Mond: gegenüberliegend
            //    float moonAngle = sunAngle + 180f;
            //    moonLight.transform.rotation = Quaternion.Euler(moonAngle, 170f, 0f);
        }

        public void UpdateLightColor(float timeInHours)
        {
            // Farbverlauf: Morgen → Tag → Abend → Nacht
            Color morning = new(1f, 0.8f, 0.6f);
            Color day = Color.white;
            Color evening = new(1f, 0.6f, 0.4f);
            Color night = new(0.2f, 0.3f, 0.5f);

            Color targetColor;
            float intensity;


            float t;
            if (timeInHours >= 3f && timeInHours < 6f) // Morgen
            {
                t = Mathf.InverseLerp(3f, 6f, timeInHours);
                targetColor = Color.Lerp(morning, day, t);
                intensity = Mathf.Lerp(0.5f, 1f, t);
            }
            else if (timeInHours >= 6f && timeInHours < 18f) // Tag
            {
                t = Mathf.InverseLerp(6f, 18f, timeInHours);
                targetColor = Color.Lerp(day, evening, t);
                intensity = 1f;
            }
            else if (timeInHours >= 18f && timeInHours < 21f) // Abend
            {
                t = Mathf.InverseLerp(18f, 21f, timeInHours);
                targetColor = Color.Lerp(evening, night, t);
                intensity = Mathf.Lerp(1f, 0.5f, t);
            }
            else if (timeInHours >= 21f && timeInHours <= 24f) // Nacht vor Mitternacht
            {
                t = Mathf.InverseLerp(21f, 24f, timeInHours);
                targetColor = Color.Lerp(night, night, t); // konstant oder leicht dunkler
                intensity = Mathf.Lerp(0.5f, 0.0f, t);     // konstant oder leicht dunkler
            }
            else // Nacht nach Mitternacht
            {
                t = Mathf.InverseLerp(0f, 3f, timeInHours);
                targetColor = Color.Lerp(night, morning, t); // Übergang Richtung Morgen
                intensity = Mathf.Lerp(0.0f, 0.5f, t);       // heller werdend
            }

            _light.color = targetColor;
            _light.intensity = intensity;
            _light.shadowStrength = Mathf.Clamp01(intensity);

            // Mondlicht aktivieren bei Nacht
            //moonLight.enabled = hour < 6f || hour >= 18f;
            //sunLight.enabled = !moonLight.enabled;
        }
    }
}
