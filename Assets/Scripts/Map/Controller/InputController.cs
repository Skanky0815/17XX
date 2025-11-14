using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Map.Controller
{
    class InputController : MonoBehaviour
    {
        public UIDocument UIDocument;

        private ClickControls.PointerActions _pointerActions;
        private int _interactableLayer;


        private void Awake()
        {
            _interactableLayer = LayerMask.GetMask("Interactables");
            _pointerActions = new ClickControls().Pointer;

            _pointerActions.MouseClick.performed += OnClick;
            _pointerActions.Enable();
        }

        private void OnDestroy()
        {
            _pointerActions.MouseClick.performed -= OnClick;
            _pointerActions.Disable();
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            var mousePosition = _pointerActions.MousePosition.ReadValue<Vector2>();

            if (IsUiElement(mousePosition)) return;

            var ray = Camera.main.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _interactableLayer)) return;

            var interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.Interact(mousePosition);
        }

        private bool IsUiElement(Vector2 mousePosition)
        {
            mousePosition.y = Screen.height - mousePosition.y;
            var panelPostition = RuntimePanelUtils.ScreenToPanel(UIDocument.rootVisualElement.panel, mousePosition);
            var hit = UIDocument.rootVisualElement.panel.Pick(panelPostition);

            return hit != null;
        }
    }
}