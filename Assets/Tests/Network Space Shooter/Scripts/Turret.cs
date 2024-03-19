using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Turret : NetworkBehaviour
    {
        [SerializeField] private GameObject m_body;
        [SerializeField] private GameObject m_projectile;
        [SerializeField] private float m_fireRate;
        [Header("Rocket")]
        [SerializeField] private GameObject m_rocketProjectile;
        [SerializeField] private float m_rocketReloadTime;

        public UnityAction<float> RocketReloadTimeUp;

        public GameObject Body => m_body;

        private float currentTime;

        [SyncVar]
        private float rocketReadyTime;

        private void Update()
        {
            if (isServer)
            {
                currentTime += Time.deltaTime;
                rocketReadyTime += Time.deltaTime;
            }

            if (rocketReadyTime <= m_rocketReloadTime)
                RocketReloadTimeUp?.Invoke(rocketReadyTime / m_rocketReloadTime);
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

            if (m_projectile == null) return;

            InstantiateProjectile(m_projectile);

            currentTime = 0;

            RpcFire();
        }

        [ClientRpc]
        private void RpcFire()
        {
            InstantiateProjectile(m_projectile);
        }

        // Rocket

        [Command]
        public void CmdFireRocket()
        {
            SvFireRocket();
        }

        [Server]
        private void SvFireRocket()
        {
            if (rocketReadyTime < m_rocketReloadTime) return;

            if (m_rocketProjectile == null) return;

            InstantiateProjectile(m_rocketProjectile);

            currentTime = 0;
            rocketReadyTime = 0;

            RpcFireRocket();
        }

        [ClientRpc]
        private void RpcFireRocket()
        {
            InstantiateProjectile(m_rocketProjectile);
        }

        private void InstantiateProjectile(GameObject projectilePrefab)
        {
            if (m_body != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, m_body.transform.position, m_body.transform.rotation);
                projectile.GetComponent<Projectile>().SetParent(transform);
            }
            else
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
                projectile.GetComponent<Projectile>().SetParent(transform);
            }
        }
    }
}
