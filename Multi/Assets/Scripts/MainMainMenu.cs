using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenuItems = null;
    [SerializeField] private GameObject m_multiPlayerItems = null;
    [SerializeField] private GameObject m_singlePlayerItems = null;

    void Start()
    {
        OpenPanel(MainMenuPanels.Main);
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    public void PlaySinglePlayerGame(GameTypeDetails gameType)
    {
        Managers.Scene.RegularSceneChange(gameType.singlePlayerLoadScene);
    }
    
    public void Pong()
    {
        Managers.Scene.RegularSceneChange(SceneCode.PongMenu);
    }

    enum MainMenuPanels
    {
        Main,
        SinglePlayer,
        MultiPlayer
    }

    void OpenPanel(MainMenuPanels panel)
    {
        m_singlePlayerItems.SetActive(panel == MainMenuPanels.SinglePlayer);
        m_mainMenuItems.SetActive(panel == MainMenuPanels.Main);
        m_multiPlayerItems.SetActive(panel == MainMenuPanels.MultiPlayer);
    }
    
    public void SinglePlayer() { OpenPanel(MainMenuPanels.SinglePlayer); }
    public void MultiPlayer() { OpenPanel(MainMenuPanels.MultiPlayer); }
    
    public void BackToMain() { OpenPanel(MainMenuPanels.Main); }
}
