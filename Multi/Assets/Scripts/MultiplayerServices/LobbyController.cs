using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Realtime;
using TMPro;

namespace MultiplayerServices
{
    public enum LobbyMode
    {
        Quickstart,
        Delaystart,
        Custom
    }

    public class LobbyController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject m_connectingToServerMessage = null;
        [SerializeField] private GameObject m_networkTestButtons = null;
        [SerializeField] private GameObject m_loadingButtons = null;
        
        [SerializeField] private GameObject m_customGameContent = null;
        [SerializeField] private GameObject m_customLobby = null;
        [SerializeField] private GameObject m_customLogin = null;
        
        [SerializeField] private TMP_InputField m_customPlayerName = null;
        
        [SerializeField] private Transform m_customRoomsContainer = null;
        [SerializeField] private GameObject m_customRoomListingPrefab = null;
        
        [SerializeField] private int m_roomSize = 4;

        [SerializeField] private MainMainMenu m_mainMainMenu = null;


        private string m_customRoomName;
        private int m_customRoomSize;
        private List<RoomInfo> m_roomListings;
        
        private LobbyMode m_lobbyMode = LobbyMode.Quickstart;

        private bool m_inCustomMatchmaking;
        
        public LobbyMode GetLobbyMode()
        {
            return m_lobbyMode;
        }

        void Start()
        {
            m_connectingToServerMessage.SetActive(!PhotonNetwork.IsConnected);
            m_networkTestButtons.SetActive(PhotonNetwork.IsConnected);
            m_loadingButtons.SetActive(false);
            m_roomListings = new List<RoomInfo>();
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
            m_networkTestButtons.SetActive(!m_inCustomMatchmaking);
            m_customGameContent.SetActive(m_inCustomMatchmaking);

            string nickName = "";
            if (PlayerPrefs.HasKey("NickName"))
                nickName = PlayerPrefs.GetString("NickName");
            
            if (nickName == "")
                PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
            else
                PhotonNetwork.NickName = nickName;

            m_customPlayerName.text = PhotonNetwork.NickName;
        }

        public void CustomPlayerNameUpdate(string nameInput)
        {
            Debug.Log("Name set to: " + nameInput);
            PhotonNetwork.NickName = nameInput;
            PlayerPrefs.SetString("NickName",nameInput);
        }

        public void CustomJoinLobbyOnClick()
        {
            m_customLogin.SetActive(false);
            m_customLobby.SetActive(true);
            PhotonNetwork.JoinLobby();
        }
        

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Room list update!");
            int tempIdx;
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo room = roomList[i];
                if (m_roomListings != null)
                    tempIdx = m_roomListings.FindIndex(r => r.Name == room.Name);
                else
                    tempIdx = -1;

                if (tempIdx != -1)
                {
                    m_roomListings.RemoveAt(tempIdx);
                    Destroy(m_customRoomsContainer.GetChild(tempIdx).gameObject);
                }

                if (room.PlayerCount > 0)
                {
                    m_roomListings.Add(room);
                    ListRoom(room);
                }
            }
        }

        void ListRoom(RoomInfo room)
        {
            if (room.IsOpen && room.IsVisible)
            {
                GameObject tempListing = Instantiate(m_customRoomListingPrefab, m_customRoomsContainer);
                CustomRoomButton tempButton = tempListing.GetComponent<CustomRoomButton>();
                tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
            }
        }

        public void OnRoomNameChanged(string nameIn)
        {
            Debug.Log("Room name set to: " + nameIn);
            m_customRoomName = nameIn;
        }

        public void OnRoomSizeChanged(string sizeIn)
        {
            int size = int.Parse(sizeIn);
            Debug.Log("Room size set to: " + size);
            m_customRoomSize = size;
        }
        
        public void QuickStart()
        {
            m_lobbyMode = LobbyMode.Quickstart;
            LobbyStart();
        }

        public void DelayStart()
        {
            m_lobbyMode = LobbyMode.Delaystart;
            LobbyStart();
        }

        public void Custom()
        {
            m_lobbyMode = LobbyMode.Custom;
            m_inCustomMatchmaking = true;
            m_customGameContent.SetActive(true);
            m_networkTestButtons.SetActive(false);
            
            m_customLogin.SetActive(true);
            m_customLobby.SetActive(false);
        }

        void LobbyStart()
        {
            m_networkTestButtons.SetActive(false);
            m_loadingButtons.SetActive(true);
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("Quick start pressed");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to join a room");
            AutoCreateRoom();
        }
        
        public void CustomCreateRoom()
        {
            RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = (byte) m_customRoomSize};
            PhotonNetwork.CreateRoom(m_customRoomName, roomOps);
            Debug.Log("Creating room: " + m_customRoomName);
        }

        void AutoCreateRoom()
        {
            RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = (byte) m_roomSize};
            int randomRoomNumber = Random.Range(0, 1000);
            string roomName = "Room" + randomRoomNumber;
            PhotonNetwork.CreateRoom(roomName, roomOps);
            Debug.Log("Creating room: " + roomName);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to create room...");
            if (m_lobbyMode != LobbyMode.Custom) AutoCreateRoom();
        }

        public void QuickCancel()
        {
            m_loadingButtons.SetActive(false);
            m_networkTestButtons.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }

        public void LeaveCustomMainLobby()
        {
            m_customLobby.SetActive(false);
            m_customLogin.SetActive(true);
            PhotonNetwork.LeaveLobby();
        }
        
        public void ExitCustom()
        {
            m_customGameContent.SetActive(false);
            m_networkTestButtons.SetActive(true);
            m_inCustomMatchmaking = false;
        }

        public void ReturningFromCustomRoom()
        {
            m_connectingToServerMessage.SetActive(true);
            m_customGameContent.SetActive(false);
        }
    }
}