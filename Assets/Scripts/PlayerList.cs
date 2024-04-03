using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
	[RequireComponent(typeof(NetworkIdentity))]
	public class PlayerList : NetworkBehaviour
    {
		public static PlayerList Instance;

		public static UnityAction<List<PlayerData>> UpdatePlayerList;

		[SerializeField] private List<PlayerData> allPlayersData = new List<PlayerData>(); // Serialize for debug
		public List<PlayerData> AllPlayersData => allPlayersData;

		private bool CheckIfListContains(int id)
		{
			foreach (var data in allPlayersData)
			{
				if (data.Id == id)
					return true;
			}
			return false;
		}

        public override void OnStartClient()
        {
            base.OnStartClient();

			allPlayersData.Clear();
		}

        [Server]
		public void SvAddPlayer(PlayerData data)
		{
			allPlayersData.Add(data);

			RpcClearPlayerDataList();

			for (int i = 0; i < allPlayersData.Count; i++)
            {
				RpcAddPlayer(allPlayersData[i]);
            }
		}

		[Server]
		public void SvRemovePlayer(PlayerData data)
		{
			for (int i = 0; i < allPlayersData.Count; i++)
			{
				if (allPlayersData[i].Id == data.Id)
				{
					allPlayersData.RemoveAt(i);
					break;
				}
			}

			RpcRemovePlayer(data);
		}

		[ClientRpc]
		private void RpcClearPlayerDataList()
		{
			if (isServer) return;

			allPlayersData.Clear();
		}

		[ClientRpc]
		private void RpcAddPlayer(PlayerData data)
		{
			if (isClient && isServer)
            {
				UpdatePlayerList?.Invoke(allPlayersData);
				return;
            }

			if (CheckIfListContains(data.Id)) return;

			allPlayersData.Add(data);

			UpdatePlayerList?.Invoke(allPlayersData);
		}

		[ClientRpc]
		private void RpcRemovePlayer(PlayerData data)
		{
			for (int i = 0; i < allPlayersData.Count; i++)
			{
				if (allPlayersData[i].Id == data.Id)
				{
					allPlayersData.RemoveAt(i);
					break;
				}
			}

			UpdatePlayerList?.Invoke(allPlayersData);
		}

        private void Awake()
        {
			Instance = this;
		}
    }
}
