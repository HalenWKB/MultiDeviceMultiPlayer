using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace PongPlayerPaddles
{
    [RequireComponent(typeof(PaddleHandler))]
    public class MultiplayerPaddleInput : PlayerPaddleInput
    {
        private PhotonView m_photonView;
        
        public void SetView(PhotonView pv)
        {
            m_photonView = pv;
        }

        void Update()
        {
            if (m_photonView != null && m_photonView.IsMine)
            {
                HandleMovement();
            }
        }
    }
}