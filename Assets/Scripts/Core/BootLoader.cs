using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class BootLoader : MonoBehaviour
    {
        private static readonly WaitForSeconds WaitForSeconds1 = new(1f);
        [SerializeField] private string nextSceneName = "MainMenu";

        private IEnumerator Start()
        {
            LocalizationManager.Instance.LoadLocalization("de");

            yield return WaitForSeconds1;

            SceneManager.LoadScene(nextSceneName);
        }

    }
}