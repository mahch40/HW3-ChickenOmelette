using System;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public static class GameEvents
    {
        // Player
        public static event Action<int, int> LivesChanged;      // current, max
        public static event Action PlayerDamaged;
        public static event Action PlayerDied;
        public static event Action<int> PowerLevelChanged;      // new power level
        public static event Action<int> MissileCountChanged;    // new missile count

        // Score and enemies
        public static event Action<int> ScoreChanged;           // new total score
        public static event Action<Vector3, int> EnemyKilled;   // world position, score value

        // Progression
        public static event Action<int> LevelStarted;           // level number
        public static event Action<int> LevelCompleted;         // level number
        public static event Action<int, int> RoundStarted;      // level number, round number
        public static event Action<int, int> RoundCompleted;    // level number, round number
        public static event Action<int> PointsAwarded;          // points not tied to a chicken kill
        public static void RaiseLivesChanged(int current, int max) => LivesChanged?.Invoke(current, max);
        public static void RaisePlayerDamaged() => PlayerDamaged?.Invoke();
        public static void RaisePlayerDied() => PlayerDied?.Invoke();
        public static void RaisePowerLevelChanged(int level) => PowerLevelChanged?.Invoke(level);
        public static void RaiseMissileCountChanged(int count) => MissileCountChanged?.Invoke(count);

        public static void RaiseScoreChanged(int total) => ScoreChanged?.Invoke(total);
        public static void RaiseEnemyKilled(Vector3 position, int scoreValue) => EnemyKilled?.Invoke(position, scoreValue);

        public static void RaiseLevelStarted(int level) => LevelStarted?.Invoke(level);
        public static void RaiseLevelCompleted(int level) => LevelCompleted?.Invoke(level);
        public static void RaiseRoundStarted(int level, int round) => RoundStarted?.Invoke(level, round);
        public static void RaiseRoundCompleted(int level, int round) => RoundCompleted?.Invoke(level, round);
        public static void RaisePointsAwarded(int points) => PointsAwarded?.Invoke(points);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearAll()
        {
            LivesChanged = null;
            PlayerDamaged = null;
            PlayerDied = null;
            PowerLevelChanged = null;
            MissileCountChanged = null;
            ScoreChanged = null;
            EnemyKilled = null;
            LevelStarted = null;
            LevelCompleted = null;
            RoundStarted = null;
            RoundCompleted = null;
            PointsAwarded = null;
        }
    }
}