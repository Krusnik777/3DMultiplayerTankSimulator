using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkChat
{
    [System.Serializable]
    public class UserData
    {
        public int Id;
        public string Nickname;

        public UserData(int id, string nickname)
        {
            Id = id;
            Nickname = nickname;
        }
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class User : NetworkBehaviour
    {
        public static User LocalUser
        {
            get
            {
                var potentialLocalUser = NetworkClient.localPlayer;

                if (potentialLocalUser != null)
                    return potentialLocalUser.GetComponent<User>();

                return null;
            }
        }

        public static UnityAction<int, string, string> ReceivedMessageToChat;
        public static UnityAction<int, string, string, int> ReceivedPrivateMessageToChat;
        public static UnityAction UserReady;
        public UnityAction OnLocalUserStopped;
        public UnityAction<bool, int> OnPrivateModeChanges;

        private UserData data;

        private UIChatController inputMaster;

        public UserData Data => data;

        public override void OnStopServer()
        {
            base.OnStopServer();

            UserList.Instance.SvRemoveCurrentUser(data.Id);
        }

        public override void OnStopLocalPlayer()
        {
            base.OnStopLocalPlayer();

            OnLocalUserStopped?.Invoke();
        }

        private void Start()
        {
            if (inputMaster == null) inputMaster = UIChatController.Instance;

            if (data == null) data = new UserData((int) netId, "Nickname");

            UserReady?.Invoke();
        }

        private void Update()
        {
            if (!isOwned) return;

            if (Input.GetKeyDown(KeyCode.Return)) SendMessageToChat();
        }

        #region Join

        public void JoinToChat()
        {
            if (inputMaster == null) inputMaster = UIChatController.Instance;

            if (data == null) data = new UserData((int)netId, "Nickname");

            data.Nickname = inputMaster.GetNickname();

            CmdAddUser(data.Id, data.Nickname);
        }

        [Command]
        private void CmdAddUser(int userId, string userNickname)
        {
            UserList.Instance.SvAddCurrentUser(userId, userNickname);
        }

        [Command]
        private void CmdRemoveUser(int userId)
        {
            UserList.Instance.SvRemoveCurrentUser(userId);
        }

        #endregion

        #region Chat
        public void SendMessageToChat()
        {
            if (!isOwned) return;

            if (inputMaster.MessageFieldIsEmpty) return;

            if (inPrivateMode)
                CmdSendMessageToChat(data.Id, data.Nickname, inputMaster.GetString(), receiverId);
            else
                CmdSendMessageToChat(data.Id, data.Nickname, inputMaster.GetString());

            inputMaster.ClearString();
        }

        [Command]
        private void CmdSendMessageToChat(int userId, string userNickname, string message)
        {
            Debug.Log($"User send message to server. Message: {message}");

            SvPostMessage(userId, userNickname, message);
        }

        [Server]
        private void SvPostMessage(int userId, string userNickname, string message)
        {
            Debug.Log($"Server received message by user. Message: {message}");

            RpcReceiveMessage(userId, userNickname, message);
        }

        [ClientRpc]
        private void RpcReceiveMessage(int userId, string userNickname, string message)
        {
            Debug.Log($"User {data.Nickname}:({data.Id}) received message. Message: {message}");

            ReceivedMessageToChat?.Invoke(userId, userNickname, message);
        }

        #region PrivateChat

        private bool inPrivateMode;
        public bool InPrivateMode => inPrivateMode;
        private int receiverId;

        public void InitializePrivateChat(int receiverUserId, bool isActive)
        {
            if (!isOwned) return;

            CmdSetPrivateMode(receiverUserId, isActive);
        }

        [Command]
        private void CmdSetPrivateMode(int receiverUserId, bool isActive)
        {
            SvSetPrivateMode(receiverUserId, isActive);
        }

        [Server]
        private void SvSetPrivateMode(int receiverUserId, bool isActive)
        {
            RpcSetPrivateMode(receiverUserId, isActive);
        }

        [ClientRpc]
        private void RpcSetPrivateMode(int receiverUserId, bool isActive)
        {
            SetPrivateMode(receiverUserId, isActive);
        }

        [Command]
        private void CmdSendMessageToChat(int userId, string userNickname, string message, int receiverId)
        {
            Debug.Log($"User send message to server. Message: {message}");

            SvPostMessage(userId, userNickname, message, receiverId);
        }

        [Server]
        private void SvPostMessage(int userId, string userNickname, string message, int receiverId)
        {
            Debug.Log($"Server received message by user. Message: {message}");

            RpcReceiveMessage(userId, userNickname, message, receiverId);
        }

        [ClientRpc]
        private void RpcReceiveMessage(int userId, string userNickname, string message,int receiverId)
        {
            Debug.Log($"User {data.Nickname}:({data.Id}) received message. Message: {message}");

            ReceivedPrivateMessageToChat?.Invoke(userId, userNickname, message, receiverId);
        }

        private void SetPrivateMode(int receiverUserId, bool isActive)
        {
            if (isActive)
            {
                inPrivateMode = true;
                receiverId = receiverUserId;
            }
            else
            {
                inPrivateMode = false;
                receiverId = 0;
            }

            OnPrivateModeChanges?.Invoke(inPrivateMode, receiverId);
        }

        #endregion

        #endregion
    }
}
