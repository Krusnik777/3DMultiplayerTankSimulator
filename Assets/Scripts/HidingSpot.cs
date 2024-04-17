using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Collider))]
    public class HidingSpot : MonoBehaviour
    {
        [SerializeField] private float m_reappearingTime = 10.0f;

        private Vehicle m_hiddenVehicle;
        public Vehicle HiddenVehicle => m_hiddenVehicle;

        private bool deactivated = false;
        public bool Deactivated => deactivated;

        private float timer;

        public void SetVisible(bool state)
        {
            if (state)
            {
                if (gameObject.layer != LayerMask.NameToLayer("Default"))
                    SetLayerToAll("Default");
            }
            else
            {
                if (gameObject.layer != LayerMask.NameToLayer("IgnoreMainCamera"))
                    SetLayerToAll("IgnoreMainCamera");
            }
        }

        public void UnHide()
        {
            m_hiddenVehicle.IsHidden = false;

            timer = m_reappearingTime;
            deactivated = true;
        }

        private void Update()
        {
            if (!NetworkSessionManager.Instance.IsServer) return;

            if (!deactivated) return;

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                deactivated = false;

                if (m_hiddenVehicle != null) m_hiddenVehicle.IsHidden = true;
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_hiddenVehicle != null)
            {
                if (other.transform.root.TryGetComponent(out Vehicle vehicle))
                {
                    if (vehicle.TeamId != m_hiddenVehicle.TeamId) m_hiddenVehicle.IsHidden = false;
                }
            }
            else
            {
                if (other.transform.root.TryGetComponent(out m_hiddenVehicle))
                {
                    m_hiddenVehicle.SetHidingSpot(this);
                    m_hiddenVehicle.IsHidden = !deactivated;
                    m_hiddenVehicle.Turret.Fired += UnHide;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (m_hiddenVehicle != null)
            {
                var vehicle = other.transform.root.GetComponent<Vehicle>();

                if (vehicle == null) return;

                if (m_hiddenVehicle != vehicle) return;

                m_hiddenVehicle.Turret.Fired -= UnHide;
                m_hiddenVehicle.IsHidden = false;
                m_hiddenVehicle.SetHidingSpot(null);
                m_hiddenVehicle = null;
            }
        }

        private void SetLayerToAll(string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);

            foreach (var t in transform.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }
    }
}
