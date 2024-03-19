using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Camera))]
    public class VehicleCamera : MonoBehaviour
    {
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

        private Camera m_camera;

        private Vector2 m_rotationControl;

        private float deltaRotationX;
        private float deltaRotationY;

        private float currentDistance;

        public void SetTarget(Vehicle target) => m_vehicle = target;

        private void Start()
        {
            m_camera = GetComponent<Camera>();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            UpdateControls();

            m_distance = Mathf.Clamp(m_distance, m_minDistance, m_maxDistance);

            // Calculate rotation and translation
            deltaRotationX += m_rotationControl.x * m_rotateSensitive;
            deltaRotationY += m_rotationControl.y * m_rotateSensitive;

            deltaRotationY = ClampAngle(deltaRotationY, m_minVerticalAngle, m_maxVerticalAngle);

            //Quaternion finalRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(deltaRotationY, deltaRotationX, 0), 0.25f);
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
                //float distanceToHit = hit.distance;

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
        }

        private void UpdateControls()
        {
            m_rotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            m_distance += -Input.mouseScrollDelta.y * m_scrollSensitive;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            /*
            angle %= 360;
            angle = angle > 180 ? angle - 360 : angle;*/

            if (angle < - 360)
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

            result.y += m_offset.y;
            result += transform.right * m_offset.x;
            result += transform.forward * m_offset.z;

            return result;
        }

    }
}
