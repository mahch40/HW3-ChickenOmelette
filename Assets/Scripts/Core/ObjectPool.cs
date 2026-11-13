using System.Collections.Generic;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class ObjectPool
    {
        private readonly GameObject prefab;
        private readonly Transform root;
        private readonly Stack<PooledObject> available = new Stack<PooledObject>();

        public int CountAll { get; private set; }
        public int CountAvailable => available.Count;
        public bool IsAlive => root != null;

        public ObjectPool(GameObject prefab, Transform parent)
        {
            this.prefab = prefab;
            GameObject rootObject = new GameObject("Pool_" + prefab.name);
            rootObject.transform.SetParent(parent, false);
            root = rootObject.transform;
        }

        public void EnsureCapacity(int count)
        {
            while (CountAll < count)
            {
                Create();
            }
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            PooledObject pooled = PopAvailable();
            if (pooled == null)
            {
                Create();
                pooled = PopAvailable();
            }

            pooled.transform.SetPositionAndRotation(position, rotation);
            pooled.gameObject.SetActive(true);
            pooled.NotifySpawned();
            return pooled.gameObject;
        }

        internal void Push(PooledObject pooled)
        {
            available.Push(pooled);
        }

        private PooledObject PopAvailable()
        {
            while (available.Count > 0)
            {
                PooledObject pooled = available.Pop();
                if (pooled != null)
                {
                    return pooled;
                }
            }
            return null;
        }

        private void Create()
        {
            GameObject instance = Object.Instantiate(prefab, root);
            instance.name = prefab.name;

            PooledObject pooled = instance.GetComponent<PooledObject>();
            if (pooled == null)
            {
                pooled = instance.AddComponent<PooledObject>();
            }

            pooled.Init(this);
            CountAll++;

            instance.SetActive(false);
            if (!pooled.InPool)
            {
                pooled.MarkInPool();
                available.Push(pooled);
            }
        }
    }
}