using UnityEngine;

namespace ChickenOmelette.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Chicken Omelette/Game Config")]
    public class GameConfigSO : ScriptableObject
    {
        [SerializeField] private LevelDataSO firstLevel;
        [Tooltip("Exact name of the main menu scene in Build Settings.")]
        [SerializeField] private string mainMenuScene = "MainMenu";

        public LevelDataSO FirstLevel => firstLevel;
        public string MainMenuScene => mainMenuScene;
    }
}