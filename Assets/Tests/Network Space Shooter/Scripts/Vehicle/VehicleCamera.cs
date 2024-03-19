using UnityEngine;

namespace NetworkSpaceShooter
{
    public class VehicleCamera : MonoBehaviour
    {
        public static VehicleCamera Instance;

        [SerializeField] private Camera m_camera;
        [SerializeField] private float m_interpollationLinear;
        [SerializeField] private float m_interpollationAngular;
        [SerializeField] private float m_CameraZOffset;
        [SerializeField] private float m_ForwardOffset;

        private Transform m_target;

        public void SetTarget(Transform newTarget)
        {
            m_target = newTarget;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void FixedUpdate()
        {
            if (m_target == null || m_camera == null) return;

            Vector2 camPos = m_camera.transform.position;
            Vector2 targetPos = m_target.position + m_target.transform.up * m_ForwardOffset;

            Vector2 newCamPos = Vector2.Lerp(camPos, targetPos, m_interpollationLinear * Time.deltaTime);

            m_camera.transform.position = new Vector3(newCamPos.x, newCamPos.y, m_CameraZOffset);

            if (m_interpollationAngular > 0)
            {
                m_camera.transform.rotation = Quaternion.Slerp(m_camera.transform.rotation, m_target.rotation, m_interpollationAngular * Time.deltaTime);
            }
        }
    }
}
