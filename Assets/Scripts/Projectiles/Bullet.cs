using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour, IPoolable
    {
        [SerializeField, Min(0f)] private float offscreenMargin = 0.5f;

        private Rigidbody2D body;
        private int damage = 1;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void Launch(float speed, int damageAmount)
        {
            damage = damageAmount;
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
            if (ScreenBounds.IsOutside(transform.position, offscreenMargin))
            {
                PoolManager.Despawn(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null)
            {
                return;
            }

            target.TakeDamage(damage);
            PoolManager.Despawn(gameObject);
        }
    }
}