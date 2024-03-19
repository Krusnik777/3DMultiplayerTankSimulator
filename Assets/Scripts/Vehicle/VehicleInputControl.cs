using UnityEngine;

namespace MultiplayerTanks
{
    public class VehicleInputControl : MonoBehaviour
    {
        [SerializeField] private Vehicle m_vehicle;

        protected virtual void Update()
        {
            m_vehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
        }
    }
}
