using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIMatchResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject m_resultPanel;
        [SerializeField] private Text m_resultText;

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

        private void OnMatchStart()
        {
            m_resultPanel.SetActive(false);
        }

        private void OnMatchEnd()
        {
            m_resultPanel.SetActive(true);

            int winTeamId = NetworkSessionManager.Match.WinTeamId;

            if (winTeamId == -1)
            {
                m_resultText.text = "Result: Draw";
                return;
            }

            if (winTeamId == Player.Local.TeamId)
            {
                m_resultText.text = "Result: Victory";
            }
            else
            {
                m_resultText.text = "Result: Defeat";
            }
        }
    }
}
