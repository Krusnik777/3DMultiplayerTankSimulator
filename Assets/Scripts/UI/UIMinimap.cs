using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIMinimap : MonoBehaviour
    {
        [SerializeField] private Transform m_mainCanvas;
        [SerializeField] private SizeMap m_sizeMap;
        [SerializeField] private UITankMark m_tankMarkPrefab;
        [SerializeField] private Image m_background;

        private UITankMark[] m_tankMarks;
        private Vehicle[] m_vehicles;

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
            if (m_tankMarks == null) return;

            for (int i = 0; i < m_tankMarks.Length; i++)
            {
                if (m_vehicles[i] == null) continue;

                if (m_vehicles[i] != Player.Local.ActiveVehicle)
                {
                    bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(m_vehicles[i].netIdentity);

                    m_tankMarks[i].gameObject.SetActive(isVisible);
                }

                if (!m_tankMarks[i].gameObject.activeSelf) continue;

                Vector3 normalizedPosition = m_sizeMap.GetNormalizedPosition(m_vehicles[i].transform.position);
                Vector3 positionInMinimap = new Vector3(normalizedPosition.x * m_background.rectTransform.sizeDelta.x * 0.5f, normalizedPosition.z * m_background.rectTransform.sizeDelta.y * 0.5f, 0);
                positionInMinimap.x *= m_mainCanvas.localScale.x;
                positionInMinimap.y *= m_mainCanvas.localScale.y;

                m_tankMarks[i].transform.position = m_background.transform.position + positionInMinimap;
            }
        }

        private void OnMatchStart()
        {
            m_vehicles = FindObjectsOfType<Vehicle>();

            m_tankMarks = new UITankMark[m_vehicles.Length];

            for (int i = 0; i < m_tankMarks.Length; i++)
            {
                m_tankMarks[i] = Instantiate(m_tankMarkPrefab);

                if (m_vehicles[i].TeamId == Player.Local.TeamId) m_tankMarks[i].SetLocalColor();
                else m_tankMarks[i].SetOtherColor();

                m_tankMarks[i].transform.SetParent(m_background.transform);
            }
        }

        private void OnMatchEnd()
        {
            for (int i = 0; i < m_background.transform.childCount;i++)
            {
                Destroy(m_background.transform.GetChild(i).gameObject);
            }

            m_tankMarks = null;
        }
    }
}
