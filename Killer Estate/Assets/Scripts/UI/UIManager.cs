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

        [SerializeField]
        private Transform _vitalWeaponInfoParent;

        [SerializeField]
        private VitalWeaponInfo _vitalWeaponInfoPrefab;

        private Canvas _canvas;
        private Camera _camera;
        private HardwareManager _hwManager;
        private Vector2 _canvasSize;
        private Vector2 _uiOffset;
        private InputController _input;
        private Vector3[] _targetPositions;
        private List<VitalWeaponInfo> _vitalWeaponInfos;
        private int _activeVitalWeaponInfoCount;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            UpdateCanvasSize();
            _camera = FindObjectOfType<CameraController>().GetComponent<Camera>();

            if (GameManager.Instance.State == GameManager.GameState.Level)
            {
                _hwManager = FindObjectOfType<HardwareManager>();
                GenerateVitalWeaponInfos();
            }
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

            UpdateScore(GameManager.Instance.Money);
        }

        private void GenerateVitalWeaponInfos()
        {
            _vitalWeaponInfos = new List<VitalWeaponInfo>();
            for (int i = 0; i < _hwManager.Rooms.Length; i++)
            {
                VitalWeaponInfo info = Instantiate
                    (_vitalWeaponInfoPrefab, _vitalWeaponInfoParent);
                _vitalWeaponInfos.Add(info);
                info.gameObject.SetActive(false);
            }
        }

        public VitalWeaponInfo ActivateWeaponInfo(WeaponMouse weapon)
        {
            VitalWeaponInfo activatedInfo = null;
            foreach (VitalWeaponInfo info in _vitalWeaponInfos)
            {
                if (!info.gameObject.activeSelf)
                {
                    info.SetWeapon(weapon);
                    info.transform.SetSiblingIndex(0);
                    info.gameObject.SetActive(true);
                    activatedInfo = info;
                    _activeVitalWeaponInfoCount++;
                    break;
                }
            }

            if (activatedInfo == null)
            {
                Debug.LogError("No more available VitalWeaponInfos.");
            }

            return activatedInfo;
        }

        public void DeactivateWeaponInfo(WeaponMouse weapon)
        {
            bool infoDeactivated = false;
            foreach (VitalWeaponInfo info in _vitalWeaponInfos)
            {
                if (info.gameObject.activeSelf && info.Weapon == weapon)
                {
                    info.SetWeapon(null);
                    info.gameObject.SetActive(false);
                    infoDeactivated = true;
                    _activeVitalWeaponInfoCount--;
                    break;
                }
            }

            if (!infoDeactivated)
            {
                Debug.LogError
                    ("No VitalWeaponInfo belonging to that weapon is active.");
            }
        }

        public void UpdateScore(int score)
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
                                          Vector3 worldPosition,
                                          Vector2 screenSpaceOffset)
        {
            uiObj.transform.localPosition =
                GetScreenSpacePosition(worldPosition, screenSpaceOffset);
        }

        public Vector2 GetScreenSpacePosition(Vector3 worldPosition,
                                              Vector2 screenSpaceOffset)
        {
            Vector2 viewPortPos = _camera.WorldToViewportPoint(worldPosition);
            Vector2 proportionalPosition = new Vector2
                (viewPortPos.x * _canvasSize.x, viewPortPos.y * _canvasSize.y);
            return proportionalPosition + _uiOffset + screenSpaceOffset;
        }

        public void ResetUI()
        {
            _hud.UpdateScore(GameManager.Instance.Money);
        }
    }
}
