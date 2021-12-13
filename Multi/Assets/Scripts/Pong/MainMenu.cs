using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PongMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayPvP() { PlayGame(GameMode.PONG_SP_PvP); }
        public void PlayPvE() { PlayGame(GameMode.PONG_SP_PvE); }
        public void PlayEvE() { PlayGame(GameMode.PONG_SP_EvE); }

        public void Quit()
        {
            Managers.Scene.RegularSceneChange(SceneCode.MainMenu);
        }
        void PlayGame(GameMode mode)
        {
            Managers.Mode.SetGameMode(mode);
            Managers.Scene.RegularSceneChange(SceneCode.PongGameplay);
        }
    }
}