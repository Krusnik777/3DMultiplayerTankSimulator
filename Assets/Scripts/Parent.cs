using UnityEngine;

namespace MultiplayerTanks
{
    public class Parent : MonoBehaviour
    {
        [SerializeField] private Transform m_parent;

        private void Awake()
        {
            transform.SetParent(m_parent);
        }
    }
}
