using NetworkSpaceShooter;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTanks
{
    public class UIAmmunitionPanel : MonoBehaviour
    {
        [SerializeField] private Transform m_ammunitionPanel;
        [SerializeField] private UIAmmunitionElement m_ammunitionElementPrefab;

        private Turret m_turret;

        private List<UIAmmunitionElement> allAmmunitionElements = new List<UIAmmunitionElement>();
        private List<Ammunition> allAmmunition = new List<Ammunition>();

        private int lastSelectedAmmunitionIndex;

        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
        }

        private void OnDisable()
        {
            if (NetworkSessionManager.Match != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnMatchStart;
                NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
            }
        }

        private void OnMatchStart()
        {
            m_turret = Player.Local.ActiveVehicle.Turret;
            m_turret.UpdateSelectedAmmunition += OnTurretUpdateSelectedAmmunition;

            allAmmunitionElements.Clear();
            allAmmunition.Clear();

            foreach (var child in m_ammunitionPanel.GetComponentsInChildren<UIAmmunitionElement>())
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < m_turret.Ammunition.Length; i++)
            {
                var ammunitionElement = Instantiate(m_ammunitionElementPrefab);
                ammunitionElement.transform.SetParent(m_ammunitionPanel);
                ammunitionElement.transform.localScale = Vector3.one;
                ammunitionElement.SetAmmunition(m_turret.Ammunition[i]);

                m_turret.Ammunition[i].AmmoCountChanged += OnAmmoCountChanged;

                allAmmunitionElements.Add(ammunitionElement);
                allAmmunition.Add(m_turret.Ammunition[i]);

                if (i == 0)
                {
                    ammunitionElement.Select();
                    lastSelectedAmmunitionIndex = i;
                }
            }
        }

        private void OnMatchEnd()
        {
            if (m_turret != null)
            {
                m_turret.UpdateSelectedAmmunition -= OnTurretUpdateSelectedAmmunition;
            }

            for (int i = 0; i < allAmmunition.Count; i++)
            {
                allAmmunition[i].AmmoCountChanged -= OnAmmoCountChanged;
            }
        }

        private void OnTurretUpdateSelectedAmmunition(int index)
        {
            allAmmunitionElements[lastSelectedAmmunitionIndex].UnSelect();
            allAmmunitionElements[index].Select();

            lastSelectedAmmunitionIndex = index;
        }

        private void OnAmmoCountChanged(int ammo)
        {
            allAmmunitionElements[m_turret.SelectedAmmunitionIndex].UpdateAmmoCount(ammo);
        }
    }
}
