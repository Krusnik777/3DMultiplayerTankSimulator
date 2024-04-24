using Mirror;
using UnityEngine;

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
        [Header("TeamId")]
        [SerializeField] private int m_teamId;
        [SerializeField] private VehicleViewer m_viewer;
        public Transform ZoomOpticsPosition => m_zoomOpticsPosition;
        public Turret Turret => m_turret;

        public int TeamId { get => m_teamId; set => m_teamId = value; }
        public VehicleViewer Viewer { get => m_viewer; set => m_viewer = value; }

        protected Vector3 targetInputControl;

        public virtual float LinearVelocity => 0;

        public bool IsStopped { get; set; }

        [SyncVar(hook = nameof(SetHidden))]
        private bool syncIsHidden;
        public bool IsHidden { get => syncIsHidden; set => syncIsHidden = value; }

        protected float syncLinearVelocity;

        private HidingSpot currentHidingSpot;
        public HidingSpot CurrentHidingSpot => currentHidingSpot;

        public float NormalizedLinearVelocity
        {
            get
            {
                if (Mathf.Approximately(0, syncLinearVelocity)) return 0;

                return Mathf.Clamp01(syncLinearVelocity / m_maxLinearSpeed);
            }
        }

        public void SetHidingSpot(HidingSpot hidingSpot) => currentHidingSpot = hidingSpot;

        public void SetTargetControl(Vector3 control) => targetInputControl = control.normalized;

        public void SetVisible(bool visible)
        {
            if (visible)
            {
                if(gameObject.layer != LayerMask.NameToLayer("Default"))
                    SetLayerToAll("Default");
            }
            else
            {
                if (gameObject.layer != LayerMask.NameToLayer("IgnoreMainCamera"))
                    SetLayerToAll("IgnoreMainCamera");
            }
        }

        public void Fire() => m_turret.Fire();

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

        [SyncVar]
        private Vector3 netAimPoint;

        private CommandMediator m_commandMediator = new CommandMediator();

        public Vector3 NetAimPoint
        {
            get => netAimPoint;

            set
            {
                netAimPoint = value; // Client

                if (isOwned) m_commandMediator.CmdSetNetAim(this, value);
            }
        }

        [Command]
        public void CmdSetNetAimPoint(Vector3 v)
        {
            netAimPoint = v;
        }

        #endregion

        [SyncVar(hook = nameof(T))]
        public NetworkIdentity Owner;

        private void T(NetworkIdentity oldValue, NetworkIdentity newValue) { }

        private void SetHidden(bool oldValue, bool newValue)
        {
            syncIsHidden = newValue;
        }

    }
}
