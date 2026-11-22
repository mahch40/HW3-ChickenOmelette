namespace ChickenOmelette.Core
{
    public class BossStateMachine
    {
        public BossState Current { get; private set; }

        public void ChangeState(BossState next)
        {
            if (next == Current)
            {
                return;
            }

            Current?.Exit();
            Current = next;
            Current?.Enter();
        }

        public void Tick(float deltaTime)
        {
            Current?.Tick(deltaTime);
        }

        public void Stop()
        {
            ChangeState(null);
        }
    }
}