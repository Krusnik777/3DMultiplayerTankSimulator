using UnityEngine;

namespace MultiplayerTanks
{
    [System.Serializable]
    public class ParameterCurve
    {
        [SerializeField] private AnimationCurve m_curve;
        [SerializeField] private float m_duration = 1;

        private float expiredTime;

        public float MoveTowards(float deltaTime)
        {
            expiredTime += deltaTime;

            return m_curve.Evaluate(expiredTime / m_duration);
        }

        public float Reset()
        {
            expiredTime = 0;

            return m_curve.Evaluate(0);
        }

        public float GetValueBetween(float startValue, float endValue, float currentValue)
        {
            if (m_curve.length == 0 || startValue == endValue) return 0;

            float startTime = m_curve.keys[0].time;
            float endTime = m_curve.keys[m_curve.length - 1].time;

            float currentTime = Mathf.Lerp(startTime, endTime, (currentValue - startValue)/(endValue - startValue));

            return m_curve.Evaluate(currentTime);
        }
    }
}
