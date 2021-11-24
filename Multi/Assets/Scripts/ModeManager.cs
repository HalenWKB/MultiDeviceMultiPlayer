using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameMode
{
    PvP,
    PvE,
    EvE
}
public class ModeManager : MonoBehaviour
{
    [SerializeField] private GameMode m_defaultMode = GameMode.PvE;
    
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
