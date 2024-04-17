using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Turret : NetworkBehaviour
    {
        [SerializeField] protected Ammunition[] m_ammunition;
        [SerializeField] protected Transform m_launchPoint;
        [SerializeField] private float m_fireRate;

        [SyncVar]
        [SerializeField] private int syncSelectedAmmunitionIndex;

        public Ammunition[] Ammunition => m_ammunition;
        public Transform LaunchPoint => m_launchPoint;
        public int SelectedAmmunitionIndex => syncSelectedAmmunitionIndex;

        public ProjectileProperties SelectedProjectileProperties => m_ammunition[syncSelectedAmmunitionIndex].ProjectileProperties;

        [SyncVar]
        private float fireTimer;
        public float FireTimerNormalized => fireTimer / m_fireRate;

        public event UnityAction<int> UpdateSelectedAmmunition;
        public event UnityAction Fired;

        public void SetSelectedProjectile(int index)
        {
            if (!isOwned) return;

            if (index < 0 || index >= m_ammunition.Length || index == syncSelectedAmmunitionIndex) return;

            CmdChangeProjectile(index);

            if (isClient) CmdReloadAmmunition();

            UpdateSelectedAmmunition?.Invoke(index);
        }

        [Command]
        private void CmdChangeProjectile(int index)
        {
            syncSelectedAmmunitionIndex = index;
        }

        protected virtual void Update()
        {
            if (fireTimer > 0) fireTimer -= Time.deltaTime;
        }

        protected virtual void OnFire() { }

        public void Fire()
        {
            if (!isOwned) return;

            if (isClient) CmdFire();
        }

        [Command]
        private void CmdReloadAmmunition()
        {
            fireTimer = m_fireRate;
        }

        [Command]
        private void CmdFire()
        {
            if (fireTimer > 0) return;

            if (!m_ammunition[syncSelectedAmmunitionIndex].SvDrawAmmo(1)) return;

            OnFire();

            fireTimer = m_fireRate;

            RpcFire();

            Fired?.Invoke();
        }

        [ClientRpc]
        private void RpcFire()
        {
            if (isServer) return;

            fireTimer = m_fireRate;

            OnFire();

            Fired?.Invoke();
        }

    }
}
