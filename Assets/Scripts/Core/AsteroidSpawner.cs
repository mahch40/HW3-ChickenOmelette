using System;
using System.Collections;
using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float spawnMargin = 1.2f;
        [SerializeField, Min(0f)] private float horizontalPadding = 0.8f;
        [Tooltip("Maximum sideways speed in units per second (random left or right).")]
        [SerializeField, Min(0f)] private float maxDrift = 1f;
        [Tooltip("Maximum spin in degrees per second (random direction).")]
        [SerializeField, Min(0f)] private float maxSpin = 90f;
        [Tooltip("Random variation of the time between asteroids (0.4 = plus or minus 40 percent).")]
        [SerializeField, Range(0f, 0.9f)] private float intervalJitter = 0.4f;

        [Header("Debug")]
        [SerializeField] private bool logAliveCount;

        private Coroutine routine;

        public bool IsSpawning { get; private set; }
        public int AliveCount { get; private set; }

        public event Action AllSpawned;

        public void StartWave(RoundConfig round)
        {
            if (round == null || round.wave == null)
            {
                Debug.LogError("AsteroidSpawner: round or wave is missing.", this);
                return;
            }

            if (round.wave.Type != WaveType.Asteroids || round.wave.EnemyPrefab == null)
            {
                Debug.LogWarning("AsteroidSpawner only spawns Asteroids waves that have an enemy prefab.", this);
                return;
            }

            StopSpawning();
            routine = StartCoroutine(SpawnRoutine(round));
        }

        public void StopSpawning()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            IsSpawning = false;
        }

        private IEnumerator SpawnRoutine(RoundConfig round)
        {
            WaveDataSO wave = round.wave;
            int count = round.ScaledCount;
            PoolManager.Prewarm(wave.EnemyPrefab, Mathf.Min(count, 10));

            IsSpawning = true;

            for (int i = 0; i < count; i++)
            {
                SpawnOne(round);

                float delay = wave.SpawnInterval * (1f + UnityEngine.Random.Range(-intervalJitter, intervalJitter));
                float waited = 0f;
                while (waited < delay)
                {
                    waited += Time.deltaTime;
                    yield return null;
                }
            }

            IsSpawning = false;
            routine = null;
            AllSpawned?.Invoke();
        }

        private void SpawnOne(RoundConfig round)
        {
            WaveDataSO wave = round.wave;
            Rect screen = ScreenBounds.Rect;

            float x = UnityEngine.Random.Range(screen.xMin + horizontalPadding, screen.xMax - horizontalPadding);
            Vector3 position = new Vector3(x, screen.yMax + spawnMargin, 0f);

            GameObject instance = PoolManager.Spawn(wave.EnemyPrefab, position, Quaternion.identity);

            if (!instance.TryGetComponent(out Asteroid asteroid))
            {
                Debug.LogError("The asteroid prefab needs an Asteroid component.", instance);
                return;
            }

            float drift = UnityEngine.Random.Range(-maxDrift, maxDrift);
            float spin = UnityEngine.Random.Range(-maxSpin, maxSpin);
            asteroid.Configure(round.ScaledHealth(asteroid.PrefabHealth), round.ScaledMoveSpeed, drift, spin, HandleRemoved);

            AliveCount++;
            LogAlive();
        }

        private void HandleRemoved(Asteroid asteroid)
        {
            AliveCount = Mathf.Max(0, AliveCount - 1);
            LogAlive();
        }

        private void LogAlive()
        {
            if (logAliveCount)
            {
                Debug.Log($"Asteroids alive: {AliveCount}", this);
            }
        }
    }
}