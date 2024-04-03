using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIPlayerLabel : MonoBehaviour
    {
        [SerializeField] private Text m_fragsText;
        [SerializeField] private Text m_nicknameText;
        [SerializeField] private Image m_backgroundImage;
        [SerializeField] private Color m_selfColor;

        private int m_netId;
        public int NetId => m_netId;

        public void Init(int netId, string nickname)
        {
            m_netId = netId;
            m_nicknameText.text = nickname;

            if (netId == Player.Local.netId) m_backgroundImage.color = m_selfColor;
        }

        public void UpdateFrags(int frags)
        {
            m_fragsText.text = frags.ToString();
        }
    }
}
