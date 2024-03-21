using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(TrackTank))]
    public class TankTrackTextureMovement : MonoBehaviour
    {
        [SerializeField] private Renderer m_leftTrackRenderer;
        [SerializeField] private Renderer m_rightTrackRenderer;
        [SerializeField] private Vector2 m_direction;
        [SerializeField] private float m_modifier;

        private TrackTank m_tank;

        private void Start()
        {
            m_tank = GetComponent<TrackTank>();
        }

        private void FixedUpdate()
        {
            float speed = m_tank.LeftWheelRpm / 60.0f * m_modifier * Time.fixedDeltaTime;
            m_leftTrackRenderer.material.SetTextureOffset("_MainTex", m_leftTrackRenderer.material.GetTextureOffset("_MainTex") + m_direction * speed);

            speed = m_tank.RightWheelRpm / 60.0f * m_modifier * Time.fixedDeltaTime;
            m_rightTrackRenderer.material.SetTextureOffset("_MainTex", m_rightTrackRenderer.material.GetTextureOffset("_MainTex") + m_direction * speed);
        }
    }
}
