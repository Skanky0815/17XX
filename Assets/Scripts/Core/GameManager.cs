using Core.States;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public MapWorldState mapWorldState;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                SceneManager.sceneUnloaded += OnSceneUnloaded;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            SaveGame();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneController.InitializeSceneObjects(scene.name, mapWorldState);
        }

        public void SwitchToScene(string sceneName)
        {
            SaveGame();
            SceneManager.LoadScene(sceneName);
        }

        private void SaveGame()
        {
            
        }

        public void LoadGame()
        {
        }
    }
}