using System.Collections;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxLives = 3;
        [SerializeField, Min(0.1f)] private float invincibleTime = 1.5f;
        [SerializeField, Min(0.02f)] private float blinkInterval = 0.1f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private int lives;
        private Coroutine invincibility;
        private WaitForSeconds blinkWait;

        public int Lives => lives;
        public int MaxLives => maxLives;
        public bool IsInvincible { get; private set; }

        private void Awake()
        {
            lives = maxLives;
            blinkWait = new WaitForSeconds(blinkInterval);
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void Start()
        {
            GameEvents.RaiseLivesChanged(lives, maxLives);
        }

        private void OnDisable()
        {
            invincibility = null;
            IsInvincible = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleContact(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            HandleContact(other);
        }

        private void HandleContact(Collider2D other)
        {
            if (IsInvincible || lives <= 0)
            {
                return;
            }

            Hazard hazard = other.GetComponentInParent<Hazard>();
            if (hazard == null)
            {
                return;
            }

            TakeDamage(hazard.Damage);
            hazard.OnHitPlayer();
        }

        public void TakeDamage(int amount = 1)
        {
            if (IsInvincible || lives <= 0)
            {
                return;
            }

            lives = Mathf.Max(0, lives - amount);
            GameEvents.RaiseLivesChanged(lives, maxLives);
            GameEvents.RaisePlayerDamaged();

            if (lives == 0)
            {
                GameEvents.RaisePlayerDied();
                gameObject.SetActive(false);
                return;
            }

            invincibility = StartCoroutine(InvincibilityRoutine());
        }

        public void SetLives(int value)
        {
            lives = Mathf.Clamp(value, 1, maxLives);
            GameEvents.RaiseLivesChanged(lives, maxLives);
        }

        private IEnumerator InvincibilityRoutine()
        {
            IsInvincible = true;
            float elapsed = 0f;

            while (elapsed < invincibleTime)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return blinkWait;
                elapsed += blinkInterval;
            }

            spriteRenderer.enabled = true;
            IsInvincible = false;
            invincibility = null;
        }
    }
}