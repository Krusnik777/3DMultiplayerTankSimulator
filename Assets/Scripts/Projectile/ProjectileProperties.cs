using UnityEngine;

namespace MultiplayerTanks
{
    public enum ProjectileType
    {
        ArmorPiercing,
        HighExplosive,
        Subcaliber
    }

    [CreateAssetMenu]
    public class ProjectileProperties : ScriptableObject
    {
        [SerializeField] private ProjectileType m_type;
        [Header("Common")]
        [SerializeField] private Projectile m_projectilePrefab;
        [Header("Movement")]
        [SerializeField] private float m_velocity;
        [SerializeField] private float m_mass;
        [SerializeField] private float m_impactForce;
        [Header("Damage")]
        [SerializeField] private float m_damage;
        [SerializeField][Range(0.0f, 1.0f)] private float m_damageSpread;

        public ProjectileType Type => m_type;

        public Projectile ProjectilePrefab => m_projectilePrefab;

        public float Velocity => m_velocity;
        public float Mass => m_mass;
        public float ImpactForce => m_impactForce;

        public float Damage => m_damage;
        public float DamageSpread => m_damageSpread;

        public float GetSpreadDamage() => m_damage + Random.Range(1 - m_damageSpread, 1 + m_damageSpread);

        /*
        [Header("Spread")]
        [SerializeField] private float m_spreadRadius = 0.2f;
        public float SpreadRadius => m_spreadRadius;*/
    }
}
