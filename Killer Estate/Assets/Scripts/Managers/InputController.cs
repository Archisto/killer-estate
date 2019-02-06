using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class InputController : MonoBehaviour
    {
        /// <summary>
        /// Screens which prevent all other input
        /// but those which close the screen.
        /// </summary>
        [SerializeField]
        private List<GameObject> _clickToExitScreens;

        /// <summary>
        ///  Initializes the object.
        /// </summary>
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameManager.Instance.State == GameManager.GameState.MainMenu)
            {
                MainMenuInput();
            }
            else
            {
                GameInput();
            }

#if UNITY_EDITOR
            DebugInput();
#endif
        }

        private void CloseClickToExitScreens()
        {
            foreach (GameObject go in _clickToExitScreens)
            {
                if (go.activeSelf)
                {
                    go.SetActive(false);
                }
            }
        }

        private void MainMenuInput()
        {
            if (Input.GetMouseButtonDown(0) ||
                Input.GetKey(KeyCode.Space) ||
                Input.GetKey(KeyCode.Return) ||
                Input.GetKey(KeyCode.Escape))
            {
                CloseClickToExitScreens();
                GameManager.Instance.UI.CloseMainMenuScreens();
            }
        }

        private void GameInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.PauseGame(!GameManager.Instance.GamePaused);
            }
        }

        private void DebugInput()
        {
            // Gain score
            if (Input.GetKeyDown(KeyCode.A))
            {
                GameManager.Instance.ChangeScore(1);
            }
            // Lose game
            if (Input.GetKeyDown(KeyCode.End))
            {
                GameManager.Instance.EndGame(false);
            }
            // Win battle
            else if (GameManager.Instance.State == GameManager.GameState.Level
                     && Input.GetKeyDown(KeyCode.Home))
            {
                GameManager.Instance.WaveStatus = GameManager.WaveState.WaveEnd;
                GameManager.Instance.EndWave();
            }
            // Pause the game in editor
            else if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Break();
            }
            // Fade out
            else if (Input.GetKeyDown(KeyCode.N))
            {
                GameManager.Instance.Fade.StartFadeOut(false);
            }
            // Fade in
            else if (Input.GetKeyDown(KeyCode.M))
            {
                GameManager.Instance.Fade.StartFadeIn(false);
            }
            // Return to the main menu
            else if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                GameManager.Instance.ReturnToMainMenu();
            }
            else
            {
                GoToLevel();
            }
        }

        private void GoToLevel()
        {
            // Level 1
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                GameManager.Instance.LoadLevel(1);
            }
            // Level 1
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                GameManager.Instance.LoadLevel(2);
            }
            // Level 3
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                GameManager.Instance.LoadLevel(3);
            }
        }
    }
}
