using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Destructible : NetworkBehaviour
    {
        [SerializeField] private int m_maxHitPoints;
        [SerializeField] private UnityEvent m_eventDestroyed;
        [SerializeField] private UnityEvent m_eventRecovered;

        [SerializeField] private int currentHitPoints; // Serialize For Debug

        [SyncVar(hook = nameof(SyncHitPoints))]
        private int syncCurrentHitPoints;

        public int MaxHitPoints => m_maxHitPoints;
        public int HitPoints => currentHitPoints;

        public event UnityAction<int> HitPointsChange;
        public event UnityAction<Destructible> Destroyed;
        public event UnityAction<Destructible> Recovered;

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();

            syncCurrentHitPoints = m_maxHitPoints;
            currentHitPoints = m_maxHitPoints;
        }

        [Server]
        public void SvApplyDamage(int damage)
        {
            syncCurrentHitPoints -= damage;

            if (syncCurrentHitPoints <= 0)
            {
                syncCurrentHitPoints = 0;

                RpcDestroy();
            }
        }

        [Server]
        protected void SvRecovery()
        {
            syncCurrentHitPoints = m_maxHitPoints;
            currentHitPoints = m_maxHitPoints;

            RpcRecovery();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcDestroy()
        {
            Destroyed?.Invoke(this);
            m_eventDestroyed?.Invoke();
        }

        [ClientRpc]
        private void RpcRecovery()
        {
            Recovered?.Invoke(this);
            m_eventRecovered?.Invoke();
        }

        private void SyncHitPoints(int oldValue, int newValue)
        {
            currentHitPoints = newValue;

            HitPointsChange?.Invoke(newValue);
        }

        #endregion
    }
}
