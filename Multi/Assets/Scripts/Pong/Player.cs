using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PongMainGameplay;
using TMPro;
using UnityEngine;

namespace PongPlayerPaddles
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
        
        [Header("Multiplayer Input Settings")]
        [SerializeField] private PhotonView m_photonView = null;

        public bool IsLocalPlayer()
        {
            return m_photonView.IsMine;
        }
        
        public void GivePoint()
        {
            m_score++;
            m_scoreDisplay.text = m_score.ToString("N0");
        }

        public int GetScore()
        {
            return m_score;
        }

        public void RelinkScoreDisplay(TextMeshProUGUI scoreDisplay)
        {
            m_scoreDisplay = scoreDisplay;
        }
        public void RelinkPlayerName()
        {
            m_name = m_photonView.Owner.NickName;
        }
        
        public DelinkedScoreDisplay GetScoreDisplay()
        {
            DelinkedScoreDisplay result = new DelinkedScoreDisplay();
            result.scoreDiplayer = m_scoreDisplay;
            result.masterLastPos = transform.position;
            return result;
        }

        
        
        public void SetupMultiplayerPlayer(Player baseOnPlayer, string name, bool flipInputs)
        {
            MultiplayerPaddleInput humanInput = m_paddle.gameObject.AddComponent<MultiplayerPaddleInput>();
            m_name = name;
            m_scoreDisplay = baseOnPlayer.m_scoreDisplay;
            humanInput.SetView(m_photonView, flipInputs);
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

