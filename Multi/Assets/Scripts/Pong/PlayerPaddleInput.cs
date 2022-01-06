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

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                float yOff = HelperFunctions.HowLeftOfRayIsPoint2D(touchPos, transform.position
                    , transform.position - transform.right);
                lInput = yOff > 0;
                rInput = !lInput;
                
                Debug.Log(transform.position.y + " - " + touchPos + " - " + touch.position);
            }
            
            if ((lInput || rInput) && !(lInput && rInput)) m_paddle.MoveInput(lInput);
            
            if (false && Input.GetKey(KeyCode.LeftShift)) m_paddle.DebugGrowInput();
        }
        
        void Update()
        {
            HandleMovement();
        }
    }
}