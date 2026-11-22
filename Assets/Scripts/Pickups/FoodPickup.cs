using UnityEngine;

namespace ChickenOmelette.Core
{
    public class FoodPickup : FallingPickup
    {
        [SerializeField, Min(0)] private int points = 50;

        protected override void OnCollected(GameObject player)
        {
            GameEvents.RaisePointsAwarded(points);
        }
    }
}