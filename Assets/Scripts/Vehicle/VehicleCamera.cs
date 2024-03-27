using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Camera))]
    public class VehicleCamera : MonoBehaviour
    {
        public static VehicleCamera Instance;

        [SerializeField] private Vehicle m_vehicle;
        [SerializeField] private Vector3 m_offset;
        [Header("SensitiveLimit")]
        [SerializeField] private float m_rotateSensitive;
        [SerializeField] private float m_scrollSensitive;
        [Header("RotationLimit")]
        [SerializeField] private float m_maxVerticalAngle;
        [SerializeField] private float m_minVerticalAngle;
        [Header("Distance")]
        [SerializeField] private float m_distance;
        [SerializeField] private float m_maxDistance;
        [SerializeField] private float m_minDistance;
        [SerializeField] private float m_distanceOffsetFromCollisionHit;
        [SerializeField] private float m_distanceLerpRate;
        [Header("ZoomOptics")]
        [SerializeField] private GameObject m_zoomMaskEffect;
        [SerializeField] private float m_zoomedFOV;
        [SerializeField] private float m_zoomedMaxVerticalAngle;

        private Camera m_camera;
        private float m_defaultFOV;

        private Vector2 m_rotationControl;

        private float deltaRotationX;
        private float deltaRotationY;

        private float currentDistance;
        private float lastDistance;

        private float defaultMaxVerticalAngle;

        private bool isZoomed;
        public bool IsZoomed => isZoomed;

        public void SetTarget(Vehicle target) => m_vehicle = target;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            m_camera = GetComponent<Camera>();
            m_defaultFOV = m_camera.fieldOfView;

            defaultMaxVerticalAngle = m_maxVerticalAngle;
        }

        private void Update()
        {
            if (m_vehicle == null) return;

            UpdateControls();

            m_distance = Mathf.Clamp(m_distance, m_minDistance, m_maxDistance);
            isZoomed = m_distance <= m_minDistance;

            // Calculate rotation and translation
            deltaRotationX += m_rotationControl.x * m_rotateSensitive;
            deltaRotationY += m_rotationControl.y * -m_rotateSensitive;

            deltaRotationY = ClampAngle(deltaRotationY, m_minVerticalAngle, m_maxVerticalAngle);

            Quaternion finalRotation = Quaternion.Euler(deltaRotationY, deltaRotationX, 0);
            Vector3 finalPosition = m_vehicle.transform.position - (finalRotation * Vector3.forward * m_distance);
            finalPosition = AddLocalOffset(finalPosition);

            // Calculate current distance
            float targetDistance = m_distance;

            RaycastHit hit;

            Debug.DrawLine(m_vehicle.transform.position + new Vector3(0, m_offset.y, 0), finalPosition, Color.red);

            if (Physics.Linecast(m_vehicle.transform.position + new Vector3(0, m_offset.y, 0), finalPosition, out hit))
            {
                float distanceToHit = Vector3.Distance(m_vehicle.transform.position + new Vector3(0, m_offset.y, 0), hit.point);

                if (hit.transform != m_vehicle)
                {
                    if (distanceToHit < m_distance)
                        targetDistance = distanceToHit - m_distanceOffsetFromCollisionHit;
                }
            }

            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * m_distanceLerpRate);
            currentDistance = Mathf.Clamp(currentDistance, m_minDistance, m_distance);

            // Correct camera position
            finalPosition = m_vehicle.transform.position - (finalRotation * Vector3.forward * currentDistance);

            // Apply transform
            transform.rotation = finalRotation;
            transform.position = finalPosition;
            transform.position = AddLocalOffset(transform.position);

            // Zoom
            m_zoomMaskEffect.SetActive(isZoomed);
            if (isZoomed)
            {
                transform.position = m_vehicle.ZoomOpticsPosition.position;
                m_camera.fieldOfView = m_zoomedFOV;
                m_maxVerticalAngle = m_zoomedMaxVerticalAngle;
            }
            else
            {
                m_camera.fieldOfView = m_defaultFOV;
                m_maxVerticalAngle = defaultMaxVerticalAngle;
            }
        }

        private void UpdateControls()
        {
            m_rotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            m_distance += -Input.mouseScrollDelta.y * m_scrollSensitive;

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isZoomed = !isZoomed;

                if (isZoomed)
                {
                    lastDistance = m_distance;
                    m_distance = m_minDistance;
                }
                else
                {
                    m_distance = lastDistance;
                    currentDistance = lastDistance;
                }
            }
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        private Vector3 AddLocalOffset(Vector3 position)
        {
            Vector3 result = position;

            result += new Vector3(0, m_offset.y, 0);
            result += transform.right * m_offset.x;
            result += transform.forward * m_offset.z;

            return result;
        }

    }
}
