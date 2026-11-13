using UnityEngine;

namespace ChickenOmelette.Core
{
    public class TestBombable : MonoBehaviour, IBombable
    {
        public void OnBombed(float bossDamageFraction)
        {
            Debug.Log($"{name} was bombed (boss fraction {bossDamageFraction:F2})");
            gameObject.SetActive(false);
        }
    }
}