using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ChickenOmelette.Data
{
    [Serializable]
    public class RoundConfig
    {
        public WaveDataSO wave;
        [Min(0.1f)] public float countMultiplier = 1f;
        [Min(0.1f)] public float healthMultiplier = 1f;
        [Min(0.1f)] public float speedMultiplier = 1f;
        [Min(0.1f)] public float fireRateMultiplier = 1f;

        public int ScaledCount => Mathf.Max(1, Mathf.RoundToInt(wave.Count * countMultiplier));
        public float ScaledMoveSpeed => wave.MoveSpeed * speedMultiplier;
        public float ScaledEggsPerSecond => wave.EggsPerSecond * fireRateMultiplier;

        public float ScaledDiveInterval => wave.DiveInterval <= 0f ? 0f : wave.DiveInterval / fireRateMultiplier;

        public int ScaledHealth(int prefabHealth)
        {
            int baseHealth = wave.EnemyHealth > 0 ? wave.EnemyHealth : prefabHealth;
            return Mathf.Max(1, Mathf.RoundToInt(baseHealth * healthMultiplier));
        }
    }

    [CreateAssetMenu(fileName = "Level_New", menuName = "Chicken Omelette/Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField, Min(1)] private int levelNumber = 1;
        [Tooltip("Exact name of the scene in Build Settings.")]
        [SerializeField] private string sceneName = "Level1";
        [Tooltip("The level that follows this one. Leave empty for the last level.")]
        [SerializeField] private LevelDataSO nextLevel;

        [Header("Look and sound")]
        [SerializeField] private string themeName = "Planet Orbit";
        [SerializeField] private Color backgroundColor = new Color32(5, 6, 15, 255);
        [Tooltip("Parallax background prefab for this level.")]
        [SerializeField] private GameObject backgroundPrefab;
        [SerializeField] private AudioClip music;
        [TextArea]
        [SerializeField] private string completionMessage = "Level complete!";

        [Header("Rounds (the last round is the boss round)")]
        [SerializeField] private RoundConfig[] rounds;

        public int LevelNumber => levelNumber;
        public string SceneName => sceneName;
        public LevelDataSO NextLevel => nextLevel;
        public bool IsLastLevel => nextLevel == null;

        public string ThemeName => themeName;
        public Color BackgroundColor => backgroundColor;
        public GameObject BackgroundPrefab => backgroundPrefab;
        public AudioClip Music => music;
        public string CompletionMessage => completionMessage;

        public int RoundCount => rounds == null ? 0 : rounds.Length;

        public RoundConfig GetRound(int roundNumber)
        {
            if (rounds == null || roundNumber < 1 || roundNumber > rounds.Length)
            {
                return null;
            }
            return rounds[roundNumber - 1];
        }

        public bool IsLastRound(int roundNumber)
        {
            return roundNumber >= RoundCount;
        }

        [ContextMenu("Log Summary")]
        private void LogSummary()
        {
            string next = nextLevel != null ? nextLevel.sceneName : "none (last level)";
            Debug.Log($"Level {levelNumber}: scene '{sceneName}', theme '{themeName}', {RoundCount} round(s), next: {next}");

            for (int i = 0; i < RoundCount; i++)
            {
                RoundConfig r = rounds[i];
                string waveLabel = r.wave != null ? r.wave.WaveName : "(no wave)";
                Debug.Log($"  Round {i + 1}: {waveLabel}, count x{r.countMultiplier}, health x{r.healthMultiplier}, " +
                          $"speed x{r.speedMultiplier}, fire rate x{r.fireRateMultiplier}");
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Validate Scene Name")]
        private void ValidateSceneName()
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled && System.IO.Path.GetFileNameWithoutExtension(scene.path) == sceneName)
                {
                    Debug.Log($"Scene '{sceneName}' found in Build Settings.");
                    return;
                }
            }
            Debug.LogWarning($"Scene '{sceneName}' is NOT in Build Settings. Check the spelling.");
        }
#endif
    }
}