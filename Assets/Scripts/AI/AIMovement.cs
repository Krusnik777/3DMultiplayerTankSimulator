using UnityEngine;
using UnityEngine.AI;

namespace MultiplayerTanks
{
    public static class TransformExtensions
    {
        public static Vector3 GetPositionZX(this Transform transform)
        {
            var pos = transform.position;
            pos.y = 0;
            return pos;
        }
    }

    public static class VectorExtensions
    {
        public static Vector3 GetPositionZX(this Vector3 vector)
        {
            var pos = vector;
            pos.y = 0;
            return pos;
        }
    }

    [RequireComponent(typeof(Vehicle))]
    public class AIMovement : MonoBehaviour
    {
        [SerializeField] private float m_stopDistance = 1;
        [SerializeField] private float m_pathUpdateRate = 0.33f;
        [SerializeField] private AIRaySensor m_sensorForward;
        [SerializeField] private AIRaySensor m_sensorBackward;
        [SerializeField] private AIRaySensor m_sensorLeft;
        [SerializeField] private AIRaySensor m_sensorRight;

        private Vehicle m_vehicle;

        private Vector3 m_target;

        private NavMeshPath m_path;
        private Vector3 nextPathPoint;
        private int cornerIndex;

        private bool hasPath;
        public bool HasPath => hasPath;
        private bool reachedDestination;
        public bool ReachedDestination => reachedDestination;

        private float timerUpdatePath;

        public void SetDestination(Vector3 target)
        {
            if (m_target == target && hasPath) return;

            m_target = target;

            CalculatePath(target);
        }

        public void ResetPath()
        {
            hasPath = false;
            reachedDestination = false;
        }

        private Vector3 GetReferenceMovementDirectionZX()
        {
            var tankPos = m_vehicle.transform.GetPositionZX();
            var targetPos = nextPathPoint.GetPositionZX();

            return (targetPos - tankPos).normalized;
        }

        private Vector3 GetTankDirectionZX()
        {
            var tankDir = m_vehicle.transform.forward.GetPositionZX();
            tankDir.Normalize();
            return tankDir;
        }

        private void Awake()
        {
            m_path = new NavMeshPath();
            m_vehicle = GetComponent<Vehicle>();
        }

        private void Update()
        {
            SetDestination(GameObject.FindGameObjectWithTag("Finish").transform.position);

            if (m_pathUpdateRate > 0)
            {
                timerUpdatePath += Time.deltaTime;

                if (timerUpdatePath > m_pathUpdateRate)
                {
                    CalculatePath(m_target);
                    timerUpdatePath = 0;
                }
            }

            UpdateTarget();

            MoveToTarget();
        }

        private void MoveToTarget()
        {
            if (nextPathPoint == null) return;

            if (reachedDestination)
            {
                m_vehicle.SetTargetControl(new Vector3(0, 1, 0));
                return;
            }

            float turnControl = 0;
            float forwardThrust = 1;

            var referenceDirection = GetReferenceMovementDirectionZX();
            var tankDirection = GetTankDirectionZX();

            var forwardSensorState = m_sensorForward.Raycast();
            var leftSensorState = m_sensorLeft.Raycast();
            var rightSensorState = m_sensorRight.Raycast();

            if (forwardSensorState.Item1)
            {
                forwardThrust = 0;

                if (!leftSensorState.Item1)
                {
                    turnControl = -1;
                    forwardThrust = -0.2f;
                }
                else if (!rightSensorState.Item1)
                {
                    turnControl = 1;
                    forwardThrust = -0.2f;
                }
                else
                {
                    forwardThrust = -1;
                }
            }
            else
            {
                turnControl = Mathf.Clamp(Vector3.SignedAngle(tankDirection, referenceDirection, Vector3.up), -90.0f, 90.0f) / 90.0f;

                float minSideDistance = 1;

                if (leftSensorState.Item1 && leftSensorState.Item2 < minSideDistance && turnControl < 0) turnControl = -turnControl;
                if (rightSensorState.Item1 && rightSensorState.Item2 < minSideDistance && turnControl < 0) turnControl = -turnControl;

                //if (turnControl > 0.5 || turnControl < -0.5f) forwardThrust = forwardThrust/2; // Correcting forward thrust
            }

            m_vehicle.SetTargetControl(new Vector3(turnControl, 0, forwardThrust));
        }

        private void CalculatePath(Vector3 target)
        {
            NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, m_path);

            hasPath = m_path.corners.Length > 0;
            reachedDestination = false;

            cornerIndex = 1;
        }

        private void UpdateTarget()
        {
            if (!hasPath) return;

            nextPathPoint = m_path.corners[cornerIndex];

            if (Vector3.Distance(transform.position, nextPathPoint) < m_stopDistance)
            {
                if (m_path.corners.Length - 1 > cornerIndex)
                {
                    cornerIndex++;
                    nextPathPoint = m_path.corners[cornerIndex];
                }
                else
                {
                    hasPath = false;
                    reachedDestination = true;
                }
            }

            for (int i = 0; i < m_path.corners.Length - 1; i++)
            {
                Debug.DrawLine(m_path.corners[i], m_path.corners[i + 1], Color.red);
            }
        }
    }
}
