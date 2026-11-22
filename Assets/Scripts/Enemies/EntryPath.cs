using System.Collections.Generic;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class EntryPath
    {
        private readonly Vector2[] samples;
        private readonly float[] cumulative;

        public float Length { get; }

        public EntryPath(IReadOnlyList<Vector2> controlPoints, int samplesPerSegment = 16)
        {
            int n = controlPoints.Count;
            List<Vector2> list = new List<Vector2>();

            for (int i = 0; i < n - 1; i++)
            {
                Vector2 p0 = controlPoints[Mathf.Max(i - 1, 0)];
                Vector2 p1 = controlPoints[i];
                Vector2 p2 = controlPoints[i + 1];
                Vector2 p3 = controlPoints[Mathf.Min(i + 2, n - 1)];

                for (int s = 0; s < samplesPerSegment; s++)
                {
                    list.Add(CatmullRom(p0, p1, p2, p3, s / (float)samplesPerSegment));
                }
            }

            list.Add(controlPoints[n - 1]);
            samples = list.ToArray();

            cumulative = new float[samples.Length];
            for (int i = 1; i < samples.Length; i++)
            {
                cumulative[i] = cumulative[i - 1] + Vector2.Distance(samples[i - 1], samples[i]);
            }

            Length = cumulative[samples.Length - 1];
        }

        public Vector2 Evaluate(float distance)
        {
            if (distance <= 0f)
            {
                return samples[0];
            }

            if (distance >= Length)
            {
                return samples[samples.Length - 1];
            }

            int low = 0;
            int high = cumulative.Length - 1;
            while (high - low > 1)
            {
                int mid = (low + high) / 2;
                if (cumulative[mid] <= distance)
                {
                    low = mid;
                }
                else
                {
                    high = mid;
                }
            }

            float segment = cumulative[high] - cumulative[low];
            float t = segment > 0f ? (distance - cumulative[low]) / segment : 0f;
            return Vector2.Lerp(samples[low], samples[high], t);
        }

        private static Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * ((2f * p1)
                           + (-p0 + p2) * t
                           + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2
                           + (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        }
    }
}