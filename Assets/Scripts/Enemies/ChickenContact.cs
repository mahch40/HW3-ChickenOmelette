namespace ChickenOmelette.Core
{
    public class ChickenContact : Hazard
    {
        public override void OnHitPlayer()
        {
            if (TryGetComponent(out IDamageable self))
            {
                self.TakeDamage(999);
            }
        }
    }
}