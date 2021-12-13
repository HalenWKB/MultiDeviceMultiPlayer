using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Realtime;
using TMPro;
using Random = UnityEngine.Random;

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
        [SerializeField] private GameObject m_networkOptionsContent = null;
        
        [SerializeField] private GameObject m_multiplayerLobby = null;
        [SerializeField] private GameObject m_multiplayerLogin = null;
        
        [SerializeField] private TMP_InputField m_customPlayerName = null;
        
        [SerializeField] private Transform m_customRoomsContainer = null;
        [SerializeField] private GameObject m_customRoomListingPrefab = null;
        
        [SerializeField] private MainMainMenu m_mainMainMenu = null;

        [SerializeField] private TextMeshProUGUI m_roomSizePrompt = null;
        
        [SerializeField] private GameObject m_roomSizeInput = null;
        [SerializeField] private GameObject m_roomSizeLocked = null;

        private string m_roomName;
        private GameTypeDetails m_roomType;
        private int m_roomSize;
        private int m_roomSizeLockedOverrride;
        private GameObject m_lastRoomTypeHighlight;
        
        private List<RoomInfo> m_roomListings;

        private bool m_returningFromRoom;

        void Start()
        {
            m_connectingToServerMessage.SetActive(!PhotonNetwork.IsConnected);
            m_networkOptionsContent.SetActive(PhotonNetwork.IsConnected);
            m_multiplayerLogin.SetActive(true);
            m_multiplayerLobby.SetActive(false);
            m_roomListings = new List<RoomInfo>();
        }

        public void Back()
        {
            //QuickCancel();
            m_mainMainMenu.BackToMain();
        }

        public override void OnConnectedToMaster()
        {
            m_connectingToServerMessage.SetActive(false);
            m_networkOptionsContent.SetActive(true);

            if (m_returningFromRoom)
            {
                m_returningFromRoom = false;
                EnterMainLobby();
            }
            else
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                string nickName = "";
                if (PlayerPrefs.HasKey("NickName"))
                    nickName = PlayerPrefs.GetString("NickName");
            
                if (nickName == "")
                    PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
                else
                    PhotonNetwork.NickName = nickName;

                m_customPlayerName.text = PhotonNetwork.NickName;
                
                
            }
        }

        public void PlayerNameUpdate(string nameInput)
        {
            PhotonNetwork.NickName = nameInput;
            PlayerPrefs.SetString("NickName",nameInput);
        }

        public void EnterMainLobby()
        {
            OpenLobbyContent();
            PhotonNetwork.JoinLobby();
        }

        public void OpenLobbyContent()
        {
            m_multiplayerLogin.SetActive(false);
            m_multiplayerLobby.SetActive(true);
        }
        

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            //Debug.Log("Room list update!");
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
            m_roomName = nameIn;
        }

        public void OnRoomSizeChanged(string sizeIn)
        {
            int size = int.Parse(sizeIn);
            m_roomSize = size;
        }

        public void OnRoomTypeSelected(GameTypeDetails gameType, GameObject childHighlight)
        {
            //Debug.Log("RoomType");
            m_roomType = gameType;
            if (m_lastRoomTypeHighlight != null)
                m_lastRoomTypeHighlight.SetActive(false);
            m_lastRoomTypeHighlight = childHighlight;
            m_lastRoomTypeHighlight.SetActive(true);
            if (gameType.minPlayerCount != gameType.maxPlayerCount)
            {
                m_roomSizePrompt.text = "Room Size (" + gameType.minPlayerCount.ToString("F0") 
                                                      + "-" + gameType.maxPlayerCount.ToString("F0")  + "):";
                m_roomSizeLocked.SetActive(false);
                m_roomSizeInput.SetActive(true);
                
                m_roomSizeLockedOverrride = -1;
            }
            else
            {
                m_roomSizePrompt.text = "Room Size:";
                m_roomSizeLocked.SetActive(true);
                m_roomSizeInput.SetActive(false);

                m_roomSizeLockedOverrride = gameType.minPlayerCount;
            }
        }

        public void OpenMultiplayerLogin()
        {
            m_multiplayerLogin.SetActive(true);
            m_multiplayerLobby.SetActive(false);
        }

        public void CreateRoom()
        {
            int roomSize = m_roomSizeLockedOverrride >= 0 ? m_roomSizeLockedOverrride : m_roomSize;
            string roomName = Managers.Network.GetPrefixedRoomNameFromDetails(m_roomType, m_roomName);
            RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = (byte) roomSize};
            PhotonNetwork.CreateRoom(roomName, roomOps);
            Debug.Log("Creating room: " + roomName);
        }

        public void LeaveRoomListLobby()
        {
            m_multiplayerLobby.SetActive(false);
            m_multiplayerLogin.SetActive(true);
            PhotonNetwork.LeaveLobby();
        }

        public void LeaveRoom()
        {
            m_returningFromRoom = true;
            
            m_connectingToServerMessage.SetActive(true);
            m_networkOptionsContent.SetActive(false);
        }
    }
}