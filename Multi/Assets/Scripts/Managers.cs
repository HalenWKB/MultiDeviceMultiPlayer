using System.Collections;
using System.Collections.Generic;
using PongMainGameplay;
using UnityEngine;

[RequireComponent(typeof(ModeManager))]
public class Managers : MonoBehaviour
{
    static public Managers Instance;
    static public ModeManager Mode;
    static public NetworkController Network;
    
    static public GameplayManager Gameplay;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            
            Mode = GetComponent<ModeManager>();
            Mode.StartManager();
            
            Network = GetComponent<NetworkController>();
            Network.StartManager();
            
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    public void GameplaySignIn(GameplayManager manager)
    {
        Gameplay = manager;
    }
}
