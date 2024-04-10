using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Projectile))]
    public class ProjectileHit : MonoBehaviour
    {
        private const float RAY_ADVANCE = 1.1f;

        private Projectile m_projectile;

        private bool isHit;
        public bool IsHit => isHit;

        private RaycastHit raycastHit;
        public RaycastHit RaycastHit => raycastHit;

        private Destructible hittedDestructible;
        public Destructible HittedDestuctible => hittedDestructible;

        public void Check()
        {
            if (isHit) return;

            if (Physics.Raycast(transform.position, transform.forward, out raycastHit, m_projectile.Properties.Velocity * Time.deltaTime * RAY_ADVANCE))
            {
                var destructible = raycastHit.transform.root.GetComponent<Destructible>();

                if (destructible != null)
                {
                    hittedDestructible = destructible;
                }

                isHit = true;
            }
        }

        private void Awake()
        {
            m_projectile = GetComponent<Projectile>();
        }
    }
}
