using System;
using UnityEngine;

namespace ChickenOmelette.Data
{
    [Serializable]
    public struct ShotSpawn
    {
        [Tooltip("Spawn position relative to the ship, in world units.")]
        public Vector2 offset;

        [Tooltip("Direction in degrees. 0 = straight up, positive = tilted to the left.")]
        public float angleDegrees;
    }

    [Serializable]
    public class PowerLevelPattern
    {
        public ShotSpawn[] shots;
    }

    [CreateAssetMenu(fileName = "Weapon_New", menuName = "Chicken Omelette/Weapon Data")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string weaponName = "New Weapon";
        [SerializeField] private Sprite sprite;

        [Header("Projectile")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField, Min(1)] private int damage = 1;
        [SerializeField, Min(0.1f)] private float bulletSpeed = 14f;

        [Header("Fire")]
        [SerializeField, Min(0.1f)] private float fireRate = 6f; 

        [Header("Pattern per power level (element 0 = power level 1)")]
        [SerializeField]
        private PowerLevelPattern[] patterns =
        {
            // Level 1: a single shot
            new PowerLevelPattern
            {
                shots = new[] { new ShotSpawn { offset = new Vector2(0f, 0f), angleDegrees = 0f } }
            },
            // Level 2: two parallel shots
            new PowerLevelPattern
            {
                shots = new[]
                {
                    new ShotSpawn { offset = new Vector2(-0.2f, 0f), angleDegrees = 0f },
                    new ShotSpawn { offset = new Vector2(0.2f, 0f), angleDegrees = 0f }
                }
            },
            // Level 3: three-way spread
            new PowerLevelPattern
            {
                shots = new[]
                {
                    new ShotSpawn { offset = new Vector2(-0.2f, 0f), angleDegrees = 15f },
                    new ShotSpawn { offset = new Vector2(0f, 0.1f), angleDegrees = 0f },
                    new ShotSpawn { offset = new Vector2(0.2f, 0f), angleDegrees = -15f }
                }
            }
        };

        public string WeaponName => weaponName;
        public Sprite Sprite => sprite;
        public GameObject BulletPrefab => bulletPrefab;
        public int Damage => damage;
        public float BulletSpeed => bulletSpeed;
        public float FireRate => fireRate;
        public float FireInterval => 1f / fireRate;
        public int MaxPowerLevel => patterns.Length;

        public ShotSpawn[] GetShots(int powerLevel)
        {
            if (patterns == null || patterns.Length == 0)
            {
                return Array.Empty<ShotSpawn>();
            }

            int index = Mathf.Clamp(powerLevel, 1, patterns.Length) - 1;
            return patterns[index].shots ?? Array.Empty<ShotSpawn>();
        }

        [ContextMenu("Log Patterns")]
        private void LogPatterns()
        {
            for (int level = 0; level <= MaxPowerLevel + 1; level++)
            {
                Debug.Log($"{weaponName}: power level {level} -> {GetShots(level).Length} shot(s)");
            }
        }
    }
}