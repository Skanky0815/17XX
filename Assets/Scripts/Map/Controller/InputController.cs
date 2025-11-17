using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Map.Controller
{
    class InputController : MonoBehaviour
    {
        public UIDocument uiDocument;

        private ClickControls.PointerActions _pointerActions;
        private int _interactableLayer;

        private readonly List<ISelectable> _selectedObjects = new();

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

            HandleInteractable(hit, mousePosition);
            HandleSelectable(hit, mousePosition);
        }

        private bool IsUiElement(Vector2 mousePosition)
        {
            mousePosition.y = Screen.height - mousePosition.y;
            var panelPosition = RuntimePanelUtils.ScreenToPanel(uiDocument.rootVisualElement.panel, mousePosition);
            var hit = uiDocument.rootVisualElement.panel.Pick(panelPosition);

            return hit != null;
        }

        private static void HandleInteractable(RaycastHit hit, Vector2 mousePosition)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.Interact(mousePosition);
        }

        private void HandleSelectable(RaycastHit hit, Vector2 mousePosition)
        {
            if (!hit.collider.TryGetComponent<ISelectable>(out var selectable)) return;

            foreach (var selected in _selectedObjects.Where(selected => selected != selectable))
            {
                selected.Deselect();
            }

            selectable.Select(mousePosition);
            _selectedObjects.Add(selectable);
        }
    }
}