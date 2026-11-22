using UnityEngine;

namespace ChickenOmelette.Core
{
    public class EmperorBoss : BossBase
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject eggPrefab;
        [Tooltip("Optional. Used by the enraged phase to summon helpers.")]
        [SerializeField] private GameObject minionPrefab;
        [SerializeField] private Vector2 eggSpawnOffset = new Vector2(0f, -1.5f);

        [Header("Movement")]
        [SerializeField, Min(0f)] private float swayAmplitude = 1.5f;
        [SerializeField, Min(0.05f)] private float swaySpeed = 0.6f;

        [Header("Phase 1: spray")]
        [SerializeField, Min(0.03f)] private float sprayInterval = 0.14f;
        [SerializeField, Range(10f, 85f)] private float sprayAngle = 65f;
        [SerializeField, Min(0.1f)] private float spraySweepSpeed = 1.6f;
        [SerializeField, Min(0.5f)] private float sprayEggSpeed = 4.5f;

        [Header("Phase 2: ring")]
        [SerializeField, Min(0.5f)] private float ringInterval = 1.8f;
        [SerializeField, Range(6, 40)] private int ringEggs = 20;
        [SerializeField, Range(60f, 360f)] private float ringArc = 240f;
        [SerializeField, Min(1)] private int ringGap = 3;
        [SerializeField, Min(0.5f)] private float ringEggSpeed = 4.5f;
        [SerializeField, Min(0.1f)] private float ringTelegraph = 0.6f;

        [Header("Phase 3: curtain")]
        [SerializeField, Min(0.5f)] private float curtainInterval = 2.2f;
        [SerializeField, Min(0.5f)] private float curtainSpacing = 1.1f;
        [SerializeField, Min(1)] private int curtainGap = 2;
        [SerializeField, Min(0.5f)] private float curtainSpeed = 4f;
        [SerializeField, Min(0.5f)] private float aimedSpeed = 6f;

        [Header("Phase 4: enraged")]
        [SerializeField, Min(0.03f)] private float enragedSprayInterval = 0.16f;
        [SerializeField, Min(0.5f)] private float enragedRingInterval = 2.6f;
        [SerializeField, Min(1)] private int enragedRingGap = 2;
        [SerializeField, Min(1f)] private float summonInterval = 6f;
        [SerializeField, Min(1)] private int summonCount = 2;
        [SerializeField, Min(1)] private int maxMinions = 6;
        [SerializeField, Min(0.5f)] private float minionSpeed = 3f;

        private SpriteRenderer sprite;
        private Vector3 rest;
        private float swayTime;

        private Vector3 Origin => transform.position + (Vector3)eggSpawnOffset;

        protected override void Awake()
        {
            base.Awake();
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void Start()
        {
            base.Start();
            if (eggPrefab != null)
            {
                PoolManager.Prewarm(eggPrefab, 80);
            }

            if (minionPrefab != null)
            {
                PoolManager.Prewarm(minionPrefab, maxMinions);
            }
        }

        protected override void OnEntranceFinished()
        {
            rest = transform.position;
            base.OnEntranceFinished();
        }

        protected override BossState[] CreatePhaseStates()
        {
            return new BossState[]
            {
                new SprayState(this),
                new RingState(this),
                new CurtainState(this),
                new EnragedState(this)
            };
        }

        private void Sway(float deltaTime, float speedFactor)
        {
            swayTime += deltaTime * swaySpeed * speedFactor;
            Vector3 target = new Vector3(rest.x + Mathf.Sin(swayTime) * swayAmplitude, rest.y, rest.z);
            transform.position = Vector3.MoveTowards(transform.position, target, 10f * deltaTime);
        }

        private void FireAtAngle(float angleDegrees, float speed)
        {
            if (eggPrefab == null)
            {
                return;
            }

            Vector2 direction = Quaternion.Euler(0f, 0f, angleDegrees) * Vector2.down;
            BossPatterns.FireEgg(eggPrefab, Origin, direction, speed);
        }

        private void FireRing(int count, float arc, int gapSize, float speed)
        {
            gapSize = Mathf.Min(gapSize, count - 1);
            int gapStart = Random.Range(0, count - gapSize + 1);

            for (int i = 0; i < count; i++)
            {
                if (i >= gapStart && i < gapStart + gapSize)
                {
                    continue;
                }

                float t = i / (float)(count - 1);
                FireAtAngle(Mathf.Lerp(-arc * 0.5f, arc * 0.5f, t), speed);
            }
        }

        private void FireCurtain()
        {
            if (eggPrefab == null)
            {
                return;
            }

            float left = ScreenBounds.Left + 0.5f;
            float width = ScreenBounds.Right - 0.5f - left;
            int cells = Mathf.Max(3, Mathf.FloorToInt(width / curtainSpacing) + 1);
            int gap = Mathf.Min(curtainGap, cells - 1);
            int gapStart = Random.Range(0, cells - gap + 1);
            float y = ScreenBounds.Top - 0.3f;

            for (int i = 0; i < cells; i++)
            {
                if (i >= gapStart && i < gapStart + gap)
                {
                    continue;
                }

                BossPatterns.FireEgg(eggPrefab, new Vector3(left + i * curtainSpacing, y, 0f), Vector2.down, curtainSpeed);
            }
        }

        private void FireAimedTriple()
        {
            if (eggPrefab == null)
            {
                return;
            }

            Vector3 origin = Origin;
            Vector3 target = BossPatterns.PlayerPosition(origin + Vector3.down);
            Vector2 toPlayer = (target - origin).normalized;
            if (toPlayer.y > -0.2f)
            {
                toPlayer = Vector2.down;
            }

            for (int i = -1; i <= 1; i++)
            {
                Vector2 direction = Quaternion.Euler(0f, 0f, i * 15f) * toPlayer;
                BossPatterns.FireEgg(eggPrefab, origin, direction, aimedSpeed);
            }
        }

        private void SummonMinions()
        {
            if (minionPrefab == null)
            {
                return;
            }

            for (int i = 0; i < summonCount; i++)
            {
                if (BossMinion.ActiveCount >= maxMinions)
                {
                    return;
                }

                Vector3 position = transform.position + new Vector3(Random.Range(-2.5f, 2.5f), -1f, 0f);
                GameObject minion = PoolManager.Spawn(minionPrefab, position, Quaternion.identity);
                if (minion.TryGetComponent(out BossMinion component))
                {
                    component.Launch(minionSpeed);
                }
            }
        }

        private void SetColor(Color color)
        {
            if (sprite != null)
            {
                sprite.color = color;
            }
        }

        private sealed class SprayState : BossState
        {
            private readonly EmperorBoss b;
            private float timer;
            private float time;

            public SprayState(EmperorBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                timer = 0.6f;
                time = 0f;
                b.SetColor(Color.white);
            }

            public override void Tick(float deltaTime)
            {
                b.Sway(deltaTime, 0.5f);
                time += deltaTime;
                timer -= deltaTime;

                if (timer <= 0f)
                {
                    timer = b.sprayInterval;
                    b.FireAtAngle(Mathf.Sin(time * b.spraySweepSpeed) * b.sprayAngle, b.sprayEggSpeed);
                }
            }
        }

        private sealed class RingState : BossState
        {
            private enum Step { Wait, Telegraph }

            private readonly EmperorBoss b;
            private Step step;
            private float timer;

            public RingState(EmperorBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                step = Step.Wait;
                timer = 1f;
                b.SetColor(Color.white);
            }

            public override void Exit()
            {
                b.SetColor(Color.white);
            }

            public override void Tick(float deltaTime)
            {
                b.Sway(deltaTime, 0.5f);
                timer -= deltaTime;

                if (step == Step.Wait)
                {
                    if (timer <= 0f)
                    {
                        step = Step.Telegraph;
                        timer = b.ringTelegraph;
                    }
                    return;
                }

                b.SetColor(Color.Lerp(Color.white, BossPatterns.Hex("#FFD54F"), Mathf.PingPong(Time.time * 10f, 1f)));
                if (timer <= 0f)
                {
                    b.SetColor(Color.white);
                    b.FireRing(b.ringEggs, b.ringArc, b.ringGap, b.ringEggSpeed);
                    step = Step.Wait;
                    timer = b.ringInterval;
                }
            }
        }

        private sealed class CurtainState : BossState
        {
            private readonly EmperorBoss b;
            private float curtainTimer;
            private float aimedTimer;

            public CurtainState(EmperorBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                curtainTimer = 0.8f;
                aimedTimer = b.curtainInterval * 0.5f + 0.8f;
                b.SetColor(Color.white);
            }

            public override void Tick(float deltaTime)
            {
                b.Sway(deltaTime, 1f);

                curtainTimer -= deltaTime;
                if (curtainTimer <= 0f)
                {
                    curtainTimer = b.curtainInterval;
                    b.FireCurtain();
                }

                aimedTimer -= deltaTime;
                if (aimedTimer <= 0f)
                {
                    aimedTimer = b.curtainInterval;
                    b.FireAimedTriple();
                }
            }
        }

        private sealed class EnragedState : BossState
        {
            private readonly EmperorBoss b;
            private float sprayTimer;
            private float ringTimer;
            private float summonTimer;
            private float time;

            public EnragedState(EmperorBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                sprayTimer = 0.4f;
                ringTimer = 1.2f;
                summonTimer = 1f;
                time = 0f;
            }

            public override void Tick(float deltaTime)
            {
                b.Sway(deltaTime, 1.5f);
                b.SetColor(Color.Lerp(BossPatterns.Hex("#FFD54F"), BossPatterns.Hex("#FF5252"), Mathf.PingPong(Time.time * 3f, 1f)));
                time += deltaTime;

                sprayTimer -= deltaTime;
                if (sprayTimer <= 0f)
                {
                    sprayTimer = b.enragedSprayInterval;
                    float angle = Mathf.Sin(time * 1.8f) * b.sprayAngle;
                    b.FireAtAngle(angle, b.sprayEggSpeed);
                    b.FireAtAngle(-angle, b.sprayEggSpeed);
                }

                ringTimer -= deltaTime;
                if (ringTimer <= 0f)
                {
                    ringTimer = b.enragedRingInterval;
                    b.FireRing(b.ringEggs, b.ringArc, b.enragedRingGap, b.ringEggSpeed);
                }

                summonTimer -= deltaTime;
                if (summonTimer <= 0f)
                {
                    summonTimer = b.summonInterval;
                    b.SummonMinions();
                }
            }
        }
    }
}