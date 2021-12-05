using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

namespace MultiplayerServices
{
    public class RoomController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private LobbyController m_lobbyController = null;

        [SerializeField] private GameObject m_lobbyPanel = null;
        [SerializeField] private GameObject m_roomPanel = null;
        
        [SerializeField] private GameObject m_startButton = null;
        [SerializeField] private TextMeshProUGUI m_needPlayersPrompt = null;
        [SerializeField] private TextMeshProUGUI m_awaitingMasterPrompt = null;
        
        [SerializeField] private Transform m_playersContainer = null;
        [SerializeField] private GameObject m_playerListingPrefab = null;

        [SerializeField] private TextMeshProUGUI m_roomNameDisplay = null;
        
        [SerializeField] private TextMeshProUGUI m_roomTypeDisplay = null;
        [SerializeField] private TextMeshProUGUI m_roomPlayersDisplay = null;

        private GameTypeDetails m_roomGameType;
        
        void ClearPlayerListings()
        {
            for (int i = m_playersContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(m_playersContainer.GetChild(i).gameObject);
            }
        }

        void ListPlayersAndUpdateCommands()
        {
            ClearPlayerListings();
            
            m_roomPlayersDisplay.text = "Players " + PhotonNetwork.PlayerList.Length.ToString("F0") 
                                                     + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
            m_needPlayersPrompt.text = "Need players! (Min " 
                                       + m_roomGameType.minPlayerCount.ToString("F0")  + ")";
            
            bool needPlayers = PhotonNetwork.PlayerList.Length < m_roomGameType.minPlayerCount;
            m_startButton.SetActive(PhotonNetwork.IsMasterClient && !needPlayers);
            m_awaitingMasterPrompt.gameObject.SetActive(!PhotonNetwork.IsMasterClient && !needPlayers);
            m_needPlayersPrompt.gameObject.SetActive(needPlayers);
            
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                if (i == 0)
                    m_awaitingMasterPrompt.text = "Awaiting " + player.NickName + "...";
                
                GameObject tempListing = Instantiate(m_playerListingPrefab, m_playersContainer);
                TextMeshProUGUI tempText = tempListing.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                tempText.text = player.NickName;
            }
        }
        
        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public override void OnJoinedRoom()
        {
            m_roomPanel.SetActive(true);
            m_lobbyPanel.SetActive(false);
            m_roomNameDisplay.text = Managers.Network.RemovePrefixFromRoomName(PhotonNetwork.CurrentRoom.Name);
            m_roomGameType = Managers.Network.GetGameTypeDetailsFromPrefixedRoomName(PhotonNetwork.CurrentRoom.Name);
            m_roomTypeDisplay.text = "Game = " + m_roomGameType.displayName;
            
            ListPlayersAndUpdateCommands();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ListPlayersAndUpdateCommands();
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            ListPlayersAndUpdateCommands();
        }
        
        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                Managers.Scene.NetworkSceneChange(m_roomGameType.multiPlayerLoadScene);
            }
        }

        IEnumerator RejoinLobby()
        {
            yield return new WaitForSeconds(1);
            PhotonNetwork.JoinLobby();
        }

        public void BackOnClick()
        {
            m_lobbyPanel.SetActive(true);
            m_roomPanel.SetActive(false);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            m_lobbyController.LeaveRoom();
            StartCoroutine(RejoinLobby());
        }
    }
}