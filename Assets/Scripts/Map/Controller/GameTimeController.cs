using UnityEngine;

namespace Map.Controller
{
    public class GameTimeController : MonoBehaviour
    {
        public GameTime CurrentTime = new();

        public bool IsPause = false;

        private void Update()
        {
            if (IsPause) return;

            CurrentTime.Advance(Time.deltaTime);
        }
    }
}
