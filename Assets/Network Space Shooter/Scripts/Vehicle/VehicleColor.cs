using UnityEngine;
using Mirror;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class VehicleColor : NetworkBehaviour
    {
        [SerializeField] private Vehicle m_vehicle;
        [SerializeField] private SpriteRenderer m_spriteRenderer;

        private void Start()
        {
            m_spriteRenderer.color = m_vehicle.Owner.GetComponent<Player>().PlayerColor;
        }
    }
}
