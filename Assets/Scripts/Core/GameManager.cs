using System.Collections;
using ChickenOmelette.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChickenOmelette.Core
{
    public class GameManager : MonoBehaviour
    {
        private const string ConfigResourceName = "GameConfig";

        public static GameManager Instance { get; private set; }

        [Tooltip("Mirrors the current values while playing.")]
        [SerializeField] private RunState live = new RunState();
        [Tooltip("Snapshot taken when a level completes. The next level starts from it.")]
        [SerializeField] private RunState checkpoint = new RunState();
        [SerializeField] private LevelDataSO currentLevel;

        private GameConfigSO config;
        private ScreenFader fader;
        private bool transitioning;
        private bool flowBusy;

        public GameConfigSO Config => config;
        public LevelDataSO CurrentLevel => currentLevel;
        public RunState Live => live;
        public RunState Checkpoint => checkpoint;

        private float LevelEndDelay => config != null ? config.LevelEndDelay : 3.5f;
        private float GameOverDelay => config != null ? config.GameOverDelay : 2.5f;
        private float FadeSeconds => config != null ? config.FadeSeconds : 0.4f;
        private string MainMenuScene => config != null ? config.MainMenuScene : "MainMenu";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (Instance == null)
            {
                new GameObject("GameManager").AddComponent<GameManager>();
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                gameObject.SetActive(false);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            config = Resources.Load<GameConfigSO>(ConfigResourceName);
            if (config == null)
            {
                Debug.LogError("GameManager: create a GameConfig asset in a Resources folder (Assets/Resources/GameConfig).", this);
            }

            fader = ScreenFader.Create(transform);
            StartNewGame();
        }

        private void OnEnable()
        {
            GameEvents.ScoreChanged += OnScoreChanged;
            GameEvents.LivesChanged += OnLivesChanged;
            GameEvents.PowerLevelChanged += OnPowerLevelChanged;
            GameEvents.MissileCountChanged += OnMissileCountChanged;
            GameEvents.LevelCompleted += OnLevelCompleted;
            GameEvents.PlayerDied += OnPlayerDied;
        }

        private void OnDisable()
        {
            GameEvents.ScoreChanged -= OnScoreChanged;
            GameEvents.LivesChanged -= OnLivesChanged;
            GameEvents.PowerLevelChanged -= OnPowerLevelChanged;
            GameEvents.MissileCountChanged -= OnMissileCountChanged;
            GameEvents.LevelCompleted -= OnLevelCompleted;
            GameEvents.PlayerDied -= OnPlayerDied;
        }

        public void StartNewGame()
        {
            live = new RunState();
            checkpoint = new RunState();
            currentLevel = config != null ? config.FirstLevel : null;
        }

        public void StartGame()
        {
            if (transitioning)
            {
                return;
            }

            StartNewGame();

            if (currentLevel == null)
            {
                Debug.LogError("GameManager: the GameConfig has no First Level.", this);
                return;
            }

            LoadScene(currentLevel.SceneName);
        }

        public void LoadMainMenu()
        {
            LoadScene(MainMenuScene);
        }

        public void LoadScene(string sceneName)
        {
            if (transitioning)
            {
                return;
            }

            StartCoroutine(TransitionRoutine(sceneName));
        }

        public void RegisterLevel(LevelDataSO level)
        {
            if (level == null || level == currentLevel)
            {
                return;
            }

            currentLevel = level;
            checkpoint = new RunState();
        }

        public bool AdvanceToNextLevel()
        {
            if (currentLevel == null || currentLevel.IsLastLevel)
            {
                return false;
            }

            currentLevel = currentLevel.NextLevel;
            return true;
        }

        private void Snapshot()
        {
            checkpoint = live.Clone();
            checkpoint.hasCarry = true;
        }

        private void OnScoreChanged(int score)
        {
            live.score = score;
        }

        private void OnLivesChanged(int current, int max)
        {
            live.lives = current;
        }

        private void OnPowerLevelChanged(int level)
        {
            live.powerLevel = level;
        }

        private void OnMissileCountChanged(int count)
        {
            live.missiles = count;
        }

        private void OnLevelCompleted(int levelNumber)
        {
            Snapshot();

            if (!flowBusy)
            {
                StartCoroutine(LevelEndRoutine());
            }
        }

        private void OnPlayerDied()
        {
            if (!flowBusy)
            {
                StartCoroutine(GameOverRoutine());
            }
        }

        private IEnumerator LevelEndRoutine()
        {
            flowBusy = true;
            yield return new WaitForSeconds(LevelEndDelay);

            if (AdvanceToNextLevel())
            {
                LoadScene(currentLevel.SceneName);
            }
            else
            {
                Debug.Log("Game complete: every level cleared. Returning to the main menu.");
                LoadMainMenu();
            }
        }

        private IEnumerator GameOverRoutine()
        {
            flowBusy = true;
            yield return new WaitForSeconds(GameOverDelay);

            Debug.Log("Game over. Returning to the main menu.");
            LoadMainMenu();
        }

        private IEnumerator TransitionRoutine(string sceneName)
        {
            transitioning = true;
            Time.timeScale = 1f;

            yield return fader.FadeTo(1f, FadeSeconds);

            AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
            if (load == null)
            {
                Debug.LogError($"GameManager: scene '{sceneName}' could not be loaded. Is it in Build Settings?", this);
            }
            else
            {
                while (!load.isDone)
                {
                    yield return null;
                }

                yield return null;
            }

            yield return fader.FadeTo(0f, FadeSeconds);

            transitioning = false;
            flowBusy = false;
        }

        [ContextMenu("Debug: Snapshot And Reload Scene")]
        private void DebugSnapshotAndReload()
        {
            Snapshot();
            LoadScene(SceneManager.GetActiveScene().name);
        }

        [ContextMenu("Debug: Complete Level Now")]
        private void DebugCompleteLevel()
        {
            if (currentLevel != null)
            {
                GameEvents.RaiseLevelCompleted(currentLevel.LevelNumber);
            }
        }
    }
}