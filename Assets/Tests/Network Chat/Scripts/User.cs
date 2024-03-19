using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkChat
{
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

        public static UnityAction<UserData, string> ReceivedMessageToChat;
        public static UnityAction<UserData, string, int> ReceivedPrivateMessageToChat;
        public static UnityAction UserReady;
        public UnityAction OnLocalUserStopped;
        public UnityAction<bool, int> OnPrivateModeChanges;

        private UserData data;

        private UIChatController inputMaster;

        public UserData Data => data;

        public override void OnStopServer()
        {
            base.OnStopServer();

            UserList.Instance.SvRemoveCurrentUser(data);
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

            CmdAddUser(data);
        }

        [Command]
        private void CmdAddUser(UserData data)
        {
            UserList.Instance.SvAddCurrentUser(data);
        }

        [Command]
        private void CmdRemoveUser(UserData data)
        {
            UserList.Instance.SvRemoveCurrentUser(data);
        }

        #endregion

        #region Chat
        public void SendMessageToChat()
        {
            if (!isOwned) return;

            if (inputMaster.MessageFieldIsEmpty) return;

            if (inPrivateMode)
                CmdSendMessageToChat(data, inputMaster.GetString(), receiverId);
            else
                CmdSendMessageToChat(data, inputMaster.GetString());

            inputMaster.ClearString();
        }

        [Command]
        private void CmdSendMessageToChat(UserData data, string message)
        {
            Debug.Log($"User {data.Id} send message to server. Message: {message}");

            SvPostMessage(data, message);
        }

        [Server]
        private void SvPostMessage(UserData data, string message)
        {
            Debug.Log($"Server received message by user {data.Id}. Message: {message}");

            RpcReceiveMessage(data, message);
        }

        [ClientRpc]
        private void RpcReceiveMessage(UserData data, string message)
        {
            Debug.Log($"User {data.Nickname}:({data.Id}) received message. Message: {message}");

            ReceivedMessageToChat?.Invoke(data, message);
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
        private void CmdSendMessageToChat(UserData data, string message, int receiverId)
        {
            Debug.Log($"User send message to server. Message: {message}");

            SvPostMessage(data, message, receiverId);
        }

        [Server]
        private void SvPostMessage(UserData data, string message, int receiverId)
        {
            Debug.Log($"Server received message by user. Message: {message}");

            RpcReceiveMessage(data, message, receiverId);
        }

        [ClientRpc]
        private void RpcReceiveMessage(UserData data, string message,int receiverId)
        {
            Debug.Log($"User {data.Nickname}:({data.Id}) received message. Message: {message}");

            ReceivedPrivateMessageToChat?.Invoke(data, message, receiverId);
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
