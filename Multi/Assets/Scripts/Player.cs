using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PlayerPaddles
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PaddleHandler m_paddle = null;

        [SerializeField] private TextMeshProUGUI m_scoreDisplay = null;
        private int m_score;

        public string m_name;

        public Vector3 GetPaddlePos()
        {
            return m_paddle.transform.position;
        }
    
        [Header("Human Input Settings")]
        [SerializeField] private KeyCode m_leftKey = KeyCode.W;
        [SerializeField] private KeyCode m_rightKey = KeyCode.S;
    
    
        [Header("AI Input Settings")]
        [SerializeField] private float m_aiReactionTimeMin = 0.05f;
        [SerializeField] private float m_aiReactionTimeMax = 0.02f;

        [SerializeField] private float m_aiReadInaccuracy = 1f;
    
        public void GivePoint()
        {
            m_score++;
            m_scoreDisplay.text = m_score.ToString("N0");
        }

        public int GetScore()
        {
            return m_score;
        }
    
        public void SetInputMode_HumanOrAI(bool humanPlayer)
        {
            if (humanPlayer)
            {
                PlayerPaddleInput humanInput = m_paddle.gameObject.AddComponent<PlayerPaddleInput>();
                humanInput.SetKeys(m_leftKey, m_rightKey);
            }
            else
            {
                AIPaddleInput aiInput = m_paddle.gameObject.AddComponent<AIPaddleInput>();
                aiInput.SetAIBehaviourValues(m_aiReactionTimeMin,m_aiReactionTimeMax,m_aiReadInaccuracy);
            }
        }
    }
}

