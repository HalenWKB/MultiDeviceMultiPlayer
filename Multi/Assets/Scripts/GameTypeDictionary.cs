using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct GameTypeDetails
{
    //Main stuff
    public string displayName;// = "";
    public string prefixCode;// = "";
    public bool active;// = false;
    
    //Single player stuff
    public SceneCode singlePlayerLoadScene;// = SceneCode.MainMenu;
    
    //Multiplayer stuff
    public SceneCode multiPlayerLoadScene;// = SceneCode.MainMenu;
    public int minPlayerCount;// = 2;
    public int maxPlayerCount;// = 2;
}

[CreateAssetMenu(fileName = "New GameTypeDictionary Dictionary", menuName = "Custom Dictionaries/GameTypeDictionary Dictionary")]
public class GameTypeDictionary : ScriptableObject
{
    public List<GameTypeDetails> Games = null;

    public GameTypeDetails GetDetailsFromPrefixedRoomName(string roomName)
    {
        string prefixCode = roomName.Substring(0, 2);
        for (int i = 0; i < Games.Count; i++)
        {
            if (Games[i].prefixCode == prefixCode) return Games[i];
        }
        Debug.LogError("Prefixed room name invalid!");
        return new GameTypeDetails();
    }
    
    public string GetPrefixedRoomNameFromDetails(GameTypeDetails gtd, string roomName)
    {
        return gtd.prefixCode + "_" + roomName;
    }

    public string RemovePrefixFromRoomName(string roomName)
    {
        return roomName.Substring(3);
    }
}
