using System;
using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Asteroid : Hazard, IDamageable, IBombable, IPoolable
    {
        [SerializeField, Min(1)] private int maxHealth = 3;
        [SerializeField, Min(0)] private int scoreValue = 5;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Tooltip("Colour the asteroid fades toward as it takes damage.")]
        [SerializeField] private Color damagedTint = new Color(1f, 0.54f, 0.4f);
        [SerializeField] private Vector2 sizeRange = new Vector2(0.8f, 1.3f);
        [Tooltip("Optional: one of these sprites is picked at random for every asteroid.")]
        [SerializeField] private Sprite[] spriteVariants;
        [SerializeField, Min(0f)] private float offscreenMargin = 2f;

        private Rigidbody2D body;
        private Vector3 baseScale;
        private int health;
        private int startHealth;
        private bool released;
        private Action<Asteroid> onRemoved;

        public int PrefabHealth => maxHealth;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            baseScale = transform.localScale;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        public void Configure(int hitPoints, float speed, float drift, float spin, Action<Asteroid> removedCallback)
        {
            health = Mathf.Max(1, hitPoints);
            startHealth = health;
            onRemoved = removedCallback;

            body.linearVelocity = new Vector2(drift, -speed);
            body.angularVelocity = spin;

            transform.localScale = baseScale * UnityEngine.Random.Range(sizeRange.x, sizeRange.y);
            transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
            if (spriteVariants != null && spriteVariants.Length > 0 && spriteRenderer != null)
            {
                spriteRenderer.sprite = spriteVariants[UnityEngine.Random.Range(0, spriteVariants.Length)];
            }
            UpdateTint();
        }

        public void TakeDamage(int amount)
        {
            if (released)
            {
                return;
            }

            health -= amount;
            if (health <= 0)
            {
                Release(true);
            }
            else
            {
                UpdateTint();
            }
        }

        public override void OnHitPlayer()
        {
            Release(false);
        }
        public void OnBombed(float bossDamageFraction)
        {
            Release(true);
        }

        public void OnSpawn()
        {
            released = false;
            health = maxHealth;
            startHealth = maxHealth;
            UpdateTint();
        }

        public void OnDespawn()
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        private void Update()
        {
            if (ScreenBounds.IsOutside(transform.position, offscreenMargin))
            {
                Release(false);
            }
        }

        private void UpdateTint()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            float t = startHealth > 0 ? Mathf.Clamp01(health / (float)startHealth) : 1f;
            spriteRenderer.color = Color.Lerp(damagedTint, Color.white, t);
        }

        private void Release(bool destroyed)
        {
            if (released)
            {
                return;
            }

            released = true;

            if (destroyed)
            {
                GameEvents.RaisePointsAwarded(scoreValue);
            }

            Action<Asteroid> callback = onRemoved;
            onRemoved = null;

            PoolManager.Despawn(gameObject);
            callback?.Invoke(this);
        }
    }
}