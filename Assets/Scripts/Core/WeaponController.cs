using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(InputReader), typeof(PlayerPower))]
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponDataSO weapon;
        [Tooltip("Where bullets leave the ship, relative to its centre.")]
        [SerializeField] private Vector2 muzzleOffset = new Vector2(0f, 0.6f);
        [SerializeField, Min(1)] private int prewarmCount = 30;
        [Tooltip("Logs the measured volleys per second each time the fire button is released.")]
        [SerializeField] private bool logFireRate;

        private InputReader input;
        private PlayerPower power;
        private Bullet bulletPrefab;
        private float cooldown;
        private bool wasHeld;
        private float holdStart;
        private int volleysThisHold;

        private void Awake()
        {
            input = GetComponent<InputReader>();
            power = GetComponent<PlayerPower>();

            if (weapon == null || weapon.BulletPrefab == null)
            {
                Debug.LogError("WeaponController needs a weapon with a bullet prefab.", this);
                enabled = false;
                return;
            }

            bulletPrefab = weapon.BulletPrefab.GetComponent<Bullet>();
        }

        private void Start()
        {
            PoolManager.Prewarm(weapon.BulletPrefab, prewarmCount);
        }

        private void Update()
        {
            bool held = input.FireHeld;
            float interval = weapon.FireInterval;

            cooldown = Mathf.Max(cooldown - Time.deltaTime, -interval);
            if (!held && cooldown < 0f)
            {
                cooldown = 0f;
            }

            if (held && !wasHeld)
            {
                holdStart = Time.time;
                volleysThisHold = 0;
            }

            if (held && cooldown <= 0f)
            {
                Fire();
                cooldown += interval;
                volleysThisHold++;
            }

            if (!held && wasHeld && logFireRate)
            {
                float seconds = Time.time - holdStart;
                if (seconds > 0.2f)
                {
                    Debug.Log($"Fire rate: {volleysThisHold / seconds:F2} volleys/s over {seconds:F1}s (weapon: {weapon.FireRate})");
                }
            }

            wasHeld = held;
        }

        private void Fire()
        {
            ShotSpawn[] shots = weapon.GetShots(power.Level);
            Vector3 origin = transform.position + (Vector3)muzzleOffset;

            for (int i = 0; i < shots.Length; i++)
            {
                Quaternion rotation = Quaternion.Euler(0f, 0f, shots[i].angleDegrees);
                Bullet bullet = PoolManager.Spawn(bulletPrefab, origin + (Vector3)shots[i].offset, rotation);
                bullet.Launch(weapon.BulletSpeed, weapon.Damage);
            }
        }
    }
}