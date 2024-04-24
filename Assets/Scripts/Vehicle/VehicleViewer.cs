using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Vehicle))]
    public class VehicleViewer : NetworkBehaviour
    {
        private const float UPDATE_INTERVAL = 0.33f;

        [SerializeField] private float m_camouflageDistance = 150.0f;
        [SerializeField] private float m_xrayDistance = 50.0f;
        [SerializeField] private float m_viewDistance = 300.0f;
        [SerializeField] private float m_hiddenDistance = 20.0f;
        [SerializeField] private float m_exitTimeFromDiscovery = 5.0f;
        [SerializeField] private Transform[] m_viewPoints;
        [SerializeField] private Color m_color; // For DEBUG

        private List<VehicleDimensions> allVehicleDimensions = new List<VehicleDimensions>();

        private readonly SyncList<NetworkIdentity> visibleVehicles = new SyncList<NetworkIdentity>();

        private List<float> remainingTimes = new List<float>();

        private Vehicle m_vehicle;
        private float remainingTimeLastUpdate;

        public List<Vehicle> GetAllVehicle()
        {
            var allVehicles = new List<Vehicle>(allVehicleDimensions.Count);

            for (int i = 0; i < allVehicleDimensions.Count; i++)
            {
                allVehicles.Add(allVehicleDimensions[i].Vehicle);
            }

            return allVehicles;
        }

        public List<Vehicle> GetAllVisibleVehicle()
        {
            var allVehicles = new List<Vehicle>(allVehicleDimensions.Count);

            for (int i = 0; i < visibleVehicles.Count; i++)
            {
                allVehicles.Add(visibleVehicles[i].GetComponent<Vehicle>());
            }

            return allVehicles;
        }

        public bool IsVisible(NetworkIdentity identity) => visibleVehicles.Contains(identity);

        private bool CheckVisibility(Vector3 point, VehicleDimensions dimensions)
        {
            var distance = Vector3.Distance(transform.position, dimensions.transform.position);

            if (dimensions.Vehicle.IsHidden && distance > m_hiddenDistance) return false;

            if (Vector3.Distance(point, dimensions.transform.position) <= m_xrayDistance) return true;

            if (distance > m_viewDistance) return false;

            var currentViewDistance = m_viewDistance;

            if (distance >= m_camouflageDistance)
            {
                var camouflage = dimensions.Vehicle.GetComponent<VehicleCamouflage>();

                if (camouflage != null) currentViewDistance = m_viewDistance - camouflage.CurrentDistance;
            }

            if (distance > currentViewDistance) return false;

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

            remainingTimeLastUpdate += Time.deltaTime;

            if (remainingTimeLastUpdate >= UPDATE_INTERVAL)
            {
                foreach (var dimensions in allVehicleDimensions)
                {
                    if (dimensions.Vehicle == null) continue;

                    bool isVisible = false;

                    foreach (var point in m_viewPoints)
                    {
                        isVisible = CheckVisibility(point.position, dimensions);

                        if (isVisible) break;
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

                remainingTimeLastUpdate = 0;
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
