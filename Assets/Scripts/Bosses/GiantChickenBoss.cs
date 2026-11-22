using UnityEngine;

namespace ChickenOmelette.Core
{
    public class GiantChickenBoss : BossBase
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject eggPrefab;
        [SerializeField] private GameObject minionPrefab;
        [SerializeField] private Vector2 eggSpawnOffset = new Vector2(0f, -1f);

        [Header("Sway")]
        [SerializeField, Min(0f)] private float swayAmplitude = 4f;
        [SerializeField, Min(0.05f)] private float swaySpeed = 0.8f;

        [Header("Phase 1: egg barrage")]
        [SerializeField, Min(0.2f)] private float barrageInterval = 1.6f;
        [SerializeField, Min(1)] private int barrageEggs = 5;
        [SerializeField, Range(10f, 160f)] private float barrageSpread = 60f;
        [SerializeField, Min(0.5f)] private float eggSpeed = 5f;

        [Header("Phase 2: charge")]
        [SerializeField, Min(0.1f)] private float chargeWindup = 0.9f;
        [SerializeField, Min(1f)] private float chargeSpeed = 14f;
        [SerializeField, Min(1f)] private float returnSpeed = 5f;
        [SerializeField, Min(0f)] private float chargeCooldown = 1.5f;
        [Tooltip("How far above the ship's height the charge stops.")]
        [SerializeField, Min(0f)] private float chargeReach = 1.5f;

        [Header("Phase 3: enraged")]
        [SerializeField, Min(0.2f)] private float enragedInterval = 0.9f;
        [SerializeField, Min(1)] private int enragedEggs = 7;
        [SerializeField, Range(10f, 160f)] private float enragedSpread = 80f;
        [SerializeField, Min(1f)] private float enragedSwayFactor = 1.8f;
        [SerializeField, Min(1f)] private float summonInterval = 5f;
        [SerializeField, Min(1)] private int summonCount = 2;
        [SerializeField, Min(1)] private int maxMinions = 6;
        [SerializeField, Min(0.5f)] private float minionSpeed = 3f;

        private ShipController ship;
        private SpriteRenderer sprite;
        private Vector3 rest;
        private float swayTime;

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
                PoolManager.Prewarm(eggPrefab, 20);
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
                new BarrageState(this),
                new ChargeState(this),
                new EnragedState(this)
            };
        }


        private void Sway(float deltaTime, float speedFactor)
        {
            swayTime += deltaTime * swaySpeed * speedFactor;
            Vector3 target = new Vector3(rest.x + Mathf.Sin(swayTime) * swayAmplitude, rest.y, rest.z);
            transform.position = Vector3.MoveTowards(transform.position, target, 12f * deltaTime);
        }

        private void FireFan(int count, float spread, bool staggered)
        {
            if (eggPrefab == null)
            {
                return;
            }

            float step = spread / Mathf.Max(1, count - 1);
            float offset = staggered ? step * 0.5f : 0f;
            Vector3 origin = transform.position + (Vector3)eggSpawnOffset;

            for (int i = 0; i < count; i++)
            {
                float t = count == 1 ? 0.5f : i / (float)(count - 1);
                float angle = Mathf.Lerp(-spread * 0.5f, spread * 0.5f, t) + offset;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.down;

                GameObject egg = PoolManager.Spawn(eggPrefab, origin, Quaternion.identity);
                if (egg.TryGetComponent(out Egg component))
                {
                    component.Launch(direction, eggSpeed);
                }
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

                Vector3 position = transform.position + new Vector3(Random.Range(-2f, 2f), -1f, 0f);
                GameObject minion = PoolManager.Spawn(minionPrefab, position, Quaternion.identity);
                if (minion.TryGetComponent(out BossMinion component))
                {
                    component.Launch(minionSpeed);
                }
            }
        }

        private Vector3 PlayerPosition()
        {
            if (ship == null)
            {
                ship = FindFirstObjectByType<ShipController>();
            }

            return ship != null
                ? ship.transform.position
                : new Vector3(rest.x, ScreenBounds.Bottom + 2f, 0f);
        }

        private static Color Hex(string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out Color color) ? color : Color.white;
        }

        private void SetColor(Color color)
        {
            if (sprite != null)
            {
                sprite.color = color;
            }
        }


        private sealed class BarrageState : BossState
        {
            private readonly GiantChickenBoss b;
            private float timer;
            private bool staggered;

            public BarrageState(GiantChickenBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                timer = 0.6f;
                b.SetColor(Color.white);
            }

            public override void Tick(float deltaTime)
            {
                b.Sway(deltaTime, 1f);

                timer -= deltaTime;
                if (timer > 0f)
                {
                    return;
                }

                timer = b.barrageInterval;
                staggered = !staggered;
                b.FireFan(b.barrageEggs, b.barrageSpread, staggered);
            }
        }

        private sealed class ChargeState : BossState
        {
            private enum Step { Cooldown, Windup, Dash, Return }

            private readonly GiantChickenBoss b;
            private Step step;
            private float timer;
            private float flashTime;
            private Vector3 windupOrigin;
            private Vector3 target;

            public ChargeState(GiantChickenBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                step = Step.Cooldown;
                timer = 0.5f;
                b.SetColor(Color.white);
            }

            public override void Exit()
            {
                b.SetColor(Color.white);
            }

            public override void Tick(float deltaTime)
            {
                switch (step)
                {
                    case Step.Cooldown:
                        b.Sway(deltaTime, 1f);
                        timer -= deltaTime;
                        if (timer <= 0f)
                        {
                            step = Step.Windup;
                            timer = b.chargeWindup;
                            flashTime = 0f;
                            windupOrigin = b.transform.position;
                        }
                        break;

                    case Step.Windup:
                        flashTime += deltaTime;
                        b.transform.position = windupOrigin + (Vector3)(Random.insideUnitCircle * 0.08f);
                        b.SetColor(Color.Lerp(Color.white, Hex("#FF8A80"), Mathf.PingPong(flashTime * 8f, 1f)));
                        timer -= deltaTime;
                        if (timer <= 0f)
                        {
                            b.SetColor(Color.white);
                            Vector3 player = b.PlayerPosition();
                            target = new Vector3(
                                Mathf.Clamp(player.x, ScreenBounds.Left + 1f, ScreenBounds.Right - 1f),
                                player.y + b.chargeReach,
                                0f);
                            step = Step.Dash;
                        }
                        break;

                    case Step.Dash:
                        b.transform.position = Vector3.MoveTowards(b.transform.position, target, b.chargeSpeed * deltaTime);
                        if ((b.transform.position - target).sqrMagnitude < 0.01f)
                        {
                            b.FireFan(5, 100f, false);
                            step = Step.Return;
                        }
                        break;

                    case Step.Return:
                        b.transform.position = Vector3.MoveTowards(b.transform.position, b.rest, b.returnSpeed * deltaTime);
                        if ((b.transform.position - b.rest).sqrMagnitude < 0.01f)
                        {
                            step = Step.Cooldown;
                            timer = b.chargeCooldown;
                        }
                        break;
                }
            }
        }

        private sealed class EnragedState : BossState
        {
            private readonly GiantChickenBoss b;
            private float eggTimer;
            private float summonTimer;
            private bool staggered;

            public EnragedState(GiantChickenBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                eggTimer = 0.4f;
                summonTimer = 1f;
                b.SetColor(Hex("#FF8A80"));
            }

            public override void Tick(float deltaTime)
            {
                b.Sway(deltaTime, b.enragedSwayFactor);

                eggTimer -= deltaTime;
                if (eggTimer <= 0f)
                {
                    eggTimer = b.enragedInterval;
                    staggered = !staggered;
                    b.FireFan(b.enragedEggs, b.enragedSpread, staggered);
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