using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileProperties m_properties;
        [SerializeField] private ProjectileMovement m_movement;
        [SerializeField] private ProjectileHit m_hit;
        [Space(5)]
        [SerializeField] private GameObject m_visualModel;
        [Space(5)]
        [SerializeField] private float m_delayBeforeDestroy;
        [SerializeField] private float m_lifeTime;

        public NetworkIdentity Owner { get; set; }
        public ProjectileProperties Properties => m_properties;
        //public Vector2 Spread { get; private set; }

        public void SetProperties(ProjectileProperties properties) => m_properties = properties;

        private void Start()
        {
            Destroy(gameObject, m_lifeTime);
            /*
            if (m_spreadRadius != 0)
                spread = new Vector2(Random.Range(-m_spreadRadius + 0.01f, m_spreadRadius), Random.Range(-m_spreadRadius + 0.01f, m_spreadRadius));
            else spread = Vector2.zero;*/
        }

        private void Update()
        {
            m_hit.Check();
            m_movement.Move();

            if (m_hit.IsHit) OnHit();
        }

        private void OnHit()
        {
            transform.position = m_hit.RaycastHit.point;

            if (NetworkSessionManager.Instance.IsServer)
            {
                ProjectileHitResult hitResult = m_hit.GetHitResult();

                if (hitResult.type == ProjectileHitType.Penetration || hitResult.type == ProjectileHitType.ModulePenetration || m_properties.Type == ProjectileType.HighExplosive && hitResult.type != ProjectileHitType.Environment)
                {
                    SvTakeDamage(hitResult);

                    SvAddFrags();
                }

                if (Owner != null)
                {
                    Player player = Owner.GetComponent<Player>();

                    if (player != null) player.SvInvokeProjectileHit(hitResult);
                }
            }

            DestroyProjectile();
        }

        private void SvTakeDamage(ProjectileHitResult hitResult)
        {
            m_hit.HittedArmor.Destructible.SvApplyDamage((int)hitResult.damage);
        }

        private void SvAddFrags()
        {
            if (m_hit.HittedArmor.Type == ArmorType.Module) return;

            if (m_hit.HittedArmor.Destructible.HitPoints <= 0)
            {
                if (Owner != null)
                {
                    var member = Owner.GetComponent<MatchMember>();

                    if (member != null) member.SvAddFrags();
                }
            }
        }

        private void DestroyProjectile()
        {
            m_visualModel.SetActive(false);
            enabled = false;

            Destroy(gameObject, m_delayBeforeDestroy);
        }

    }
}
