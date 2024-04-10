using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class GameEventCollector : NetworkBehaviour
    {
        public event UnityAction<Vehicle> PlayerVehicleSpawned;

        [Server]
        public void SvOnAddPlayer()
        {
            RpcOnAddPlayer();
        }

        [ClientRpc]
        public void RpcOnAddPlayer()
        {
            Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            PlayerVehicleSpawned?.Invoke(vehicle);
        }
    }
}
