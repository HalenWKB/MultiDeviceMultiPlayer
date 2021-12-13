using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace PongPlayerPaddles
{
    [RequireComponent(typeof(PaddleHandler))]
    public class MultiplayerPaddleInput : MonoBehaviour
    {
        private KeyCode m_leftKey = KeyCode.W;
        private KeyCode m_rightKey = KeyCode.S;

        private PhotonView m_photonView;
        
        public void SetView(PhotonView pv, bool flipInputs)
        {
            m_photonView = pv;
            if (flipInputs)
            {
                (m_leftKey, m_rightKey) = (m_rightKey, m_leftKey);
            }
        }

        private PaddleHandler m_paddle;

        void Start()
        {
            m_paddle = GetComponent<PaddleHandler>();
        }

        void Update()
        {
            if (m_photonView != null && m_photonView.IsMine)
            {
                bool lInput = Input.GetKey(m_leftKey);
                bool rInput = Input.GetKey(m_rightKey);

                if ((lInput || rInput) && !(lInput && rInput)) m_paddle.MoveInput(lInput);
            }
        }
    }
}