using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class CustomRoomButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_nameText = null;
    [SerializeField] private TextMeshProUGUI m_sizeText = null;

    private string m_roomName;
    private int m_roomSize;
    private int m_playerCount;

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(m_roomName);
    }

    public void SetRoom(string nameInput, int sizeInput, int countInput)
    {
        m_roomName = nameInput;
        m_roomSize = sizeInput;
        m_playerCount = countInput;

        m_nameText.text = Managers.Network.GetGameTypeDetailsFromPrefixedRoomName(nameInput).displayName 
                          + " - " + Managers.Network.RemovePrefixFromRoomName(nameInput);
        m_sizeText.text = countInput + "/" + sizeInput;
    }
}
