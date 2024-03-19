using UnityEngine;
using UnityEngine.UI;

namespace NetworkChat
{
    public class UIUserBox : MonoBehaviour
    {
        [SerializeField] private Text m_idText;
        [SerializeField] private Text m_nicknameText;
        private Button m_button;

        private int userId;
        private bool clicked = false;
        private ColorBlock defaultButtonColorBlock;

        public void SetText(int id, string nicknametext)
        {
            userId = id;
            m_idText.text = id.ToString();
            m_nicknameText.text = nicknametext;
        }

        public void SetPrivateChat()
        {
            if (User.LocalUser == null) return;

            clicked = !clicked;

            User.LocalUser.InitializePrivateChat(userId, clicked);

            ChangeButtonColor(clicked);

            if (clicked) User.LocalUser.OnPrivateModeChanges += OnPrivateModeChanges;
            else User.LocalUser.OnPrivateModeChanges -= OnPrivateModeChanges;
        }

        private void Start()
        {
            m_button = GetComponent<Button>();
            defaultButtonColorBlock = m_button.colors;
        }

        private void ChangeButtonColor(bool isPressed)
        {
            if (isPressed)
            {
                ColorBlock cb = m_button.colors;

                cb.normalColor = m_button.colors.pressedColor;
                cb.highlightedColor = m_button.colors.pressedColor;
                cb.selectedColor = m_button.colors.pressedColor;

                m_button.colors = cb;
            }
            else
            {
                m_button.colors = defaultButtonColorBlock;
            }
        }

        private void OnPrivateModeChanges(bool state, int receiver)
        {
            if (receiver != userId)
            {
                clicked = !clicked;

                ChangeButtonColor(clicked);

                User.LocalUser.OnPrivateModeChanges -= OnPrivateModeChanges;
            }
        }
    }
}
