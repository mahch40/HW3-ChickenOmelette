using UnityEngine;

namespace ChickenOmelette.Core
{
    public class MechaChickenBoss : BossBase
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject eggPrefab;
        [SerializeField] private Vector2 tipOffset = new Vector2(0f, -1.5f);

        [Header("V pattern")]
        [SerializeField, Min(1)] private int vEggsPerSide = 3;
        [SerializeField, Min(0.1f)] private float vSpacing = 0.8f;
        [SerializeField, Min(0f)] private float vDepth = 0.5f;
        [SerializeField, Min(0.5f)] private float vSpeed = 5f;

        [Header("Lanes")]
        [Tooltip("Distance between the centre lane and the left or right lane.")]
        [SerializeField, Min(0.5f)] private float laneOffset = 4f;

        [Header("Phase 1: lane volleys")]
        [SerializeField, Min(0.5f)] private float laneMoveSpeed = 7f;
        [SerializeField, Min(0f)] private float aimTime = 0.5f;
        [SerializeField, Min(0f)] private float laneWait = 0.9f;

        [Header("Phase 2: strafe")]
        [SerializeField, Min(0.5f)] private float strafeSpeed = 6f;
        [SerializeField, Min(0.5f)] private float burstInterval = 2f;
        [SerializeField, Min(1)] private int burstVolleys = 3;
        [SerializeField, Min(0.05f)] private float volleyGap = 0.25f;

        [Header("Phase 3: overdrive")]
        [SerializeField, Min(0.5f)] private float overdriveSpeed = 9f;
        [SerializeField, Min(0.2f)] private float overdriveInterval = 0.8f;
        [SerializeField, Min(1)] private int overdriveEggsPerSide = 4;

        private SpriteRenderer sprite;
        private Vector3 rest;

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
                PoolManager.Prewarm(eggPrefab, 40);
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
                new LaneVolleyState(this),
                new StrafeState(this),
                new OverdriveState(this)
            };
        }

        private float LaneX(int lane)
        {
            return rest.x + (lane - 1) * laneOffset;
        }

        private bool MoveToX(float x, float speed, float deltaTime)
        {
            Vector3 p = transform.position;
            p.x = Mathf.MoveTowards(p.x, x, speed * deltaTime);
            p.y = Mathf.MoveTowards(p.y, rest.y, speed * deltaTime);
            transform.position = p;
            return Mathf.Approximately(p.x, x);
        }

        private void FireStraightV(int eggsPerSide)
        {
            Vector3 tip = transform.position + (Vector3)tipOffset;
            BossPatterns.FireV(eggPrefab, tip, Vector2.down, eggsPerSide, vSpacing, vDepth, vSpeed);
        }

        private void FireAimedV(int eggsPerSide)
        {
            Vector3 tip = transform.position + (Vector3)tipOffset;
            Vector3 target = BossPatterns.PlayerPosition(tip + Vector3.down);
            Vector2 direction = target - tip;
            direction.y = Mathf.Min(direction.y, -1f);
            BossPatterns.FireV(eggPrefab, tip, direction, eggsPerSide, vSpacing, vDepth, vSpeed);
        }

        private void SetColor(Color color)
        {
            if (sprite != null)
            {
                sprite.color = color;
            }
        }

        private sealed class LaneVolleyState : BossState
        {
            private enum Step { Move, Aim, Wait }

            private readonly MechaChickenBoss b;
            private Step step;
            private int lane;
            private float timer;

            public LaneVolleyState(MechaChickenBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                lane = 1;
                step = Step.Move;
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
                    case Step.Move:
                        if (b.MoveToX(b.LaneX(lane), b.laneMoveSpeed, deltaTime))
                        {
                            step = Step.Aim;
                            timer = b.aimTime;
                            b.SetColor(BossPatterns.Hex("#80D8FF"));
                        }
                        break;

                    case Step.Aim:
                        timer -= deltaTime;
                        if (timer <= 0f)
                        {
                            b.SetColor(Color.white);
                            b.FireStraightV(b.vEggsPerSide);
                            step = Step.Wait;
                            timer = b.laneWait;
                        }
                        break;

                    case Step.Wait:
                        timer -= deltaTime;
                        if (timer <= 0f)
                        {
                            int next;
                            do
                            {
                                next = Random.Range(0, 3);
                            }
                            while (next == lane);

                            lane = next;
                            step = Step.Move;
                        }
                        break;
                }
            }
        }

        private sealed class StrafeState : BossState
        {
            private readonly MechaChickenBoss b;
            private int targetLane;
            private float burstTimer;
            private float volleyTimer;
            private int volleysLeft;

            public StrafeState(MechaChickenBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                targetLane = b.transform.position.x >= b.rest.x ? 0 : 2;
                burstTimer = 1f;
                volleysLeft = 0;
                b.SetColor(Color.white);
            }

            public override void Tick(float deltaTime)
            {
                if (b.MoveToX(b.LaneX(targetLane), b.strafeSpeed, deltaTime))
                {
                    targetLane = targetLane == 0 ? 2 : 0;
                }

                burstTimer -= deltaTime;
                if (volleysLeft == 0 && burstTimer <= 0f)
                {
                    volleysLeft = b.burstVolleys;
                    volleyTimer = 0f;
                    burstTimer = b.burstInterval;
                }

                if (volleysLeft > 0)
                {
                    volleyTimer -= deltaTime;
                    if (volleyTimer <= 0f)
                    {
                        b.FireAimedV(b.vEggsPerSide);
                        volleysLeft--;
                        volleyTimer = b.volleyGap;
                    }
                }
            }
        }

        private sealed class OverdriveState : BossState
        {
            private readonly MechaChickenBoss b;
            private int targetLane;
            private float timer;
            private bool aimed;

            public OverdriveState(MechaChickenBoss boss) : base(boss) { b = boss; }

            public override void Enter()
            {
                targetLane = b.transform.position.x >= b.rest.x ? 0 : 2;
                timer = 0.5f;
            }

            public override void Tick(float deltaTime)
            {
                b.SetColor(Color.Lerp(Color.white, BossPatterns.Hex("#FF5252"), Mathf.PingPong(Time.time * 4f, 1f)));

                if (b.MoveToX(b.LaneX(targetLane), b.overdriveSpeed, deltaTime))
                {
                    targetLane = targetLane == 0 ? 2 : 0;
                }

                timer -= deltaTime;
                if (timer > 0f)
                {
                    return;
                }

                timer = b.overdriveInterval;
                aimed = !aimed;
                if (aimed)
                {
                    b.FireAimedV(b.overdriveEggsPerSide);
                }
                else
                {
                    b.FireStraightV(b.overdriveEggsPerSide);
                }
            }
        }
    }
}