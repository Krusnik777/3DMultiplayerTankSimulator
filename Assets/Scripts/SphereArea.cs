using UnityEngine;

namespace MultiplayerTanks
{
    public class SphereArea : MonoBehaviour
    {
        [SerializeField] private float m_radius;
        [SerializeField] private Color m_color = Color.green;

        public Vector3 RandomInside
        {
            get
            {
                var pos = Random.insideUnitSphere * m_radius + transform.position;

                pos.y = transform.position.y;

                return pos;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_color;
            Gizmos.DrawSphere(transform.position, m_radius);
        }
    }
}
