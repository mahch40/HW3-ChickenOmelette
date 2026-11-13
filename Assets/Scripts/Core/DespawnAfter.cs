using UnityEngine;

namespace ChickenOmelette.Core
{
    public class DespawnAfter : MonoBehaviour
    {
        [SerializeField] private float lifetime = 2f;

        private void OnEnable()
        {
            Invoke(nameof(Deactivate), lifetime);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(Deactivate));
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}