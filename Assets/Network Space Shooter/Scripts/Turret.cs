using UnityEngine;
using Mirror;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Turret : NetworkBehaviour
    {
        [SerializeField] private GameObject m_projectile;
        [SerializeField] private float m_fireRate;

        private float currentTime;

        private void Update()
        {
            if (isServer)
            {
                currentTime += Time.deltaTime;
            }
        }

        [Command]
        public void CmdFire()
        {
            SvFire();
        }

        [Server]
        private void SvFire()
        {
            if (currentTime < m_fireRate) return;

            GameObject projectile = Instantiate(m_projectile, transform.position, transform.rotation);
            projectile.GetComponent<Projectile>().SetParent(transform);

            currentTime = 0;

            RpcFire();
        }

        [ClientRpc]
        private void RpcFire()
        {
            GameObject projectile = Instantiate(m_projectile, transform.position, transform.rotation);
            projectile.GetComponent<Projectile>().SetParent(transform);
        }
    }
}
