using UnityEngine;

namespace ChickenOmelette.Core
{

    [DisallowMultipleComponent]
    public class PooledObject : MonoBehaviour
    {
        private ObjectPool pool;
        private IPoolable[] poolables;
        private bool hasBeenSpawned;

        public bool InPool { get; private set; }

        internal void Init(ObjectPool owner)
        {
            pool = owner;
            poolables = GetComponentsInChildren<IPoolable>(true);
            InPool = false;
        }

        internal void MarkInPool()
        {
            InPool = true;
        }

        internal void NotifySpawned()
        {
            InPool = false;
            hasBeenSpawned = true;
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnSpawn();
            }
        }

        private void OnDisable()
        {
            if (pool == null || InPool || !pool.IsAlive)
            {
                return;
            }

            InPool = true;
            if (hasBeenSpawned)
            {
                for (int i = 0; i < poolables.Length; i++)
                {
                    poolables[i].OnDespawn();
                }
            }

            pool.Push(this);
        }
    }
}