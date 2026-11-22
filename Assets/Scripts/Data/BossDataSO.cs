using UnityEngine;

namespace ChickenOmelette.Data
{
    [CreateAssetMenu(fileName = "Boss_New", menuName = "Chicken Omelette/Boss Data")]
    public class BossDataSO : ScriptableObject
    {
        [SerializeField] private string bossName = "Giant Chicken";
        [SerializeField, Min(1)] private int maxHealth = 60;
        [SerializeField, Min(0)] private int scoreValue = 1000;
        [Tooltip("Health fraction at which each phase begins, highest first. The last phase is Enraged.")]
        [SerializeField] private float[] phaseStartFractions = { 1f, 0.65f, 0.3f };

        public string BossName => bossName;
        public int MaxHealth => maxHealth;
        public int ScoreValue => scoreValue;
        public float[] PhaseStartFractions => phaseStartFractions;
    }
}