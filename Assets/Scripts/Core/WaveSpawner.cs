using System;
using System.Collections;
using System.Collections.Generic;
using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private FormationController formation;

        public bool IsSpawning { get; private set; }
        public FormationController Formation => formation;

        public event Action AllSpawned;
        public event Action<RoundConfig> WaveStarted;

        public void StartWave(RoundConfig round)
        {
            if (round == null || round.wave == null)
            {
                Debug.LogError("WaveSpawner: round or wave is missing.", this);
                return;
            }

            if (round.wave.Type != WaveType.Chickens || round.wave.EnemyPrefab == null)
            {
                Debug.LogWarning("WaveSpawner only spawns Chickens waves that have an enemy prefab.", this);
                return;
            }

            StartCoroutine(SpawnRoutine(round));
            WaveStarted?.Invoke(round);
        }

        private IEnumerator SpawnRoutine(RoundConfig round)
        {
            WaveDataSO wave = round.wave;
            int count = round.ScaledCount;

            formation.Begin(wave, count);
            EntryPath path = BuildPath(wave);
            PoolManager.Prewarm(wave.EnemyPrefab, count);

            WaitForSeconds wait = new WaitForSeconds(wave.SpawnInterval);
            IsSpawning = true;

            for (int i = 0; i < count; i++)
            {
                GameObject instance = PoolManager.Spawn(wave.EnemyPrefab, path.Evaluate(0f), Quaternion.identity);

                if (instance.TryGetComponent(out Chicken chicken))
                {
                    chicken.Configure(round.ScaledHealth(chicken.PrefabHealth));
                }

                if (instance.TryGetComponent(out ChickenMover mover))
                {
                    mover.BeginEntry(formation, i, path, round.ScaledMoveSpeed);
                }
                else
                {
                    Debug.LogError("The chicken prefab needs a ChickenMover component.", instance);
                }

                yield return wait;
            }

            IsSpawning = false;
            AllSpawned?.Invoke();
        }

        private static EntryPath BuildPath(WaveDataSO wave)
        {
            List<Vector2> points = new List<Vector2>();
            Vector2[] waypoints = wave.EntryWaypoints;

            if (waypoints != null)
            {
                for (int i = 0; i < waypoints.Length; i++)
                {
                    points.Add(ScreenBounds.FromViewport(waypoints[i]));
                }
            }

            if (points.Count < 2)
            {
                points.Clear();
                points.Add(ScreenBounds.FromViewport(new Vector2(0.5f, 1.2f)));
                points.Add(ScreenBounds.FromViewport(wave.FormationCenter));
            }

            return new EntryPath(points);
        }
    }
}