using UnityEngine;

namespace ChickenOmelette.Core
{
    public class DebugBoss : BossBase
    {
        protected override BossState[] CreatePhaseStates()
        {
            return new BossState[]
            {
                new DebugState(this, "Phase 1", "#FFFFFF"),
                new DebugState(this, "Phase 2", "#FFD54F"),
                new DebugState(this, "Phase 3 (Enraged)", "#E53935")
            };
        }

        private class DebugState : BossState
        {
            private readonly string label;
            private readonly SpriteRenderer sprite;
            private readonly Color color = Color.white;
            private float timer;

            public DebugState(BossBase boss, string label, string hex) : base(boss)
            {
                this.label = label;
                sprite = boss.GetComponentInChildren<SpriteRenderer>();
                ColorUtility.TryParseHtmlString(hex, out color);
            }

            public override void Enter()
            {
                Debug.Log($"Boss ENTER {label}");
                timer = 0f;
                if (sprite != null)
                {
                    sprite.color = color;
                }
            }

            public override void Tick(float deltaTime)
            {
                timer += deltaTime;
                if (timer >= 2f)
                {
                    timer = 0f;
                    Debug.Log($"Boss TICK {label} (health {Boss.HealthFraction:P0})");
                }
            }

            public override void Exit()
            {
                Debug.Log($"Boss EXIT {label}");
            }
        }
    }
}