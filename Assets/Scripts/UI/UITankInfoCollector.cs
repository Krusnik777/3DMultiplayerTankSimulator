using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTanks
{
    public class UITankInfoCollector : MonoBehaviour
    {
        [SerializeField] private Transform m_tankInfoPanel;
        [SerializeField] private UITankInfo m_tankInfoPrefab;

        private UITankInfo[] m_tanksInfo;
        private List<Vehicle> m_notLocalVehicles;

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

        private void Update()
        {
            if (m_tanksInfo == null) return;

            for (int i = 0; i < m_tanksInfo.Length; i++)
            {
                if (m_tanksInfo[i] == null || m_notLocalVehicles[i] == null) continue;

                if (Player.Local == null || Player.Local.ActiveVehicle == null) continue;

                bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(m_tanksInfo[i].Tank.netIdentity);

                m_tanksInfo[i].gameObject.SetActive(isVisible);

                if (!m_tanksInfo[i].gameObject.activeSelf) continue;

                Vector3 pos = m_tanksInfo[i].Tank.transform.position + (VehicleCamera.Instance.IsZoomed ? m_tanksInfo[i].WorldZoomOffset : m_tanksInfo[i].WorldOffset);
                pos.y = Mathf.Round(pos.y);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);

                if (screenPos.z > 0)
                {
                    m_tanksInfo[i].transform.position = screenPos;
                }
            }
        }

        private void OnMatchStart()
        {
            var vehicles = FindObjectsOfType<Vehicle>();

            m_notLocalVehicles = new List<Vehicle>();

            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i] == Player.Local.ActiveVehicle) continue;

                m_notLocalVehicles.Add(vehicles[i]);
            }

            m_tanksInfo = new UITankInfo[m_notLocalVehicles.Count];

            for (int i = 0; i < m_notLocalVehicles.Count; i++)
            {
                m_tanksInfo[i] = Instantiate(m_tankInfoPrefab);

                m_tanksInfo[i].SetTank(m_notLocalVehicles[i]);
                m_tanksInfo[i].transform.SetParent(m_tankInfoPanel);
            }
        }

        private void OnMatchEnd()
        {
            for (int i = 0; i < m_tankInfoPanel.childCount; i++)
            {
                Destroy(m_tankInfoPanel.transform.GetChild(i).gameObject);
            }

            m_tanksInfo = null;
        }
    }
}
