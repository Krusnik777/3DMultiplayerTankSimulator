using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkChat
{
	[RequireComponent(typeof(NetworkIdentity))]
	public class UserList : NetworkBehaviour
    {
		public static UserList Instance;

		public static UnityAction<List<UserData>> UpdateUserList;

		private List<UserData> allUsersData = new List<UserData>();
		public List<UserData> AllUsersData => allUsersData;

		private bool CheckIfListContains(int id)
		{
			foreach (var data in allUsersData)
			{
				if (data.Id == id)
					return true;
			}
			return false;
		}

        public override void OnStartClient()
        {
            base.OnStartClient();

			allUsersData.Clear();
		}

        [Server]
		public void SvAddCurrentUser(int userId, string userNickname)
		{
			var data = new UserData(userId, userNickname);

			allUsersData.Add(data);

			if (isServerOnly)
				RpcClearUserDataList();

			for (int i = 0; i < allUsersData.Count; i++)
            {
				RpcAddCurrentUser(allUsersData[i].Id, allUsersData[i].Nickname);
            }
		}

		[Server]
		public void SvRemoveCurrentUser(int userId)
		{
			for (int i = 0; i < allUsersData.Count; i++)
			{
				if (allUsersData[i].Id == userId)
				{
					allUsersData.RemoveAt(i);
					break;
				}
			}

			RpcRemoveCurrentUser(userId);
		}

		[ClientRpc]
		private void RpcClearUserDataList()
		{
			allUsersData.Clear();
		}

		[ClientRpc]
		private void RpcAddCurrentUser(int userId, string userNickname)
		{
			if (isClient && isServer)
            {
				UpdateUserList?.Invoke(allUsersData);
				return;
            }

			var data = new UserData(userId, userNickname);

			if (CheckIfListContains(data.Id)) return;

			allUsersData.Add(data);

			UpdateUserList?.Invoke(allUsersData);
		}

		[ClientRpc]
		private void RpcRemoveCurrentUser(int userId)
		{
			for (int i = 0; i < allUsersData.Count; i++)
			{
				if (allUsersData[i].Id == userId)
				{
					allUsersData.RemoveAt(i);
					break;
				}
			}

			UpdateUserList?.Invoke(allUsersData);
		}

        private void Awake()
        {
			Instance = this;
		}
    }
}
