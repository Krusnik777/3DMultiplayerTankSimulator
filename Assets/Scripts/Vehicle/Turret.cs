using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Turret : NetworkBehaviour
    {
        [SerializeField] protected ProjectileProperties m_projectileProperties;
        [SerializeField] protected Transform m_launchPoint;
        [SerializeField] private float m_fireRate;

        [SyncVar]
        [SerializeField] protected int m_ammoCount;
        public int AmmoCount => m_ammoCount;

        public event UnityAction<int> AmmoChanged;
        
        public event UnityAction Fired;

        public ProjectileProperties ProjectileProperties => m_projectileProperties;

        public Transform LaunchPoint => m_launchPoint;

        private float fireTimer;
        public float FireTimerNormalized => fireTimer / m_fireRate;

        protected virtual void Update()
        {
            if (fireTimer > 0) fireTimer -= Time.deltaTime;
        }

        #region ProjectileChange


        #endregion

        #region Ammo

        [Server]
        public void SvAddAmmo(int count)
        {
            m_ammoCount += count;
            RpcAmmoChanged(m_ammoCount);
        }

        [Server]
        protected virtual bool SvDrawAmmo(int count)
        {
            if (m_ammoCount == 0) return false;

            if (m_ammoCount >= count)
            {
                m_ammoCount -= count;

                RpcAmmoChanged(m_ammoCount);
                return true;
            }

            return false;
        }

        [ClientRpc]
        private void RpcAmmoChanged(int ammo)
        {
            AmmoChanged?.Invoke(ammo);
        }

        #endregion

        #region Fire

        protected virtual void OnFire() 
        {
            Fired?.Invoke();
        }

        public void Fire()
        {
            if (!isOwned) return;

            if (isClient) CmdFire();
        }

        [Command]
        private void CmdFire()
        {
            if (fireTimer > 0) return;

            if (!SvDrawAmmo(1)) return;

            OnFire();

            fireTimer = m_fireRate;

            RpcFire();
        }

        [ClientRpc]
        private void RpcFire()
        {
            if (isServer) return;

            fireTimer = m_fireRate;

            OnFire();
        }

        #endregion
    }
}
