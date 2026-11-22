using UnityEngine;

namespace ChickenOmelette.Core
{
    public class Shockwave : MonoBehaviour, IPoolable
    {
        [SerializeField, Min(0.05f)] private float duration = 0.6f;
        [SerializeField, Min(0f)] private float startScale = 0.1f;
        [SerializeField, Min(0.1f)] private float endScale = 4f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private float elapsed;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void OnSpawn()
        {
            elapsed = 0f;
            Apply(0f);
        }

        public void OnDespawn()
        {
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Apply(t);

            if (t >= 1f)
            {
                PoolManager.Despawn(gameObject);
            }
        }

        private void Apply(float t)
        {
            float eased = 1f - (1f - t) * (1f - t);
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, eased);

            Color color = spriteRenderer.color;
            color.a = 1f - t;
            spriteRenderer.color = color;
        }
    }
}