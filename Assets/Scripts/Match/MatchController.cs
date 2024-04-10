using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    public interface IMatchCondition
    {
        bool IsTriggered { get; }

        void OnServerMatchStart(MatchController controller);
        void OnServerMatchEnd(MatchController controller);
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class MatchController : NetworkBehaviour
    {
        public event UnityAction MatchStart;
        public event UnityAction MatchEnd;

        public event UnityAction SvMatchStart;
        public event UnityAction SvMatchEnd;

        [SyncVar]
        private bool m_matchActive;
        public bool IsMatchActive => m_matchActive;

        public int WinTeamId = -1;

        private IMatchCondition[] m_matchConditions;

        private void Awake()
        {
            m_matchConditions = GetComponentsInChildren<IMatchCondition>();
        }

        private void Update()
        {
            if (isServer)
            {
                if (m_matchActive)
                {
                    foreach (var condition in m_matchConditions)
                    {
                        if (condition.IsTriggered)
                        {
                            SvEndMatch();
                            break;
                        }
                    }
                }
            }
        }

        [Server]
        public void SvRestartMatch()
        {
            if (m_matchActive) return;

            m_matchActive = true;

            foreach (var p in FindObjectsOfType<Player>())
            {
                if (p.ActiveVehicle != null)
                {
                    NetworkServer.UnSpawn(p.ActiveVehicle.gameObject);
                    Destroy(p.ActiveVehicle.gameObject);
                    p.ActiveVehicle = null;
                }
            }

            foreach (var p in FindObjectsOfType<Player>())
            {
                p.SvSpawnClientVehicle();
            }

            foreach (var condition in m_matchConditions)
            {
                condition.OnServerMatchStart(this);
            }

            SvMatchStart?.Invoke();

            RpcMatchStart();
        }

        [Server]
        public void SvEndMatch()
        {
            foreach (var condition in m_matchConditions)
            {
                condition.OnServerMatchEnd(this);

                if (condition is ConditionDeathMatch)
                {
                    WinTeamId = (condition as ConditionDeathMatch).WinTeamId;
                }

                if (condition is ConditionCaptureBase)
                {
                    if ((condition as ConditionCaptureBase).RedBaseCaptureLevel == 100) WinTeamId = TeamSide.TeamBlue;

                    if ((condition as ConditionCaptureBase).BlueBaseCaptureLevel == 100) WinTeamId = TeamSide.TeamRed;
                }
            }

            m_matchActive = false;

            SvMatchEnd?.Invoke();

            RpcMatchEnd(WinTeamId);
        }

        [ClientRpc]
        private void RpcMatchStart()
        {
            MatchStart?.Invoke();
        }

        [ClientRpc]
        private void RpcMatchEnd(int winTeamId)
        {
            WinTeamId = winTeamId;

            MatchEnd?.Invoke();
        }
    }
}
