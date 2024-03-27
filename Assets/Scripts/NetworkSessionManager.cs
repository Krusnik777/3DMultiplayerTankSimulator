using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    public class NetworkSessionManager : NetworkManager
    {
        [SerializeField] private GameEventCollector m_gameEventCollector;
        [SerializeField] private SphereArea[] m_spawnZonesRed;
        [SerializeField] private SphereArea[] m_spawnZonesBlue;

        public Vector3 RandomSpawnPointRed => m_spawnZonesRed[Random.Range(0, m_spawnZonesRed.Length)].RandomInside;
        public Vector3 RandomSpawnPointBlue => m_spawnZonesBlue[Random.Range(0, m_spawnZonesBlue.Length)].RandomInside;

        public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
        public static GameEventCollector Events => Instance.m_gameEventCollector;

        public bool IsServer => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly;
        public bool IsClient => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            m_gameEventCollector.SvOnAddPlayer();
        }
    }
}
