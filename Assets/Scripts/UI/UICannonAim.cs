using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UICannonAim : MonoBehaviour
    {
        [SerializeField] private Image m_aim;

        [SerializeField] private Image m_reloadSlider;

        private Vector3 aimPosition;

        private Turret m_turret;

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDisable()
        {
            if (NetworkSessionManager.Instance != null)
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            m_turret = vehicle.Turret;

            m_reloadSlider.fillAmount = m_turret.FireTimerNormalized;

            StartCoroutine(UpdateReloadSlider());
        }

        private void Update()
        {
            if (m_turret == null || Player.Local == null || Player.Local.ActiveVehicle == null) return;

            aimPosition = VehicleInputControl.TraceAimPointWithoutPlayerVehicle(m_turret.LaunchPoint.position, m_turret.LaunchPoint.forward);

            var result = Camera.main.WorldToScreenPoint(aimPosition);

            if (result.z > 0)
            {
                result.z = 0;

                m_aim.transform.position = result;
            }

        }

        private IEnumerator UpdateReloadSlider()
        {
            while (true)
            {
                if (m_turret == null) break;

                m_reloadSlider.fillAmount = m_turret.FireTimerNormalized;

                yield return new WaitForSeconds(0.1f);
            }
        }

    }
}
