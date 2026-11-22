using UnityEngine;

namespace ChickenOmelette.Core
{
    public class ScoreManager : MonoBehaviour
    {
        public int Score { get; private set; }

        private void OnEnable()
        {
            GameEvents.EnemyKilled += OnEnemyKilled;
            GameEvents.PointsAwarded += Add;
        }

        private void OnDisable()
        {
            GameEvents.EnemyKilled -= OnEnemyKilled;
            GameEvents.PointsAwarded -= Add;
        }
        
        private void Start()
        {
            GameEvents.RaiseScoreChanged(Score);
        }

        public void Add(int points)
        {
            if (points <= 0)
            {
                return;
            }

            SetScore(Score + points);
        }

        public void SetScore(int score)
        {
            Score = Mathf.Max(0, score);
            GameEvents.RaiseScoreChanged(Score);
        }

        private void OnEnemyKilled(Vector3 position, int scoreValue)
        {
            Add(scoreValue);
        }
    }
}