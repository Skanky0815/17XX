using Assets.Scripts.Core.States;
using Core;
using Map;
using Map.Controller;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public static class SceneController
    {
        public static void InitializeSceneObjects(string sceneName, MapWorldState worldState)
        {
            if (sceneName == "MapScene")
            {
                var player = GameObject.FindWithTag("Player").GetComponent<Player>();
                var gameTimeController = GameObject.FindWithTag("Map").GetComponent<GameTimeController>();

                GameTimeManager.Initialize(gameTimeController);
                FactionManager.Initialize(player, worldState);
                RegionManager.Initialize(gameTimeController);
                GameManager.Instance.LoadGame();
            }
        }
    }
}