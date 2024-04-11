using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIAmmunitionElement : MonoBehaviour
    {
        [SerializeField] private Text m_ammoCountText;
        [SerializeField] private Image m_projectileIcon;
        [SerializeField] private GameObject m_selectBorder;

        public void SetAmmunition(Ammunition ammunition)
        {
            m_projectileIcon.sprite = ammunition.ProjectileProperties.Icon;

            UpdateAmmoCount(ammunition.AmmoCount);
        }

        public void UpdateAmmoCount(int count)
        {
            m_ammoCountText.text = count.ToString();
        }

        public void Select()
        {
            m_selectBorder.SetActive(true);
        }

        public void UnSelect()
        {
            m_selectBorder.SetActive(false);
        }
    }
}
