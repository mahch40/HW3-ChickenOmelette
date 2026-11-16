using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChickenOmelette.Core
{
    public class ScreenFader : MonoBehaviour
    {
        private CanvasGroup group;

        public static ScreenFader Create(Transform parent)
        {
            GameObject root = new GameObject("ScreenFader");
            root.transform.SetParent(parent, false);

            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            GameObject imageObject = new GameObject("Black");
            imageObject.transform.SetParent(root.transform, false);
            Image image = imageObject.AddComponent<Image>();
            image.color = Color.black;
            image.raycastTarget = false;

            RectTransform rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            ScreenFader fader = root.AddComponent<ScreenFader>();
            fader.group = canvasGroup;
            return fader;
        }

        public IEnumerator FadeTo(float target, float duration)
        {
            float start = group.alpha;

            for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
            {
                group.alpha = Mathf.Lerp(start, target, t / duration);
                yield return null;
            }

            group.alpha = target;
        }
    }
}