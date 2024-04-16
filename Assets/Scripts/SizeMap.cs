using UnityEngine;

namespace MultiplayerTanks
{
    public class SizeMap : MonoBehaviour
    {
        [SerializeField] private Vector2 m_size;

        public Vector2 Size { get => m_size; }

        public Vector3 GetNormalizedPosition(Vector3 position) => new Vector3(position.x / (m_size.x * 0.5f), 0, position.z / (m_size.y * 0.5f));

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(m_size.x, 0, m_size.y));
        }
    }
}
