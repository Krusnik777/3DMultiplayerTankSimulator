using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class MatchTimer : NetworkBehaviour, IMatchCondition
    {
        [SerializeField] private float m_matchTime;

        [SyncVar]
        private float m_timeLeft;

        public float MatchTime => m_matchTime;
        public float TimeLeft => m_timeLeft;

        private bool timerEnd = false;

        public bool IsTriggered => timerEnd;

        public void OnServerMatchStart(MatchController controller)
        {
            Reset();
        }

        public void OnServerMatchEnd(MatchController controller)
        {
            enabled = false;
        }

        private void Start()
        {
            m_timeLeft = m_matchTime;

            if (isServer)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if (isServer)
            {
                m_timeLeft -= Time.deltaTime;

                if (m_timeLeft < 0)
                {
                    m_timeLeft = 0;

                    timerEnd = true;
                }
            }
        }

        private void Reset()
        {
            enabled = true;
            m_timeLeft = m_matchTime;
            timerEnd = false;
        }
    }
}
