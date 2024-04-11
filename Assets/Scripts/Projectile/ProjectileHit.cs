using UnityEngine;

namespace MultiplayerTanks
{
    public enum ProjectileHitType
    {
        Penetration,
        NoPenetration,
        Ricochet,
        ModulePenetration,
        ModuleNoPenetration,
        Environment
    }

    public class ProjectileHitResult
    {
        public ProjectileHitType type;
        public float damage;
        public Vector3 point;
    }

    [RequireComponent(typeof(Projectile))]
    public class ProjectileHit : MonoBehaviour
    {
        private const float RAY_ADVANCE = 1.1f;

        private Projectile m_projectile;

        private bool isHit;
        public bool IsHit => isHit;

        private RaycastHit raycastHit;
        public RaycastHit RaycastHit => raycastHit;

        private Armor hittedArmor;
        public Armor HittedArmor => hittedArmor;

        public ProjectileHitResult GetHitResult()
        {
            ProjectileHitResult hitResult = new ProjectileHitResult();
            hitResult.damage = 0;

            if (hittedArmor == null)
            {
                hitResult.type = ProjectileHitType.Environment;
                hitResult.point = raycastHit.point;

                return hitResult;
            }

            float normalization = m_projectile.Properties.NormalizationAngle;

            if (m_projectile.Properties.Caliber > hittedArmor.Thickness * 2)
            {
                normalization = (m_projectile.Properties.NormalizationAngle * 1.4f * m_projectile.Properties.Caliber) / hittedArmor.Thickness;
            }

            float angle = Mathf.Abs(Vector3.SignedAngle(-m_projectile.transform.forward, raycastHit.normal, m_projectile.transform.right)) - normalization;
            float reducedArmor = hittedArmor.Thickness / Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectilePenetration = m_projectile.Properties.GetSpreadArmorPenetration();

            // Visual angles
            Debug.DrawRay(raycastHit.point, -m_projectile.transform.forward, Color.red);
            Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.green);
            Debug.DrawRay(raycastHit.point, m_projectile.transform.right, Color.yellow);

            if (angle > m_projectile.Properties.RicochetAngle && m_projectile.Properties.Caliber < hittedArmor.Thickness * 3 && hittedArmor.Type == ArmorType.Vehicle)
                hitResult.type = ProjectileHitType.Ricochet;
            else if (projectilePenetration >= reducedArmor)
            {
                hitResult.type = ProjectileHitType.Penetration;
                hitResult.damage = m_projectile.Properties.GetSpreadDamage();
            }
            else if (projectilePenetration < reducedArmor)
                hitResult.type = ProjectileHitType.NoPenetration;

            //Debug.LogError($"armor: {hittedArmor.Thickness}, reducedArmor: {reducedArmor}, angle: {angle}, norm: {normalization}, penetration: {projectilePenetration}, hitType: {hitResult.type}.");

            hitResult.point = raycastHit.point;

            if (hittedArmor.Type == ArmorType.Module)
            {
                if (hitResult.type == ProjectileHitType.Penetration)
                    hitResult.type = ProjectileHitType.ModulePenetration;

                if (hitResult.type == ProjectileHitType.NoPenetration)
                    hitResult.type = ProjectileHitType.ModuleNoPenetration;
            }

            if (m_projectile.Properties.Type == ProjectileType.HighExplosive)
            {
                if (hitResult.type == ProjectileHitType.Ricochet) hitResult.type = ProjectileHitType.NoPenetration;

                if (hitResult.type == ProjectileHitType.NoPenetration)
                {
                    hitResult.damage = m_projectile.Properties.GetSpreadDamage() - hittedArmor.Thickness;
                }

                if (hitResult.type == ProjectileHitType.ModuleNoPenetration)
                {
                    hitResult.damage = m_projectile.Properties.GetSpreadDamage() - hittedArmor.Thickness * 3;
                }
            }

            return hitResult;
        }

        public void Check()
        {
            if (isHit) return;

            if (Physics.Raycast(transform.position, transform.forward, out raycastHit, m_projectile.Properties.Velocity * Time.deltaTime * RAY_ADVANCE))
            {
                var armor = raycastHit.collider.GetComponent<Armor>();

                if (armor != null)
                {
                    hittedArmor = armor;
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
