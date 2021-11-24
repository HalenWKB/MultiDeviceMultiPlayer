using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerPaddles
{
    [RequireComponent(typeof(PaddleHandler))]
    public class PlayerPaddleInput : MonoBehaviour
    {
        private KeyCode m_leftKey = KeyCode.W;
        private KeyCode m_rightKey = KeyCode.S;

        public void SetKeys(KeyCode leftKey, KeyCode rightKey)
        {
            m_leftKey = leftKey;
            m_rightKey = rightKey;
        }

        private PaddleHandler m_paddle;

        void Start()
        {
            m_paddle = GetComponent<PaddleHandler>();
        }

        void Update()
        {
            bool lInput = Input.GetKey(m_leftKey);
            bool rInput = Input.GetKey(m_rightKey);

            if ((lInput || rInput) && !(lInput && rInput)) m_paddle.MoveInput(lInput);
        }
    }
}