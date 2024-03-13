using UnityEngine;
using Mirror;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Vehicle[] m_vehiclePrefabs;

        public Vehicle ActiveVehicle { get; set; }

        [SyncVar]
        private Color playerColor;
        public Color PlayerColor => playerColor;

        public override void OnStartServer()
        {
            base.OnStartServer();

            playerColor = PlayerColorPallete.Instance.TakeRandomColor();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            PlayerColorPallete.Instance.PutColor(playerColor);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isOwned)
            {
                CmdSpawnVehicle();
            }
        }

        [Command]
        private void CmdSpawnVehicle()
        {
            SvSpawnClientVehicle();
        }

        [Server]
        public void SvSpawnClientVehicle()
        {
            if (ActiveVehicle != null) return;

            int index = Random.Range(0, m_vehiclePrefabs.Length);

            GameObject playerVehicle = Instantiate(m_vehiclePrefabs[index].gameObject, transform.position, Quaternion.identity);
            NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

            ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
            ActiveVehicle.Owner = netIdentity;

            RpcSetVehicle(ActiveVehicle.netIdentity);
        }

        [ClientRpc]
        private void RpcSetVehicle(NetworkIdentity vehicle)
        {
            ActiveVehicle = vehicle.GetComponent<Vehicle>();

            if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
            {
                VehicleCamera.Instance.SetTarget(ActiveVehicle.transform);
            }
        }

    }
}
