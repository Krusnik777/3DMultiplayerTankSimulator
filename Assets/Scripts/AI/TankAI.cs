using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    public enum AIBehaviourType
    {
        Patrol,
        Support,
        InvaderBase,
        SmartInvader,
        Interceptor,
        HidingFromAttacks
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class TankAI : NetworkBehaviour
    {
        [SerializeField] private AIBehaviourType m_behaviourType;
        [SerializeField][Range(0.0f, 1.0f)] private float m_patrolChance;
        [SerializeField][Range(0.0f, 1.0f)] private float m_supportChance;
        [SerializeField][Range(0.0f, 1.0f)] private float m_invaderBaseChance;
        [SerializeField][Range(0.0f, 1.0f)] private float m_smartInvaderBaseChance;
        [SerializeField][Range(0.0f, 1.0f)] private float m_interceptorChance;
        [SerializeField] private Vehicle m_vehicle;
        [SerializeField] private AIMovement m_movement;
        [SerializeField] private AIShooter m_shooter;

        private Vehicle m_fireTarget;
        private Vector3 m_movementTarget;

        private int startCountTeamMember;
        private int countTeamMember;

        private TeamBase m_ownBase;

        private bool ignoreEnemies = false;

        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
            m_vehicle.Destroyed += OnVehicleDestroyed;

            m_movement.enabled = false;
            m_shooter.enabled = false;

            CalcTeamMembers();
            SetStartBehaviour();
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Match != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            }
            
            if (m_vehicle != null) m_vehicle.Destroyed -= OnVehicleDestroyed;
        }

        private void Update()
        {
            if (isServer)
            {
                UpdateBehaviour();
            }
        }

        private void OnMatchStart()
        {
            m_movement.enabled = true;
            m_shooter.enabled = true;

            var bases = FindObjectsOfType<TeamBase>();

            for (int i = 0; i <  bases.Length; i++)
            {
                if (bases[i].TeamId == m_vehicle.TeamId)
                {
                    m_ownBase = bases[i];
                    break;
                }
            }
        }

        private void OnVehicleDestroyed(Destructible dest)
        {
            m_movement.enabled = false;
            m_shooter.ResetTarget();
            m_shooter.enabled = false;
        }

        private void CalcTeamMembers()
        {
            Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i].TeamId == m_vehicle.TeamId)
                {
                    if (vehicles[i] != m_vehicle)
                    {
                        startCountTeamMember++;
                        vehicles[i].Destroyed += OnTeamMemberDestroyed;
                    }
                }
            }

            countTeamMember = startCountTeamMember;
        }

        #region Behaviour

        private void SetStartBehaviour()
        {
            float chance = Random.Range(0.0f, m_patrolChance + m_supportChance + m_invaderBaseChance + m_smartInvaderBaseChance);

            if (chance >= 0.0f && chance <= m_patrolChance)
            {
                StartBehaviour(AIBehaviourType.Patrol);
                return;
            }

            if (chance >= m_patrolChance && chance <= m_patrolChance + m_supportChance)
            {
                StartBehaviour(AIBehaviourType.Support);
                return;
            }

            if (chance >= m_patrolChance + m_supportChance && chance <= m_patrolChance + m_supportChance + m_invaderBaseChance)
            {
                StartBehaviour(AIBehaviourType.InvaderBase);
                return;
            }

            if (chance >= m_patrolChance + m_supportChance + m_invaderBaseChance && chance <= m_patrolChance + m_supportChance + m_invaderBaseChance + m_smartInvaderBaseChance)
            {
                StartBehaviour(AIBehaviourType.SmartInvader);
                return;
            }
        }

        private void StartBehaviour(AIBehaviourType type)
        {
            m_behaviourType = type;

            if (m_behaviourType == AIBehaviourType.InvaderBase)
            {
                m_movementTarget = AIPath.Instance.GetBasePoint(m_vehicle.TeamId);
            }

            if (m_behaviourType == AIBehaviourType.Patrol)
            {
                m_movementTarget = AIPath.Instance.GetRandomPatrolPoint();
            }

            if (m_behaviourType == AIBehaviourType.Support)
            {
                m_movementTarget = AIPath.Instance.GetRandomFirePoint(m_vehicle.TeamId);
            }

            if (m_behaviourType == AIBehaviourType.SmartInvader)
            {
                m_movementTarget = AIPath.Instance.GetRandomStartInvadePoint();
                ignoreEnemies = true;
            }

            if (m_behaviourType == AIBehaviourType.Interceptor)
            {
                m_movementTarget = m_ownBase.transform.position;
            }

            if (m_behaviourType == AIBehaviourType.HidingFromAttacks)
            {
                m_movementTarget = AIPath.Instance.GetRandomCover(m_vehicle.TeamId);
            }

            m_movement.ResetPath();
        }

        private void UpdateBehaviour()
        {
            if (m_vehicle.HasCriticalHealth && m_behaviourType != AIBehaviourType.HidingFromAttacks)
            {
                StartBehaviour(AIBehaviourType.HidingFromAttacks);
            }

            if (!ignoreEnemies)
                m_shooter.FindTarget();

            if (m_ownBase != null)
            {
                if (m_ownBase.CaptureLevel > 0)
                {
                    if (Random.value > 1 - m_interceptorChance && !m_vehicle.HasCriticalHealth)
                        StartBehaviour(AIBehaviourType.Interceptor);
                }
            }

            if (m_movement.ReachedDestination)
            {
                OnReachedDestination();
            }

            if (!m_movement.HasPath)
            {
                m_movement.SetDestination(m_movementTarget);
            }
        }

        private void OnReachedDestination()
        {
            if (m_behaviourType == AIBehaviourType.Patrol)
            {
                m_movementTarget = AIPath.Instance.GetRandomPatrolPoint();
            }

            if (m_behaviourType == AIBehaviourType.SmartInvader)
            {
                m_movementTarget = AIPath.Instance.GetBasePoint(m_vehicle.TeamId);
                ignoreEnemies = false;
            }

            if (m_behaviourType == AIBehaviourType.Interceptor)
            {
                if (m_ownBase != null)
                {
                    if (m_ownBase.CaptureLevel <= 0) SetStartBehaviour();
                    else m_movementTarget = m_ownBase.transform.position;
                }
            }

            if (m_behaviourType == AIBehaviourType.HidingFromAttacks)
            {
                if (m_vehicle.TargetedByEnemy) 
                    m_movementTarget = AIPath.Instance.GetRandomCover(m_vehicle.TeamId);

                // or do something else
            }

            m_movement.ResetPath();
        }

        private void OnTeamMemberDestroyed(Destructible dest)
        {
            countTeamMember--;
            dest.Destroyed -= OnTeamMemberDestroyed;

            if ((float)countTeamMember / (float)startCountTeamMember < 0.4f || countTeamMember <= 2)
            {
                StartBehaviour(AIBehaviourType.Patrol);
            }
        }

        #endregion
    }
}
