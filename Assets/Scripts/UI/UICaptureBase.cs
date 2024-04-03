using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UICaptureBase : MonoBehaviour
    {
        [SerializeField] private ConditionCaptureBase m_conditionCaptureBase;
        [SerializeField] private Slider m_localTeamSlider;
        [SerializeField] private Slider m_otherTeamSlider;

        private void Update()
        {
            if (Player.Local == null) return;

            if (Player.Local.TeamId == TeamSide.TeamRed)
            {
                UpdateSlider(m_localTeamSlider, m_conditionCaptureBase.RedBaseCaptureLevel);
                UpdateSlider(m_otherTeamSlider, m_conditionCaptureBase.BlueBaseCaptureLevel);
            }

            if (Player.Local.TeamId == TeamSide.TeamBlue)
            {
                UpdateSlider(m_localTeamSlider, m_conditionCaptureBase.BlueBaseCaptureLevel);
                UpdateSlider(m_otherTeamSlider, m_conditionCaptureBase.RedBaseCaptureLevel);
            }

        }

        private void UpdateSlider(Slider slider, float value)
        {
            if (value == 0) 
                slider.gameObject.SetActive(false);
            else
            {
                if (!slider.gameObject.activeSelf)
                    slider.gameObject.SetActive(true);

                slider.value = value;
            }
        }
    }
}
