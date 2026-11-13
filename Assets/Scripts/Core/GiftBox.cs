using UnityEngine;

namespace ChickenOmelette.Core
{
    public class GiftBox : FallingPickup
    {
        protected override void OnCollected(GameObject player)
        {
            PlayerPower power = player.GetComponent<PlayerPower>();
            if (power != null)
            {
                power.Increase();
            }
        }
    }
}