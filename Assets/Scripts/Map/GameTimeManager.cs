using Core.States;
using Map.Controller;
using Map.Objects;

namespace Map
{
    public static class GameTimeManager
    {
        private static GameTimeController _gameTimeController;

        public static void Initialize(GameTimeController gameTimeControllerRef)
        {
            _gameTimeController = gameTimeControllerRef;
        }

        public static void Pause()
        {
            if (_gameTimeController == null) return;

            _gameTimeController.isPause = true;
        }

        public static void Resume()
        {
            if (_gameTimeController == null) return;

            _gameTimeController.isPause = false;
        }

        public static void Save(MapWorldState worldState)
        {
            if (_gameTimeController == null) return;

            var gameTime = _gameTimeController.CurrentTime;
            worldState.timeState = new TimeState
            {
                day = gameTime.Day,
                hour = gameTime.Hour,
                minute = gameTime.Minute,
            };
        }

        public static void Load(MapWorldState worldState)
        {
            if (worldState.timeState.IsNotSaved()) return;

            _gameTimeController.CurrentTime = new GameTime(
                worldState.timeState.day,
                worldState.timeState.hour,
                worldState.timeState.minute
            );
        }
    }
}