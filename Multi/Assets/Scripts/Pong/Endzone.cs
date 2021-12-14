using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace PongPlayerPaddles
{
    public class Endzone : MonoBehaviour
    {
        [SerializeField] private Player m_playerRef = null;

        public void BallHit()
        {
            m_playerRef.MyEndzoneWasHit();
        }
    }
}
