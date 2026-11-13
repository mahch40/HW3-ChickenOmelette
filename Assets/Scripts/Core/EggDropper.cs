using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class EggDropper : MonoBehaviour
    {
        [SerializeField] private WaveSpawner spawner;
        [SerializeField] private FormationController formation;
        [SerializeField] private GameObject eggPrefab;
        [SerializeField] private Vector2 spawnOffset = new Vector2(0f, -0.4f);
        [Tooltip("Shortest allowed gap between two eggs, in seconds.")]
        [SerializeField, Min(0f)] private float minDelay = 0.2f;
        [Tooltip("As chickens die the drop rate falls, but never below this fraction of the wave's rate.")]
        [SerializeField, Range(0f, 1f)] private float minRateFactor = 0.35f;
        [SerializeField, Min(1)] private int prewarmCount = 10;

        private float baseRate;
        private float fallSpeed;
        private int startCount;
        private float timer;
        private bool active;

        private void OnEnable()
        {
            spawner.WaveStarted += Begin;
        }

        private void OnDisable()
        {
            spawner.WaveStarted -= Begin;
        }

        public void Begin(RoundConfig round)
        {
            baseRate = round.ScaledEggsPerSecond;
            fallSpeed = round.wave.EggFallSpeed;
            startCount = Mathf.Max(1, round.ScaledCount);

            PoolManager.Prewarm(eggPrefab, prewarmCount);
            ScheduleNext();
            active = true;
        }

        public void Stop()
        {
            active = false;
        }

        private void Update()
        {
            if (!active)
            {
                return;
            }

            timer -= Time.deltaTime;
            if (timer > 0f)
            {
                return;
            }

            if (TryDrop())
            {
                ScheduleNext();
            }
            else
            {
                timer = 0.25f;
            }
        }

        private bool TryDrop()
        {
            var members = formation.Members;
            int ready = 0;
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].IsInFormation)
                {
                    ready++;
                }
            }

            if (ready == 0)
            {
                return false;
            }

            int pick = Random.Range(0, ready);
            for (int i = 0; i < members.Count; i++)
            {
                if (!members[i].IsInFormation)
                {
                    continue;
                }

                if (pick-- > 0)
                {
                    continue;
                }

                Vector3 position = members[i].transform.position + (Vector3)spawnOffset;
                GameObject egg = PoolManager.Spawn(eggPrefab, position, Quaternion.identity);
                if (egg.TryGetComponent(out Egg component))
                {
                    component.Launch(fallSpeed);
                }

                return true;
            }

            return false;
        }

        private void ScheduleNext()
        {
            float fraction = Mathf.Clamp01(formation.Members.Count / (float)startCount);
            float rate = baseRate * Mathf.Max(minRateFactor, fraction);

            if (rate <= 0f)
            {
                timer = 0.5f;
                return;
            }

            float u = Mathf.Max(Random.value, 0.0001f);
            timer = Mathf.Max(minDelay, -Mathf.Log(u) / rate);
        }
    }
}