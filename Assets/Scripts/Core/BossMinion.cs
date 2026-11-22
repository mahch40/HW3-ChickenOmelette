using System.Collections.Generic;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class BossMinion : MonoBehaviour, IPoolable
    {
        private static readonly List<BossMinion> Active = new List<BossMinion>();

        [SerializeField, Min(0f)] private float swayAmplitude = 1.5f;
        [SerializeField, Min(0.1f)] private float swayFrequency = 2f;
        [SerializeField, Min(0f)] private float bottomMargin = 1.5f;

        private float fallSpeed = 3f;
        private float startX;
        private float time;
        private float phaseOffset;

        public static int ActiveCount
        {
            get
            {
                Active.RemoveAll(m => m == null);
                return Active.Count;
            }
        }

        public static void DespawnAll()
        {
            BossMinion[] copy = Active.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (copy[i] != null)
                {
                    PoolManager.Despawn(copy[i].gameObject);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Active.Clear();
        }

        public void Launch(float speed)
        {
            fallSpeed = speed;
            startX = transform.position.x;
            phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        public void OnSpawn()
        {
            time = 0f;
            Active.Add(this);
        }

        public void OnDespawn()
        {
            Active.Remove(this);
        }

        private void Update()
        {
            time += Time.deltaTime;
            Vector3 p = transform.position;
            p.x = startX + Mathf.Sin(time * swayFrequency + phaseOffset) * swayAmplitude;
            p.y -= fallSpeed * Time.deltaTime;
            transform.position = p;

            if (p.y < ScreenBounds.Bottom - bottomMargin)
            {
                PoolManager.Despawn(gameObject);
            }
        }
    }
}