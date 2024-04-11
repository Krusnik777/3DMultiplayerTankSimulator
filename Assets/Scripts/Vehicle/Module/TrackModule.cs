using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(TrackTank))]
    public class TrackModule : NetworkBehaviour
    {
        [Header("Visual")]
        [SerializeField] private GameObject m_leftTrackMesh;
        [SerializeField] private GameObject m_leftTrackBrokenMesh;
        [SerializeField] private GameObject m_rightTrackMesh;
        [SerializeField] private GameObject m_rightTrackBrokenMesh;
        [Space(5)]
        [SerializeField] private VehicleModule m_leftTrack;
        [SerializeField] private VehicleModule m_rightTrack;

        private TrackTank m_tank;

        public VehicleModule LeftTrack => m_leftTrack;
        public VehicleModule RightTrack => m_rightTrack;

        private void Start()
        {
            m_tank = GetComponent<TrackTank>();

            m_leftTrack.Destroyed += OnLeftTrackDestroyed;
            m_rightTrack.Destroyed += OnRightTrackDestroyed;

            m_leftTrack.Recovered += OnLeftTrackRecovered;
            m_rightTrack.Recovered += OnRightTrackRecovered;
        }

        private void OnDestroy()
        {
            m_leftTrack.Destroyed -= OnLeftTrackDestroyed;
            m_rightTrack.Destroyed -= OnRightTrackDestroyed;

            m_leftTrack.Recovered -= OnLeftTrackRecovered;
            m_rightTrack.Recovered -= OnRightTrackRecovered;
        }

        private void OnLeftTrackDestroyed(Destructible destructible)
        {
            ChangeActiveObjects(m_leftTrackMesh, m_leftTrackBrokenMesh);

            TakeAwayMobility();
        }

        private void OnLeftTrackRecovered(Destructible destructible)
        {
            ChangeActiveObjects(m_leftTrackBrokenMesh, m_leftTrackMesh);

            if (m_rightTrack.HitPoints > 0)
            {
                RegainMobility();
            }
        }

        private void OnRightTrackDestroyed(Destructible destructible)
        {
            ChangeActiveObjects(m_rightTrackMesh, m_rightTrackBrokenMesh);

            TakeAwayMobility();
        }

        private void OnRightTrackRecovered(Destructible destructible)
        {
            ChangeActiveObjects(m_rightTrackBrokenMesh, m_rightTrackMesh);

            if (m_leftTrack.HitPoints > 0)
            {
                RegainMobility();
            }
        }

        private void ChangeActiveObjects(GameObject prevVisual, GameObject newVisual)
        {
            prevVisual.SetActive(false);
            newVisual.SetActive(true);
        }

        private void TakeAwayMobility()
        {
            m_tank.IsStopped = true;
            m_tank.enabled = false;
        }

        private void RegainMobility()
        {
            m_tank.IsStopped = false;
            m_tank.enabled = true;
        }

    }
}
