using UnityEngine;

namespace MultiplayerTanks
{
    public class Vehicle : MonoBehaviour
    {
        [SerializeField] protected float m_maxLinearSpeed;
        [Header("EngineSound")]
        [SerializeField] private AudioSource m_engineSound;
        [SerializeField] private float m_enginePitchModifier;
        [Header("ZoomOptics")]
        [SerializeField] protected Transform m_zoomOpticsPosition;
        public Transform ZoomOpticsPosition => m_zoomOpticsPosition;

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

    }
}
