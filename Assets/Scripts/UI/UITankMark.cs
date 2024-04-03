using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UITankMark : MonoBehaviour
    {
        [SerializeField] private Image m_image;
        [SerializeField] private Color m_localTeamColor;
        [SerializeField] private Color m_otherTeamColor;

        public void SetLocalColor() => m_image.color = m_localTeamColor;
        public void SetOtherColor() => m_image.color = m_otherTeamColor;
    }
}
