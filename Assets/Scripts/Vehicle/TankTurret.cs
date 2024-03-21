using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(TrackTank))]
    public class TankTurret : MonoBehaviour
    {
        [SerializeField] private Transform m_aim;
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

        public void Fire()
        {
            FireSFX();
        }

        private void Start()
        {
            m_tank = GetComponent<TrackTank>();
            m_tankRigidbody = m_tank.GetComponent<Rigidbody>();

            m_maxTopAngle = -m_maxTopAngle;
        }

        private void Update()
        {
            // TEMP
            if (Input.GetMouseButtonDown(0)) Fire();

            ControlTurretAim();
        }

        private void ControlTurretAim()
        {
            // Tower
            Vector3 locPos = m_tower.InverseTransformPoint(m_aim.position);
            locPos.y = 0;
            Vector3 globPos = m_tower.TransformPoint(locPos);

            m_tower.rotation = Quaternion.RotateTowards(m_tower.rotation, Quaternion.LookRotation((globPos - m_tower.position).normalized, m_tower.up), m_horizontalRotatationSpeed * Time.deltaTime);

            // Mask
            m_mask.localRotation = Quaternion.identity;

            locPos = m_mask.InverseTransformPoint(m_aim.position);
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
