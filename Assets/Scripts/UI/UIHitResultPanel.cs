using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIHitResultPanel : MonoBehaviour
    {
        [SerializeField] private Transform m_spawnPanel;
        [SerializeField] private UIHitResultPopup m_hitResultPopup;

        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
        }

        private void OnDisable()
        {
            if (NetworkSessionManager.Match != null)
                NetworkSessionManager.Match.MatchStart -= OnMatchStart;

            if (Player.Local != null)
                Player.Local.ProjectileHitted -= OnProjectileHitted;
        }

        private void OnMatchStart()
        {
            Player.Local.ProjectileHitted += OnProjectileHitted;
        }

        private void OnProjectileHitted(ProjectileHitResult hitResult)
        {
            if (hitResult.type == ProjectileHitType.Environment) return;

            var hitPopup = Instantiate(m_hitResultPopup);
            hitPopup.transform.SetParent(m_spawnPanel);
            hitPopup.transform.localScale = Vector3.one;
            hitPopup.transform.position = Camera.main.WorldToScreenPoint(hitResult.point);

            if (hitResult.type == ProjectileHitType.Penetration) hitPopup.SetTypeResult("Armor Penetration!");
            if (hitResult.type == ProjectileHitType.Ricochet) hitPopup.SetTypeResult("Ricochet!");
            if (hitResult.type == ProjectileHitType.NoPenetration) hitPopup.SetTypeResult("No Armor Penetration!");
            if (hitResult.type == ProjectileHitType.ModulePenetration) hitPopup.SetTypeResult("Module Penetration!");
            if (hitResult.type == ProjectileHitType.ModuleNoPenetration) hitPopup.SetTypeResult("No Module Penetration!");

            hitPopup.SetDamageResult(hitResult.damage);
        }
    }
}
