using UnityEngine;

namespace Map
{
    public interface ISelectable
    {
        void Select(Vector2 position);

        void Deselect();
    }
}