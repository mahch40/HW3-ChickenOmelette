using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Egg : Hazard, IDamageable, IBombable, IPoolable
    {
        [SerializeField, Min(0f)] private float offscreenMargin = 0.5f;

        private Rigidbody2D body;
        private bool released;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void Launch(float speed)
        {
            body.linearVelocity = Vector2.down * speed;
        }

        public void TakeDamage(int amount)
        {
            Release();
        }

        public void OnBombed(float bossDamageFraction)
        {
            Release();
        }

        public override void OnHitPlayer()
        {
            Release();
        }

        public void OnSpawn()
        {
            released = false;
        }

        public void OnDespawn()
        {
            body.linearVelocity = Vector2.zero;
        }

        private void Update()
        {
            if (ScreenBounds.IsOutside(transform.position, offscreenMargin))
            {
                Release();
            }
        }

        private void Release()
        {
            if (released)
            {
                return;
            }

            released = true;
            PoolManager.Despawn(gameObject);
        }
    }
}