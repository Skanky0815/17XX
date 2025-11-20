using UnityEngine;

namespace Map.Controller
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 20f;
        public float dragSpeed = .5f;
        public float edgeSize = 20f;

        [Header("Zoom")]
        public float zoomSpeed = 80f;
        public float minZoom = 10f;
        public float maxZoom = 45f;

        [Header("Bounds")]
        public Vector2 xZoomedIn = new(-20f, 20f);
        public Vector2 zZoomedIn = new(-38f, -12f);
        public Vector2 xZoomedOut = new(0f, 0f);
        public Vector2 zZoomedOut = new(-30f, -30f);


        private Camera _cam;
        private Vector2 _moveInput;
        private Vector2 _mouseDelta;
        private Vector2 _mousePos;
        private float _zoomVelocity;
        private bool _isDragging;

        private CameraControls _input;

        private void Awake()
        {
            _cam = Camera.main;
            _input = new CameraControls();
            _input.Camera.Enable();


            _input.Camera.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
            _input.Camera.Move.canceled += _ => _moveInput = Vector2.zero;

            _input.Camera.MouseDelta.performed += ctx => _mouseDelta = ctx.ReadValue<Vector2>();
            _input.Camera.MousePosition.performed += ctx => _mousePos = ctx.ReadValue<Vector2>();
            _input.Camera.MiddleClick.performed += _ => _isDragging = true;
            _input.Camera.MiddleClick.canceled += _ => _isDragging = false;
        }

        private void Update()
        {
            var move = new Vector3(_moveInput.x, 0, _moveInput.y);

            if (_mousePos.x >= Screen.width - edgeSize) move += Vector3.right;
            if (_mousePos.x <= edgeSize) move += Vector3.left;
            if (_mousePos.y >= Screen.height - edgeSize) move += Vector3.forward;
            if (_mousePos.y <= edgeSize) move += Vector3.back;

            if (_isDragging)
            {
                move -= new Vector3(_mouseDelta.x, 0, -_mouseDelta.y) * (dragSpeed * Time.deltaTime);
            }

            transform.Translate(move * (moveSpeed * Time.deltaTime), Space.World);

            var scrollDelta = _input.Camera.Scroll.ReadValue<Vector2>().y;
            _zoomVelocity += scrollDelta * zoomSpeed;
            _zoomVelocity = Mathf.Lerp(_zoomVelocity, 0f, Time.deltaTime * 5f);
            
            _cam.fieldOfView -= _zoomVelocity * Time.deltaTime;
            _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, minZoom, maxZoom);

            var zoomFactor = (_cam.fieldOfView - minZoom) / (maxZoom - minZoom);

            var xMin = Mathf.Lerp(xZoomedIn.x, xZoomedOut.x, zoomFactor);
            var xMax = Mathf.Lerp(xZoomedIn.y, xZoomedOut.y, zoomFactor);
            var zMin = Mathf.Lerp(zZoomedIn.x, zZoomedOut.x, zoomFactor);
            var zMax = Mathf.Lerp(zZoomedIn.y, zZoomedOut.y, zoomFactor);

            var clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, xMin, xMax);
            clampedPos.z = Mathf.Clamp(clampedPos.z, zMin, zMax);
            transform.position = clampedPos;
        }
    }
}
