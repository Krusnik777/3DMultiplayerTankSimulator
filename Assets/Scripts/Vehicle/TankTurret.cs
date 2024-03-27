using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(TrackTank))]
    public class TankTurret : Turret
    {
        [SerializeField] private Transform m_tower;
        [SerializeField] private Transform m_mask;
        [SerializeField] private float m_horizontalRotatationSpeed;
        [SerializeField] private float m_verticalRotatationSpeed;
        [SerializeField] private float m_maxTopAngle;
        [SerializeField] private float m_maxButtonAngle;
        [Header("SFX")]
        [SerializeField] private AudioSource m_fireSound;
        [SerializeField] private ParticleSystem m_muzzle;
        [SerializeField] private float m_forceRecoil;

        private TrackTank m_tank;
        private Rigidbody m_tankRigidbody;

        private float maskCurrentAngle;

        protected override void Start()
        {
            base.Start();

            m_tank = GetComponent<TrackTank>();
            m_tankRigidbody = m_tank.GetComponent<Rigidbody>();

            m_maxTopAngle = -m_maxTopAngle;
        }

        protected override void Update()
        {
            base.Update();

            ControlTurretAim();
        }

        protected override void OnFire()
        {
            base.OnFire();

            var projectile = Instantiate(ActiveProjectile.gameObject);

            projectile.transform.position = m_launchPoint.position;
            projectile.transform.forward = m_launchPoint.forward;

            FireSFX();
        }

        private void ControlTurretAim()
        {
            // Tower
            Vector3 locPos = m_tower.InverseTransformPoint(m_tank.NetAimPoint);
            locPos.y = 0;
            Vector3 globPos = m_tower.TransformPoint(locPos);

            m_tower.rotation = Quaternion.RotateTowards(m_tower.rotation, Quaternion.LookRotation((globPos - m_tower.position).normalized, m_tower.up), m_horizontalRotatationSpeed * Time.deltaTime);

            // Mask
            m_mask.localRotation = Quaternion.identity;

            locPos = m_mask.InverseTransformPoint(m_tank.NetAimPoint);
            locPos.x = 0;
            globPos = m_mask.TransformPoint(locPos);

            float targetAngle = -Vector3.SignedAngle((globPos - m_mask.position).normalized, m_mask.forward, m_mask.right);
            targetAngle = Mathf.Clamp(targetAngle, m_maxTopAngle, m_maxButtonAngle);
            maskCurrentAngle = Mathf.MoveTowards(maskCurrentAngle, targetAngle, Time.deltaTime * m_verticalRotatationSpeed);
            m_mask.localRotation = Quaternion.Euler(maskCurrentAngle, 0, 0);
        }

        private void FireSFX()
        {
            m_fireSound.Play();
            m_muzzle.Play();

            m_tankRigidbody.AddForceAtPosition(-m_mask.forward * m_forceRecoil, m_mask.position, ForceMode.Impulse);
        }
    }
}
