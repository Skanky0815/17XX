using System.Collections;
using Core;
using Map.Controller;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace UI.Map
{
    public class Clock : MonoBehaviour
    {
        public GameTimeController gameTimeController;
        public Sprite[] timeSprites;
        
        private Label _dayLabel;
        private VisualElement _clockFront;
        private VisualElement _clockBack;
        private VisualElement _clockHandElement;

        private int _currentDay;
        private Texture2D[] _clockTextures;

        private Texture2D _currentClockImage;
        private Texture2D _nextClockImage;

        private string _text;

        private void Awake()
        {
            _text = LocalizationManager.Instance.GetText("clock.day");
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;


            _clockTextures = new Texture2D[timeSprites.Length];
            for (var i = 0; i < timeSprites.Length; i++)
            {
                _clockTextures[i] = SpriteConverter.ToTexture(timeSprites[i]);
            }

            var clockPanel = root.Q<VisualElement>("ClockPanel");

            _clockFront = clockPanel.Q<VisualElement>("ClockFront");
            _clockBack = clockPanel.Q<VisualElement>("ClockBack");
            _clockHandElement = clockPanel.Q<VisualElement>("ClockHand");
            _dayLabel = clockPanel.Q<Label>("Day");

            _clockBack.style.backgroundImage = new StyleBackground(_clockTextures[1]);
        }

        private void Start()
        {
            gameTimeController.CurrentTime.OnNewDay += OnNewDay;
            gameTimeController.CurrentTime.OnNewHour += OnNewHour;
        }

        private void OnDestroy()
        {
            gameTimeController.CurrentTime.OnNewDay -= OnNewDay;
            gameTimeController.CurrentTime.OnNewHour -= OnNewHour;
        }

        private void OnNewDay(int day)
        {
            _currentDay = day;
        }

        private void OnNewHour(int hour)
        {
            _dayLabel.text = string.Format(_text, _currentDay);

            var clock = hour switch
            {
                >= 3 and < 6 => _clockTextures[0],
                >= 6 and < 18 => _clockTextures[1],
                >= 18 and < 21 => _clockTextures[2],
                _ => _clockTextures[3]
            };

            _clockHandElement.experimental.animation.Rotation(Quaternion.Euler(0, 0, hour * 30f), 500);

            if (_currentClockImage == clock) return;
            SetClockBackground(clock);
            _currentClockImage = clock;
        }

        private void SetClockBackground(Texture2D clock)
        {
            _nextClockImage = clock;

            _clockFront.style.backgroundImage = new StyleBackground(_nextClockImage);
            _clockFront.style.opacity = 0f;
            _clockFront.BringToFront();

            _clockFront.experimental.animation.Start(new StyleValues { opacity = 1f }, 2000);

            StartCoroutine(SwapClockLayers(2.5f));
        }

        private IEnumerator SwapClockLayers(float delay)
        {
            yield return new WaitForSeconds(delay);

            _clockBack.style.backgroundImage = new StyleBackground(_nextClockImage);
            _clockBack.style.opacity = 1f;
            _clockFront.style.opacity = 0f;
            _clockFront.SendToBack();
        }
    }
}