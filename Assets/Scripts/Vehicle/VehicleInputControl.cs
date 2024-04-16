using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Player))]
    public class VehicleInputControl : MonoBehaviour
    {
        public const float AimDistance = 1000;

        private Player m_player;

        public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
        {
            Ray ray = new Ray(start, direction);

            RaycastHit[] hits = Physics.RaycastAll(ray, AimDistance);

            var vehicle = Player.Local.ActiveVehicle;

            for (int i = hits.Length - 1; i >= 0; i--)
            {
                if (hits[i].collider.isTrigger) continue;

                if (hits[i].collider.transform.root.GetComponent<Vehicle>() == vehicle) continue;

                return hits[i].point;
            }

            return ray.GetPoint(AimDistance);
        }

        private void Awake()
        {
            m_player = GetComponent<Player>();
        }

        protected virtual void Update()
        {
            if (m_player == null) return;

            if (m_player.ActiveVehicle == null) return;

            if (m_player.isOwned && m_player.isLocalPlayer)
            {
                m_player.ActiveVehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
                m_player.ActiveVehicle.NetAimPoint = TraceAimPointWithoutPlayerVehicle(VehicleCamera.Instance.transform.position, VehicleCamera.Instance.transform.forward);

                if (Input.GetMouseButtonDown(0)) m_player.ActiveVehicle.Fire();

                
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    m_player.ActiveVehicle.Turret.SetSelectedProjectile(0);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    m_player.ActiveVehicle.Turret.SetSelectedProjectile(1);
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    m_player.ActiveVehicle.Turret.SetSelectedProjectile(2);
                }
            }
        }
    }
}
