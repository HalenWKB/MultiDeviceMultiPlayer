using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace PongPlayerPaddles
{
    public struct PaddleBounceResult
    {
        public Vector3 resultingVect;
        public bool hitFront;
    }
    
    public class PaddleHandler : MonoBehaviour
    {
        [SerializeField] private float m_paddleSpeed = 10;
        [SerializeField] private float m_paddleSkewBallMod = 0.5f;

        [SerializeField] private float m_paddleMoveConstraintDist = 5f;
        
        [SerializeField] private GameObject m_debugPreGrowthIndicator = null;
        [SerializeField] private float m_debugGrowthSpeed = 1;
        
        [SerializeField] private float m_bounceRegisterDistCheck = 3;

        private Vector3 m_startPos;
    
        void Start()
        {
            m_startPos = transform.position;
            m_debugPreGrowthIndicator.SetActive(false);
        }
    
        public void MoveInput(bool isLeftInput)
        {
            float leftMoveMod = m_paddleSpeed * Time.deltaTime * (isLeftInput ? -1 : 1);

            float moveModOutOfBoundsBy =
                Mathf.Max(0, (m_startPos - (transform.position + transform.up * leftMoveMod)).magnitude - m_paddleMoveConstraintDist);

            leftMoveMod += isLeftInput ? moveModOutOfBoundsBy : -moveModOutOfBoundsBy;
        
            transform.position += transform.up * leftMoveMod;
            m_debugPreGrowthIndicator.transform.position = new Vector3(transform.position.x,transform.position.y
                ,m_debugPreGrowthIndicator.transform.position.z);
        }

        public void DebugGrowInput()
        {
            transform.localScale += new Vector3(0, m_debugGrowthSpeed * Time.deltaTime, 0);
            m_debugPreGrowthIndicator.SetActive(true);
        }


        bool IsBallHittingFrontOfPaddle(Vector3 ballHitNorm)
        {
            return HelperFunctions.IsLeftOfOrOnRay2D(transform.position + (ballHitNorm * 100),
                transform.position - transform.right, -transform.up) ;
        }
    
        public PaddleBounceResult GetBallBounceVectorFromHit(RaycastHit hit, Vector3 currentVect)
        {
            PaddleBounceResult result = new PaddleBounceResult();
            if (IsBallHittingFrontOfPaddle(hit.normal))
            {
                Vector3 resultVectOnPaddleNorm = Vector3.Project(-currentVect, hit.normal);
                resultVectOnPaddleNorm -= transform.up 
                                          * HelperFunctions.HowLeftOfRayIsPoint2D(hit.point, transform.position, -transform.right)
                                          * m_paddleSkewBallMod;
                result.hitFront = true;
                result.resultingVect = resultVectOnPaddleNorm;
            }
            else
            {
                result.hitFront = false;
                result.resultingVect = HelperFunctions.ReflectVectorOnNormal(currentVect, hit.normal);
            }

            return result;
        }

    }
}
