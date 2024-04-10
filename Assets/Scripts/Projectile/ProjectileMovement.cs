using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(Projectile))]
    public class ProjectileMovement : MonoBehaviour
    {
        private Projectile m_projectile;

        private Vector3 step;

        private void Awake()
        {
            m_projectile = GetComponent<Projectile>();

            step = new Vector3();
        }

        public void Move()
        {
            transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * m_projectile.Properties.Mass)).normalized;

            step = transform.forward * m_projectile.Properties.Velocity * Time.deltaTime;
            //Vector3 step = transform.forward * m_velocity * Time.deltaTime + new Vector3(spread.x, spread.y, 0);

            transform.position += step;
        }
    }
}
