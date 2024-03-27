using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    public class Vehicle : Destructible
    {
        [SerializeField] protected float m_maxLinearSpeed;
        [Header("EngineSound")]
        [SerializeField] private AudioSource m_engineSound;
        [SerializeField] private float m_enginePitchModifier;
        [Header("ZoomOptics")]
        [SerializeField] protected Transform m_zoomOpticsPosition;
        [Header("Turret")]
        [SerializeField] protected Turret m_turret;
        public Transform ZoomOpticsPosition => m_zoomOpticsPosition;
        public Turret Turret => m_turret;

        protected Vector3 targetInputControl;

        public virtual float LinearVelocity => 0;

        public float NormalizedLinearVelocity
        {
            get
            {
                if (Mathf.Approximately(0, LinearVelocity)) return 0;

                return Mathf.Clamp01(LinearVelocity / m_maxLinearSpeed);
            }
        }

        public void SetTargetControl(Vector3 control) => targetInputControl = control.normalized;

        public void SetVisible(bool visible)
        {
            if (visible) SetLayerToAll("Default");
            else SetLayerToAll("IgnoreMainCamera");
        }

        public void Fire() => m_turret.Fire();

        public void ChangeProjectile(int index) => m_turret.ChangeProjectile(index);

        protected virtual void Update()
        {
            UpdateEngineSFX();
        }

        private void UpdateEngineSFX()
        {
            if (m_engineSound != null)
            {
                m_engineSound.pitch = 1.0f + NormalizedLinearVelocity * m_enginePitchModifier;
                m_engineSound.volume = 0.5f + NormalizedLinearVelocity;
            }
        }

        private void SetLayerToAll(string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);

            foreach(var t in transform.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }

        #region NetAim

        private Vector3 aim;

        public Vector3 NetAimPoint => aim;

        public void SetNetAim(Vector3 v) => aim = v;

        #endregion

    }
}
