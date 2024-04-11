using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIHitResultPopup : UIPopup
    {
        [SerializeField] private Text m_typeText;
        [SerializeField] private Text m_damageText;

        public void SetTypeResult(string textResult) => m_typeText.text = textResult;

        public void SetDamageResult(float damage)
        {
            if (damage <= 0)
            {
                m_damageText.text = "";
                return;
            }

            m_damageText.text = "-" + damage.ToString("F0");
        }
    }
}
