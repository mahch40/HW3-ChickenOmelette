using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public static class FormationLayout
    {
        private const float RingFlatten = 0.5f;

        public static Vector2[] GetOffsets(FormationType type, int count, int columns, Vector2 spacing)
        {
            Vector2[] offsets = new Vector2[Mathf.Max(0, count)];

            switch (type)
            {
                case FormationType.Grid:
                    FillGrid(offsets, columns, spacing);
                    break;
                case FormationType.VShape:
                    FillV(offsets, spacing);
                    break;
                case FormationType.Circle:
                    FillRing(offsets, spacing);
                    break;
                default:
                    FillLine(offsets, spacing);
                    break;
            }

            Recenter(offsets);
            return offsets;
        }

        private static void FillGrid(Vector2[] offsets, int columns, Vector2 spacing)
        {
            int cols = Mathf.Max(1, columns);
            for (int i = 0; i < offsets.Length; i++)
            {
                int row = i / cols;
                int col = i % cols;
                int inRow = Mathf.Min(cols, offsets.Length - row * cols);
                float x = (col - (inRow - 1) * 0.5f) * spacing.x;
                float y = -row * spacing.y;
                offsets[i] = new Vector2(x, y);
            }
        }

        private static void FillV(Vector2[] offsets, Vector2 spacing)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                if (i == 0)
                {
                    offsets[i] = Vector2.zero;
                    continue;
                }

                int step = (i + 1) / 2;
                float side = i % 2 == 1 ? -1f : 1f;
                offsets[i] = new Vector2(side * step * spacing.x * 0.5f, step * spacing.y * 0.55f);
            }
        }

        private static void FillRing(Vector2[] offsets, Vector2 spacing)
        {
            int n = offsets.Length;
            if (n == 0)
            {
                return;
            }

            float radiusX = Mathf.Max(spacing.x, spacing.x * n / 4.845f);
            float radiusY = radiusX * RingFlatten;
            for (int i = 0; i < n; i++)
            {
                float angle = Mathf.PI * 0.5f + i * Mathf.PI * 2f / n;
                offsets[i] = new Vector2(Mathf.Cos(angle) * radiusX, Mathf.Sin(angle) * radiusY);
            }
        }

        private static void FillLine(Vector2[] offsets, Vector2 spacing)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                offsets[i] = new Vector2((i - (offsets.Length - 1) * 0.5f) * spacing.x, 0f);
            }
        }

        private static void Recenter(Vector2[] offsets)
        {
            if (offsets.Length == 0)
            {
                return;
            }

            Vector2 min = offsets[0];
            Vector2 max = offsets[0];
            for (int i = 1; i < offsets.Length; i++)
            {
                min = Vector2.Min(min, offsets[i]);
                max = Vector2.Max(max, offsets[i]);
            }

            Vector2 center = (min + max) * 0.5f;
            for (int i = 0; i < offsets.Length; i++)
            {
                offsets[i] -= center;
            }
        }
    }
}