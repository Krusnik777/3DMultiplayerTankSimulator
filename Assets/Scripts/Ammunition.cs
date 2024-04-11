using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Ammunition : NetworkBehaviour
    {
        [SerializeField] private ProjectileProperties m_projectileProperties;

        [SyncVar(hook = nameof(SyncAmmoCount))]
        [SerializeField] protected int syncAmmoCount;

        public ProjectileProperties ProjectileProperties => m_projectileProperties;
        public int AmmoCount => syncAmmoCount;

        public event UnityAction<int> AmmoCountChanged;

        #region Server

        [Server]
        public void SvAddAmmo(int count)
        {
            syncAmmoCount += count;
        }

        [Server]
        public bool SvDrawAmmo(int count)
        {
            if (syncAmmoCount == 0) return false;

            if (syncAmmoCount >= count)
            {
                syncAmmoCount -= count;

                return true;
            }

            return false;
        }

        #endregion

        #region Client

        private void SyncAmmoCount(int oldValue, int newValue)
        {
            AmmoCountChanged?.Invoke(newValue);
        }

        #endregion
    }
}
