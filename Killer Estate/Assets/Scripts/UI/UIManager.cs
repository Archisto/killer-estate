using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KillerEstate.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private MainMenu _mainMenu;

        [SerializeField]
        private PauseMenu _pauseMenu;

        [SerializeField]
        private HeadsUpDisplay _hud;

        private Canvas _canvas;
        private Vector2 _canvasSize;
        private Vector2 _uiOffset;
        private Camera _camera;
        private InputController _input;
        private Vector3[] _targetPositions;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            UpdateCanvasSize();
            _camera = FindObjectOfType<CameraController>().GetComponent<Camera>();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            // TODO
        }

        public void OnSceneChanged(GameManager.GameState gameState)
        {
            switch (gameState)
            {
                case GameManager.GameState.MainMenu:
                {
                    _mainMenu.Activate(true);
                    ActivatePauseMenu(false);
                    ActivateHUD(false);
                    break;
                }
                case GameManager.GameState.Level:
                {
                    _mainMenu.Activate(false);
                    ActivatePauseMenu(false);
                    ActivateHUD(true);
                    break;
                }
            }

            UpdateMoney(GameManager.Instance.Money);
        }

        public void UpdateMoney(int score)
        {
            _hud.UpdateScore(score);
        }

        public void ActivateHUD(bool activate)
        {
            _hud.Activate(activate);
        }

        public void ActivatePauseMenu(bool pause)
        {
            _pauseMenu.Activate(pause);
        }

        public void EndGame(bool win)
        {
            // TODO: When the player exits the game from a menu, this is called.
        }

        public void CloseScreens()
        {
            _mainMenu.Activate(false);
            _pauseMenu.Activate(false);
            _hud.Activate(false);
        }

        public void CloseMainMenuScreens()
        {
            _mainMenu.CloseExtraScreens();
        }

        public void UpdateCanvasSize()
        {
            _canvasSize = _canvas.pixelRect.size;
            _uiOffset = new Vector2(-0.5f * _canvasSize.x, -0.5f * _canvasSize.y);
        }

        public void MoveUIObjToWorldPoint(Image uiObj,
                                          Vector3 worldPoint,
                                          Vector2 screenSpaceOffset)
        {
            Vector2 viewPortPos = _camera.WorldToViewportPoint(worldPoint);
            Vector2 proportionalPosition = new Vector2
                (viewPortPos.x * _canvasSize.x, viewPortPos.y * _canvasSize.y);
            uiObj.transform.localPosition =
                proportionalPosition + _uiOffset + screenSpaceOffset;
        }

        public void ResetUI()
        {
            _hud.UpdateScore(GameManager.Instance.Money);
        }
    }
}
