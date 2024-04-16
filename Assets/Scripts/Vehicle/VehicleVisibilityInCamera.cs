using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTanks
{
    public class VehicleVisibilityInCamera : MonoBehaviour
    {
        private List<Vehicle> allVehicles = new List<Vehicle>();

        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Match != null)
                NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        }

        private void Update()
        {
            foreach (var vehicle in allVehicles)
            {
                bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(vehicle.netIdentity);

                vehicle.SetVisible(isVisible);
            }
        }

        private void OnMatchStart()
        {
            Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

            allVehicles.Clear();

            foreach (var vehicle in vehicles)
            {
                if (vehicle == Player.Local.ActiveVehicle) continue;

                allVehicles.Add(vehicle);
            }
        }
    }
}
