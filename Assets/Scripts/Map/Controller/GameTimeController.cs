using Core.States;
using UnityEngine;

namespace Map.Controller
{
    public class GameTimeController : MonoBehaviour
    {
        public MapWorldState worldState;

        public bool isPause = false;

        private void Update()
        {
            if (isPause) return;

            worldState.gameTime.Advance(Time.deltaTime);
        }
    }
}
