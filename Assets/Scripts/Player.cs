using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [System.Serializable]
    public class PlayerData
    {
        public int Id;
        public string Nickname;
        public int TeamId;

        public PlayerData(int id, string nickname, int teamId)
        {
            Id = id;
            Nickname = nickname;
            TeamId = teamId;
        }
    }

    public static class PlayerDataWriteRead
    {
        public static void WritePlayerData(this NetworkWriter writer, PlayerData data)
        {
            writer.WriteInt(data.Id);
            writer.WriteString(data.Nickname);
            writer.WriteInt(data.TeamId);
        }

        public static PlayerData ReadPlayerData(this NetworkReader reader)
        {
            return new PlayerData(reader.ReadInt(), reader.ReadString(), reader.ReadInt());
        }
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Vehicle m_vehiclePrefab;
        [SerializeField] private VehicleInputControl m_vehicleInputControl;
        [Header("Player")]
        [SyncVar(hook = nameof(OnNicknameChanged))]
        public string Nickname;
        [SyncVar]
        [SerializeField] private int m_teamId;
        public int TeamId => m_teamId;

        public UnityAction<Vehicle> VehicleSpawned;

        public Vehicle ActiveVehicle { get; set; }

        private PlayerData m_playerData;
        public PlayerData Data => m_playerData;

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

        public override void OnStopServer()
        {
            base.OnStopServer();

            PlayerList.Instance.SvRemovePlayer(m_playerData);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isOwned)
            {
                CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

                NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

                m_playerData = new PlayerData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, m_teamId);

                CmdAddPlayer(m_playerData);

                CmdUpdatePlayerData(m_playerData);
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (isOwned)
            {
                NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
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
                    NetworkSessionManager.Match.SvRestartMatch();
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
            ActiveVehicle.Owner = netIdentity;
            
            if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
            {
                VehicleCamera.Instance.SetTarget(ActiveVehicle);
            }

            m_vehicleInputControl.enabled = true;

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

        #region PlayerList

        [Command]
        private void CmdAddPlayer(PlayerData data)
        {
            PlayerList.Instance.SvAddPlayer(data);
        }

        [Command]
        private void CmdUpdatePlayerData(PlayerData data)
        {
            m_playerData = data;
        }

        #endregion

        #region Frags

        public static UnityAction<int, int> ChangeFrags;

        [SyncVar(hook = nameof(OnFragsChanged))]
        private int m_frags;
        public int Frags
        {
            get => m_frags;
            set
            {
                m_frags = value;
                // Server
                ChangeFrags?.Invoke((int)netId, m_frags);
            }
        }

        // Client
        private void OnFragsChanged(int oldValue, int newValue)
        {
            ChangeFrags?.Invoke((int)netId,newValue);
        }

        #endregion

        private void OnMatchEnd()
        {
            if (ActiveVehicle != null)
            {
                ActiveVehicle.SetTargetControl(Vector3.zero);
                m_vehicleInputControl.enabled = false;
            }
        }
    }
}
