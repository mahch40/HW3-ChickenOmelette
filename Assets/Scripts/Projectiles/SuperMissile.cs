using System.Collections.Generic;
using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SuperMissile : MonoBehaviour, IPoolable
    {
        private static readonly HashSet<IBombable> BombedThisBlast = new HashSet<IBombable>();

        [SerializeField, Min(1f)] private float speed = 16f;
        [Tooltip("Detonation height as a fraction of the screen height (0 = bottom, 1 = top).")]
        [SerializeField, Range(0.1f, 1f)] private float detonateHeight = 0.65f;
        [SerializeField] private GameObject shockwavePrefab;

        private Rigidbody2D body;
        private float bossDamageFraction;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void Launch(float bossFraction)
        {
            bossDamageFraction = bossFraction;
            body.linearVelocity = (Vector2)transform.up * speed;
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            body.linearVelocity = Vector2.zero;
        }

        private void Update()
        {
            Rect screen = ScreenBounds.Rect;
            if (transform.position.y >= screen.yMin + screen.height * detonateHeight)
            {
                Detonate(screen);
            }
        }

        private void Detonate(Rect screen)
        {
            if (shockwavePrefab != null)
            {
                PoolManager.Spawn(shockwavePrefab, transform.position, Quaternion.identity);
            }

            BombedThisBlast.Clear();
            Collider2D[] hits = Physics2D.OverlapBoxAll(screen.center, screen.size, 0f);
            for (int i = 0; i < hits.Length; i++)
            {
                IBombable target = hits[i].GetComponentInParent<IBombable>();
                if (target != null && BombedThisBlast.Add(target))
                {
                    target.OnBombed(bossDamageFraction);
                }
            }

            PoolManager.Despawn(gameObject);
        }
    }
}