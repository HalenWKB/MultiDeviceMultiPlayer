using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace PongPlayerPaddles
{
    public class PaddleHandler : MonoBehaviour
    {
        [SerializeField] private float m_paddleSpeed = 10;
        [SerializeField] private float m_paddleSkewBallMod = 0.5f;

        [SerializeField] private float m_paddleMoveConstraintDist = 5f;


        private Vector3 m_startPos;
    
        void Start()
        {
            m_startPos = transform.position;
        }
    
        public void MoveInput(bool isLeftInput)
        {
            float leftMoveMod = m_paddleSpeed * Time.deltaTime * (isLeftInput ? -1 : 1);

            float moveModOutOfBoundsBy =
                Mathf.Max(0, (m_startPos - (transform.position + transform.up * leftMoveMod)).magnitude - m_paddleMoveConstraintDist);

            leftMoveMod += isLeftInput ? moveModOutOfBoundsBy : -moveModOutOfBoundsBy;
        
            transform.position += transform.up * leftMoveMod;
        }


        bool IsBallHittingFrontOfPaddle(Vector3 ballHitNorm)
        {
            return HelperFunctions.IsLeftOfOrOnRay2D(transform.position + (ballHitNorm * 100),
                transform.position - transform.right, -transform.up) ;
        }
    
        public Vector3 GetBallBounceVectorFromHit(RaycastHit hit, Vector3 currentVect)
        {
            if (IsBallHittingFrontOfPaddle(hit.normal))
            {
                Vector3 resultVectOnPaddleNorm = Vector3.Project(-currentVect, hit.normal);
                resultVectOnPaddleNorm -= transform.up 
                                          * HelperFunctions.HowLeftOfRayIsPoint2D(hit.point, transform.position, -transform.right)
                                          * m_paddleSkewBallMod;
                return resultVectOnPaddleNorm;
            }
            else return HelperFunctions.ReflectVectorOnNormal(currentVect, hit.normal);
        }
    }
}
