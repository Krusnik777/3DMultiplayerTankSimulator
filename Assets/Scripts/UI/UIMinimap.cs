using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIMinimap : MonoBehaviour
    {
        [SerializeField] private SizeMap m_sizeMap;
        [SerializeField] private UITankMark m_tankMarkPrefab;
        [SerializeField] private Image m_background;

        private UITankMark[] m_tankMarks;
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
            if (m_tankMarks == null) return;

            for (int i = 0; i < m_tankMarks.Length; i++)
            {
                if (m_players[i] == null) continue;

                Vector3 normalizedPosition = m_sizeMap.GetNormalizedPosition(m_players[i].ActiveVehicle.transform.position);
                Vector3 positionInMinimap = new Vector3(normalizedPosition.x * m_background.rectTransform.sizeDelta.x * 0.5f, normalizedPosition.z * m_background.rectTransform.sizeDelta.y * 0.5f, 0);

                m_tankMarks[i].transform.position = m_background.transform.position + positionInMinimap;
            }
        }

        private void OnMatchStart()
        {
            m_players = FindObjectsOfType<Player>();

            m_tankMarks = new UITankMark[m_players.Length];

            for (int i = 0; i < m_tankMarks.Length; i++)
            {
                m_tankMarks[i] = Instantiate(m_tankMarkPrefab);

                if (m_players[i].TeamId == Player.Local.TeamId) m_tankMarks[i].SetLocalColor();
                else m_tankMarks[i].SetOtherColor();

                m_tankMarks[i].transform.SetParent(m_background.transform);
            }
        }

        private void OnMatchEnd()
        {
            for (int i = 0; i < m_background.transform.childCount;i++)
            {
                Destroy(m_background.transform.GetChild(i).gameObject);
            }

            m_tankMarks = null;
        }
    }
}
