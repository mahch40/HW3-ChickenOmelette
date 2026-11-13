using UnityEngine;

namespace ChickenOmelette.Core
{
    public class PlayerPower : MonoBehaviour
    {
        public const int MinLevel = 1;

        [SerializeField, Range(1, 5)] private int startLevel = 1;

        private int maxLevel = 3;

        public int Level { get; private set; }
        public int MaxLevel => maxLevel;

        private void Awake()
        {
            Level = Mathf.Clamp(startLevel, MinLevel, maxLevel);
        }

        private void OnEnable()
        {
            GameEvents.PlayerDamaged += Decrease;
        }

        private void OnDisable()
        {
            GameEvents.PlayerDamaged -= Decrease;
        }

        private void Start()
        {
            GameEvents.RaisePowerLevelChanged(Level);
        }

        public void SetMaxLevel(int max)
        {
            maxLevel = Mathf.Max(MinLevel, max);
            if (Level > maxLevel)
            {
                SetLevel(maxLevel);
            }
        }

        [ContextMenu("Increase Power")]
        public void Increase()
        {
            SetLevel(Level + 1);
        }

        [ContextMenu("Decrease Power")]
        public void Decrease()
        {
            SetLevel(Level - 1);
        }

        public void SetLevel(int level)
        {
            int clamped = Mathf.Clamp(level, MinLevel, maxLevel);
            if (clamped == Level)
            {
                return;
            }

            Level = clamped;
            GameEvents.RaisePowerLevelChanged(Level);
        }
    }
}