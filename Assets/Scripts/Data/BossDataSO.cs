using UnityEngine;

namespace ChickenOmelette.Data
{
    [CreateAssetMenu(fileName = "Boss_New", menuName = "Chicken Omelette/Boss Data")]
    public class BossDataSO : ScriptableObject
    {
        [SerializeField] private string bossName = "Giant Chicken";
        [SerializeField, Min(1)] private int maxHealth = 60;
        [SerializeField, Min(0)] private int scoreValue = 1000;

        public string BossName => bossName;
        public int MaxHealth => maxHealth;
        public int ScoreValue => scoreValue;
    }
}