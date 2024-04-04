using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Collider))]
    public class DamageZone : MonoBehaviour
    {
        [SerializeField] private int m_damage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.TryGetComponent(out Destructible destructible))
            {
                destructible.SvApplyDamage(m_damage);
            }
        }
    }
}
