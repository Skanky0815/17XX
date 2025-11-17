using Map.Objects;
using UnityEngine;

namespace Map.Controller
{
    public class GameTimeController : MonoBehaviour
    {
        public GameTime CurrentTime = new();

        public bool isPause = false;

        private void Update()
        {
            if (isPause) return;

            CurrentTime.Advance(Time.deltaTime);
        }
    }
}
