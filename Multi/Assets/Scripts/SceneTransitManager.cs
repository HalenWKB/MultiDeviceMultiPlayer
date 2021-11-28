using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneCode
{
    MainMenu,
    PongMenu,
    PongGameplay,
    NetworkTest
}

public class SceneTransitManager : MonoBehaviour
{

    int GetBuildIndexOfSceneCode(SceneCode code)
    {
        switch (code)
        {
            case SceneCode.MainMenu: return 0;
            case SceneCode.PongMenu: return 1;
            case SceneCode.PongGameplay: return 2;
            case SceneCode.NetworkTest: return 3;
        }
        Debug.LogError("Scene code does not map to build idx");
        return -1;
    }

    public void RegularSceneChange(SceneCode scene)
    {
        SceneManager.LoadScene(GetBuildIndexOfSceneCode(scene));
    }
    
    public void NetworkSceneChange(SceneCode scene)
    {
        PhotonNetwork.LoadLevel(GetBuildIndexOfSceneCode(scene));
    }
    
    public void StartManager()
    {
        //ATM nothing to do here...
    }
}
