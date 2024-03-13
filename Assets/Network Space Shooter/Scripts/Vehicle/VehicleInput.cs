using UnityEngine;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(Player))]
    public class VehicleInput : MonoBehaviour
    {
        private Player m_player;

        private void Awake()
        {
            m_player = GetComponent<Player>();
        }

        private void Update()
        {
            if (m_player.isOwned && m_player.isLocalPlayer)
            {
                UpdateControl();
            }
        }

        private void UpdateControl()
        {
            if (m_player.ActiveVehicle == null) return;

            float thrust = 0;
            float torque = 0;
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                thrust = 1.0f;

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                thrust = -1.0f;

            if (m_player.ActiveVehicle.Type == Vehicle.VehicleType.Tank)
            {
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                    torque = 1.0f;

                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    torque = -1.0f;
            }

            if (Input.GetMouseButtonDown(0))
                m_player.ActiveVehicle.Fire();

            if (Input.GetMouseButtonDown(1))
                m_player.ActiveVehicle.FireRocket();

            m_player.ActiveVehicle.ThrustControl = thrust;
            m_player.ActiveVehicle.TorqueControl = torque;
            m_player.ActiveVehicle.HeadingDirection = direction;
        }
    }
}
