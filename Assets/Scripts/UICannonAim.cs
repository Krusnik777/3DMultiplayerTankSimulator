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

        private Coroutine reloadCouroutine;

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

            if (m_turret != null)
                m_turret.Fired -= OnFired;
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            m_turret = vehicle.Turret;

            m_reloadSlider.fillAmount = m_turret.FireTimerNormalized;

            m_turret.Fired += OnFired;
        }

        private void OnFired()
        {
            if (reloadCouroutine != null) StopCoroutine(reloadCouroutine);

            reloadCouroutine = StartCoroutine(UpdateReloadSlider());
        }

        private void Update()
        {
            if (m_turret == null) return;

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
            yield return new WaitUntil(() => m_turret.FireTimerNormalized > 0);

            m_reloadSlider.fillAmount = m_turret.FireTimerNormalized;

            while (m_turret.FireTimerNormalized > 0.05f)
            {
                m_reloadSlider.fillAmount = m_turret.FireTimerNormalized;

                yield return new WaitForFixedUpdate();
            }

            m_reloadSlider.fillAmount = 0;

            reloadCouroutine = null;
        }

    }
}
