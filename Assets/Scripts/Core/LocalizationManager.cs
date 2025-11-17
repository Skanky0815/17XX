using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Core
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }
        private Dictionary<string, string> _localizedTexts;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadLocalization("de");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadLocalization(string languageCode)
        {
            var localizationFile = Resources.Load<TextAsset>($"Localization/{languageCode}_map");
            if (localizationFile != null)
            {
                _localizedTexts = JsonConvert.DeserializeObject<Dictionary<string, string>>(localizationFile.text);
            }
            else
            {
                Debug.LogError($"Localization file for language '{languageCode}' not found.");
                _localizedTexts = new Dictionary<string, string>();
            }
        }

        public string GetText(string key)
        {
            if (_localizedTexts != null && _localizedTexts.TryGetValue(key, out var value))
                return value;

            return $"#{key}";
        }
    }
}