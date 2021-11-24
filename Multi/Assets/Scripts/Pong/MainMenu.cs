using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PongMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayPvP() { PlayGame(GameMode.PvP); }
        public void PlayPvE() { PlayGame(GameMode.PvE); }
        public void PlayEvE() { PlayGame(GameMode.EvE); }

        public void Quit()
        {
            SceneManager.LoadScene(0);
        }
        void PlayGame(GameMode mode)
        {
            Managers.Mode.SetGameMode(mode);
            SceneManager.LoadScene(2);
        }
    }
}