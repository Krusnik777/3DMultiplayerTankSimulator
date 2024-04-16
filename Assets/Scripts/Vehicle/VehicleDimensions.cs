using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Vehicle))]
    public class VehicleDimensions : MonoBehaviour
    {
        [SerializeField] private Transform[] m_points;

        private Vehicle m_vehicle;
        public Vehicle Vehicle => m_vehicle;

        public bool IsVisibleFromPoint(Transform source, Vector3 point, Color color)
        {
            bool visible = true;

            foreach (var p in m_points)
            {
                visible = true;

                Debug.DrawLine(point, p.position, color);

                RaycastHit[] hits = Physics.RaycastAll(point, (p.position - point).normalized, Vector3.Distance(point, p.position));

                foreach (var hit in hits)
                {
                    if (hit.collider.transform.root == source) continue;

                    if (hit.collider.transform.root == transform.root) continue;

                    visible = false;
                }

                if (visible) return visible;
            }

            return visible;
        }

        private void Awake()
        {
            m_vehicle = GetComponent<Vehicle>();
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_points == null) return;

            Gizmos.color = Color.blue;
            for (int i = 0; i < m_points.Length; i++)
            {
                Gizmos.DrawSphere(m_points[i].position, 0.1f);
            }
        }
        #endif
    }
}
