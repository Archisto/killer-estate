using System;
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

        private const string MainMenuKey = "MainMenu";
        private const string LevelKey = "Level";

        public enum GameState
        {
            MainMenu = 0,
            Level = 1
        }

        public enum WaveState
        {
            Intro = 0,
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

        private int _waveNum;
        private Level _currentLevel;
        private List<Level> _levels;
        private LevelObject[] _levelObjects;

        #region Properties

        public GameState State { get; private set; }

        public WaveState WaveStatus { get; set; }

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

        public MouseController Mouse { get; private set; }

        public List<int> KeyCodes { get; private set; }

        public Room CurrentRoom { get; set; }

        public int Score { get; private set; }

        public int Money { get; private set; }

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
                return;
            }

            DontDestroyOnLoad(gameObject);
            Init();
        }

        private void Init()
        {
            if (SceneManager.GetActiveScene().name.Equals(MainMenuKey))
            {
                State = GameState.MainMenu;
            }
            else
            {
                State = GameState.Level;
            }

            Transition = SceneTransition.InScene;

            // Initializes the save system and loads data
            _saveSystem = new SaveSystem(new JSONPersistence(SavePath));
            LoadGame();

            InitLocalization();
            InitScene();
            InitLevels();
            KeyCodes = new List<int>();

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
            // TODO: Update the UI if the player changes the language.
        }

        private void InitScene()
        {
            UI = FindObjectOfType<UIManager>();
            UI.OnSceneChanged(State);
            Fade = FindObjectOfType<FadeToColor>();
            Mouse = FindObjectOfType<MouseController>();
        }

        private void InitLevels()
        {
            _levels = new List<Level>();
            _levels.Add(new Level(1, "Main Level"));
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
                    _sceneJustStarted = false;

                    //if (State == GameState.Battle)
                    //{
                    //    if (_framesWaited < _waitFramesBeforeSettingUpScene)
                    //    {
                    //        _framesWaited++;
                    //    }
                    //    else
                    //    {
                    //        _sceneJustStarted = false;
                    //    }
                    //}
                    //else
                    //{
                    //    _sceneJustStarted = false;
                    //}
                }
            }
        }

        private Level GetLevel(int level)
        {
            if (level >= 1 && level <= _levels.Count)
            {
                return _levels[level - 1];
            }
            else
            {
                Debug.LogError("Invalid level number: " + level);
                return _levels[0];
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
                    _sceneToLoad = MainMenuKey;
                    break;
                }
                case GameState.Level:
                {
                    _sceneToLoad = LevelKey + _currentLevel.Number;
                    break;
                }
            }

            if (_sceneToLoad != null && _sceneToLoad.Length > 0)
            {
                StartLoadingScene();
            }
            else
            {
                Debug.LogError("Invalid scene name.");
            }
        }

        public void LoadNewGame()
        {
            _gameRunning = false;
            State = GameState.Level;
            LoadScene(GameState.Level);
        }

        public void LoadLevel(int levelNumber)
        {
            if (Transition == SceneTransition.InScene)
            {
                _currentLevel = GetLevel(levelNumber);
                Debug.Log("Loading level " + _currentLevel.Number
                          + ": " + _currentLevel.Name);

                State = GameState.Level;
                LoadScene(GameState.Level);
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

                // Initializes a level if the game
                // was started straight from one
                if (State == GameState.Level)
                {
                    InitLevel();
                }

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
                case GameState.Level:
                {
                    InitLevel();
                    break;
                }
                default:
                {
                    Debug.LogError("GameState: " + State);
                    break;
                }
            }
        }

        #endregion SceneManagement

        #region Gameplay

        private void InitLevel()
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
            Score = 0;
            Money = 0;
            _waveNum = 0;
            UI.UpdateScore(Money);
            _gameRunning = true;
        }

        public void EndGame(bool win)
        {
            Debug.Log(win ? "You win!" : "You lose!");
            _gameRunning = false;
            UI.EndGame(win);
        }

        public AudioSource EndWave()
        {
            if (WaveStatus == WaveState.Lost)
            {
                Debug.Log("Game Over!");
                return SFXPlayer.Instance.Play(Sound.Asdfghj, volumeFactor: 0.4f);
            }
            else if (WaveStatus == WaveState.WaveEnd)
            {
                WinWave();
                return SFXPlayer.Instance.Play(Sound.GetRekt2, volumeFactor: 0.4f);
            }
            else
            {
                Debug.LogError("No wave was active");
                return null;
            }
        }

        public void WinWave()
        {
            Debug.Log("Wave " + _waveNum + " survived");
            ChangeScore(100);
            NextWave();
        }

        public void NextWave()
        {
            _waveNum++;
            Debug.Log("Now starting wave " + _waveNum);
        }

        public event Action<Room> RoomOpened;

        public void OpenRoom(Room room, bool open)
        {
            if (open)
            {
                RoomOpened?.Invoke(room);
            }
        }

        /// <summary>
        /// Changes the score and money. If the <paramref name="change"/>
        /// is negative, only money is affected.
        /// </summary>
        /// <param name="change">The change in score and money</param>
        /// <param name="updateUIImmediately">Does the UI immediately
        /// reflect the change</param>
        /// <returns>Is there enough money to withdraw</returns>
        public bool ChangeScore(int change, bool updateUIImmediately = true)
        {
            if (change > 0)
            {
                Score += change;
            }

            return ChangeMoney(change, updateUIImmediately);
        }

        private bool ChangeMoney(int change, bool updateUIImmediately)
        {
            if (change < 0 &&
                Money + change < 0)
            {
                return false;
            }
            else
            {
                Money += change;

                if (updateUIImmediately)
                {
                    UI.UpdateScore(Money);
                }

                return true;
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
            if (_gameRunning)
            {
                Debug.Log("Progress lost");
                _gameRunning = false;
            }

            State = GameState.MainMenu;
            MusicPlayer.Instance.Stop();
            Debug.Log("Returning to the main menu");
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

        private void SaveGame()
        {
            // TODO
        }

        private void LoadGame()
        {
            // TODO
        }

        #endregion Persistence

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            L10n.LanguageLoaded -= OnLanguageLoaded;
            RoomOpened = null;
        }
    }
}
