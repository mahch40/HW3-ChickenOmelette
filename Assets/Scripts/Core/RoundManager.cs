using System.Collections;
using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class RoundManager : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField] private LevelDataSO level;
        [Tooltip("Start the level automatically when the scene loads.")]
        [SerializeField] private bool autoStart = true;

        [Header("Spawners")]
        [SerializeField] private WaveSpawner chickenSpawner;
        [SerializeField] private AsteroidSpawner asteroidSpawner;
        [SerializeField] private EggDropper eggDropper;
        [SerializeField] private DiveAttacker diveAttacker;

        [Header("Presentation")]
        [SerializeField] private RoundBanner banner;
        [SerializeField, Min(0f)] private float bannerSeconds = 1.8f;
        [SerializeField, Min(0f)] private float gapBetweenRounds = 1f;
        [Tooltip("Placeholder: how long a boss round lasts until the real boss exists.")]
        [SerializeField, Min(0f)] private float bossPlaceholderSeconds = 3f;
        [SerializeField] private bool logProgress = true;

        private Coroutine levelRoutine;

        public LevelDataSO Level => level;
        public int CurrentRound { get; private set; }
        public bool IsRunning { get; private set; }

        private void OnEnable()
        {
            GameEvents.PlayerDied += Halt;
        }

        private void OnDisable()
        {
            GameEvents.PlayerDied -= Halt;
        }

        private void Start()
        {
            if (level != null && GameManager.Instance != null)
            {
                GameManager.Instance.RegisterLevel(level);
            }

            if (autoStart && level != null)
            {
                StartLevel(level);
            }
        }

        public void StartLevel(LevelDataSO data)
        {
            Halt();
            level = data;
            levelRoutine = StartCoroutine(RunLevel());
        }

        public void Halt()
        {
            if (levelRoutine != null)
            {
                StopCoroutine(levelRoutine);
                levelRoutine = null;
            }

            IsRunning = false;

            if (chickenSpawner != null)
            {
                chickenSpawner.StopSpawning();
            }

            if (asteroidSpawner != null)
            {
                asteroidSpawner.StopSpawning();
            }

            StopAttacks();
        }

        private IEnumerator RunLevel()
        {
            IsRunning = true;
            Log($"Level {level.LevelNumber} started ({level.RoundCount} rounds)");
            GameEvents.RaiseLevelStarted(level.LevelNumber);

            yield return new WaitForSeconds(0.5f);

            for (int roundNumber = 1; roundNumber <= level.RoundCount; roundNumber++)
            {
                RoundConfig round = level.GetRound(roundNumber);
                if (round == null || round.wave == null)
                {
                    Debug.LogError($"Round {roundNumber} has no wave assigned.", this);
                    continue;
                }

                CurrentRound = roundNumber;
                Log($"Round {roundNumber}/{level.RoundCount}: {round.wave.WaveName} ({round.wave.Type})");
                GameEvents.RaiseRoundStarted(level.LevelNumber, roundNumber);

                if (banner != null)
                {
                    banner.Show(BuildTitle(round, roundNumber), bannerSeconds);
                }

                yield return new WaitForSeconds(bannerSeconds);
                yield return RunRound(round);

                StopAttacks();
                Log($"Round {roundNumber} cleared");
                GameEvents.RaiseRoundCompleted(level.LevelNumber, roundNumber);

                if (!level.IsLastRound(roundNumber))
                {
                    yield return new WaitForSeconds(gapBetweenRounds);
                }
            }

            IsRunning = false;
            levelRoutine = null;
            Log($"Level {level.LevelNumber} complete");
            GameEvents.RaiseLevelCompleted(level.LevelNumber);

            if (banner != null)
            {
                banner.Show(level.CompletionMessage, bannerSeconds + 1f);
            }
        }

        private IEnumerator RunRound(RoundConfig round)
        {
            switch (round.wave.Type)
            {
                case WaveType.Chickens:
                    chickenSpawner.StartWave(round);
                    yield return new WaitUntil(() =>
                        !chickenSpawner.IsSpawning && chickenSpawner.Formation.Members.Count == 0);
                    break;

                case WaveType.Asteroids:
                    asteroidSpawner.StartWave(round);
                    yield return new WaitUntil(() =>
                        !asteroidSpawner.IsSpawning && asteroidSpawner.AliveCount == 0);
                    break;

                case WaveType.Boss:
                    Log("Boss round: placeholder, the boss is not built yet");
                    yield return new WaitForSeconds(bossPlaceholderSeconds);
                    break;
            }
        }

        private static string BuildTitle(RoundConfig round, int roundNumber)
        {
            return round.wave.Type == WaveType.Boss ? "BOSS FIGHT" : $"ROUND {roundNumber}";
        }

        private void StopAttacks()
        {
            if (eggDropper != null)
            {
                eggDropper.Stop();
            }

            if (diveAttacker != null)
            {
                diveAttacker.Stop();
            }
        }

        private void Log(string message)
        {
            if (logProgress)
            {
                Debug.Log(message, this);
            }
        }
    }
}