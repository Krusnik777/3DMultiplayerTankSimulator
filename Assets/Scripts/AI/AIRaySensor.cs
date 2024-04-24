using UnityEngine;

namespace MultiplayerTanks
{
    public class AIRaySensor : MonoBehaviour
    {
        [SerializeField] private Transform[] m_rays;
        [SerializeField] private float m_raycastDistance;
        public float RaycastDistance => m_raycastDistance;

        public (bool, float) Raycast()
        {
            float dist = -1;

            foreach(var ray in m_rays)
            {
                RaycastHit hit;

                if (Physics.Raycast(ray.position, ray.forward, out hit, m_raycastDistance))
                {
                    if (dist < 0 || hit.distance < dist) dist = hit.distance;
                }
            }

            return (dist > 0, dist);
        }

        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            foreach (var ray in m_rays)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(ray.position, ray.position + ray.forward * m_raycastDistance);
            }
        }

        #endif
    }
}
