using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(InputReader))]
    public class MissileLauncher : MonoBehaviour
    {
        [SerializeField] private SuperMissile missilePrefab;
        [SerializeField, Min(0)] private int startMissiles = 2;
        [Tooltip("Fraction of a boss's maximum health removed by one blast.")]
        [SerializeField, Range(0.15f, 0.20f)] private float bossDamageFraction = 0.18f;
        [SerializeField, Min(0f)] private float cooldownSeconds = 0.5f;
        [SerializeField] private Vector2 muzzleOffset = new Vector2(0f, 0.6f);

        private InputReader input;
        private float nextLaunchTime;

        public int Count { get; private set; }

        private void Awake()
        {
            input = GetComponent<InputReader>();
            Count = startMissiles;
        }

        private void OnEnable()
        {
            input.BombPerformed += TryLaunch;
        }

        private void OnDisable()
        {
            input.BombPerformed -= TryLaunch;
        }

        private void Start()
        {
            PoolManager.Prewarm(missilePrefab.gameObject, 3);
            GameEvents.RaiseMissileCountChanged(Count);
        }

        public void AddMissile(int amount = 1)
        {
            SetCount(Count + amount);
        }

        public void SetCount(int count)
        {
            Count = Mathf.Max(0, count);
            GameEvents.RaiseMissileCountChanged(Count);
        }

        private void TryLaunch()
        {
            if (Count <= 0 || Time.time < nextLaunchTime)
            {
                return;
            }

            nextLaunchTime = Time.time + cooldownSeconds;
            SetCount(Count - 1);

            SuperMissile missile = PoolManager.Spawn(missilePrefab, transform.position + (Vector3)muzzleOffset, Quaternion.identity);
            missile.Launch(bossDamageFraction);
        }
    }
}