using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameMode
{
    PONG_SP_PvP,
    PONG_SP_PvE,
    PONG_SP_EvE,
    PONG_MP_PvP,
    PONG_SP_Menu,
    
    TEST,
    
    _MainMenu
}
public class ModeManager : MonoBehaviour
{
    [SerializeField] private GameMode m_defaultMode = GameMode._MainMenu;
    
    private GameMode m_currentGameMode;

    public void StartManager()
    {
        m_currentGameMode = m_defaultMode;
    }
    
    public void SetGameMode(GameMode mode)
    {
        m_currentGameMode = mode;
    }

    public GameMode GetGameMode()
    {
        return m_currentGameMode;
    }
}
