using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{    
    public class Player : MatchMember
    {
        [SerializeField] private Vehicle m_vehiclePrefab;
        [SerializeField] private VehicleInputControl m_vehicleInputControl;

        public event UnityAction<Vehicle> VehicleSpawned;

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

            m_teamId = MatchController.GetNextTeam();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            MatchMemberList.Instance.SvRemoveMatchMember(m_data);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isOwned)
            {
                CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

                NetworkSessionManager.Match.MatchStart += OnMatchStart;
                NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

                m_data = new MatchMemberData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, m_teamId, netIdentity);

                CmdAddPlayer(m_data);

                CmdUpdateData(m_data);
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

        private void Start()
        {
            m_vehicleInputControl.enabled = false;
            if (ActiveVehicle != null) ActiveVehicle.IsStopped = true;
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                if (ActiveVehicle != null)
                {
                    ActiveVehicle.SetVisible(!VehicleCamera.Instance.IsZoomed);

                    if (ActiveVehicle.CurrentHidingSpot != null)
                    {
                        ActiveVehicle.CurrentHidingSpot.SetVisible(!VehicleCamera.Instance.IsZoomed);
                    }    
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
            ActiveVehicle.TeamId = m_teamId;

            RpcSetVehicle(ActiveVehicle.netIdentity);
        }

        [ClientRpc]
        private void RpcSetVehicle(NetworkIdentity vehicle)
        {
            if (vehicle == null) return;

            ActiveVehicle = vehicle.GetComponent<Vehicle>();
            ActiveVehicle.Owner = netIdentity;
            ActiveVehicle.TeamId = m_teamId;

            if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
            {
                VehicleCamera.Instance.SetTarget(ActiveVehicle);
            }

            m_vehicleInputControl.enabled = false;
            ActiveVehicle.IsStopped = true;

            VehicleSpawned?.Invoke(ActiveVehicle);
        }

        #region PlayerList

        [Command]
        private void CmdAddPlayer(MatchMemberData data)
        {
            MatchMemberList.Instance.SvAddMatchMember(data);
        }

        #endregion

        #region ProjectileHit

        public event UnityAction<ProjectileHitResult> ProjectileHitted;

        [Server]
        public void SvInvokeProjectileHit(ProjectileHitResult hitResult)
        {
            ProjectileHitted?.Invoke(hitResult);

            RpcInvokeProjectileHit(hitResult.type, hitResult.damage, hitResult.point);
        }

        [ClientRpc]
        public void RpcInvokeProjectileHit(ProjectileHitType type, float damage, Vector3 hitPoint)
        {
            var hitResult = new ProjectileHitResult();
            hitResult.type = type;
            hitResult.damage = damage;
            hitResult.point = hitPoint;

            ProjectileHitted?.Invoke(hitResult);
        }

        #endregion

        private void OnMatchStart()
        {
            m_vehicleInputControl.enabled = true;
            if (ActiveVehicle != null) ActiveVehicle.IsStopped = false;
        }

        private void OnMatchEnd()
        {
            if (ActiveVehicle != null)
            {
                ActiveVehicle.SetTargetControl(Vector3.zero);
                m_vehicleInputControl.enabled = false;
                ActiveVehicle.IsStopped = true;
            }
        }
    }
}
