using UnityEngine;

namespace MultiplayerTanks
{
    public class MinimapController : MonoBehaviour
    {
        [SerializeField] private MinimapMark m_minimapMarkPrefab;

        private MinimapMark[] m_minimapMarks;
        private Vehicle[] m_vehicles;

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
                if (m_vehicles[i] == null) continue;

                if (m_vehicles[i] != Player.Local.ActiveVehicle)
                {
                    bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(m_vehicles[i].netIdentity);

                    m_minimapMarks[i].gameObject.SetActive(isVisible);
                }

                if (!m_minimapMarks[i].gameObject.activeSelf) continue;

                m_minimapMarks[i].transform.position = new Vector3(m_vehicles[i].transform.position.x,m_minimapMarks[i].transform.position.y, m_vehicles[i].transform.position.z);
            }
        }

        private void OnMatchStart()
        {
            m_vehicles = FindObjectsOfType<Vehicle>();

            m_minimapMarks = new MinimapMark[m_vehicles.Length];

            for (int i = 0; i < m_minimapMarks.Length; i++)
            {
                m_minimapMarks[i] = Instantiate(m_minimapMarkPrefab);

                if (m_vehicles[i].TeamId == Player.Local.TeamId) m_minimapMarks[i].SetLocalColor();
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
