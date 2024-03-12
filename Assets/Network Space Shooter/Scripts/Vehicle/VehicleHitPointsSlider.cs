using UnityEngine;
using UnityEngine.UI;

namespace NetworkSpaceShooter
{
    public class VehicleHitPointsSlider : MonoBehaviour
    {
        [SerializeField] private Vehicle m_vehicle;
        [SerializeField] private Image m_fillImage;
        [SerializeField] private Slider m_slider;

        private void Start()
        {
            m_vehicle.HitPointsChange += OnHitPointsChange;

            m_fillImage.color = m_vehicle.Owner.GetComponent<Player>().PlayerColor;

            m_slider.maxValue = m_vehicle.MaxHitPoints;
            m_slider.value = m_vehicle.HitPoints;
        }

        private void OnDestroy()
        {
            m_vehicle.HitPointsChange -= OnHitPointsChange;
        }

        private void OnHitPointsChange(int hitPoints)
        {
            m_slider.value = hitPoints;
        }
    }
}
