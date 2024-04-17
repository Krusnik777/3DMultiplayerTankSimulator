using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Vehicle))]
    public class VehicleDimensions : MonoBehaviour
    {
        [SerializeField] private Transform[] m_points;

        private Vehicle m_vehicle;
        public Vehicle Vehicle => m_vehicle;

        private RaycastHit[] hits = new RaycastHit[10];

        public bool IsVisibleFromPoint(Transform source, Vector3 point, Color color)
        {
            bool visible = true;
            
            for (int i = 0; i < m_points.Length; i++)
            {
                visible = true;

                //Debug.DrawLine(point, m_points[i].position, color);

                int count = Physics.RaycastNonAlloc(point, (m_points[i].position - point).normalized, hits, Vector3.Distance(point, m_points[i].position));

                for (int j = 0; j < count; j++)
                {
                    if (hits[j].collider.transform.root == source) continue;

                    if (hits[j].collider.transform.root == transform.root) continue;

                    visible = false;
                }

                if (visible) return visible;
            }

            return false;
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
