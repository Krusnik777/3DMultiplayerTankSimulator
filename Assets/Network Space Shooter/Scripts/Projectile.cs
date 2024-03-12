using UnityEngine;

namespace NetworkSpaceShooter
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private int m_damage;
        [SerializeField] private float m_lifeTime;
        [SerializeField] private GameObject m_destroySfx;

        private Transform m_parent;

        public void SetParent(Transform parent) => m_parent = parent;

        private void Start()
        {
            Destroy(gameObject, m_lifeTime);
        }

        private void Update()
        {
            float stepLength = Time.deltaTime * m_movementSpeed;
            Vector2 step = transform.up * stepLength;

            transform.position += new Vector3(step.x, step.y, 0);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, Time.deltaTime * m_movementSpeed);

            if (hit)
            {
                if (hit.collider.transform.root != m_parent)
                {
                    if (NetworkSessionManager.Instance.IsServer)
                    {
                        var dest = hit.collider.transform.root.GetComponent<Destructible>();

                        if (dest != null)
                        {
                            dest.SvApplyDamage(m_damage);
                        }
                    }

                    if (NetworkSessionManager.Instance.IsClient)
                    {
                        if (m_destroySfx != null)
                        {
                            Instantiate(m_destroySfx, transform.position, Quaternion.identity);
                        }
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}
