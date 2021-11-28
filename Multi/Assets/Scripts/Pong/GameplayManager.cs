using System.Collections;
using System.Collections.Generic;
using PongPlayerPaddles;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PongMainGameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private List<Player> m_players = null;
        [SerializeField] private GameObject m_ballPrefab = null;
        [SerializeField] private Transform m_ballSpawn = null;
        [SerializeField] private int m_scoreToWin = 11;
        [SerializeField] private TextMeshProUGUI m_announcementText = null;

        [SerializeField] private bool m_debugStopBallSpawn = false;
        
        private BallHandler m_ball;
        private Player m_server;
        
        public Vector3 GetBallPos()
        {
            if (m_ball != null)
                return m_ball.transform.position;
            else
                return m_ballSpawn.position;
        }
        
        void Start()
        {
            Managers.Instance.GameplaySignIn(this);
            SetPlayersBasedOnGameMode();
            m_server = m_players[0];
            StartCoroutine( WaitWithTextBeforeAction("Get Ready!",SpawnBall));
        }

        void SetPlayersBasedOnGameMode()
        {
            GameMode mode = Managers.Mode.GetGameMode();
            
            m_players[0].SetInputMode_HumanOrAI(mode != GameMode.EvE);
            m_players[1].SetInputMode_HumanOrAI(mode == GameMode.PvP);
            
            if (m_players.Count > 2) Debug.LogWarning("TODO 3+ player functionality");
        }
        
        public void SomebodiesEndzoneWasHit(Player paddleHit)
        {
            string whoScored = "";
            int topScore = 0;
            string leadingPlayer;
            // Give a point to all players who didn't stuff up (Left like this for expandable 1v1v1v1 modes!
            for (int i = 0; i < m_players.Count; i++)
            {
                if (m_players[i] != paddleHit)
                {
                    m_players[i].GivePoint();
                    int score = m_players[i].GetScore();
                    if (score > topScore)
                    {
                        topScore = score;
                        leadingPlayer = m_players[i].m_name;
                    }
                    whoScored = m_players[i].m_name;
                }
            }

            m_server = paddleHit;

            if (topScore >= m_scoreToWin) 
                StartCoroutine( WaitWithTextBeforeAction(whoScored + " wins the match!",EndGame, 5));
            else 
                StartCoroutine( WaitWithTextBeforeAction(whoScored + " scored a point!",SpawnBall));
        }

        void SpawnBall()
        {
            if (m_debugStopBallSpawn) return;
            m_ball = Instantiate(m_ballPrefab, m_ballSpawn.position, m_ballSpawn.rotation).GetComponent<BallHandler>();
            m_ball.ServeBall((m_server.GetPaddlePos() - m_ball.transform.position).normalized);
        }

        void EndGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        IEnumerator WaitWithTextBeforeAction(string text, UnityAction action, float waitTime = 2)
        {
            m_announcementText.text = text;
            yield return new WaitForSeconds(waitTime);
            m_announcementText.text = "";
            action.Invoke();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Managers.Scene.RegularSceneChange(SceneCode.PongMenu);
            }
        }
    }
}

