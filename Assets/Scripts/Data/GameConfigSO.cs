using UnityEngine;

namespace ChickenOmelette.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Chicken Omelette/Game Config")]
    public class GameConfigSO : ScriptableObject
    {
        [SerializeField] private LevelDataSO firstLevel;
        [Tooltip("Exact name of the main menu scene in Build Settings.")]
        [SerializeField] private string mainMenuScene = "MainMenu";

        [Header("Flow timing (seconds)")]
        [Tooltip("Time the level-complete banner stays before the next level loads.")]
        [SerializeField, Min(0f)] private float levelEndDelay = 3.5f;
        [Tooltip("Time between the player's last life being lost and leaving the level.")]
        [SerializeField, Min(0f)] private float gameOverDelay = 2.5f;
        [SerializeField, Min(0f)] private float fadeSeconds = 0.4f;

        public LevelDataSO FirstLevel => firstLevel;
        public string MainMenuScene => mainMenuScene;
        public float LevelEndDelay => levelEndDelay;
        public float GameOverDelay => gameOverDelay;
        public float FadeSeconds => fadeSeconds;
    }
}