using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Destructible : NetworkBehaviour
    {
        [SerializeField] private int m_maxHitPoints;
        [SerializeField] private GameObject m_destroySfx;
        [SerializeField] private UnityEvent<Destructible> m_onDestroyed;
        public UnityEvent<Destructible> OnEventDeath => m_onDestroyed;

        [SyncVar(hook = nameof(T))]
        public NetworkIdentity Owner;

        [SyncVar(hook = nameof(ChangeHitPoints))]
        private int syncCurrentHitPoints;

        public int MaxHitPoints => m_maxHitPoints;

        private int currentHitPoints;
        public int HitPoints => currentHitPoints;

        public UnityAction<int> HitPointsChange;

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
                if (m_destroySfx != null)
                {
                    var sfx = Instantiate(m_destroySfx, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(sfx);
                }

                syncCurrentHitPoints = 0;

                RpcDestroy();
            }
        }

        [ClientRpc]
        private void RpcDestroy()
        {
            OnDestructibleDestroy();
        }

        protected virtual void OnDestructibleDestroy()
        {
            m_onDestroyed?.Invoke(this);
        }

        private void ChangeHitPoints(int oldValue, int newValue)
        {
            currentHitPoints = newValue;

            HitPointsChange?.Invoke(newValue);
        }

        private void T(NetworkIdentity oldValue, NetworkIdentity newValue) { }
    }
}
