using UnityEngine;

namespace MultiplayerTanks
{
    public class UIHiddenPanel : MonoBehaviour
    {
        [SerializeField] private GameObject m_icon;

        private Vehicle m_vehicle;

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDisable()
        {
            if (NetworkSessionManager.Instance != null)
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        }

        private void Update()
        {
            if (m_vehicle == null) return;

            m_icon.SetActive(m_vehicle.IsHidden);
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            m_vehicle = vehicle;
        }
    }
}
