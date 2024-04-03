using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private GameObject m_visualModel;
        [SerializeField] private float m_velocity;
        [SerializeField] private float m_lifeTime;
        [SerializeField] private float m_mass;
        [SerializeField] private float m_damage;
        [Range(0.0f, 1.0f)][SerializeField] private float m_damageScatter;
        [SerializeField] private float m_impactForce;
        [Header("Spread")]
        [SerializeField] private float m_spreadRadius = 0.2f;

        private const float RayAdvance = 1.1f;

        public NetworkIdentity Owner { get; set; } // Player

        private Vector2 spread;

        private void Start()
        {
            Destroy(gameObject, m_lifeTime);
            if (m_spreadRadius != 0)
                spread = new Vector2(Random.Range(-m_spreadRadius + 0.01f, m_spreadRadius), Random.Range(-m_spreadRadius + 0.01f, m_spreadRadius));
            else spread = Vector2.zero;
        }

        private void Update()
        {
            UpdateProjectile();
        }

        private void UpdateProjectile()
        {
            transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * m_mass)).normalized;

            Vector3 step = transform.forward * m_velocity * Time.deltaTime + new Vector3(spread.x, spread.y, 0);

            RaycastHit hit;

            // Raycast hit effect
            if (Physics.Raycast(transform.position, transform.forward, out hit, m_velocity * Time.deltaTime * RayAdvance))
            {
                transform.position = hit.point;

                var destructible = hit.transform.root.GetComponent<Destructible>();

                if (destructible != null)
                {
                    // is your projectile?
                    // if yes send command to server

                    if (NetworkSessionManager.Instance.IsServer)
                    {
                        float damage = m_damage + Random.Range(-m_damageScatter, m_damageScatter) * m_damageScatter;

                        destructible.SvApplyDamage((int)damage);

                        if (destructible.HitPoints <= 0)
                        {
                            if (Owner != null)
                            {
                                var player = Owner.GetComponent<Player>();

                                if (player != null) player.Frags++;
                            }
                        }
                    }
                }

                OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);

                return;
            }

            transform.position += step;
        }

        private void OnProjectileLifeEnd(Collider col, Vector3 pos, Vector3 normal)
        {
            m_visualModel.SetActive(false);
            enabled = false;
        }
    }
}
