using System.Collections;
using System.Collections.Generic;
using MainGameplay;
using UnityEngine;

[RequireComponent(typeof(ModeManager))]
public class Managers : MonoBehaviour
{
    static public Managers Instance;
    static public GameplayManager Gameplay;
    static public ModeManager Mode;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            
            Mode = GetComponent<ModeManager>();
            Mode.StartManager();
            
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    public void GameplaySignIn(GameplayManager manager)
    {
        Gameplay = manager;
    }
}
