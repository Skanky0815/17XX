using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Map
{
    public class Menu : MonoBehaviour
    {
        private Button _menuButton;

        private void Awake()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            _menuButton = root.Q<Button>("MenuButton");
        }

        private void Start()
        {
            _menuButton.RegisterCallback<MouseUpEvent>(evt =>
            {
                GameManager.Instance.SwitchToScene("MainMenu");
            });
        }
    }
}