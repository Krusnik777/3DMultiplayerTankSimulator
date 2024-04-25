using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Vehicle))]
    public class AIShooter : MonoBehaviour
    {
        [SerializeField] private VehicleViewer m_viewer;
        [SerializeField] private Transform m_firePosition;

        private Vehicle m_vehicle;
        private Vehicle m_target;
        private Transform m_lookTransform;

        public bool HasTarget => m_target != null;

        public void FindTarget()
        {
            var vehicles = m_viewer.GetAllVisibleVehicle();

            float minDist = float.MaxValue;
            int index = -1;

            for (int i = 0; i < vehicles.Count; i++)
            {
                if (vehicles[i].HitPoints == 0) continue;

                if (vehicles[i].TeamId == m_vehicle.TeamId) continue;

                float dist = Vector3.Distance(transform.position, vehicles[i].transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }

            if (index != -1)
            {
                m_target = vehicles[index];

                var vehicleDimensions = m_target.GetComponent<VehicleDimensions>();

                if (vehicleDimensions == null) return;

                m_lookTransform = vehicleDimensions.GetPriorityFirePoint();
                m_target.TargetedByEnemy = true;
            }
            else
            {
                ResetTarget();
            }
        }

        public void ResetTarget()
        {
            if (m_target != null) m_target.TargetedByEnemy = false;

            m_target = null;
            m_lookTransform = null;
        }

        private void Awake()
        {
            m_vehicle = GetComponent<Vehicle>();
        }

        private void Update()
        {
            LookOnTarget();
            TryFire();
        }

        private void LookOnTarget()
        {
            if (m_lookTransform == null) return;

            m_vehicle.NetAimPoint = m_lookTransform.position;
        }

        private void TryFire()
        {
            if (m_target == null) return;

            RaycastHit hit;

            if (Physics.Raycast(m_firePosition.position, m_firePosition.forward, out hit, 1000,1, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.transform.root == m_target.transform.root)
                {
                    m_vehicle.Turret.SvFire();
                    m_target.TargetedByEnemy = false;
                }
            }
        }
    }
}
