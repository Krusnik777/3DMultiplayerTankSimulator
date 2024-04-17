using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    [System.Serializable]
    public class TrackWheelRow
    {
        [SerializeField] private WheelCollider[] m_colliders;
        [SerializeField] private Transform[] m_meshes;

        public float MinRpm { get; set; }

        public void ApplyMotorTorque(float motorTorque)
        {
            for (int i = 0; i < m_colliders.Length; i++)
            {
                m_colliders[i].motorTorque = motorTorque;
            }
        }

        public void ApplyBrakeTorque(float brakeTorque)
        {
            for (int i = 0; i < m_colliders.Length; i++)
            {
                m_colliders[i].brakeTorque = brakeTorque;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < m_colliders.Length;i++)
            {
                m_colliders[i].brakeTorque = 0;
                m_colliders[i].motorTorque = 0;
            }
        }

        public void SetSidewayStiffness(float stiffness)
        {
            WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();

            for (int i = 0; i < m_colliders.Length; i++)
            {
                wheelFrictionCurve = m_colliders[i].sidewaysFriction;
                wheelFrictionCurve.stiffness = stiffness;

                m_colliders[i].sidewaysFriction = wheelFrictionCurve;
            }
        }

        public void UpdateMeshTransform()
        {
            // Find min rpm
            List<float> allRpm = new List<float>();

            foreach (var collider in m_colliders)
            {
                if (collider.isGrounded) allRpm.Add(collider.rpm);
            }

            if (allRpm.Count > 0)
            {
                MinRpm = Mathf.Abs(allRpm[0]);

                foreach (var rpm in allRpm)
                {
                    if (Mathf.Abs(rpm) < MinRpm) MinRpm = Mathf.Abs(rpm);
                }

                MinRpm = MinRpm * Mathf.Sign(allRpm[0]);
            }

            float angle = MinRpm * 360.0f / 60.0f * Time.fixedDeltaTime;

            for (int i = 0; i < m_meshes.Length; i++)
            {
                Vector3 position;
                Quaternion rotation;

                m_colliders[i].GetWorldPose(out position, out rotation);

                m_meshes[i].position = position;
                m_meshes[i].Rotate(angle, 0, 0);
            }
        }

        public void UpdateMeshRotationByRpm(float rpm)
        {
            float angle = rpm * 360.0f / 60.0f * Time.fixedDeltaTime;

            for (int i = 0; i < m_meshes.Length; i++)
            {
                Vector3 position;
                Quaternion rotation;

                m_colliders[i].GetWorldPose(out position, out rotation);

                m_meshes[i].position = position;
                m_meshes[i].Rotate(angle, 0, 0);
            }
        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public class TrackTank : Vehicle
    {
        [Header("VisualModel")]
        [SerializeField] private GameObject m_visualModel;
        [SerializeField] private GameObject m_destroyedPrefab;
        [Header("CenterOfMass")]
        [SerializeField] private Transform m_centerOfMass;
        [Header("Tracks")]
        [SerializeField] private TrackWheelRow m_leftWheelRow;
        [SerializeField] private TrackWheelRow m_rightWheelRow;
        [Header("Movement")]
        [SerializeField] private ParameterCurve m_forwardTorqueCurve;
        [SerializeField] private float m_maxForwardTorque;
        [SerializeField] private ParameterCurve m_backwardTorqueCurve;
        [SerializeField] private float m_maxBackwardTorque;
        [SerializeField] private float m_brakeTorque;
        [SerializeField] private float m_rollingResistance;
        [Header("Rotation")]
        [SerializeField] private float m_rotateTorqueInPlace;
        [SerializeField] private float m_rotateBrakeInPlace;
        [Space(2)]
        [SerializeField] private float m_rotateTorqueInMotion;
        [SerializeField] private float m_rotateBrakeInMotion;
        [Header("Friction")]
        [SerializeField] private float m_minSidewayStiffnessInPlace;
        [SerializeField] private float m_minSidewayStiffnessInMotion;

        private Rigidbody m_rigidBody;
        public Rigidbody Rigidbody => m_rigidBody == null ? GetComponent<Rigidbody>() : m_rigidBody;

        private float currentMotorTorque;

        public float LeftWheelRpm => m_leftWheelRow.MinRpm;
        public float RightWheelRpm => m_rightWheelRow.MinRpm;

        public override float LinearVelocity => Rigidbody.velocity.magnitude;

        private void Start()
        {
            m_rigidBody = GetComponent<Rigidbody>();
            m_rigidBody.centerOfMass = m_centerOfMass.localPosition;

            // To Reset SideWayStiffness
            m_leftWheelRow.SetSidewayStiffness(1.0f + m_minSidewayStiffnessInMotion - Mathf.Abs(1));
            m_rightWheelRow.SetSidewayStiffness(1.0f + m_minSidewayStiffnessInMotion - Mathf.Abs(1));

            Destroyed += OnDestroyed;
        }

        private void OnDestroy()
        {
            Destroyed -= OnDestroyed;
        }

        private void FixedUpdate()
        {
            if (isOwned)
            {
                UpdateMotorTorque();

                if (!IsStopped) CmdUpdateWheelRpm(LeftWheelRpm, RightWheelRpm);

                CmdUpdateLinearVelocity(LinearVelocity);
            }
        }

        private void UpdateMotorTorque()
        {
            float targetMotorTorque = (targetInputControl.z > 0 ? m_maxForwardTorque : m_maxBackwardTorque) * Mathf.RoundToInt(targetInputControl.z);
            float targetBrakeTorque = m_brakeTorque * targetInputControl.y;
            float targetSteering = targetInputControl.x;

            // Update target torque
            if (targetMotorTorque > 0)
            {
                currentMotorTorque = m_forwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
            }

            if (targetMotorTorque < 0)
            {
                currentMotorTorque = m_backwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
            }

            if (targetMotorTorque == 0)
            {
                currentMotorTorque = m_forwardTorqueCurve.Reset();
                currentMotorTorque = m_backwardTorqueCurve.Reset();
            }

            // Brake
            m_leftWheelRow.ApplyBrakeTorque(targetBrakeTorque);
            m_rightWheelRow.ApplyBrakeTorque(targetBrakeTorque);

            // Rolling
            if (targetMotorTorque == 0 && targetSteering == 0)
            {
                m_leftWheelRow.ApplyBrakeTorque(m_rollingResistance);
                m_rightWheelRow.ApplyBrakeTorque(m_rollingResistance);
            }
            else
            {
                m_leftWheelRow.Reset();
                m_rightWheelRow.Reset();
            }

            // Rotate in place
            if (targetMotorTorque == 0 && targetSteering != 0)
            {
                if (Mathf.Abs(m_leftWheelRow.MinRpm) < 1 || Mathf.Abs(m_rightWheelRow.MinRpm) < 1)
                {
                    m_leftWheelRow.ApplyMotorTorque(m_rotateTorqueInPlace);
                    m_rightWheelRow.ApplyMotorTorque(m_rotateTorqueInPlace);
                }
                else
                {
                    if (targetSteering < 0)
                    {
                        m_leftWheelRow.ApplyBrakeTorque(m_rotateBrakeInPlace);
                        m_rightWheelRow.ApplyMotorTorque(m_rotateTorqueInPlace);
                    }

                    if (targetSteering > 0)
                    {
                        m_leftWheelRow.ApplyMotorTorque(m_rotateTorqueInPlace);
                        m_rightWheelRow.ApplyBrakeTorque(m_rotateBrakeInPlace);
                    }
                }

                m_leftWheelRow.SetSidewayStiffness(1.0f + m_minSidewayStiffnessInPlace - Mathf.Abs(targetSteering));
                m_rightWheelRow.SetSidewayStiffness(1.0f + m_minSidewayStiffnessInPlace - Mathf.Abs(targetSteering));
            }

            // Move
            if (targetMotorTorque != 0)
            {
                if (targetSteering == 0)
                {
                    if (LinearVelocity < m_maxLinearSpeed)
                    {
                        m_leftWheelRow.ApplyMotorTorque(currentMotorTorque);
                        m_rightWheelRow.ApplyMotorTorque(currentMotorTorque);
                    }
                }

                if (targetSteering != 0 && (Mathf.Abs(m_leftWheelRow.MinRpm) < 1 || Mathf.Abs(m_rightWheelRow.MinRpm) < 1))
                {
                    m_leftWheelRow.ApplyMotorTorque(m_rotateTorqueInMotion);
                    m_rightWheelRow.ApplyMotorTorque(m_rotateTorqueInMotion);
                }
                else
                {
                    if (targetSteering < 0)
                    {
                        m_leftWheelRow.ApplyBrakeTorque(m_rotateBrakeInMotion);
                        m_rightWheelRow.ApplyMotorTorque(m_rotateTorqueInMotion);
                    }

                    if (targetSteering > 0)
                    {
                        m_leftWheelRow.ApplyMotorTorque(m_rotateTorqueInMotion);
                        m_rightWheelRow.ApplyBrakeTorque(m_rotateBrakeInMotion);
                    }
                }

                m_leftWheelRow.SetSidewayStiffness(1.0f + m_minSidewayStiffnessInMotion - Mathf.Abs(targetSteering));
                m_rightWheelRow.SetSidewayStiffness(1.0f + m_minSidewayStiffnessInMotion - Mathf.Abs(targetSteering));
            }

            m_leftWheelRow.UpdateMeshTransform();
            m_rightWheelRow.UpdateMeshTransform();
        }

        private void OnDestroyed(Destructible destuctible)
        {
            var destroyedModel = Instantiate(m_destroyedPrefab);
            destroyedModel.transform.position = m_visualModel.transform.position;
            destroyedModel.transform.rotation = m_visualModel.transform.rotation;
        }

        #region UpdateMeshesForServer

        [Command]
        private void CmdUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            SvUpdateWheelRpm(leftRpm,rightRpm);
        }

        [Server]
        private void SvUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            RpcUpdateWheelRpm(leftRpm, rightRpm);
        }

        [ClientRpc(includeOwner = false)]
        private void RpcUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            m_leftWheelRow.MinRpm = leftRpm;
            m_rightWheelRow.MinRpm = rightRpm;

            m_leftWheelRow.UpdateMeshRotationByRpm(leftRpm);
            m_rightWheelRow.UpdateMeshRotationByRpm(rightRpm);
        }

        #endregion

        [Command]
        private void CmdUpdateLinearVelocity(float velocity)
        {
            syncLinearVelocity = velocity;
        }

    }
}
