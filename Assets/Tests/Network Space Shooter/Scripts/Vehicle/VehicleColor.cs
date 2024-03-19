using UnityEngine;
using Mirror;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class VehicleColor : NetworkBehaviour
    {
        [SerializeField] private Vehicle m_vehicle;
        [SerializeField] private SpriteRenderer m_vehicleSpriteRenderer;
        [SerializeField] private SpriteRenderer m_turretSpriteRenderer;

        private void Start()
        {
            var color = m_vehicle.Owner.GetComponent<Player>().PlayerColor;

            m_vehicleSpriteRenderer.color = color;

            if (m_turretSpriteRenderer != null)
                m_turretSpriteRenderer.color = color;
        }
    }
}
