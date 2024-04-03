using Mirror.Examples.Tanks;
using NetworkSpaceShooter;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIHealthSilder : MonoBehaviour
    {
        [SerializeField] private Slider m_slider;
        [SerializeField] private Image m_sliderImage;
        [SerializeField] private Color m_localTeamColor;
        [SerializeField] private Color m_otherTeamColor;

        private Destructible m_destructible;

        public void Init(Destructible destructible, int destructibleTeamId, int localPlayerTeamId)
        {
            m_destructible = destructible;

            destructible.HitPointsChange += OnHitPointsChange;
            m_slider.maxValue = destructible.MaxHitPoints;
            m_slider.value = m_slider.maxValue;

            if (localPlayerTeamId == destructibleTeamId)
            {
                SetLocalColor();
            }
            else
            {
                SetOtherColor();
            }
        }

        private void OnDestroy()
        {
            if (m_destructible == null) return;

            m_destructible.HitPointsChange -= OnHitPointsChange;
        }

        private void OnHitPointsChange(int hitPoints)
        {
            m_slider.value = hitPoints;
        }

        private void SetLocalColor()
        {
            m_sliderImage.color = m_localTeamColor;
        }

        private void SetOtherColor()
        {
            m_sliderImage.color = m_otherTeamColor;
        }
    }
}
