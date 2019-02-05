using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using KillerEstate.UI;
using KillerEstate.Persistence;
using KillerEstate.Localization;

using L10n = KillerEstate.Localization.Localization;

namespace KillerEstate
{
    public class GameManager : MonoBehaviour
    {
        // Sets the object up as a Singleton
        #region Statics
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        #endregion Statics

        public enum GameState
        {
            MainMenu = 0,
            Map = 1,
            Battle = 2
        }

        public enum WaveState
        {
            NoBattle = 0,
            Active = 1,
            WaveEnd = 2,
            Lost = 3
        }

        public enum SceneTransition
        {
            InScene = 0,
            ExitingScene = 1,
            EnteringScene = 2
        }

        [SerializeField]
        private LangCode _defaultLanguage = LangCode.EN;

        [SerializeField]
        private float gameSpeedModifier = 1f;

        private SaveSystem _saveSystem;
        private SceneTransition _sceneTransition;
        private string _sceneToLoad;
        private bool _gameRunning;
        private bool _gameJustStarted = true;
        private bool _sceneJustStarted;
        private int _waitFramesBeforeSettingUpScene = 3;
        private int _framesWaited = 0;

        private int _score;
        private int _waveNum;
        private LevelObject[] _levelObjects;

        #region Properties

        public GameState State { get; private set; }

        public WaveState BattleStatus { get; set; }

        public SceneTransition Transition
        {
            get
            {
                return _sceneTransition;
            }
            private set
            {
                _sceneTransition = value;
                //Debug.Log("Scene transition: " + value);
            }
        }

        public UIManager UI { get; private set; }

        public FadeToColor Fade { get; private set; }

        public bool GamePaused { get; private set; }

        public List<int> KeyCodes { get; private set; }

        public bool PlayReady
        {
            get
            {
                return !GamePaused && Transition == SceneTransition.InScene;
            }
        }

        public bool FadeActive
        {
            get
            {
                return Fade.Active;
            }
        }

        public float DeltaTime
        {
            get
            {
                if (GamePaused)
                {
                    return 0f;
                }
                else
                {
                    return Time.deltaTime;
                }
            }
        }

        public float DeltaTimeVariable
        {
            get
            {
                if (GamePaused)
                {
                    return 0f;
                }
                else
                {
                    return Time.deltaTime * gameSpeedModifier;
                }
            }
        }

        public string SavePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath,
                    "saveData");
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            Init();
        }

        private void Init()
        {
            State = GameState.MainMenu;
            Transition = SceneTransition.InScene;

            // Initializes the save system and loads data
            _saveSystem = new SaveSystem(new JSONPersistence(SavePath));
            LoadGame();

            InitLocalization();
            InitScene();
            InitKeyCodes();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Initializes localization.
        /// </summary>
        private void InitLocalization()
        {
            LangCode currentLang = _defaultLanguage;
            //(LangCode) PlayerPrefs.GetInt(LanguageKey, (int) _defaultLanguage);
            L10n.LoadLanguage(currentLang);
            L10n.LanguageLoaded += OnLanguageLoaded;
        }

        /// <summary>
        /// Called when a LanguageLoaded event is fired.
        /// </summary>
        private void OnLanguageLoaded()
        {
            // TODO: Update the UI if the player changed the language.
        }

        private void InitScene()
        {
            UI = FindObjectOfType<UIManager>();
            UI.OnSceneChanged(State);
            Fade = FindObjectOfType<FadeToColor>();
        }

        private void InitKeyCodes()
        {
            KeyCodes = new List<int>();
        }

        #endregion Initialization

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (Transition != SceneTransition.InScene)
            {
                UpdateSceneTransition();

                if (Transition == SceneTransition.EnteringScene
                    && _sceneJustStarted)
                {
                    if (State == GameState.Battle)
                    {
                        if (_framesWaited < _waitFramesBeforeSettingUpScene)
                        {
                            _framesWaited++;
                        }
                        else
                        {
                            _sceneJustStarted = false;
                        }
                    }
                    else
                    {
                        _sceneJustStarted = false;
                    }
                }
            }
        }

        #region SceneManagement

        private void StartLoadingScene()
        {
            Transition = SceneTransition.ExitingScene;
            Fade.StartFadeOut(false);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadScene(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                {
                    _sceneToLoad = "MainMenu";
                    break;
                }
                case GameState.Map:
                {
                    _sceneToLoad = "MapScene";
                    break;
                }
                case GameState.Battle:
                {
                    _sceneToLoad = "BattleScene";
                    break;
                }
            }

            if (_sceneToLoad != null && _sceneToLoad.Length > 0)
            {
                StartLoadingScene();
            }
        }

        public void LoadNewGame()
        {
            _gameRunning = false;
            State = GameState.Map;
            LoadScene(GameState.Map);
        }

        public void LoadBattleScene()
        {
            State = GameState.Battle;
            LoadScene(GameState.Battle);
        }

        public void LoadMapScene()
        {
            if (Transition == SceneTransition.InScene)
            {
                State = GameState.Map;
                LoadScene(GameState.Map);
            }
        }

        private void UpdateSceneTransition()
        {
            if (Transition == SceneTransition.ExitingScene && Fade.FadedOut)
            {
                LoadScene(_sceneToLoad);
                _sceneToLoad = "";
            }
            else if (Transition == SceneTransition.EnteringScene && Fade.FadedIn)
            {
                Transition = SceneTransition.InScene;
                PauseGame(false);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_gameJustStarted)
            {
                _gameJustStarted = false;
                return;
            }

            Transition = SceneTransition.EnteringScene;
            InitScene();
            Fade.StartFadeIn(false);
            _sceneJustStarted = true;
            _framesWaited = 0;

            _levelObjects = FindObjectsOfType<LevelObject>();
            SFXPlayer.Instance.InitAudioSrcPool();

            switch (State)
            {
                case GameState.MainMenu:
                {
                    //ui.mainMenu.Activate(true);
                    //MusicPlayer.Instance.Play(0, true);
                    break;
                }
                case GameState.Map:
                {
                    InitMapScreen();
                    break;
                }
                case GameState.Battle:
                {
                    StartBattle();
                    break;
                }
            }
        }

        #endregion SceneManagement

        #region Gameplay

        private void InitMapScreen()
        {
            if (!_gameRunning)
            {
                StartNewGame();
            }

            MusicPlayer.Instance.Play(0, true);
        }

        private void StartNewGame()
        {
            Debug.Log("New game started");
            _score = 0;
            UI.UpdateScore(_score);
            _gameRunning = true;
        }

        public void EndGame(bool win)
        {
            _gameRunning = false;
            UI.EndGame(win);
        }

        private void StartBattle()
        {
            Debug.Log("hear!");
            BattleStatus = WaveState.Active;
            MusicPlayer.Instance.Play(1, true);
        }

        public AudioSource EndWave()
        {
            if (BattleStatus == WaveState.Lost)
            {
                Debug.Log("Game Over!");
                return SFXPlayer.Instance.Play(Sound.Asdfghj, volumeFactor: 0.4f);
            }
            else if (BattleStatus == WaveState.WaveEnd)
            {
                WinWave();
                return SFXPlayer.Instance.Play(Sound.GetRekt2, volumeFactor: 0.4f);
            }
            else
            {
                BattleStatus = WaveState.NoBattle;
                Debug.LogError("Wave ended without winner");
                return null;
            }
        }

        public void WinWave()
        {
            Debug.Log("Wave " + _waveNum + " survived");
            NextWave();
        }

        public void NextWave()
        {
            _waveNum++;
            ChangeScore(100);
            Debug.Log("Now starting wave " + _waveNum);
        }

        public int GetScore()
        {
            return _score;
        }

        public void ChangeScore(int scoreChange, bool updateUIImmediately = true)
        {
            _score += scoreChange;
            if (_score < 0)
            {
                _score = 0;
            }

            if (updateUIImmediately)
            {
                UI.UpdateScore(_score);
            }
        }

        public void PauseGame(bool pause)
        {
            if (GamePaused != pause)
            {
                GamePaused = pause;
                UI.ActivatePauseMenu(GamePaused);
            }
        }

        public void ReturnToMainMenu()
        {
            State = GameState.MainMenu;
            MusicPlayer.Instance.Stop();
            LoadScene(GameState.MainMenu);
        }

        public void AddKeyCode(int keyCode, bool addIfNew)
        {
            if (addIfNew)
            {
                KeyCodes.AddIfNew(keyCode);
            }
            else
            {
                KeyCodes.Add(keyCode);
            }
        }

        public void RemoveKeyCode(int keyCode, bool removeAll)
        {
            if (KeyCodes.Contains(keyCode))
            {
                if (!removeAll)
                {
                    KeyCodes.Remove(keyCode);
                }
                else
                {
                    int startCount = KeyCodes.Count;
                    for (int i = startCount - 1; i >= 0; i--)
                    {
                        if (KeyCodes[i] == keyCode)
                        {
                            KeyCodes.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Can't remove key code '" + keyCode +
                                 "' because it doesn't exist.");
            }
        }

        public bool CheckKeyCode(int keyCode)
        {
            return KeyCodes.Contains(keyCode);
        }

        public void ResetLevel()
        {
            UI.ResetUI();

            foreach (LevelObject obj in _levelObjects)
            {
                obj.ResetObject();
            }
        }

        #endregion Gameplay

        #region Persistence

        private void LoadGame()
        {
            // TODO
        }

        #endregion Persistence

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            L10n.LanguageLoaded -= OnLanguageLoaded;
        }
    }
}
