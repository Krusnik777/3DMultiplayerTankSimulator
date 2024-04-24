using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    public class NetworkSessionManager : NetworkManager
    {
        [SerializeField] private MatchController m_matchController;
        [SerializeField] private GameEventCollector m_gameEventCollector;
        [SerializeField] private SphereArea[] m_spawnZonesRed;
        [SerializeField] private SphereArea[] m_spawnZonesBlue;

        public Vector3 RandomSpawnPointRed => m_spawnZonesRed[Random.Range(0, m_spawnZonesRed.Length)].RandomInside;
        public Vector3 RedZoneForward => m_spawnZonesRed[Random.Range(0, m_spawnZonesRed.Length)].transform.forward;
        public Vector3 RandomSpawnPointBlue => m_spawnZonesBlue[Random.Range(0, m_spawnZonesBlue.Length)].RandomInside;
        public Vector3 BlueZoneForward => m_spawnZonesBlue[Random.Range(0, m_spawnZonesRed.Length)].transform.forward;

        public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
        public static GameEventCollector Events => Instance?.m_gameEventCollector;
        public static MatchController Match => Instance?.m_matchController;

        public bool IsServer => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly;
        public bool IsClient => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly;

        public Vector3 GetSpawnPointByTeam(int teamId) => teamId % 2 == 0 ? RandomSpawnPointRed : RandomSpawnPointBlue;
        public Vector3 GetForwardByTeamZone(int teamId) => teamId % 2 == 0 ? RedZoneForward : BlueZoneForward;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            m_gameEventCollector.SvOnAddPlayer();
        }
    }
}
