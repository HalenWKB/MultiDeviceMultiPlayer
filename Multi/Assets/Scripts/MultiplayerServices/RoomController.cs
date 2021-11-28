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

        [SerializeField] private GameObject m_customLobbyPanel = null;
        [SerializeField] private GameObject m_customRoomPanel = null;
        
        [SerializeField] private GameObject m_customStartButton = null;
        
        [SerializeField] private Transform m_customPlayersContainer = null;
        [SerializeField] private GameObject m_customPlayerListingPrefab = null;

        [SerializeField] private TextMeshProUGUI m_customRoomNameDisplay = null;

        void ClearPlayerListings()
        {
            for (int i = m_customPlayersContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(m_customPlayersContainer.GetChild(i).gameObject);
            }
        }

        void ListPlayers()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                GameObject tempListing = Instantiate(m_customPlayerListingPrefab, m_customPlayersContainer);
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
            Debug.Log("Joined room");
            switch (m_lobbyController.GetLobbyMode())
            {
                case LobbyMode.Quickstart:
                    QuickStartGame();
                    return;
                case LobbyMode.Delaystart:
                    DelayStartGame();
                    return;
                case LobbyMode.Custom:
                    CustomJoinedRoom();
                    return;
            }
        }

        void CustomJoinedRoom()
        {
            m_customRoomPanel.SetActive(true);
            m_customLobbyPanel.SetActive(false);
            m_customRoomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
            if (PhotonNetwork.IsMasterClient)
            {
                m_customStartButton.SetActive(true);
            }
            else
            {
                m_customStartButton.SetActive(false);
            }
            
            ClearPlayerListings();
            ListPlayers();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ClearPlayerListings();
            ListPlayers();
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            ClearPlayerListings();
            ListPlayers();
            if (PhotonNetwork.IsMasterClient)
            {
                m_customStartButton.SetActive(true);
            }
        }
        
        void QuickStartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Master starting game");
                Managers.Scene.NetworkSceneChange(SceneCode.NetworkTest);
            }
        }
        
        void DelayStartGame()
        {
            PhotonNetwork.LoadLevel(4);
        }
        
        public void CustomStartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                Managers.Scene.NetworkSceneChange(SceneCode.NetworkTest);
            }
        }

        IEnumerator RejoinLobby()
        {
            yield return new WaitForSeconds(1);
            PhotonNetwork.JoinLobby();
        }

        public void BackOnClick()
        {
            m_customLobbyPanel.SetActive(true);
            m_customRoomPanel.SetActive(false);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            m_lobbyController.ReturningFromCustomRoom();
            StartCoroutine(RejoinLobby());
        }
    }
}