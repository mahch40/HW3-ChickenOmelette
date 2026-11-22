using UnityEngine;

namespace ChickenOmelette.Core
{
    public static class BossPatterns
    {
        private static ShipController ship;

        public static Color Hex(string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out Color color) ? color : Color.white;
        }

        public static Vector3 PlayerPosition(Vector3 fallback)
        {
            if (ship == null)
            {
                ship = Object.FindFirstObjectByType<ShipController>();
            }

            return ship != null ? ship.transform.position : fallback;
        }

        public static void FireEgg(GameObject eggPrefab, Vector3 position, Vector2 direction, float speed)
        {
            GameObject egg = PoolManager.Spawn(eggPrefab, position, Quaternion.identity);
            if (egg.TryGetComponent(out Egg component))
            {
                component.Launch(direction, speed);
            }
        }

        public static void FireV(GameObject eggPrefab, Vector3 tip, Vector2 direction,
                                 int eggsPerSide, float spacing, float depth, float speed)
        {
            if (eggPrefab == null)
            {
                return;
            }

            direction.Normalize();
            Vector2 side = new Vector2(-direction.y, direction.x);

            for (int k = -eggsPerSide; k <= eggsPerSide; k++)
            {
                Vector2 offset = side * (k * spacing) - direction * (Mathf.Abs(k) * depth);
                FireEgg(eggPrefab, tip + (Vector3)offset, direction, speed);
            }
        }
    }
}