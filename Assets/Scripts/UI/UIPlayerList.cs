using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTanks
{
    public class UIPlayerList : MonoBehaviour
    {
        [SerializeField] private UIPlayerLabel m_playerLabelPrefab;
        [SerializeField] private Transform m_localTeamPanel;
        [SerializeField] private Transform m_otherTeamPanel;

        private List<UIPlayerLabel> m_allPlayerLabels = new List<UIPlayerLabel>();

        private void Start()
        {
            PlayerList.UpdatePlayerList += OnUpdatePlayerList;
            Player.ChangeFrags += OnChangeFrags;
        }

        private void OnDisable()
        {
            PlayerList.UpdatePlayerList -= OnUpdatePlayerList;
            Player.ChangeFrags -= OnChangeFrags;
        }

        private void OnUpdatePlayerList(List<PlayerData> playerDataList)
        {
            for (int i = 0; i < m_localTeamPanel.childCount; i++)
            {
                Destroy(m_localTeamPanel.GetChild(i).gameObject);
            }

            for (int i = 0; i < m_otherTeamPanel.childCount; i++)
            {
                Destroy(m_otherTeamPanel.GetChild(i).gameObject);
            }

            m_allPlayerLabels.Clear();

            for (int i = 0; i < playerDataList.Count; i++)
            {
                if (playerDataList[i].TeamId == Player.Local.TeamId) AddPlayerLabel(playerDataList[i], m_playerLabelPrefab, m_localTeamPanel);
                if (playerDataList[i].TeamId != Player.Local.TeamId) AddPlayerLabel(playerDataList[i], m_playerLabelPrefab, m_otherTeamPanel);
            }
        }

        private void AddPlayerLabel(PlayerData playerData, UIPlayerLabel playerLabel, Transform parent)
        {
            UIPlayerLabel label = Instantiate(playerLabel);
            label.transform.SetParent(parent);
            label.Init(playerData.Id, playerData.Nickname);

            m_allPlayerLabels.Add(label);
        }

        private void OnChangeFrags(int playerNetId, int frags)
        {
            for (int i = 0; i < m_allPlayerLabels.Count; i++)
            {
                if (m_allPlayerLabels[i].NetId == playerNetId)
                {
                    m_allPlayerLabels[i].UpdateFrags(frags);
                }
            }
        }
    }
}
