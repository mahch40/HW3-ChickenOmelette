namespace ChickenOmelette.Core
{
    public interface IPoolable
    {
        void OnSpawn();

        void OnDespawn();
    }
}