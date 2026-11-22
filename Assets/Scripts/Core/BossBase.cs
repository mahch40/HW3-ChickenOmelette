using System.Collections;
using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class BossBase : MonoBehaviour, IDamageable, IBombable
    {
        [SerializeField] private BossDataSO data;

        [Header("Used only when no BossDataSO is assigned")]
        [SerializeField] private string fallbackName = "BOSS";
        [SerializeField, Min(1)] private int fallbackHealth = 60;
        [SerializeField, Min(0)] private int fallbackScore = 1000;

        [Header("Entrance")]
        [Tooltip("Resting height as a fraction of the screen height (0 = bottom, 1 = top).")]
        [SerializeField, Range(0.4f, 0.95f)] private float restHeight = 0.78f;
        [SerializeField, Min(0.1f)] private float entranceSeconds = 2.5f;

        public string BossName { get; private set; }
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public int ScoreValue { get; private set; }
        public float HealthFraction => MaxHealth <= 0 ? 0f : (float)Health / MaxHealth;
        public bool IsVulnerable { get; private set; }
        public bool IsDefeated { get; private set; }

        protected virtual void Awake()
        {
            BossName = data != null ? data.BossName : fallbackName;
            MaxHealth = data != null ? data.MaxHealth : fallbackHealth;
            ScoreValue = data != null ? data.ScoreValue : fallbackScore;
            Health = MaxHealth;

            Rect screen = ScreenBounds.Rect;
            transform.position = new Vector3(screen.center.x, screen.yMax + 3f, 0f);
        }

        protected virtual void Start()
        {
            GameEvents.RaiseBossAppeared(BossName, MaxHealth);
            GameEvents.RaiseBossHealthChanged(Health, MaxHealth);
            StartCoroutine(Entrance());
        }

        private IEnumerator Entrance()
        {
            Rect screen = ScreenBounds.Rect;
            Vector3 from = transform.position;
            Vector3 to = new Vector3(screen.center.x, screen.yMin + screen.height * restHeight, 0f);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / entranceSeconds;
                transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            transform.position = to;
            IsVulnerable = true;
            OnEntranceFinished();
        }

        public void TakeDamage(int amount)
        {
            ApplyDamage(amount);
        }

        public void OnBombed(float bossDamageFraction)
        {
            ApplyDamage(Mathf.Max(1, Mathf.CeilToInt(MaxHealth * bossDamageFraction)));
        }

        private void ApplyDamage(int amount)
        {
            if (!IsVulnerable || IsDefeated)
            {
                return;
            }

            Health = Mathf.Max(0, Health - amount);
            GameEvents.RaiseBossHealthChanged(Health, MaxHealth);
            OnDamaged(amount);

            if (Health <= 0)
            {
                IsDefeated = true;
                IsVulnerable = false;
                GameEvents.RaiseEnemyKilled(transform.position, ScoreValue);
                GameEvents.RaiseBossDefeated();
                OnDefeated();
            }
        }

        protected virtual void OnEntranceFinished() { }
        protected virtual void OnDamaged(int amount) { }

        protected virtual void OnDefeated()
        {
            Destroy(gameObject);
        }
    }
}