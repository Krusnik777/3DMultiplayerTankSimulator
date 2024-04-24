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

        }

        private void TryFire()
        {

        }
    }
}
