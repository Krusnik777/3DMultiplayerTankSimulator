using UnityEngine;

namespace MultiplayerTanks
{
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private Vector2 m_movementDirection;
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private float m_lifeTime;

        private void Start()
        {
            Destroy(gameObject, m_lifeTime);
        }

        private void Update()
        {
            transform.Translate(m_movementDirection * m_movementSpeed * Time.deltaTime);
        }
    }
}
