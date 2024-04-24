using UnityEngine;

namespace MultiplayerTanks
{
    public class AIPath : MonoBehaviour
    {
        public static AIPath Instance;

        [SerializeField] private Transform m_baseRedPoint;
        [SerializeField] private Transform m_baseBluePoint;
        [SerializeField] private Transform[] m_fireRedPoints;
        [SerializeField] private Transform[] m_fireBluePoints;
        [SerializeField] private Transform[] m_patrolPoints;
        [SerializeField] private Transform[] m_smartInvadePoints;

        public Vector3 GetBasePoint(int teamId)
        {
            if (teamId == TeamSide.TeamRed) return m_baseBluePoint.position;
            if (teamId == TeamSide.TeamBlue) return m_baseRedPoint.position;

            return Vector3.zero;
        }

        public Vector3 GetRandomFirePoint(int teamId)
        {
            if (teamId == TeamSide.TeamRed) return m_fireRedPoints[Random.Range(0, m_fireRedPoints.Length)].position;
            if (teamId == TeamSide.TeamBlue) return m_fireBluePoints[Random.Range(0, m_fireBluePoints.Length)].position;

            return Vector3.zero;
        }

        public Vector3 GetRandomPatrolPoint() => m_patrolPoints[Random.Range(0, m_patrolPoints.Length)].position;
        public Vector3 GetRandomStartInvadePoint() => m_smartInvadePoints[Random.Range(0, m_smartInvadePoints.Length)].position;
        

        private void Awake()
        {
            Instance = this;
        }

    }
}
