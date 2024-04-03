using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class ConditionCaptureBase : NetworkBehaviour, IMatchCondition
    {
        [SerializeField] private TeamBase m_redBase;
        [SerializeField] private TeamBase m_blueBase;

        [SyncVar]
        private float redBaseCaptureLevel;
        public float RedBaseCaptureLevel => redBaseCaptureLevel;

        [SyncVar]
        private float blueBaseCaptureLevel;
        public float BlueBaseCaptureLevel => blueBaseCaptureLevel;

        private bool triggered;

        public bool IsTriggered => triggered;

        public void OnServerMatchEnd(MatchController controller)
        {
            enabled = false;
        }

        public void OnServerMatchStart(MatchController controller)
        {
            Reset();
        }

        private void Start()
        {
            enabled = false;
        }

        private void Update()
        {
            if (isServer)
            {
                redBaseCaptureLevel = m_redBase.CaptureLevel;
                blueBaseCaptureLevel = m_blueBase.CaptureLevel;

                if (redBaseCaptureLevel == 100 || blueBaseCaptureLevel == 100) triggered = true;
            }   
        }

        private void Reset()
        {
            m_redBase.Reset();
            m_blueBase.Reset();

            redBaseCaptureLevel = 0;
            blueBaseCaptureLevel = 0;

            triggered = false;
            enabled = true;
        }
    }
}
