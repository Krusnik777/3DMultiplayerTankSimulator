using UnityEngine;
using Mirror;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Vehicle : Destructible
    {
        /// <summary>
        /// Mass for rigid
        /// </summary>
        [Header("Space ship")]
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

        public Vector2 MovingSpeed => m_rigid.velocity;

        public void Fire()
        {
            m_turret.CmdFire();
        }

        #endregion

        #region UnityEvents

        private void Start()
        {
            m_rigid = GetComponent<Rigidbody2D>();
            m_rigid.mass = m_mass;

            m_rigid.inertia = 1;
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
            m_rigid.AddForce(ThrustControl * m_thrust * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);

            m_rigid.AddForce(-m_rigid.velocity * (m_thrust/m_maxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

            m_rigid.AddTorque(TorqueControl * m_mobility * Time.fixedDeltaTime, ForceMode2D.Force);

            m_rigid.AddTorque(-m_rigid.angularVelocity * (m_mobility / m_maxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }
}
