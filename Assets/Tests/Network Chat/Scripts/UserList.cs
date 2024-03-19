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
		public void SvAddCurrentUser(UserData data)
		{
			allUsersData.Add(data);

			if (isServerOnly)
				RpcClearUserDataList();

			for (int i = 0; i < allUsersData.Count; i++)
            {
				RpcAddCurrentUser(allUsersData[i]);
            }
		}

		[Server]
		public void SvRemoveCurrentUser(UserData data)
		{
			for (int i = 0; i < allUsersData.Count; i++)
			{
				if (allUsersData[i].Id == data.Id)
				{
					allUsersData.RemoveAt(i);
					break;
				}
			}

			RpcRemoveCurrentUser(data);
		}

		[ClientRpc]
		private void RpcClearUserDataList()
		{
			allUsersData.Clear();
		}

		[ClientRpc]
		private void RpcAddCurrentUser(UserData data)
		{
			if (isClient && isServer)
            {
				UpdateUserList?.Invoke(allUsersData);
				return;
            }

			if (CheckIfListContains(data.Id)) return;

			allUsersData.Add(data);

			UpdateUserList?.Invoke(allUsersData);
		}

		[ClientRpc]
		private void RpcRemoveCurrentUser(UserData data)
		{
			for (int i = 0; i < allUsersData.Count; i++)
			{
				if (allUsersData[i].Id == data.Id)
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
