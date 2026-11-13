using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class FallingPickup : MonoBehaviour, IPoolable
    {
        [SerializeField, Min(0.1f)] private float fallSpeed = 3f;
        [SerializeField, Min(0f)] private float bottomMargin = 1f;

        private Rigidbody2D body;

        protected virtual void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public virtual void OnSpawn()
        {
            body.linearVelocity = Vector2.down * fallSpeed;
        }

        public virtual void OnDespawn()
        {
            body.linearVelocity = Vector2.zero;
        }

        private void Update()
        {
            if (transform.position.y < ScreenBounds.Bottom - bottomMargin)
            {
                PoolManager.Despawn(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            OnCollected(other.gameObject);
            PoolManager.Despawn(gameObject);
        }

        protected abstract void OnCollected(GameObject player);
    }
}