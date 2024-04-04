using UnityEngine;

namespace MultiplayerTanks
{
    public class MinimapController : MonoBehaviour
    {
        [SerializeField] private MinimapMark m_minimapMarkPrefab;

        private MinimapMark[] m_minimapMarks;
        private Player[] m_players;

        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
        }

        private void OnDisable()
        {
            if (NetworkSessionManager.Match != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnMatchStart;
                NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
            }
        }

        private void Update()
        {
            if (m_minimapMarks == null) return;

            for (int i = 0; i < m_minimapMarks.Length; i++)
            {
                if (m_players[i] == null) continue;

                m_minimapMarks[i].transform.position = new Vector3(m_players[i].ActiveVehicle.transform.position.x,m_minimapMarks[i].transform.position.y, m_players[i].ActiveVehicle.transform.position.z);
            }
        }

        private void OnMatchStart()
        {
            m_players = FindObjectsOfType<Player>();

            m_minimapMarks = new MinimapMark[m_players.Length];

            for (int i = 0; i < m_minimapMarks.Length; i++)
            {
                m_minimapMarks[i] = Instantiate(m_minimapMarkPrefab);

                if (m_players[i].TeamId == Player.Local.TeamId) m_minimapMarks[i].SetLocalColor();
                else m_minimapMarks[i].SetOtherColor();
            }
        }

        private void OnMatchEnd()
        {
            for (int i = 0; i < m_minimapMarks.Length; i++)
            {
                Destroy(m_minimapMarks[i].gameObject);
            }

            m_minimapMarks = null;
        }
    }
}
