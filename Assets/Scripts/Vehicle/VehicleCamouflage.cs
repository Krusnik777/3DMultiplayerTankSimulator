using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Vehicle))]
    public class VehicleCamouflage : MonoBehaviour
    {
        [SerializeField] private float m_baseDistance;
        [SerializeField][Range(0.0f, 1.0f)] private float m_percent; // SerializeField for DEBUG
        [SerializeField] private float m_percentLerpRate;
        [SerializeField] private float m_percentOfFire;

        private Vehicle m_vehicle;

        private float targetPercent;
        private float currentDistance;
        public float CurrentDistance => currentDistance;

        private void Start()
        {
            if (!NetworkSessionManager.Instance.IsServer) return;

            m_vehicle = GetComponent<Vehicle>();
            m_vehicle.Turret.Fired += OnFired;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                if (!NetworkSessionManager.Instance.IsServer) return;

                if (m_vehicle != null && m_vehicle.Turret != null)
                    m_vehicle.Turret.Fired -= OnFired;
            }
        }

        private void Update()
        {
            if (!NetworkSessionManager.Instance.IsServer) return;

            if (m_vehicle.NormalizedLinearVelocity > 0.01f) targetPercent = 0.5f;

            if (m_vehicle.NormalizedLinearVelocity <= 0.01f) targetPercent = 1.0f;

            m_percent = Mathf.MoveTowards(m_percent, targetPercent, m_percentLerpRate * Time.deltaTime);
            m_percent = Mathf.Clamp01(m_percent);

            currentDistance = m_baseDistance * m_percent;
        }

        private void OnFired()
        {
            m_percent = m_percentOfFire;
        }
    }
}
