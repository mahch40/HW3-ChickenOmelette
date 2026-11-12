using UnityEngine;

namespace ChickenOmelette.Core
{
    public static class ScreenBounds
    {
        private static Camera cachedCamera;

        private static Camera Cam
        {
            get
            {
                if (cachedCamera == null)
                {
                    cachedCamera = Camera.main;
                }
                return cachedCamera;
            }
        }

        public static Rect Rect
        {
            get
            {
                Camera cam = Cam;
                float halfHeight = cam.orthographicSize;
                float halfWidth = halfHeight * cam.aspect;
                Vector3 center = cam.transform.position;
                return new Rect(center.x - halfWidth, center.y - halfHeight, halfWidth * 2f, halfHeight * 2f);
            }
        }

        public static float Left => Rect.xMin;
        public static float Right => Rect.xMax;
        public static float Bottom => Rect.yMin;
        public static float Top => Rect.yMax;

        public static Vector2 Clamp(Vector2 position, Vector2 padding)
        {
            Rect r = Rect;
            position.x = Mathf.Clamp(position.x, r.xMin + padding.x, r.xMax - padding.x);
            position.y = Mathf.Clamp(position.y, r.yMin + padding.y, r.yMax - padding.y);
            return position;
        }

        public static bool IsOutside(Vector2 position, float margin = 0f)
        {
            Rect r = Rect;
            return position.x < r.xMin - margin || position.x > r.xMax + margin
                || position.y < r.yMin - margin || position.y > r.yMax + margin;
        }
    }
}