using UnityEngine;

namespace ChickenOmelette.Core
{
    public class PickupDropper : MonoBehaviour
    {
        [SerializeField] private GameObject giftBoxPrefab;
        [SerializeField, Range(0f, 1f)] private float giftChance = 0.08f;
        [SerializeField, Min(1)] private int prewarmCount = 5;

        [Header("Food")]
        [Tooltip("Drumstick, ham and so on. One is picked at random.")]
        [SerializeField] private GameObject[] foodPrefabs;
        [SerializeField, Range(0f, 1f)] private float foodChance = 0.35f;

        private void OnEnable()
        {
            GameEvents.EnemyKilled += OnEnemyKilled;
        }

        private void OnDisable()
        {
            GameEvents.EnemyKilled -= OnEnemyKilled;
        }

        private void Start()
        {
            if (giftBoxPrefab != null)
            {
                PoolManager.Prewarm(giftBoxPrefab, prewarmCount);
            }

            if (foodPrefabs != null)
            {
                for (int i = 0; i < foodPrefabs.Length; i++)
                {
                    if (foodPrefabs[i] != null)
                    {
                        PoolManager.Prewarm(foodPrefabs[i], prewarmCount);
                    }
                }
            }
        }

        private void OnEnemyKilled(Vector3 position, int scoreValue)
        {
            if (giftBoxPrefab != null && Random.value < giftChance)
            {
                PoolManager.Spawn(giftBoxPrefab, position, Quaternion.identity);
            }

            if (foodPrefabs != null && foodPrefabs.Length > 0 && Random.value < foodChance)
            {
                GameObject food = foodPrefabs[Random.Range(0, foodPrefabs.Length)];
                if (food != null)
                {
                    Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), 0f, 0f);
                    PoolManager.Spawn(food, position + offset, Quaternion.identity);
                }
            }
        }

        [ContextMenu("Drop Test Gift Box")]
        private void DropTestGiftBox()
        {
            Rect r = ScreenBounds.Rect;
            Vector3 position = new Vector3(Random.Range(r.xMin + 1f, r.xMax - 1f), r.yMax - 1f, 0f);
            PoolManager.Spawn(giftBoxPrefab, position, Quaternion.identity);
        }
    }
}