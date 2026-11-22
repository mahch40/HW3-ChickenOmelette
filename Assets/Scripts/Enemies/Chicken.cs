using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Hazard))]
    public class Chicken : MonoBehaviour, IDamageable, IBombable, IPoolable
    {
        [SerializeField, Min(1)] private int maxHealth = 3;
        [SerializeField, Min(0)] private int scoreValue = 10;

        private int health;

        public int PrefabHealth => maxHealth;
        public int Health => health;
        public bool IsAlive => health > 0;

        protected virtual void Awake()
        {
            health = maxHealth;
        }

        public virtual void OnSpawn()
        {
            health = maxHealth;
        }

        public virtual void OnDespawn()
        {
        }

        public void Configure(int startingHealth)
        {
            health = Mathf.Max(1, startingHealth);
        }

        public virtual void TakeDamage(int amount)
        {
            if (!IsAlive)
            {
                return;
            }

            health -= amount;
            if (health <= 0)
            {
                Die();
            }
        }

        public virtual void OnBombed(float bossDamageFraction)
        {
            if (!IsAlive)
            {
                return;
            }

            health = 0;
            Die();
        }

        protected virtual void Die()
        {
            GameEvents.RaiseEnemyKilled(transform.position, scoreValue);
            PoolManager.Despawn(gameObject);
        }
    }
}