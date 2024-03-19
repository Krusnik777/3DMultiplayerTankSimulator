using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NetworkSpaceShooter
{
    public class CircleArea : MonoBehaviour
    {
        [SerializeField] private float m_radius;
        public float Radius => m_radius;

        public Vector2 GetRandomInsideZone()
        {
            var value = (Vector2)transform.position + (Vector2)UnityEngine.Random.insideUnitSphere * m_radius;

            return value;
        }

        #if UNITY_EDITOR

        private static Color gizmoColor = new Color(0, 1, 0, 0.3f);

        private void OnDrawGizmosSelected()
        {
            Handles.color = gizmoColor;
            Handles.DrawSolidDisc(transform.position, transform.forward, m_radius);
        }

        #endif
    }
}
