using UnityEngine;
using UnityEngine.UI;

namespace NetworkSpaceShooter
{
    public class RocketReloadFill : MonoBehaviour
    {
        [SerializeField] private Turret m_turret;
        [SerializeField] private Image m_backgroundImage;
        [SerializeField] private Image m_fillImage;

        private Vehicle vehicle;

        private void Start()
        {
            m_fillImage.fillAmount = 0;

            vehicle = m_turret.GetComponent<Vehicle>();

            if (!vehicle.Owner.isOwned && !vehicle.Owner.isLocalPlayer)
            {
                m_backgroundImage.enabled = false;
                return;
            }

            m_turret.RocketReloadTimeUp += OnReload;
        }

        private void OnDestroy()
        {
            m_turret.RocketReloadTimeUp -= OnReload;
        }

        private void OnReload(float rocketReadiness)
        {
            if (vehicle == null) return;

            if (vehicle.Owner.isOwned && vehicle.Owner.isLocalPlayer)
            {
                m_fillImage.fillAmount = rocketReadiness;
            }
        }
    }
}
