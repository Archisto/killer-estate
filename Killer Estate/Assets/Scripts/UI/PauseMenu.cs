using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KillerEstate.UI
{
    public class PauseMenu : Screen
    {
        public void ResumeGame()
        {
            GameManager.Instance.PauseGame(false);
        }

        public void RestartGame()
        {
            GameManager.Instance.LoadNewGame();
        }

        public void ReturnToMainMenu()
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}
