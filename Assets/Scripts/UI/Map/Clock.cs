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
        public GameTimeController GameTimeController;

        public Sprite[] TimeSprites;

        private Label _dayLabel;

        private VisualElement _clockFront;
        private VisualElement _clockBack;
        private VisualElement _clochHandElement;

        private int _currentDay;
        private Texture2D[] _clockTextures;

        private Texture2D _currentClockImage;
        private Texture2D _nextClockImage;

        private string _text;

        private void Awake()
        {
            _text = LocalizationManager.Instance.GetText("clock.day");
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;


            _clockTextures = new Texture2D[TimeSprites.Length];
            for (var i = 0; i < TimeSprites.Length; i++)
            {
                _clockTextures[i] = SpriteConverter.ToTexture(TimeSprites[i]);
            }

            var clockPanel = root.Q<VisualElement>("ClockPanel");

            _clockFront = clockPanel.Q<VisualElement>("ClockFront");
            _clockBack = clockPanel.Q<VisualElement>("ClockBack");
            _clochHandElement = clockPanel.Q<VisualElement>("ClockHand");
            _dayLabel = clockPanel.Q<Label>("Day");

            _clockBack.style.backgroundImage = new StyleBackground(_clockTextures[1]);
        }

        private void Start()
        {
            GameTimeController.CurrentTime.OnNewDay += OnNewDay;
            GameTimeController.CurrentTime.OnNewHoure += OnNewHoure;
        }

        private void OnDestroy()
        {
            GameTimeController.CurrentTime.OnNewDay -= OnNewDay;
            GameTimeController.CurrentTime.OnNewHoure -= OnNewHoure;
        }

        private void OnNewDay(int day)
        {
            _currentDay = day;
        }

        private void OnNewHoure(int houre)
        {
            _dayLabel.text = string.Format(_text, _currentDay);

            Texture2D clock;
            if (3 <= houre && houre < 6) clock = _clockTextures[0];
            else if (6 <= houre && houre < 18) clock = _clockTextures[1];
            else if (18 <= houre && houre < 21) clock = _clockTextures[2];
            else clock = _clockTextures[3];

            _clochHandElement.experimental.animation.Rotation(Quaternion.Euler(0, 0, houre * 30f), 500);

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