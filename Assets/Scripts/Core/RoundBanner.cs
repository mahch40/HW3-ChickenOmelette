using System.Collections;
using TMPro;
using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public class RoundBanner : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField, Min(0f)] private float fadeIn = 0.3f;
        [SerializeField, Min(0f)] private float fadeOut = 0.5f;

        private CanvasGroup group;
        private Coroutine routine;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            if (label == null)
            {
                label = GetComponentInChildren<TMP_Text>();
            }
        }

        public void Show(string text, float seconds)
        {
            label.text = text;

            if (routine != null)
            {
                StopCoroutine(routine);
            }

            routine = StartCoroutine(Play(seconds));
        }

        private IEnumerator Play(float seconds)
        {
            for (float t = 0f; t < fadeIn; t += Time.deltaTime)
            {
                group.alpha = fadeIn > 0f ? t / fadeIn : 1f;
                yield return null;
            }

            group.alpha = 1f;

            float hold = Mathf.Max(0f, seconds - fadeIn - fadeOut);
            yield return new WaitForSeconds(hold);

            for (float t = 0f; t < fadeOut; t += Time.deltaTime)
            {
                group.alpha = 1f - t / fadeOut;
                yield return null;
            }

            group.alpha = 0f;
            routine = null;
        }
    }
}