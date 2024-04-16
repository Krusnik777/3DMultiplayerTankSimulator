using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UITankInfo : MonoBehaviour
    {
        [SerializeField] private Text m_nickname;
        [SerializeField] private UIHealthSilder m_healthSlider;
        [SerializeField] private Vector3 m_worldOffset;
        [SerializeField] private Vector3 m_worldZoomOffset;

        public Vector3 WorldOffset => m_worldOffset;
        public Vector3 WorldZoomOffset => m_worldZoomOffset;

        private Vehicle m_tank;
        public Vehicle Tank => m_tank;

        public void SetTank(Vehicle tank)
        {
            m_tank = tank;

            m_healthSlider.Init(m_tank, m_tank.TeamId, Player.Local.TeamId);
            m_nickname.text = m_tank.Owner?.GetComponent<Player>().Nickname;
            m_nickname.color = m_healthSlider.ActiveColor;
        }
    }
}
