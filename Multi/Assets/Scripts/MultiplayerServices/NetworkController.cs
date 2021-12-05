using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MultiplayerServices
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameTypeDictionary m_gameTypeDict = null;
        
        // Start is called before the first frame update
        public void StartManager()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("We have connected to " + PhotonNetwork.CloudRegion);
        }

        public GameTypeDetails GetGameTypeDetailsFromPrefixedRoomName(string roomName)
        {
            return m_gameTypeDict.GetDetailsFromPrefixedRoomName(roomName);
        }

        public string GetPrefixedRoomNameFromDetails(GameTypeDetails gtd, string roomName)
        {
            return m_gameTypeDict.GetPrefixedRoomNameFromDetails(gtd, roomName);
        }
        
        public string RemovePrefixFromRoomName(string roomName)
        {
            return m_gameTypeDict.RemovePrefixFromRoomName(roomName);
        }
    }
}
