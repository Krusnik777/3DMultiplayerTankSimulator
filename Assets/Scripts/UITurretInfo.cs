using NetworkSpaceShooter;
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

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

            if (m_turret != null)
            {
                m_turret.AmmoChanged -= OnAmmoChanged;
                m_turret.ProjectileChanged -= OnProjectileChanged;
            }
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            m_turret = vehicle.Turret;

            m_turret.AmmoChanged += OnAmmoChanged;
            m_turret.ProjectileChanged += OnProjectileChanged;

            m_text.text = m_turret.AmmoCount.ToString();
            m_projectileIcon.sprite = m_projectilesSprites[m_turret.ActiveProjectileIndex];
        }

        private void OnAmmoChanged(int ammo)
        {
            m_text.text = ammo.ToString();
        }

        private void OnProjectileChanged(int index, int ammo)
        {
            m_text.text = ammo.ToString();
            m_projectileIcon.sprite = m_projectilesSprites[index];
        }
    }
}
