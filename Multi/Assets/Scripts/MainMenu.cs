using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayPvP() { PlayGame(GameMode.PvP); }
        public void PlayPvE() { PlayGame(GameMode.PvE); }
        public void PlayEvE() { PlayGame(GameMode.EvE); }

        public void Quit()
        {
            Application.Quit();
        }
        void PlayGame(GameMode mode)
        {
            Managers.Mode.SetGameMode(mode);
            SceneManager.LoadScene(1);
        }
    }
}