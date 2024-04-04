using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UITankInfoCollector : MonoBehaviour
    {
        [SerializeField] private Transform m_tankInfoPanel;
        [SerializeField] private UITankInfo m_tankInfoPrefab;

        private UITankInfo[] m_tanksInfo;
        private List<Player> m_playersWithoutLocal;

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
                if (m_tanksInfo[i] == null || m_playersWithoutLocal[i].ActiveVehicle == null) continue;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(m_tanksInfo[i].Tank.transform.position + (VehicleCamera.Instance.IsZoomed ? m_tanksInfo[i].WorldZoomOffset : m_tanksInfo[i].WorldOffset));

                if (screenPos.z > 0)
                {
                    m_tanksInfo[i].transform.position = screenPos;
                }
            }
        }

        private void OnMatchStart()
        {
            Player[] players = FindObjectsOfType<Player>();

            m_playersWithoutLocal = new List<Player>();

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == Player.Local) continue;

                m_playersWithoutLocal.Add(players[i]);
            }

            m_tanksInfo = new UITankInfo[m_playersWithoutLocal.Count];

            for (int i = 0; i < m_playersWithoutLocal.Count; i++)
            {
                m_tanksInfo[i] = Instantiate(m_tankInfoPrefab);

                m_tanksInfo[i].SetTank(m_playersWithoutLocal[i].ActiveVehicle);
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
