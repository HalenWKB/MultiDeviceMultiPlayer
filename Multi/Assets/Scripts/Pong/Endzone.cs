using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PongPlayerPaddles
{
    public class Endzone : MonoBehaviour
    {
        [SerializeField] private Player m_playerRef = null;

        public void BallHit()
        {
            Managers.Gameplay.SomebodiesEndzoneWasHit(m_playerRef);
        }
    }
}
