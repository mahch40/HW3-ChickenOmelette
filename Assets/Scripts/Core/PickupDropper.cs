using UnityEngine;

namespace ChickenOmelette.Core
{
    public class PickupDropper : MonoBehaviour
    {
        [SerializeField] private GameObject giftBoxPrefab;
        [SerializeField, Range(0f, 1f)] private float giftChance = 0.08f;
        [SerializeField, Min(1)] private int prewarmCount = 5;

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
        }

        private void OnEnemyKilled(Vector3 position, int scoreValue)
        {
            if (giftBoxPrefab != null && Random.value < giftChance)
            {
                PoolManager.Spawn(giftBoxPrefab, position, Quaternion.identity);
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