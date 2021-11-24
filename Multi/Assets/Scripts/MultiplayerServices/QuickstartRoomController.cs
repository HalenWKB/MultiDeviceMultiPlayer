using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuickstartRoomController : MonoBehaviourPunCallbacks
{
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
        StartGame();
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master starting game");
            PhotonNetwork.LoadLevel(3);
        }
    }
}
