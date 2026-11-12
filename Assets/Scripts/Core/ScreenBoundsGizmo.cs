using UnityEngine;

namespace ChickenOmelette.Core
{
    public class ScreenBoundsGizmo : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            if (Camera.main == null)
            {
                return;
            }

            Rect r = ScreenBounds.Rect;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(r.center, r.size);
        }

        [ContextMenu("Log Bounds")]
        private void LogBounds()
        {
            Debug.Log($"Screen bounds: {ScreenBounds.Rect}");
        }
    }
}