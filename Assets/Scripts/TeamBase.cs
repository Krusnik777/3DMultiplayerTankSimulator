using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Collider))]
    public class TeamBase : MonoBehaviour
    {
        [SerializeField] private float m_captureLevel;
        [SerializeField] private float m_captureAmountPerVehicle;
        [SerializeField] private int m_teamId;
        [SerializeField] private List<Vehicle> m_allVehicles = new List<Vehicle>(); // Serialize for Debug

        public float CaptureLevel => m_captureLevel;

        public void Reset()
        {
            m_captureLevel = 0;

            for (int i = 0; i < m_allVehicles.Count; i++)
            {
                m_allVehicles[i].HitPointsChange -= OnHitPointsChange;
            }

            m_allVehicles.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            var vehicle = other.transform.root.GetComponent<Vehicle>();

            if (vehicle == null) return;

            if (vehicle.HitPoints == 0) return;

            if (m_allVehicles.Contains(vehicle)) return;

            if (vehicle.Owner.GetComponent<Player>().TeamId == m_teamId) return;

            vehicle.HitPointsChange += OnHitPointsChange;

            m_allVehicles.Add(vehicle);
        }

        private void OnTriggerExit(Collider other)
        {
            var vehicle = other.transform.root.GetComponent<Vehicle>();

            if (vehicle == null) return;

            vehicle.HitPointsChange -= OnHitPointsChange;

            m_allVehicles.Remove(vehicle);
        }

        private void Update()
        {
            if (NetworkSessionManager.Instance.IsServer)
            {
                bool isAllDead = true;

                for (int i = 0; i < m_allVehicles.Count; i++)
                {
                    if (m_allVehicles[i].HitPoints != 0)
                    {
                        isAllDead = false;

                        m_captureLevel += m_captureAmountPerVehicle * Time.deltaTime;
                        m_captureLevel = Mathf.Clamp(m_captureLevel, 0, 100);
                    }
                }

                if (m_allVehicles.Count == 0 || isAllDead)
                {
                    m_captureLevel = 0;
                }
            }
        }

        private void OnHitPointsChange(int hitPoints)
        {
            m_captureLevel = 0;
        }
    }
}
