using UnityEngine;

namespace MultiplayerTanks
{
    public enum ArmorType
    {
        Vehicle,
        Module
    }

    public class Armor : MonoBehaviour
    {
        [SerializeField] private Transform m_parent;
        [SerializeField] private ArmorType m_type;
        [SerializeField] private Destructible m_destructible;
        [SerializeField] private int m_thickness;

        public ArmorType Type => m_type;
        public Destructible Destructible => m_destructible;
        public int Thickness => m_thickness;

        public void SetDestructible(Destructible destructible) => m_destructible = destructible;

        private void Awake()
        {
            transform.SetParent(m_parent);
        }
    }
}
