using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UITankInfo : MonoBehaviour
    {
        [SerializeField] private UIHealthSilder m_healthSlider;
        [SerializeField] private Vector3 m_worldOffset;

        public Vector3 WorldOffset => m_worldOffset;

        private Vehicle m_tank;
        public Vehicle Tank => m_tank;

        public void SetTank(Vehicle tank)
        {
            m_tank = tank;

            m_healthSlider.Init(m_tank, m_tank.Owner.GetComponent<Player>().TeamId, Player.Local.TeamId);
        }
    }
}
