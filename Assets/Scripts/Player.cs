using UnityEngine;
using UnityEngine.Events;
using Mirror;
using NetworkSpaceShooter;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Vehicle m_vehiclePrefab;
        [Header("Player")]
        [SyncVar(hook = nameof(OnNicknameChanged))]
        public string Nickname;
        [SyncVar]
        [SerializeField] private int m_teamId;
        public int TeamId => m_teamId;
        public UnityAction<Vehicle> VehicleSpawned;

        public Vehicle ActiveVehicle { get; set; }

        public static Player Local
        {
            get
            {
                var player = NetworkClient.localPlayer;

                if (player != null) return player.GetComponent<Player>();

                return null;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            m_teamId = TeamIdCounter % 2;
            TeamIdCounter++;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isOwned)
            {
                CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);
            }
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                if (ActiveVehicle != null)
                {
                    ActiveVehicle.SetVisible(!VehicleCamera.Instance.IsZoomed);
                }
            }

            if (isServer)
            {
                if (Input.GetKeyDown(KeyCode.F9))
                {
                    foreach(var p in FindObjectsOfType<Player>())
                    {
                        if (p.ActiveVehicle != null)
                        {
                            var vehicle = p.ActiveVehicle;

                            NetworkServer.UnSpawn(vehicle.gameObject);
                            Destroy(vehicle.gameObject);

                            p.ActiveVehicle = null;
                        }
                    }

                    foreach(var p in FindObjectsOfType<Player>())
                    {
                        p.SvSpawnClientVehicle();
                    }
                }
            }

            if (isOwned)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Cursor.lockState = Cursor.lockState != CursorLockMode.Locked ? CursorLockMode.Locked : CursorLockMode.None;
                }
            }
        }

        [Server]
        public void SvSpawnClientVehicle()
        {
            if (ActiveVehicle != null) return;

            GameObject playerVehicle = Instantiate(m_vehiclePrefab.gameObject, transform.position, Quaternion.identity);

            playerVehicle.transform.position = m_teamId % 2 == 0 ? NetworkSessionManager.Instance.RandomSpawnPointRed : NetworkSessionManager.Instance.RandomSpawnPointBlue;

            NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

            ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
            ActiveVehicle.Owner = netIdentity;

            RpcSetVehicle(ActiveVehicle.netIdentity);
        }

        [ClientRpc]
        private void RpcSetVehicle(NetworkIdentity vehicle)
        {
            if (vehicle == null) return;

            ActiveVehicle = vehicle.GetComponent<Vehicle>();
            
            if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
            {
                VehicleCamera.Instance.SetTarget(ActiveVehicle);
            }

            VehicleSpawned?.Invoke(ActiveVehicle);
        }

        #region Nickname

        [Command]
        public void CmdSetName(string name)
        {
            Nickname = name;
            gameObject.name = "Player_" + name;
        }

        private void OnNicknameChanged(string oldValue, string newValue)
        {
            gameObject.name = "Player_" + newValue; // on Client
        }

        #endregion

        #region TeamId

        private static int TeamIdCounter;

        [Command]
        public void CmdSetTeamID(int teamID)
        {
            m_teamId = teamID;
        }

        #endregion

        #region SetAim

        [SyncVar]
        private Vector3 netAimPoint;

        public Vector3 NetAimPoint
        {
            get => netAimPoint;

            set
            {
                netAimPoint = value; // Client
                CmdSetNetAimPoint(value); // Server
            }
        }

        [Command]
        private void CmdSetNetAimPoint(Vector3 v)
        {
            SvSetNetAimPoint(v);
        }

        [Server]
        private void SvSetNetAimPoint(Vector3 v)
        {
            netAimPoint = v;
            ActiveVehicle.SetNetAim(netAimPoint);
            RpcSetNetAimPoint(v);
        }

        [ClientRpc]
        private void RpcSetNetAimPoint(Vector3 v)
        {
            netAimPoint = v;
            ActiveVehicle.SetNetAim(netAimPoint);
        }

        #endregion

    }
}
