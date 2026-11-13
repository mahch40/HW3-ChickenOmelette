using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class DiveAttacker : MonoBehaviour
    {
        [SerializeField] private WaveSpawner spawner;
        [SerializeField] private FormationController formation;
        [SerializeField] private Transform player;
        [SerializeField, Min(1)] private int maxSimultaneous = 2;
        [Tooltip("Random variation of the interval, as a fraction (0.3 = plus or minus 30 percent).")]
        [SerializeField, Range(0f, 0.9f)] private float intervalJitter = 0.3f;
        [Tooltip("Seconds after the wave starts before the first dive.")]
        [SerializeField, Min(0f)] private float firstDelay = 3f;
        [SerializeField, Min(0f)] private float exitMargin = 1.5f;

        private float interval;
        private float speed;
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
            interval = round.ScaledDiveInterval;
            speed = round.wave.DiveSpeed;
            active = interval > 0f;
            timer = firstDelay;
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

            if (TryDive())
            {
                timer = interval * (1f + Random.Range(-intervalJitter, intervalJitter));
            }
            else
            {
                timer = 0.5f;
            }
        }

        private bool TryDive()
        {
            if (player == null || !player.gameObject.activeInHierarchy)
            {
                return false;
            }

            var members = formation.Members;
            int ready = 0;
            int diving = 0;
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].IsInFormation)
                {
                    ready++;
                }
                else if (members[i].IsDiving)
                {
                    diving++;
                }
            }

            if (ready == 0 || diving >= maxSimultaneous)
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

                EntryPath path = BuildDivePath(members[i].transform.position, player.position);
                return members[i].BeginDive(path, speed);
            }

            return false;
        }

        private EntryPath BuildDivePath(Vector2 start, Vector2 playerPosition)
        {
            Rect screen = ScreenBounds.Rect;
            float minX = screen.xMin + 0.5f;
            float maxX = screen.xMax - 0.5f;

            Vector2 target = new Vector2(Mathf.Clamp(playerPosition.x, minX, maxX), playerPosition.y);
            float side = start.x <= target.x ? -1f : 1f;

            Vector2 swing = new Vector2(Mathf.Clamp(start.x + side * 2f, minX, maxX), start.y - 1.2f);
            Vector2 sweep = new Vector2(Mathf.Lerp(swing.x, target.x, 0.7f), Mathf.Lerp(start.y, target.y, 0.55f));
            Vector2 exit = new Vector2(Mathf.Clamp(target.x - side * 1.5f, minX, maxX), screen.yMin - exitMargin);

            return new EntryPath(new[] { start, swing, sweep, target, exit });
        }
    }
}