using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [System.Serializable]
    public class ProjectileData
    {
        public Projectile Projectile;
        public int Ammo;
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class Turret : NetworkBehaviour
    {
        [SerializeField] protected ProjectileData[] m_projectiles;
        [SerializeField] protected Transform m_launchPoint;
        [SerializeField] private float m_fireRate;

        [SyncVar]
        protected int m_ammoCount;

        [SyncVar]
        protected int activeProjectileIndex;

        public UnityAction<int,int> ProjectileChanged;
        public UnityAction<int> AmmoChanged;
        
        public UnityAction Fired;

        public ProjectileData[] Projectiles => m_projectiles;

        private int[] currentAmmoForProjectiles;

        public Projectile ActiveProjectile => m_projectiles[activeProjectileIndex].Projectile;

        public Transform LaunchPoint => m_launchPoint;
        public int AmmoCount => m_ammoCount;
        public int ActiveProjectileIndex => activeProjectileIndex;

        private float fireTimer;
        public float FireTimerNormalized => fireTimer / m_fireRate;

        public void InitializeTurret()
        {
            if (currentAmmoForProjectiles != null) return;

            activeProjectileIndex = 0;
            m_ammoCount = m_projectiles[activeProjectileIndex].Ammo;

            currentAmmoForProjectiles = new int[m_projectiles.Length];

            for (int i = 0; i < currentAmmoForProjectiles.Length; i++)
            {
                currentAmmoForProjectiles[i] = m_projectiles[i].Ammo;
            }
        }

        protected virtual void Start()
        {
            InitializeTurret();
        }

        protected virtual void Update()
        {
            if (fireTimer > 0) fireTimer -= Time.deltaTime;
        }

        #region ProjectileChange

        public void ChangeProjectile(int index)
        {
            if (!isOwned) return;

            if (isClient) CmdChangeProjectile(index);
        }

        [Command]
        private void CmdChangeProjectile(int index)
        {
            if (index == activeProjectileIndex) return;

            SvChangeProjectile(index);
        }

        [Server]
        protected void SvChangeProjectile(int index)
        {
            currentAmmoForProjectiles[activeProjectileIndex] = m_ammoCount;

            activeProjectileIndex = index;
            m_ammoCount = currentAmmoForProjectiles[activeProjectileIndex];

            RpcProjectileChanged(activeProjectileIndex, m_ammoCount);
        }

        [ClientRpc]
        private void RpcProjectileChanged(int index, int ammo)
        {
            ProjectileChanged?.Invoke(index, ammo);
        }

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
