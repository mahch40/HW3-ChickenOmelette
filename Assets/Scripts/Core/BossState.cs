namespace ChickenOmelette.Core
{
    public abstract class BossState
    {
        protected readonly BossBase Boss;

        protected BossState(BossBase boss)
        {
            Boss = boss;
        }

        public virtual void Enter() { }
        public virtual void Tick(float deltaTime) { }
        public virtual void Exit() { }
    }
}