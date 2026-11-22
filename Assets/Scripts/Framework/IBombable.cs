namespace ChickenOmelette.Core
{
    public interface IBombable
    {
        /// <param name="bossDamageFraction">Fraction of a boss's maximum health the blast removes (about 0.15 to 0.20).</param>
        void OnBombed(float bossDamageFraction);
    }
}