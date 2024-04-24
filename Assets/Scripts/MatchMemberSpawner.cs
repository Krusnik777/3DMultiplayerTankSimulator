using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class MatchMemberSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject m_botPrefab;
        [SerializeField][Range(0, 15)] private int m_targetAmountMemberTeam;

        [Server]
        public void SvRespawnAllMembers()
        {
            SvRespawnPlayerVehicle();
            SvRespawnBotVehicle();
        }

        [Server]
        private void SvRespawnPlayerVehicle()
        {
            foreach (var player in FindObjectsOfType<Player>())
            {
                if (player.ActiveVehicle != null)
                {
                    NetworkServer.UnSpawn(player.ActiveVehicle.gameObject);
                    Destroy(player.ActiveVehicle.gameObject);
                    player.ActiveVehicle = null;
                }
            }

            foreach (var player in FindObjectsOfType<Player>())
            {
                player.SvSpawnClientVehicle();
            }
        }

        [Server]
        private void SvRespawnBotVehicle()
        {
            foreach(var bot in FindObjectsOfType<Bot>())
            {
                NetworkServer.UnSpawn(bot.gameObject);
                Destroy(bot.gameObject);
            }

            int botAmount = m_targetAmountMemberTeam * 2 - MatchMemberList.Instance.MembersDataCount;

            for (int i = 0; i < botAmount; i++)
            {
                GameObject bot = Instantiate(m_botPrefab);
                NetworkServer.Spawn(bot);
            }
        }
    }
}
