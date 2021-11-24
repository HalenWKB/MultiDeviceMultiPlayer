using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Realtime;


public class QuickstartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject m_connectingToServerMessage = null;
    [SerializeField] private GameObject m_networkTestButtons = null;
    [SerializeField] private GameObject m_quickStartLoadingButtons = null;
    [SerializeField] private int m_roomSize = 2;

    [SerializeField] private MainMainMenu m_mainMainMenu = null;

    void Start()
    {
        m_connectingToServerMessage.SetActive(!PhotonNetwork.IsConnected);
        m_networkTestButtons.SetActive(PhotonNetwork.IsConnected);
        m_quickStartLoadingButtons.SetActive(false);
    }
    
    public void Back()
    {
        //QuickCancel();
        m_mainMainMenu.BackToMain();
    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        m_connectingToServerMessage.SetActive(false);
        m_networkTestButtons.SetActive(true);
    }

    public void QuickStart()
    {
        m_networkTestButtons.SetActive(false);
        m_quickStartLoadingButtons.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Quick start pressed");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    void CreateRoom()
    {
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = (byte) m_roomSize};
        int randomRoomNumber = Random.Range(0, 1000);
        string roomName = "Room" + randomRoomNumber;
        PhotonNetwork.CreateRoom(roomName, roomOps);
        Debug.Log("Creating room: "+roomName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... trying again");
        CreateRoom();
    }

    public void QuickCancel()
    {
        m_quickStartLoadingButtons.SetActive(false);
        m_networkTestButtons.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
