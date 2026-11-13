using UnityEngine;

namespace ChickenOmelette.Data
{
    public enum WaveType
    {
        Chickens,
        Asteroids,
        Boss
    }

    public enum FormationType
    {
        Grid,
        VShape,
        Circle,
        Line
    }

    [CreateAssetMenu(fileName = "Wave_New", menuName = "Chicken Omelette/Wave Data")]
    public class WaveDataSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string waveName = "New Wave";
        [SerializeField] private WaveType waveType = WaveType.Chickens;

        [Header("Enemy")]
        [Tooltip("Chicken or asteroid prefab for this wave (a boss prefab for boss waves).")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField, Min(1)] private int count = 15;
        [Tooltip("Hits each enemy can take. 0 uses the health set on the prefab.")]
        [SerializeField, Min(0)] private int enemyHealth;

        [Header("Movement")]
        [Tooltip("Units per second along the entry path (chickens) or falling speed (asteroids).")]
        [SerializeField, Min(0.1f)] private float moveSpeed = 4f;
        [Tooltip("Seconds between one enemy and the next entering or falling.")]
        [SerializeField, Min(0.01f)] private float spawnInterval = 0.25f;
        [Tooltip("Entry path in viewport coordinates: (0,0) is bottom-left, (1,1) is top-right, and y above 1 is off-screen at the top.")]
        [SerializeField]
        private Vector2[] entryWaypoints =
        {
            new Vector2(0.5f, 1.2f),
            new Vector2(0.15f, 0.9f),
            new Vector2(0.85f, 0.75f)
        };

        [Header("Formation (chickens)")]
        [SerializeField] private FormationType formation = FormationType.Grid;
        [SerializeField, Min(1)] private int columns = 5;
        [Tooltip("Distance between neighbours in world units.")]
        [SerializeField] private Vector2 spacing = new Vector2(1.7f, 1.4f);
        [Tooltip("Formation centre in viewport coordinates.")]
        [SerializeField] private Vector2 formationCenter = new Vector2(0.5f, 0.72f);
        [SerializeField, Min(0f)] private float swayAmplitude = 1.5f;
        [SerializeField, Min(0f)] private float swaySpeed = 1f;

        [Header("Attacks (chickens)")]
        [Tooltip("Average number of eggs dropped per second by the whole wave.")]
        [SerializeField, Min(0f)] private float eggsPerSecond = 0.8f;
        [SerializeField, Min(0.1f)] private float eggFallSpeed = 5f;
        [Tooltip("Seconds between dive-bomb attacks. 0 turns them off.")]
        [SerializeField, Min(0f)] private float diveInterval;
        [SerializeField, Min(0.1f)] private float diveSpeed = 9f;

        public string WaveName => waveName;
        public WaveType Type => waveType;

        public GameObject EnemyPrefab => enemyPrefab;
        public int Count => count;
        public int EnemyHealth => enemyHealth;

        public float MoveSpeed => moveSpeed;
        public float SpawnInterval => spawnInterval;
        public Vector2[] EntryWaypoints => entryWaypoints;

        public FormationType Formation => formation;
        public int Columns => columns;
        public Vector2 Spacing => spacing;
        public Vector2 FormationCenter => formationCenter;
        public float SwayAmplitude => swayAmplitude;
        public float SwaySpeed => swaySpeed;

        public float EggsPerSecond => eggsPerSecond;
        public float EggFallSpeed => eggFallSpeed;
        public float DiveInterval => diveInterval;
        public float DiveSpeed => diveSpeed;

        [ContextMenu("Log Summary")]
        private void LogSummary()
        {
            Debug.Log($"{waveName}: {waveType}, {count} enemies, formation {formation}, " +
                      $"speed {moveSpeed}, eggs/s {eggsPerSecond}, dive every {diveInterval}s, " +
                      $"{(entryWaypoints == null ? 0 : entryWaypoints.Length)} waypoints");
        }
    }
}