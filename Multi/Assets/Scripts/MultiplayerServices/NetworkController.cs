using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MultiplayerServices
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        public void StartManager()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("We have connected to " + PhotonNetwork.CloudRegion);
        }
    }
}
