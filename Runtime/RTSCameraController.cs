using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSCameraController.Runtime
{
    public class RTSCameraController : MonoBehaviour
    {

        [Header("Controls")]
        [Space]
        public InputActionReference pan;
        public InputActionReference cursor;
        public InputActionReference zoom;
        public InputActionReference rotate;

        [Header("Movement")]
        [Space]
        public float minPanSpeed = 3.0f;
        public float maxPanSpeed = 7.0f;
        public float secToMaxSpeed = 3.0f; //seconds taken to reach max speed;
        public bool enableMovementLimits = false;
        public Vector3 minLimit = Vector3.one * -1000.0f;
        public Vector3 maxLimit = Vector3.one * 1000.0f;

        [Header("Rotation")]
        [Space]
        public bool rotationEnabled = true;
        public float rotateSpeed = 10.0f;

        [Header("Zoom")]
        [Space]
        public bool enableZoomLimits = true;
        public Vector2 zoomLimit = new Vector2(15, 65);
        public float zoomSpeed = 3.0f;

        [Header("Camera")]
        [Space]
        public new Camera camera = null;
        [SerializeField]
        public BorderThickness ScreenEdgeBorderThickness = BorderThickness.one() * 10.0f;


        private Vector3 up = Vector3.up;
        private Vector3 right = Vector3.right;
        private Vector3 forward = Vector3.forward;
        private float panSpeed = 0.0f;
        private float panIncrease = 0.0f;



        // Use this for initialization
        void Start()
        {
            if (camera == null) camera = Camera.main;
            rotate.action.Enable();
            pan.action.Enable();
            zoom.action.Enable();
            cursor.action.Enable();
        }


        void Update()
        {
            up = transform.up;
            right = transform.right;
            forward = transform.forward;

            HandleMovement();
            HandleZoom();
            HandleRotation();
            LimitMovement();
        }

        private void LimitMovement()
        {
            if (enableMovementLimits == true)
            {
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, minLimit.x, maxLimit.x);
                pos.y = Mathf.Clamp(pos.y, minLimit.y, maxLimit.y);
                pos.z = Mathf.Clamp(pos.z, minLimit.z, maxLimit.z);
                transform.position = pos;
            }
        }

        private void HandleRotation()
        {
            if (rotationEnabled)
            {
                float rotateAmt = rotate.action.ReadValue<float>();
                transform.RotateAround(transform.position, up, rotateAmt * rotateSpeed * Time.deltaTime);
            }
        }

        private void HandleZoom()
        {
            if (camera == null) throw new System.Exception("No camera set on RTSCameraController and Camera.main fallback failed!");
            float zoomMovement = zoom.action.ReadValue<float>();
            bool isZooming = Mathf.Abs(zoomMovement) > 0.1;
            float zSpeed = isZooming ? zoomSpeed : 0;
            camera.fieldOfView -= zoomMovement * zSpeed * Time.deltaTime;
            if (enableZoomLimits)
            {
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, zoomLimit.x, zoomLimit.y);
            }
        }

        private void HandleMovement()
        {
            Vector2 panAxis = pan.action.ReadValue<Vector2>();
            panAxis += MouseMovementAsAxis();
            bool isMoving = panAxis.sqrMagnitude > 0.1;
            float xySpeed = isMoving ? panSpeed : 0;

            Vector3 panDirection = (right * panAxis.x) + (forward * panAxis.y);
            Vector3 panMovement = panDirection.normalized * xySpeed * Time.deltaTime;

            transform.Translate(panMovement, Space.World);

            if (isMoving)
            {
                panIncrease += Time.deltaTime / secToMaxSpeed;
                panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panIncrease);
            }
            else
            {
                // When we stop moving reset speed to minimum
                panIncrease = 0;
                panSpeed = minPanSpeed;
            }
        }

        private Vector2 MouseMovementAsAxis()
        {
            Vector2 movement = Vector2.zero;
            Vector2 mousePosition = this.cursor.action.ReadValue<Vector2>();

            bool mouseIsOffScreen =
                mousePosition.y <= 0 || mousePosition.y >= Screen.height ||
                mousePosition.x <= 0 || mousePosition.x >= Screen.width;

            if (mouseIsOffScreen) return Vector2.zero;

            if (mousePosition.y >= Screen.height - ScreenEdgeBorderThickness.Top)
            {
                movement += Vector2.up;
            }
            if (mousePosition.x >= Screen.width - ScreenEdgeBorderThickness.Right)
            {
                movement += Vector2.right;
            }
            if (mousePosition.y <= ScreenEdgeBorderThickness.Bottom)
            {
                movement += Vector2.down;
            }
            if (mousePosition.x <= ScreenEdgeBorderThickness.Left)
            {
                movement += Vector2.left;
            }
            return movement;
        }
    }
}