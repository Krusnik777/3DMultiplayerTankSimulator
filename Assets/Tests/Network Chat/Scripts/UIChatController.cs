using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace NetworkChat
{
    public class UIChatController : MonoBehaviour
    {
        public static UIChatController Instance;

        [SerializeField] NetworkManager m_networkManager;
        [Header("Panels")]
        [SerializeField] private GameObject m_joinPanel;
        [SerializeField] private Button m_hostButton;
        [SerializeField] private Button m_clientButton;
        [SerializeField] private GameObject m_mainPanel;
        [SerializeField] private Button m_sendButton;
        [Header("InputFields")]
        [SerializeField] private InputField m_messageInputField;
        [SerializeField] private InputField m_nicknameInputField;

        public bool MessageFieldIsEmpty => m_messageInputField.text == "";
        public bool NicknameFieldIsEmpty => m_nicknameInputField.text == "";

        public string GetString() => m_messageInputField.text;

        public void ClearString() => m_messageInputField.text = "";

        public string GetNickname() => m_nicknameInputField.text;

        public void SendMessageToChat() => User.LocalUser.SendMessageToChat();

        public void HostChat()
        {
            if (!NicknameFieldIsEmpty)
            {
                m_networkManager.StartHost();

                User.UserReady += OnUserReady;
            }
        }

        public void JoinToChat()
        {
            if (!NicknameFieldIsEmpty)
            {
                if (User.LocalUser == null)
                {
                    m_networkManager.StartClient();

                    User.UserReady += OnUserReady;
                    NetworkClient.OnDisconnectedEvent += OnDisconnected; // if not connected when no server
                }
                else
                {
                    User.LocalUser.JoinToChat();
                    m_joinPanel.SetActive(false);
                    m_mainPanel.SetActive(true);
                }
            }
        }

        public void ExitChat()
        {
            if (User.LocalUser == null) return;

            m_networkManager.StopHost();
        }

        public void ToggleButtons()
        {
            m_hostButton.interactable = !string.IsNullOrWhiteSpace(m_nicknameInputField.text);
            m_clientButton.interactable = !string.IsNullOrWhiteSpace(m_nicknameInputField.text);
        }

        public void ToggleSendButton()
        {
            m_sendButton.interactable = !string.IsNullOrWhiteSpace(m_messageInputField.text);
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnUserReady()
        {
            if (User.LocalUser != null)
            {
                User.LocalUser.JoinToChat();
                m_joinPanel.SetActive(false);
                m_mainPanel.SetActive(true);

                User.LocalUser.OnLocalUserStopped += CloseChat;
            }

            User.UserReady -= OnUserReady;
            NetworkClient.OnDisconnectedEvent -= OnDisconnected;
        }

        private void OnDisconnected()
        {
            User.UserReady -= OnUserReady;
            NetworkClient.OnDisconnectedEvent -= OnDisconnected;
        }

        private void CloseChat()
        {
            m_joinPanel.SetActive(true);
            m_mainPanel.SetActive(false);

            User.LocalUser.OnLocalUserStopped -= CloseChat;
        }
    }
}
