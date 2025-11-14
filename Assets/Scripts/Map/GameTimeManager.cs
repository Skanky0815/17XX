using Assets.Scripts.Core.States;
using Map.Controller;

namespace Map
{
    public static class GameTimeManager
    {
        private static GameTimeController gameTimeController;

        public static void Initialize(GameTimeController gameTimeControllerRef)
        {
            gameTimeController = gameTimeControllerRef;
        }

        public static void Pause()
        {
            if (gameTimeController == null) return;

            gameTimeController.IsPause = true;
        }

        public static void Resume()
        {
            if (gameTimeController == null) return;

            gameTimeController.IsPause = false;
        }

        public static void Save(MapWorldState worldState)
        {
            if (gameTimeController == null) return;

            var gameTime = gameTimeController.CurrentTime;
            worldState.timeState = new TimeState
            {
                Day = gameTime.Day,
                Houre = gameTime.Hour,
                Minute = gameTime.Minute,
            };
        }

        public static void Load(MapWorldState worldState)
        {
            if (worldState.timeState.IsNotSaved()) return;

            gameTimeController.CurrentTime = new GameTime(
                worldState.timeState.Day,
                worldState.timeState.Houre,
                worldState.timeState.Minute
            );
        }
    }
}