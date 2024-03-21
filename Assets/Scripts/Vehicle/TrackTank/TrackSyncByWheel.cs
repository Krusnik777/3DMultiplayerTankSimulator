using UnityEngine;

namespace MultiplayerTanks
{
    [System.Serializable]
    public class WheelSynPoint
    {
        public Transform Bone;
        public Transform Mesh;
        public Vector3 Offset { get; set; }
    }

    public class TrackSyncByWheel : MonoBehaviour
    {
        [SerializeField] private WheelSynPoint[] m_syncPoints;

        private void Start()
        {
            for (int i = 0; i < m_syncPoints.Length; i++)
            {
                m_syncPoints[i].Offset = m_syncPoints[i].Bone.localPosition - m_syncPoints[i].Mesh.localPosition;
            }
        }

        private void Update()
        {
            for (int i = 0; i < m_syncPoints.Length; i++)
            {
                m_syncPoints[i].Bone.localPosition = m_syncPoints[i].Mesh.localPosition + m_syncPoints[i].Offset;
            }
        }
    }
}
