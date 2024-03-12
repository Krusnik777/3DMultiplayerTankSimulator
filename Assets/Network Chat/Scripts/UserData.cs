using Mirror;

namespace NetworkChat
{
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

    public static class UserDataWriteRead
    {
        public static void WriteUserData(this NetworkWriter writer, UserData data)
        {
            writer.WriteInt(data.Id);
            writer.WriteString(data.Nickname);
        }

        public static UserData ReadUserData(this NetworkReader reader)
        {
            return new UserData(reader.ReadInt(), reader.ReadString());
        }
    }
}
