using System.Collections.Generic;
using ChickenOmelette.Data;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class FormationController : MonoBehaviour
    {
        [Tooltip("Half the width of one chicken, used to keep the swaying formation on screen.")]
        [SerializeField, Min(0f)] private float chickenHalfWidth = 0.7f;
        [SerializeField, Min(0f)] private float screenMargin = 0.3f;

        private readonly List<ChickenMover> members = new List<ChickenMover>();
        private Vector2[] offsets = System.Array.Empty<Vector2>();
        private float baseX;
        private float swayAmplitude;
        private float swaySpeed;
        private float time;

        public IReadOnlyList<ChickenMover> Members => members;

        public void Begin(WaveDataSO wave, int count)
        {
            offsets = FormationLayout.GetOffsets(wave.Formation, count, wave.Columns, wave.Spacing);

            Vector2 center = ScreenBounds.FromViewport(wave.FormationCenter);
            transform.position = center;
            baseX = center.x;
            time = 0f;
            swaySpeed = wave.SwaySpeed;

            float halfWidth = chickenHalfWidth;
            for (int i = 0; i < offsets.Length; i++)
            {
                halfWidth = Mathf.Max(halfWidth, Mathf.Abs(offsets[i].x) + chickenHalfWidth);
            }

            Rect screen = ScreenBounds.Rect;
            float room = Mathf.Min(center.x - screen.xMin, screen.xMax - center.x) - halfWidth - screenMargin;
            swayAmplitude = Mathf.Clamp(wave.SwayAmplitude, 0f, Mathf.Max(0f, room));

            members.Clear();
        }

        public Vector3 GetSlotPosition(int index)
        {
            return transform.position + (Vector3)offsets[index];
        }

        public void Register(ChickenMover member)
        {
            if (!members.Contains(member))
            {
                members.Add(member);
            }
        }

        public void Unregister(ChickenMover member)
        {
            members.Remove(member);
        }

        private void Update()
        {
            time += Time.deltaTime;
            Vector3 position = transform.position;
            position.x = baseX + Mathf.Sin(time * swaySpeed) * swayAmplitude;
            transform.position = position;
        }
    }
}