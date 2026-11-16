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

        public GameConfigSO Config => config;
        public LevelDataSO CurrentLevel => currentLevel;
        public RunState Live => live;
        public RunState Checkpoint => checkpoint;

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

            StartNewGame();
        }

        private void OnEnable()
        {
            GameEvents.ScoreChanged += OnScoreChanged;
            GameEvents.LivesChanged += OnLivesChanged;
            GameEvents.PowerLevelChanged += OnPowerLevelChanged;
            GameEvents.MissileCountChanged += OnMissileCountChanged;
            GameEvents.LevelCompleted += OnLevelCompleted;
        }

        private void OnDisable()
        {
            GameEvents.ScoreChanged -= OnScoreChanged;
            GameEvents.LivesChanged -= OnLivesChanged;
            GameEvents.PowerLevelChanged -= OnPowerLevelChanged;
            GameEvents.MissileCountChanged -= OnMissileCountChanged;
            GameEvents.LevelCompleted -= OnLevelCompleted;
        }

        public void StartNewGame()
        {
            live = new RunState();
            checkpoint = new RunState();
            currentLevel = config != null ? config.FirstLevel : null;
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
            checkpoint = live.Clone();
            checkpoint.hasCarry = true;
        }

        [ContextMenu("Debug: Snapshot And Reload Scene")]
        private void DebugSnapshotAndReload()
        {
            OnLevelCompleted(currentLevel != null ? currentLevel.LevelNumber : 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}