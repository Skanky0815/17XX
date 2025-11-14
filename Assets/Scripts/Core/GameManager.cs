using Assets.Scripts.Core;
using Assets.Scripts.Core.States;
using Map;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public MapWorldState mapWorldState;

        public GameConfig gameConfig;

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

        public void SwitchToScene(string name)
        {
            SaveGame();
            SceneManager.LoadScene(name);
        }


        public void SaveGame()
        {
            GameTimeManager.Save(mapWorldState);
            FactionManager.Save(mapWorldState);
            RegionManager.Save(mapWorldState);
        }

        public void LoadGame()
        {
            GameTimeManager.Load(mapWorldState);
            FactionManager.Load(mapWorldState);
            RegionManager.Load(mapWorldState);
        }
    }
}