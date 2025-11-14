using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class BootLoader : MonoBehaviour
    {
        private static WaitForSeconds _waitForSeconds1 = new(1f);
        [SerializeField] private string nextSceneName = "MainMenu";

        private IEnumerator Start()
        {
            LocalizationManager.Instance.LoadLocalization("de");

            yield return _waitForSeconds1;

            SceneManager.LoadScene(nextSceneName);
        }

    }
}