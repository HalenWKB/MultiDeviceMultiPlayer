using System.Collections;
using System.Collections.Generic;
using PlayerPaddles;
using UnityEngine;

namespace MainGameplay
{
    public class BallHandler : MonoBehaviour
    {
        [SerializeField] private float m_serveSpeed = 2;
        [SerializeField] private float m_serveHitSpeedBoostMod = 2;
    
        [SerializeField] private float m_speedUpVal = 1.1f;

        [SerializeField] private Vector3 m_startVelo;
        [SerializeField] private bool m_debugUseStartVelo;

        void Start()
        {
            if (!m_debugUseStartVelo) return;
            m_velocity = m_startVelo;
        }
        
        private Vector3 m_velocity;
        private bool serving;
    
        public void ServeBall(Vector3 direction)
        {
            m_velocity = direction * m_serveSpeed;
            serving = true;
        }

        void Update()
        {
            bool hitSomething;
            float castDist = m_velocity.magnitude * Time.deltaTime;
            do
            {
                RaycastHit hitInfo;
                hitSomething = Physics.BoxCast(transform.position, transform.localScale / 2, m_velocity
                    , out hitInfo, Quaternion.identity, castDist);
                
                if (hitSomething)
                {
                    Endzone endZoneHit = hitInfo.collider.GetComponent<Endzone>();

                    if (endZoneHit != null)
                    {
                        endZoneHit.BallHit();
                        Destroy(gameObject);
                        return;
                    }

                    PaddleHandler paddleHit = hitInfo.collider.GetComponent<PaddleHandler>();
                    bool hitIsOnPaddle = paddleHit != null;

                    m_velocity = hitIsOnPaddle
                        ? paddleHit.GetBallBounceVectorFromHit(hitInfo, m_velocity) * m_speedUpVal *
                          (serving ? m_serveHitSpeedBoostMod : 1)
                        : HelperFunctions.ReflectVectorOnNormal(m_velocity, hitInfo.normal);

                    serving = serving && !hitIsOnPaddle;

                    castDist = Mathf.Max(0, castDist - hitInfo.distance);
                }
            } while (hitSomething);
            
            
            transform.position += m_velocity * Time.deltaTime;
        }
    }
}
