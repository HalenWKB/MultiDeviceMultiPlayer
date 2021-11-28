using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenuItems = null;
    [SerializeField] private GameObject m_networkTestItems = null;
    
    public void Quit()
    {
        Application.Quit();
    }
    
    public void Pong()
    {
        Managers.Scene.RegularSceneChange(SceneCode.PongMenu);
    }
    
    public void NetworkTest()
    {
        m_mainMenuItems.SetActive(false);
        m_networkTestItems.SetActive(true);
    }
    
    public void BackToMain()
    {
        m_mainMenuItems.SetActive(true);
        m_networkTestItems.SetActive(false);
    }
}
