using UnityEngine;
using Mirror;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Vehicle : Destructible
    {
        public enum VehicleType
        {
            Ship,
            Tank
        }

        [SerializeField] private VehicleType m_type;
        public VehicleType Type => m_type;
        /// <summary>
        /// Mass for rigid
        /// </summary>
        [Header("VehicleParams")]
        [SerializeField] private float m_mass;

        /// <summary>
        /// Driving forward force
        /// </summary>
        [SerializeField] private float m_thrust;

        /// <summary>
        /// Rotation force
        /// </summary>
        [SerializeField] private float m_mobility;

        /// <summary>
        /// Maximal linear velocity, grad/sec
        /// </summary>
        [SerializeField] private float m_maxLinearVelocity;
        public float Speed => m_maxLinearVelocity;

        /// <summary>
        /// Maximal angular velocity
        /// </summary>
        [SerializeField] private float m_maxAngularVelocity;
        public float Agility => m_maxAngularVelocity;

        [SerializeField] private Turret m_turret;
        public Turret Turret => m_turret;

        /// <summary>
        /// Saved reference to rigid
        /// </summary>
        private Rigidbody2D m_rigid;

        #region PublicAPI

        /// <summary>
        /// Thrust (driving forward force) Control: -1.0 to 1.0
        /// </summary>
        public float ThrustControl { get; set; }

        /// <summary>
        /// Torque (rotation force) Control: -1.0 to 1.0
        /// </summary>
        public float TorqueControl { get; set; }

        public Vector2 HeadingDirection { get; set; }

        public float MovingSpeed => m_rigid.velocity.magnitude;

        public void Fire()
        {
            m_turret.CmdFire();
        }

        public void FireRocket()
        {
            m_turret.CmdFireRocket();
        }

        #endregion

        #region UnityEvents

        private void Start()
        {
            m_rigid = GetComponent<Rigidbody2D>();
            m_rigid.mass = m_mass;

            if (m_type == VehicleType.Ship) m_rigid.inertia = 1;
            if (m_type == VehicleType.Tank) m_rigid.inertia = 0.1f;
        }

        private void FixedUpdate()
        {
            if (isOwned || netIdentity.connectionToClient == null)
            {
                UpdateRigidBody();
            }
        }

        #endregion

        /// <summary>
        /// Method for applying forces to ship movement
        /// </summary>
        private void UpdateRigidBody()
        {
            if (m_type == VehicleType.Ship)
            {
                m_rigid.AddForce(ThrustControl * m_thrust * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);
                m_rigid.AddForce(-m_rigid.velocity * (m_thrust / m_maxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

                Vector2 dir = HeadingDirection - new Vector2(transform.position.x, transform.position.y);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                //transform.rotation = targetRotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_mobility * Time.fixedDeltaTime);
            }

            if (m_type == VehicleType.Tank)
            {
                m_rigid.velocity = ThrustControl * m_thrust * transform.up * Time.fixedDeltaTime;

                m_rigid.AddTorque(TorqueControl * m_mobility * Time.fixedDeltaTime, ForceMode2D.Force);
                m_rigid.AddTorque(-m_rigid.angularVelocity * (m_mobility / m_maxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

                if (m_turret.Body != null)
                {
                    Vector2 dir = HeadingDirection - new Vector2(m_turret.Body.transform.position.x, m_turret.Body.transform.position.y);
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                    //transform.rotation = targetRotation;
                    m_turret.Body.transform.rotation = Quaternion.Slerp(m_turret.Body.transform.rotation, targetRotation, m_mobility * Time.fixedDeltaTime);
                }
            }
        }
    }
}
