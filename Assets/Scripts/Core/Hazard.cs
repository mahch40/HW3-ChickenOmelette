using UnityEngine;

namespace ChickenOmelette.Core
{
    public class Hazard : MonoBehaviour
    {
        [SerializeField, Min(1)] private int damage = 1;

        public int Damage => damage;

        public virtual void OnHitPlayer()
        {
        }
    }
}