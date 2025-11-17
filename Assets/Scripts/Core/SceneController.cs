using Core.States;

namespace Core
{
    public static class SceneController
    {
        public static void InitializeSceneObjects(string sceneName, MapWorldState worldState)
        {
            if (sceneName != "MapScene") return;
            
            GameManager.Instance.LoadGame();
        }
    }
}