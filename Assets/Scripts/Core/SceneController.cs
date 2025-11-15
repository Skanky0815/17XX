using Assets.Scripts.Core.States;
using Map;
using Map.Controller;
using Map.Loader;
using Map.Player;
using UnityEngine;

namespace Core
{
    public static class SceneController
    {
        private static string currentSceneName = "";

        public static void InitializeSceneObjects(string sceneName, MapWorldState worldState)
        {
            currentSceneName = sceneName;

            if (sceneName == "MapScene")
            {
                var player = GameObject.FindWithTag("Player").GetComponent<Player>();
                var gameTimeController = GameObject.FindWithTag("Map").GetComponent<GameTimeController>();

                GameTimeManager.Initialize(gameTimeController);
                // FactionManager.Initialize(player, worldState);
                RegionManager.Initialize(gameTimeController);
                GameManager.Instance.LoadGame();
            }
        }
    }
}