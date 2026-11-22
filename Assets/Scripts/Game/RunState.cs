using System;

namespace ChickenOmelette.Core
{
    [Serializable]
    public class RunState
    {
        [UnityEngine.Tooltip("False for a brand new game: the player keeps its default values.")]
        public bool hasCarry;
        public int score;
        public int lives;
        public int powerLevel = 1;
        public int missiles;

        public RunState Clone()
        {
            return (RunState)MemberwiseClone();
        }
    }
}