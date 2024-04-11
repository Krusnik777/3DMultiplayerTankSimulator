using Mirror;
using UnityEngine;

namespace MultiplayerTanks
{
    public class VehicleModule : Destructible
    {
        [SerializeField] private string m_title;
        [SerializeField] private Armor m_armor;
        [SerializeField] private float m_recoveredTime;

        [SyncVar]
        private float remainingRecoveryTime;

        public float RecoveryRate => 1 - remainingRecoveryTime / m_recoveredTime;

        private void Awake()
        {
            m_armor.SetDestructible(this);
        }

        private void Start()
        {
            Destroyed += OnModuleDestroyed;
            enabled = false;
        }

        private void OnDestroy()
        {
            Destroyed -= OnModuleDestroyed;
        }

        private void Update()
        {
            if (isServer)
            {
                remainingRecoveryTime -= Time.deltaTime;

                if (remainingRecoveryTime <=0)
                {
                    remainingRecoveryTime = 0.0f;

                    SvRecovery();

                    enabled = false;
                }
            }
        }

        private void OnModuleDestroyed(Destructible destructible)
        {
            remainingRecoveryTime = m_recoveredTime;
            enabled = true;
        }
    }
}
