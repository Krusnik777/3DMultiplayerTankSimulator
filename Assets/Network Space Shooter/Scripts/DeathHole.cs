using UnityEngine;

namespace NetworkSpaceShooter
{
    public class DeathHole : MonoBehaviour
    {
        [SerializeField] private CircleArea m_movementArea;
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private int m_damage;

        private Vector3 m_targetPosition;

        private void SetMovementTarget() => m_targetPosition = m_movementArea.GetRandomInsideZone();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.root.TryGetComponent(out Destructible dest))
            {
                if (NetworkSessionManager.Instance.IsServer)
                {
                    dest.SvApplyDamage(m_damage);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.root.TryGetComponent(out Destructible dest))
            {
                if (NetworkSessionManager.Instance.IsServer)
                {
                    dest.SvApplyDamage(1);
                }
            }
        }

        private void Start()
        {
            transform.position = m_movementArea.GetRandomInsideZone();
        }

        private void Update()
        {
            if (m_targetPosition == null || transform.position == m_targetPosition) SetMovementTarget();

            transform.position = Vector2.MoveTowards(transform.position, m_targetPosition, m_movementSpeed * Time.deltaTime);
        }
    }
}
