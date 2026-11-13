using UnityEngine;

namespace ChickenOmelette.Core
{
    public class GameEventsLogger : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.LivesChanged += OnLivesChanged;
            GameEvents.PlayerDamaged += OnPlayerDamaged;
            GameEvents.PlayerDied += OnPlayerDied;
            GameEvents.PowerLevelChanged += OnPowerLevelChanged;
            GameEvents.MissileCountChanged += OnMissileCountChanged;
            GameEvents.ScoreChanged += OnScoreChanged;
            GameEvents.EnemyKilled += OnEnemyKilled;
            GameEvents.LevelStarted += OnLevelStarted;
            GameEvents.LevelCompleted += OnLevelCompleted;
            GameEvents.RoundStarted += OnRoundStarted;
            GameEvents.RoundCompleted += OnRoundCompleted;
        }

        private void OnDisable()
        {
            GameEvents.LivesChanged -= OnLivesChanged;
            GameEvents.PlayerDamaged -= OnPlayerDamaged;
            GameEvents.PlayerDied -= OnPlayerDied;
            GameEvents.PowerLevelChanged -= OnPowerLevelChanged;
            GameEvents.MissileCountChanged -= OnMissileCountChanged;
            GameEvents.ScoreChanged -= OnScoreChanged;
            GameEvents.EnemyKilled -= OnEnemyKilled;
            GameEvents.LevelStarted -= OnLevelStarted;
            GameEvents.LevelCompleted -= OnLevelCompleted;
            GameEvents.RoundStarted -= OnRoundStarted;
            GameEvents.RoundCompleted -= OnRoundCompleted;
        }

        private void OnLivesChanged(int current, int max) => Debug.Log($"[Event] LivesChanged {current}/{max}");
        private void OnPlayerDamaged() => Debug.Log("[Event] PlayerDamaged");
        private void OnPlayerDied() => Debug.Log("[Event] PlayerDied");
        private void OnPowerLevelChanged(int level) => Debug.Log($"[Event] PowerLevelChanged {level}");
        private void OnMissileCountChanged(int count) => Debug.Log($"[Event] MissileCountChanged {count}");
        private void OnScoreChanged(int total) => Debug.Log($"[Event] ScoreChanged {total}");
        private void OnEnemyKilled(Vector3 position, int value) => Debug.Log($"[Event] EnemyKilled at {position} value {value}");
        private void OnLevelStarted(int level) => Debug.Log($"[Event] LevelStarted {level}");
        private void OnLevelCompleted(int level) => Debug.Log($"[Event] LevelCompleted {level}");
        private void OnRoundStarted(int level, int round) => Debug.Log($"[Event] RoundStarted {level}-{round}");
        private void OnRoundCompleted(int level, int round) => Debug.Log($"[Event] RoundCompleted {level}-{round}");

        [ContextMenu("Raise Test Events")]
        private void RaiseTestEvents()
        {
            GameEvents.RaiseLevelStarted(1);
            GameEvents.RaiseRoundStarted(1, 1);
            GameEvents.RaiseLivesChanged(2, 3);
            GameEvents.RaisePlayerDamaged();
            GameEvents.RaisePowerLevelChanged(2);
            GameEvents.RaiseMissileCountChanged(1);
            GameEvents.RaiseEnemyKilled(new Vector3(1f, 2f, 0f), 10);
            GameEvents.RaiseScoreChanged(10);
            GameEvents.RaiseRoundCompleted(1, 1);
            GameEvents.RaiseLevelCompleted(1);
            GameEvents.RaisePlayerDied();
        }
    }
}