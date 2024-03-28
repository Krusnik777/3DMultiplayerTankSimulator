using NetworkSpaceShooter;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIHealthText : MonoBehaviour
    {
        [SerializeField] private Text m_text;

        private Destructible m_destructible;

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

            if (m_destructible != null)
                m_destructible.HitPointsChange -= OnHitPointsChange;
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            m_destructible = vehicle;

            m_text.text = m_destructible.HitPoints.ToString();
            m_destructible.HitPointsChange += OnHitPointsChange;
        }

        private void OnHitPointsChange(int amount)
        {
            m_text.text = amount.ToString();
        }
    }
}
