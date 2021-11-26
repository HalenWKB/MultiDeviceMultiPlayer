using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DelaystartWaitingRoomController : MonoBehaviourPunCallbacks
{
    private PhotonView m_myPhotonView;

    private int m_playerCount;
    private int m_roomSize;

    [SerializeField] private int m_minPlayersToStart = 0;
    
    [SerializeField] private TextMeshProUGUI m_roomCountDisplay = null;
    [SerializeField] private TextMeshProUGUI m_timerToStartDisplay = null;

    private bool m_readyToCountdown;
    private bool m_readyToStart;
    private bool m_startingGame;
    
    private float m_timerToStartGame;
    private float m_notFullGameTimer;
    private float m_fullGameTimer;
    
    [SerializeField] private float m_maxWaitTime = 0;
    [SerializeField] private float m_maxFullGameWaitTime = 0;


    void Start()
    {
        m_myPhotonView = GetComponent<PhotonView>();
        m_fullGameTimer = m_maxFullGameWaitTime;
        m_notFullGameTimer = m_maxWaitTime;
        m_timerToStartGame = m_maxWaitTime;

        PlayerCountUpdate();
    }

    void PlayerCountUpdate()
    {
        m_playerCount = PhotonNetwork.PlayerList.Length;
        m_roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        m_roomCountDisplay.text = m_playerCount + ":" + m_roomSize;

        if (m_playerCount == m_roomSize)
        {
            m_readyToStart = true;
        }
        else if (m_playerCount >= m_minPlayersToStart)
        {
            m_readyToCountdown = true;
        }
        else
        {
            m_readyToCountdown = false;
            m_readyToStart = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();

        if (PhotonNetwork.IsMasterClient)
        {
            m_myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, m_timerToStartGame);
        }
    }

    [PunRPC]
    void RPC_SendTimer(float timeIn)
    {
        m_timerToStartGame = timeIn;
        m_notFullGameTimer = timeIn;
        if (timeIn < m_fullGameTimer)
        {
            m_fullGameTimer = timeIn;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }

    void Update()
    {
        WaitingForMorePlayers();
    }

    void WaitingForMorePlayers()
    {
        if (m_playerCount <= 1)
        {
            ResetTimer();
        }

        if (m_readyToStart)
        {
            m_fullGameTimer -= Time.deltaTime;
            m_timerToStartGame = m_fullGameTimer;
        }
        else if (m_readyToCountdown)
        {
            m_notFullGameTimer -= Time.deltaTime;
            m_timerToStartGame = m_notFullGameTimer;
        }

        string tempTimer = string.Format("{0:00}", m_timerToStartGame);
        m_timerToStartDisplay.text = tempTimer;
        if (m_timerToStartGame <= 0)
        {
            if (m_startingGame)
                return;
            StartGame();
        }
    }

    void ResetTimer()
    {
        m_timerToStartGame = m_maxWaitTime;
        m_notFullGameTimer = m_maxWaitTime;
        m_fullGameTimer = m_maxFullGameWaitTime;
    }

    void StartGame()
    {
        m_startingGame = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(3);
    }

    public void DelayCancel()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
