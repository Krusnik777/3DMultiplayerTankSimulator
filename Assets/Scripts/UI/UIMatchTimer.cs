using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTanks
{
    public class UIMatchTimer : MonoBehaviour
    {
        [SerializeField] private MatchTimer m_timer;
        [SerializeField] private Text m_text;

        private Coroutine m_timerRoutine;

        private void Start()
        {
            if (m_timerRoutine != null) StopCoroutine(m_timerRoutine);

            m_timerRoutine = StartCoroutine(UpdateTimer());
        }

        private IEnumerator UpdateTimer()
        {
            while (true)
            {
                m_text.text = TimeSpan.FromSeconds(m_timer.TimeLeft).ToString(@"mm\:ss");

                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
