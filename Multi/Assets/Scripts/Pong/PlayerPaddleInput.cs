using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace PongPlayerPaddles
{
    [RequireComponent(typeof(PaddleHandler))]
    public class PlayerPaddleInput : MonoBehaviour
    {
        protected KeyCode m_leftKey = KeyCode.W;
        protected KeyCode m_rightKey = KeyCode.S;
        
        public void SetKeys(KeyCode leftKey, KeyCode rightKey)
        {
            m_leftKey = leftKey;
            m_rightKey = rightKey;
        }

        protected PaddleHandler m_paddle;

        public void PaddleStart()
        {
            m_paddle = GetComponent<PaddleHandler>();
        }

        protected void HandleMovement()
        {
            bool lInput = Input.GetKey(m_leftKey);
            bool rInput = Input.GetKey(m_rightKey);

            if ((lInput || rInput) && !(lInput && rInput)) m_paddle.MoveInput(lInput);
            
            if (Input.GetKey(KeyCode.LeftShift)) m_paddle.DebugGrowInput();
        }
        
        void Update()
        {
            HandleMovement();
        }
    }
}