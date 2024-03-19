using System.Collections.Generic;
using UnityEngine;

namespace NetworkChat
{
    public class UIMessageViewer : MonoBehaviour
    {
        [Header("MessageBoxes")]
        [SerializeField] private Transform m_messagePanel;
        [SerializeField] private UIMessageBox m_messageBox;
        [SerializeField] private UIMessageBox m_privateMessageBox;
        [SerializeField] private UIMessageBox m_personalMessageBox;
        [SerializeField] private UIMessageBox m_privatePersonalMessageBox;
        [Header("UserBoxes")]
        [SerializeField] private Transform m_userListPanel;
        [SerializeField] private UIUserBox m_userBox;
        [SerializeField] private UIUserBox m_personalUserBox;

        private void Start()
        {
            User.ReceivedMessageToChat += OnReceivedMessageToChat;
            User.ReceivedPrivateMessageToChat += OnReceivedPrivateMessageToChat;
            UserList.UpdateUserList += OnUpdateUserList;
        }
        
        private void OnDestroy()
        {
            User.ReceivedMessageToChat -= OnReceivedMessageToChat;
            User.ReceivedPrivateMessageToChat -= OnReceivedPrivateMessageToChat;
            UserList.UpdateUserList -= OnUpdateUserList;
        }
        
        private void OnReceivedMessageToChat(UserData data, string message)
        {
            AppendMessage(data, message);
        }

        private void OnReceivedPrivateMessageToChat(UserData data, string message, int receiverId)
        {
            AppendMessage(data, message, receiverId);
        }

        private void AppendMessage(UserData data, string message)
        {
            bool isLocalUser = data.Id == User.LocalUser.Data.Id;

            var messageBox = InstantiateMessageBox(isLocalUser);

            messageBox.SetText(data.Nickname, message);

            messageBox.transform.SetParent(m_messagePanel);
            messageBox.transform.localScale = Vector3.one;

            /*
            #region AnotherSolution

            if (userId == User.LocalUser.Data.Id)
            {
                messageBox.SetStyleBySelf();
            }
            else
                messageBox.SetStyleBySender();
            #endregion
            */
        }

        private void AppendMessage(UserData data, string message, int receiverId)
        {
            if (User.LocalUser.Data.Id != data.Id && User.LocalUser.Data.Id != receiverId) return;

            bool isLocalUser = data.Id == User.LocalUser.Data.Id;

            var messageBox = InstantiatePrivateMessageBox(isLocalUser);

            messageBox.SetText("[ " + data.Nickname + " ]", message);

            messageBox.transform.SetParent(m_messagePanel);
            messageBox.transform.localScale = Vector3.one;
        }

        private void OnUpdateUserList(List<UserData> userList)
        {
            for (int i = 0; i < m_userListPanel.childCount; i++)
            {
                Destroy(m_userListPanel.GetChild(i).gameObject);
            }

            for (int i = 0; i < userList.Count; i++)
            {
                bool isLocalUser = userList[i].Id == User.LocalUser.Data.Id;

                var userBox = InstantiateUserBox(isLocalUser);

                userBox.SetText(userList[i].Id, userList[i].Nickname);

                userBox.transform.SetParent(m_userListPanel);
                userBox.transform.localScale = Vector3.one;
            }
        }

        private UIMessageBox InstantiateMessageBox(bool isLocalUser)
        {
            if (isLocalUser) return Instantiate(m_personalMessageBox);

            return Instantiate(m_messageBox);
        }

        private UIMessageBox InstantiatePrivateMessageBox(bool isLocalUser)
        {
            if (isLocalUser) return Instantiate(m_privatePersonalMessageBox);

            return Instantiate(m_privateMessageBox);
        }

        private UIUserBox InstantiateUserBox(bool isLocalUser)
        {
            if (isLocalUser) return Instantiate(m_personalUserBox);

            return Instantiate(m_userBox);
        }
    }
}
