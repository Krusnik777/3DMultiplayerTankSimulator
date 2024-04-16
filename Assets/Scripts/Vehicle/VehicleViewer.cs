using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Vehicle))]
    public class VehicleViewer : NetworkBehaviour
    {
        [SerializeField] private float m_xrayDistance = 50.0f;
        [SerializeField] private float m_viewDistance;
        [SerializeField] private float m_exitTimeFromDiscovery = 10.0f;
        [SerializeField] private Transform[] m_viewPoints;
        [SerializeField] private Color m_color; // For DEBUG

        private List<VehicleDimensions> allVehicleDimensions = new List<VehicleDimensions>();

        private readonly SyncList<NetworkIdentity> visibleVehicles = new SyncList<NetworkIdentity>();

        private List<float> remainingTimes = new List<float>();

        private Vehicle m_vehicle;

        public bool IsVisible(NetworkIdentity identity) => visibleVehicles.Contains(identity);

        private bool CheckVisibility(Vector3 point, VehicleDimensions dimensions)
        {
            var distance = Vector3.Distance(transform.position, dimensions.transform.position);

            if (distance > m_viewDistance) return false;

            return dimensions.IsVisibleFromPoint(transform.root, point, m_color);
            
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            m_vehicle = GetComponent<Vehicle>();

            NetworkSessionManager.Match.SvMatchStart += OnSvMatchStart;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (NetworkSessionManager.Match != null)
                NetworkSessionManager.Match.SvMatchStart -= OnSvMatchStart;
        }

        private void Update()
        {
            if (!isServer) return;

            foreach (var dimensions in allVehicleDimensions)
            {
                if (dimensions.Vehicle == null) continue;

                bool isVisible = false;

                if (Vector3.Distance(m_vehicle.transform.position, dimensions.transform.position) < m_xrayDistance) isVisible = true;

                if (!isVisible)
                {
                    foreach (var point in m_viewPoints)
                    {
                        isVisible = CheckVisibility(point.position, dimensions);

                        if (isVisible) break;
                    }
                }

                if (isVisible && !visibleVehicles.Contains(dimensions.Vehicle.netIdentity))
                {
                    visibleVehicles.Add(dimensions.Vehicle.netIdentity);
                    remainingTimes.Add(-1);
                }

                if (isVisible && visibleVehicles.Contains(dimensions.Vehicle.netIdentity))
                {
                    remainingTimes[visibleVehicles.IndexOf(dimensions.Vehicle.netIdentity)] = -1;
                }

                if (!isVisible && visibleVehicles.Contains(dimensions.Vehicle.netIdentity))
                {
                    if (remainingTimes[visibleVehicles.IndexOf(dimensions.Vehicle.netIdentity)] == -1)
                        remainingTimes[visibleVehicles.IndexOf(dimensions.Vehicle.netIdentity)] = m_exitTimeFromDiscovery;
                    //visibleVehicles.Remove(dimensions.Vehicle.netIdentity);
                }
            }

            for (int i = 0; i < remainingTimes.Count; i++)
            {
                if (remainingTimes[i] > 0)
                {
                    remainingTimes[i] -= Time.deltaTime;
                    if (remainingTimes[i] <= 0) remainingTimes[i] = 0;
                }

                if (remainingTimes[i] == 0)
                {
                    remainingTimes.RemoveAt(i);
                    visibleVehicles.RemoveAt(i);
                }
            }
        }

        private void OnSvMatchStart()
        {
            m_color = Random.ColorHSV();

            Vehicle[] allVehicles = FindObjectsOfType<Vehicle>();

            foreach(var vehicle in allVehicles)
            {
                if (m_vehicle == vehicle) continue;

                var dimensions = vehicle.GetComponent<VehicleDimensions>();

                if (dimensions == null) continue;

                if (m_vehicle.TeamId != vehicle.TeamId)
                    allVehicleDimensions.Add(dimensions);
                else
                {
                    visibleVehicles.Add(dimensions.Vehicle.netIdentity);
                    remainingTimes.Add(-1);
                }
            }
        }

        
    }
}
