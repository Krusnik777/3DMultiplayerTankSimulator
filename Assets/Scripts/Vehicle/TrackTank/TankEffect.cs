using UnityEngine;

namespace MultiplayerTanks
{
    [RequireComponent(typeof(TrackTank))]
    public class TankEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] m_exhaust;
        [SerializeField] private ParticleSystem[] m_exhaustAtMovementStart;
        [SerializeField] private Vector2 m_minMaxExhaustEmission;

        private TrackTank m_tank;

        private bool isTankStopped;

        private void Start()
        {
            m_tank = GetComponent<TrackTank>();
        }

        private void Update()
        {
            float exhaustEmission = Mathf.Lerp(m_minMaxExhaustEmission.x, m_minMaxExhaustEmission.y, m_tank.NormalizedLinearVelocity);

            for (int i = 0; i < m_exhaust.Length; i++)
            {
                ParticleSystem.EmissionModule emission = m_exhaust[i].emission;
                emission.rateOverTime = exhaustEmission;
            }

            if (m_tank.LinearVelocity < 0.1f) isTankStopped = true;

            if (m_tank.LinearVelocity > 1)
            {
                if (isTankStopped)
                {
                    for (int i = 0; i < m_exhaustAtMovementStart.Length; i++)
                    {
                        m_exhaustAtMovementStart[i].Play();
                    }
                }

                isTankStopped = false;
            }

        }
    }
}
