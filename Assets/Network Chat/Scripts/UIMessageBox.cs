using UnityEngine;
using UnityEngine.UI;

namespace NetworkChat
{
    public class UIMessageBox : MonoBehaviour
    {
        [SerializeField] private Text m_nicknameText;
        [SerializeField] private Text m_messageText;

        public void SetText(string nickname, string text)
        {
            m_nicknameText.text = nickname + ":";
            m_messageText.text = text;
        }

        /*
        #region AnotherSolution

        [SerializeField] private Image m_backgroundImage;
        [SerializeField] private Color m_bgColorForSelf;
        [SerializeField] private Color m_bgColorForSender;

        public void SetStyleBySelf()
        {
            m_backgroundImage.color = m_bgColorForSelf;
            m_text.alignment = TextAnchor.MiddleLeft;
        }

        public void SetStyleBySender()
        {
            m_backgroundImage.color = m_bgColorForSender;
            m_text.alignment = TextAnchor.MiddleRight;
        }

        #endregion
        */
    }
}
