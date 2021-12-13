using System.Collections;
using System.Collections.Generic;
using MultiplayerServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenuItems = null;
    [SerializeField] private GameObject m_multiPlayerItems = null;
    [SerializeField] private GameObject m_singlePlayerItems = null;

    [SerializeField] private LobbyController m_lobbyController = null;
    [SerializeField] private RoomController m_roomController = null;
    
    void Start()
    {
        if (Managers.Mode.GetGameMode() == GameMode.PONG_MP_PvP)
        {
            Managers.Mode.SetGameMode(GameMode._MainMenu);
            OpenPanel(MainMenuPanels.MultiPlayer);
            m_lobbyController.OpenLobbyContent();
            if (PhotonNetwork.InRoom)
                m_roomController.OpenRoomContent();
        }
        else
            OpenPanel(MainMenuPanels.Main);
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    public void PlaySinglePlayerGame(GameTypeDetails gameType)
    {
        Managers.Mode.SetGameMode(gameType.singlePlayerGameMode);
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
