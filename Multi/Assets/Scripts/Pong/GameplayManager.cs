using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using PongPlayerPaddles;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PongMainGameplay
{
    public struct DelinkedScoreDisplay
    {
        public Vector3 masterLastPos;
        public TextMeshProUGUI scoreDiplayer;
    }
    
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private List<Player> m_players = null;
        [SerializeField] private GameObject m_ballPrefab = null;
        [SerializeField] private Transform m_ballSpawn = null;
        [SerializeField] private int m_scoreToWin = 11;
        [SerializeField] private TextMeshProUGUI m_announcementText = null;

        [SerializeField] private bool m_debugStopBallSpawn = false;

        

        private List<DelinkedScoreDisplay> m_delinkedScoreDisplays;
        
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
            StartCoroutine( WaitWithTextBeforeAction("Awaiting Players...",WireUpPlayers,0,HaveAllPlayersJoined));
            StartCoroutine( WaitWithTextBeforeAction("Get Ready!",SpawnBall));
        }

        void SetPlayersBasedOnGameMode()
        {
            GameMode mode = Managers.Mode.GetGameMode();

            if (mode == GameMode.PONG_MP_PvP)
            {
                int playerID = PhotonNetwork.IsMasterClient ? 0 : 1;
                m_delinkedScoreDisplays = new List<DelinkedScoreDisplay>();
                for (int i = m_players.Count - 1; i >= 0; i--)
                {
                    Player oldPlayer = m_players[i];
                    if (playerID == i)
                    {
                        m_players[i] = SwitchToNewMultiplayerPlayer(m_players[i]);
                        m_players[i].SetupMultiplayerPlayer(oldPlayer
                            , PhotonNetwork.LocalPlayer.NickName, !PhotonNetwork.IsMasterClient);
                    }
                    else
                    {
                        m_delinkedScoreDisplays.Add(m_players[i].GetScoreDisplay());
                        m_players.RemoveAt(i);
                    }
                    Destroy(oldPlayer.gameObject);
                }
            }
            else
            {
                m_players[0].SetInputMode_HumanOrAI(mode != GameMode.PONG_SP_EvE);
                m_players[1].SetInputMode_HumanOrAI(mode == GameMode.PONG_SP_PvP);
            }
            
            if (m_players.Count > 2) Debug.LogWarning("TODO 3+ player functionality");
        }

        Player SwitchToNewMultiplayerPlayer(Player oldPlayer)
        {
            Vector3 loc = oldPlayer.transform.position;
            Quaternion rot = oldPlayer.transform.rotation;
            return PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PongPlayer"), loc, rot).GetComponent<Player>();
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
            bool multiplayerMode = Managers.Mode.GetGameMode() == GameMode.PONG_MP_PvP;
            if (m_debugStopBallSpawn 
                || (multiplayerMode && !PhotonNetwork.IsMasterClient)) return;
            if (multiplayerMode)
                m_ball = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PongBall")
                    , m_ballSpawn.position, m_ballSpawn.rotation).GetComponent<BallHandler>();
            else 
                m_ball = Instantiate(m_ballPrefab, m_ballSpawn.position, m_ballSpawn.rotation).GetComponent<BallHandler>();
            m_ball.ServeBall((m_server.GetPaddlePos() - m_ball.transform.position).normalized);
        }

        bool HaveAllPlayersJoined()
        {
            m_recentlyFoundPlayers = FindObjectsOfType<Player>();
            return m_recentlyFoundPlayers.Length == 2;
        }

        private Player[] m_recentlyFoundPlayers;
        void WireUpPlayers()
        {
            for (int i = 0; i < m_recentlyFoundPlayers.Length; i++)
            {
                if (!m_recentlyFoundPlayers[i].IsLocalPlayer())
                {
                    m_players.Add(m_recentlyFoundPlayers[i]);
                    m_recentlyFoundPlayers[i].RelinkPlayerName();
                    
                    int closestSDIdx = -1;
                    float closestSDDist = Mathf.Infinity;
                    for (int j = 0; j < m_delinkedScoreDisplays.Count; j++)
                    {
                        float dist = (m_delinkedScoreDisplays[j].masterLastPos
                                      - m_recentlyFoundPlayers[i].transform.position).magnitude;
                        if (dist < closestSDDist)
                        {
                            closestSDDist = dist;
                            closestSDIdx = j;
                        }
                    }

                    if (closestSDIdx >= 0)
                    {
                        m_recentlyFoundPlayers[i]
                            .RelinkScoreDisplay(m_delinkedScoreDisplays[closestSDIdx].scoreDiplayer);
                        m_delinkedScoreDisplays.RemoveAt(closestSDIdx);
                    }
                }
            }
        }

        void EndGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        IEnumerator WaitWithTextBeforeAction(string text, UnityAction action, float waitTime = 2, Func<bool> waitUntil = null)
        {
            m_announcementText.text = text;
            yield return new WaitForSeconds(waitTime);
            if (waitUntil != null)
            {
                yield return new WaitUntil(waitUntil);
            }
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

