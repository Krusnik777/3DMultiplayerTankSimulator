using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UITurretInfo : MonoBehaviour
    {
        [SerializeField] private Sprite[] m_projectilesSprites;
        [SerializeField] private Image m_projectileIcon;
        [SerializeField] private Text m_text;

        private Turret m_turret;

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDisable()
        {
            if (NetworkSessionManager.Instance != null)
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

            if (m_turret != null)
            {
                m_turret.AmmoChanged -= OnAmmoChanged;
            }
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            m_turret = vehicle.Turret;

            m_text.text = m_turret.AmmoCount.ToString();
            m_projectileIcon.sprite = m_projectilesSprites[0];

            m_turret.AmmoChanged += OnAmmoChanged;
        }

        private void OnAmmoChanged(int ammo)
        {
            m_text.text = ammo.ToString();
        }
    }
}
