using UnityEngine;

namespace MultiplayerTanks
{
    public class MinimapMark : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private Color m_localTeamColor;
        [SerializeField] private Color m_otherTeamColor;

        public void SetLocalColor() => m_spriteRenderer.color = m_localTeamColor;
        public void SetOtherColor() => m_spriteRenderer.color = m_otherTeamColor;
    }
}
