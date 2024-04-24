using UnityEngine;

namespace MultiplayerTanks
{
    public class Bot : MatchMember
    {
        [SerializeField] private Vehicle m_vehicle;

        public override void OnStartServer()
        {
            base.OnStartServer();

            m_teamId = MatchController.GetNextTeam();
            m_nickname = "b_" + GetRandomName();

            m_data = new MatchMemberData((int)netId, m_nickname, m_teamId, netIdentity);

            transform.position = NetworkSessionManager.Instance.GetSpawnPointByTeam(m_teamId);

            ActiveVehicle = m_vehicle;
            ActiveVehicle.TeamId = m_teamId;
            ActiveVehicle.Owner = netIdentity;
            ActiveVehicle.name = m_nickname;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            MatchMemberList.Instance.SvRemoveMatchMember(m_data);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            ActiveVehicle = m_vehicle;
            ActiveVehicle.TeamId = m_teamId;
            ActiveVehicle.Owner = netIdentity;
            ActiveVehicle.name = m_nickname;
        }

        private void Start()
        {
            if (isServer)
            {
                MatchMemberList.Instance.SvAddMatchMember(m_data);
            }
        }

        private string GetRandomName()
        {
            string[] names =
            {
                "Несронтор",
                "Карпимон",
                "Клеиопонт",
                "Ислистрат",
                "Крофирпия",
                "Фатакрат",
                "Ксетарор",
                "Ипаронтий",
                "Ифипридора",
                "Панапиот",
                "Теокарпота",
                "Флеалеон",
                "Линтелдул",
                "Софталма",
                "Макгартор",
                "Придигност",
                "Пенистла",
                "Зинерина",
                "Порфилпонт",
                "Пераат",
                "Леонает",
                "Аргоая",
                "Лидприита",
                "Ксефилпонт",
                "Мильтрина",
                "Тимлакон",
                "Ксеелопия",
                "Меласарх",
                "Некадром",
                "Мораммен",
                "Несоай",
                "Перонкл",
                "Катдосела",
                "Неотрат",
                "Ораптий",
                "Кибаглия",
                "Неоронург",
                "Митаггонт",
                "Хрофекла",
                "Мергенина",
                "Кироай",
                "Карпприсий",
                "Метприик",
                "Менетена",
                "Иягенус",
                "Флеислия",
                "Ифитией",
                "Хиоддул",
                "Тиагпатра",
                "Элаест"
            };

            return names[Random.Range(0, names.Length)];
        }
    }
}
