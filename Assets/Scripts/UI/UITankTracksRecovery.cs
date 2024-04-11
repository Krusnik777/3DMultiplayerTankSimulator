using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UITankTracksRecovery : MonoBehaviour
    {
        [SerializeField] private Slider m_leftTrackSlider;
        [SerializeField] private Slider m_rightTrackSlider;

        private TrackModule m_trackModule;

        private Coroutine m_leftTrackRecoveryRoutine;
        private Coroutine m_rightTrackRecoveryRoutine;

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

            if (m_trackModule != null)
            {
                m_trackModule.LeftTrack.Destroyed -= OnLeftTrackDestroyed;
                m_trackModule.LeftTrack.Recovered -= OnLeftTrackRecovered;

                m_trackModule.RightTrack.Recovered -= OnRightTrackRecovered;
                m_trackModule.RightTrack.Destroyed -= OnRightTrackDestroyed;
            }
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            if (vehicle is not TrackTank) return;

            m_trackModule = vehicle.GetComponent<TrackModule>();

            m_trackModule.LeftTrack.Destroyed += OnLeftTrackDestroyed;
            m_trackModule.LeftTrack.Recovered += OnLeftTrackRecovered;

            m_trackModule.RightTrack.Recovered += OnRightTrackRecovered;
            m_trackModule.RightTrack.Destroyed += OnRightTrackDestroyed;
        }

        private void OnLeftTrackDestroyed(Destructible destructible)
        {
            if (m_leftTrackRecoveryRoutine != null) StopCoroutine(m_leftTrackRecoveryRoutine);

            if (m_trackModule.LeftTrack == null) return;

            m_leftTrackRecoveryRoutine = StartCoroutine(TrackRecovery(m_leftTrackSlider, m_trackModule.LeftTrack));
        }

        private void OnLeftTrackRecovered(Destructible destructible)
        {
            if (m_leftTrackRecoveryRoutine != null) StopCoroutine(m_leftTrackRecoveryRoutine);

            HideSlider(m_leftTrackSlider);
        }

        private void OnRightTrackDestroyed(Destructible destructible)
        {
            if (m_rightTrackRecoveryRoutine != null) StopCoroutine(m_rightTrackRecoveryRoutine);

            if (m_trackModule.RightTrack == null) return;

            m_rightTrackRecoveryRoutine = StartCoroutine(TrackRecovery(m_rightTrackSlider, m_trackModule.RightTrack));
        }

        private void OnRightTrackRecovered(Destructible destructible)
        {
            if (m_rightTrackRecoveryRoutine != null) StopCoroutine(m_rightTrackRecoveryRoutine);

            HideSlider(m_rightTrackSlider);
        }

        private void HideSlider(Slider slider)
        {
            slider.gameObject.SetActive(false);
        }

        private IEnumerator TrackRecovery(Slider slider, VehicleModule vehicleModule)
        {
            slider.value = 0;
            slider.gameObject.SetActive(true);

            while (true)
            {
                slider.value = vehicleModule.RecoveryRate;

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
