using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PongPlayerPaddles;
using UnityEngine;

namespace PongMainGameplay
{
    public class BallHandler : MonoBehaviour
    {
        [SerializeField] private float m_serveSpeed = 2;
        [SerializeField] private float m_serveHitSpeedBoostMod = 2;
    
        [SerializeField] private float m_speedUpVal = 1.1f;

        [SerializeField] private Vector3 m_startVelo = new Vector3();
        [SerializeField] private bool m_debugUseStartVelo = false;

        [SerializeField] private PhotonView m_photonView = null;
        
        void Start()
        {
            if (!m_debugUseStartVelo) return;
            m_velocity = m_startVelo;
        }
        
        private Vector3 m_velocity;
        private bool serving;
        private bool dying = false;
        
        public void ServeBall(Vector3 direction)
        {
            m_velocity = direction * m_serveSpeed;
            serving = true;
        }

        void Update()
        {
            bool isOnline = Managers.Mode.GetGameMode() == GameMode.PONG_MP_PvP;
            if (dying) return;
            bool hitSomething;
            bool veloChanged = false;
            bool hitPaddleFront = false;
            float castDist = m_velocity.magnitude * Time.deltaTime;
            do
            {
                veloChanged = true;
                RaycastHit hitInfo;
                hitSomething = Physics.BoxCast(transform.position, transform.localScale / 2, m_velocity
                    , out hitInfo, Quaternion.identity, castDist);
                
                if (hitSomething)
                {
                    Endzone endZoneHit = hitInfo.collider.GetComponent<Endzone>();

                    if (endZoneHit != null)
                    {
                        HitEndzone(endZoneHit);
                        break;
                    }

                    PaddleHandler paddleHit = hitInfo.collider.GetComponent<PaddleHandler>();
                    bool hitIsOnPaddle = paddleHit != null;

                    if (hitIsOnPaddle)
                    {
                        PaddleBounceResult paddleBounce = paddleHit.GetBallBounceVectorFromHit(hitInfo, m_velocity);
                        m_velocity = paddleBounce.resultingVect * m_speedUpVal * (serving ? m_serveHitSpeedBoostMod : 1);
                        if (isOnline && paddleBounce.hitFront && !PhotonNetwork.IsMasterClient)
                            hitPaddleFront = true;
                    }
                    else
                        m_velocity = HelperFunctions.ReflectVectorOnNormal(m_velocity, hitInfo.normal);

                    
                    
                    serving = serving && !hitIsOnPaddle;

                    castDist = Mathf.Max(0, castDist - hitInfo.distance);
                }
            } while (hitSomething);

            Vector3 newPos = transform.position + m_velocity * Time.deltaTime;
            if (isOnline)
            {
                if (veloChanged && PhotonNetwork.IsMasterClient)
                    m_photonView.RPC("RPCVeloSync", RpcTarget.Others, m_velocity);
                if (hitPaddleFront && !PhotonNetwork.IsMasterClient)
                    m_photonView.RPC("RPCPaddleBounce", RpcTarget.Others, m_velocity, newPos);
            }
            
            if (!isOnline || PhotonNetwork.IsMasterClient)
                transform.position = newPos;
        }

        
        
        [PunRPC]
        void RPCVeloSync(Vector3 velo)
        {
            if (!PhotonNetwork.IsMasterClient)
                m_velocity = velo;
        }
        
        [PunRPC]
        void RPCPaddleBounce(Vector3 veloAtHit, Vector3 posAtHit)
        {
            m_velocity = veloAtHit;
            if (PhotonNetwork.IsMasterClient)
            {
                transform.position = posAtHit;
                m_photonView.RPC("RPCVeloSync", RpcTarget.Others, m_velocity);
            }
            m_RPCPaddleHitScoreOverride = true;
        }
        
        void HitEndzone(Endzone ez)
        {
            if (dying) return;
            dying = true;
            StartCoroutine(DelayedDestroyAndScore(ez));
        }

        private bool m_RPCPaddleHitScoreOverride;
        IEnumerator DelayedDestroyAndScore(Endzone endzoneHit)
        {
            m_RPCPaddleHitScoreOverride = false;
            yield return new WaitForSeconds(1f);
            if (m_RPCPaddleHitScoreOverride)
                dying = false;
            else
            {
                if (Managers.Mode.GetGameMode() == GameMode.PONG_MP_PvP)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(gameObject);
                        endzoneHit.BallHit();
                    }
                }
                else
                {
                    Destroy(gameObject);
                    endzoneHit.BallHit();
                }
                    
            }
        }
    }
}
