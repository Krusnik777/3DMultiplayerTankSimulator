using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace NetworkSpaceShooter
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class PlayerColorPallete : NetworkBehaviour
    {
        public static PlayerColorPallete Instance;

        [SerializeField] private List<Color> m_allColors;

        private List<Color> availableColors;

        public Color TakeRandomColor()
        {
            int index = Random.Range(0, availableColors.Count);
            var color = availableColors[index];

            availableColors.RemoveAt(index);

            return color;
        }

        public void PutColor(Color color)
        {
            if (m_allColors.Contains(color))
            {
                if (!availableColors.Contains(color))
                {
                    availableColors.Add(color);
                }
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            availableColors = new List<Color>();
            m_allColors.CopyTo(availableColors);
        }
    }
}
