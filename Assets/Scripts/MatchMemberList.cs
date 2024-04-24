using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
	[RequireComponent(typeof(NetworkIdentity))]
	public class MatchMemberList : NetworkBehaviour
    {
		public static MatchMemberList Instance;

		public static event UnityAction<List<MatchMemberData>> UpdateList;

		[SerializeField] private List<MatchMemberData> allMembersData = new List<MatchMemberData>(); // Serialize for debug

		public int MembersDataCount => allMembersData.Count;

        private bool CheckIfListContains(int id)
		{
			foreach (var data in allMembersData)
			{
				if (data.Id == id)
					return true;
			}
			return false;
		}

        public override void OnStartClient()
        {
            base.OnStartClient();

			allMembersData.Clear();
		}

        [Server]
		public void SvAddMatchMember(MatchMemberData data)
		{
			allMembersData.Add(data);

			RpcClearList();

			for (int i = 0; i < allMembersData.Count; i++)
            {
				RpcAddMatchMember(allMembersData[i]);
            }
		}

		[Server]
		public void SvRemoveMatchMember(MatchMemberData data)
		{
			for (int i = 0; i < allMembersData.Count; i++)
			{
				if (allMembersData[i].Id == data.Id)
				{
					allMembersData.RemoveAt(i);
					break;
				}
			}

			RpcRemoveMatchMember(data);
		}

		[ClientRpc]
		private void RpcClearList()
		{
			if (isServer) return;

			allMembersData.Clear();
		}

		[ClientRpc]
		private void RpcAddMatchMember(MatchMemberData data)
		{
			if (isClient && isServer)
            {
				UpdateList?.Invoke(allMembersData);
				return;
            }

			if (CheckIfListContains(data.Id)) return;

			allMembersData.Add(data);

			UpdateList?.Invoke(allMembersData);
		}

		[ClientRpc]
		private void RpcRemoveMatchMember(MatchMemberData data)
		{
			for (int i = 0; i < allMembersData.Count; i++)
			{
				if (allMembersData[i].Id == data.Id)
				{
					allMembersData.RemoveAt(i);
					break;
				}
			}

			UpdateList?.Invoke(allMembersData);
		}

        private void Awake()
        {
			Instance = this;
		}
    }
}
