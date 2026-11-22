using System.Collections.Generic;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager instance;

        private readonly Dictionary<GameObject, ObjectPool> pools = new Dictionary<GameObject, ObjectPool>();

        private static PoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<PoolManager>();
                    if (instance == null)
                    {
                        instance = new GameObject("PoolManager").AddComponent<PoolManager>();
                    }
                }
                return instance;
            }
        }

        public static void Prewarm(GameObject prefab, int count)
        {
            Instance.GetPool(prefab).EnsureCapacity(count);
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Instance.GetPool(prefab).Get(position, rotation);
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, position, rotation).GetComponent<T>();
        }

        public static void Despawn(GameObject spawned)
        {
            spawned.SetActive(false);
        }

        public static bool TryGetPool(GameObject prefab, out ObjectPool pool)
        {
            return Instance.pools.TryGetValue(prefab, out pool);
        }

        private ObjectPool GetPool(GameObject prefab)
        {
            if (!pools.TryGetValue(prefab, out ObjectPool pool))
            {
                pool = new ObjectPool(prefab, transform);
                pools.Add(prefab, pool);
            }
            return pool;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}