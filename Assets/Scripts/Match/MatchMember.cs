using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerTanks
{
    [System.Serializable]
    public class MatchMemberData
    {
        public int Id;
        public string Nickname;
        public int TeamId;
        public NetworkIdentity Member;

        public MatchMemberData(int id, string nickname, int teamId, NetworkIdentity member)
        {
            Id = id;
            Nickname = nickname;
            TeamId = teamId;
            Member = member;
        }
    }

    public static class MatchMemberDataExtension
    {
        public static void WriteMatchMemberData(this NetworkWriter writer, MatchMemberData data)
        {
            writer.WriteInt(data.Id);
            writer.WriteString(data.Nickname);
            writer.WriteInt(data.TeamId);
            writer.WriteNetworkIdentity(data.Member);
        }

        public static MatchMemberData ReadMatchMemberData(this NetworkReader reader)
        {
            return new MatchMemberData(reader.ReadInt(), reader.ReadString(), reader.ReadInt(), reader.ReadNetworkIdentity());
        }
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class MatchMember : NetworkBehaviour
    {
        public static event UnityAction<MatchMember, int> ChangeFrags;

        public Vehicle ActiveVehicle { get; set; }

        #region Data

        protected MatchMemberData m_data;
        public MatchMemberData Data => m_data;

        [Command]
        protected void CmdUpdateData(MatchMemberData data)
        {
            m_data = data;
        }

        #endregion

        #region Frags

        [SyncVar(hook = nameof(OnFragsChanged))]
        protected int m_fragsAmount;

        [Server]
        public void SvAddFrags()
        {
            m_fragsAmount++;

            ChangeFrags?.Invoke(this, m_fragsAmount);
        }

        [Server]
        public void SvResetFlags()
        {
            m_fragsAmount = 0;
        }

        // Client
        private void OnFragsChanged(int oldValue, int newValue)
        {
            ChangeFrags?.Invoke(this, newValue);
        }

        #endregion

        #region Nickname

        [SyncVar(hook = nameof(OnNicknameChanged))]
        protected string m_nickname;
        public string Nickname => m_nickname;

        [Command]
        protected void CmdSetName(string name)
        {
            m_nickname = name;
            gameObject.name = name;
        }

        private void OnNicknameChanged(string oldValue, string newValue)
        {
            gameObject.name = newValue; // on Client
        }

        #endregion

        #region TeamId

        [SyncVar]
        protected int m_teamId;
        public int TeamId => m_teamId;

        #endregion
    }
}
