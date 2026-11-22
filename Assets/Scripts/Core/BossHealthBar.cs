using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChickenOmelette.Core
{
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private Vector2 size = new Vector2(800f, 32f);
        [Tooltip("Distance from the top of the screen (reference pixels). Keep it below the round label.")]
        [SerializeField] private float topOffset = 120f;
        [SerializeField, Min(0.1f)] private float smoothing = 8f;

        private GameObject root;
        private RectTransform fill;
        private TMP_Text nameLabel;
        private float target = 1f;
        private float shown = 1f;

        private void Awake()
        {
            root = new GameObject("BossBar", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            RectTransform rootRect = (RectTransform)root.transform;
            rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 1f);
            rootRect.pivot = new Vector2(0.5f, 1f);
            rootRect.anchoredPosition = new Vector2(0f, -topOffset);
            rootRect.sizeDelta = size;

            CreateImage("Back", rootRect, "#1A1A1A", 4f);
            RectTransform fillRect = CreateImage("Fill", rootRect, "#E53935", 4f);
            fillRect.pivot = new Vector2(0f, 0.5f);
            fill = fillRect;

            GameObject labelObject = new GameObject("Name", typeof(RectTransform));
            labelObject.transform.SetParent(rootRect, false);
            RectTransform labelRect = (RectTransform)labelObject.transform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI text = labelObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = 22f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            text.color = Color.white;
            nameLabel = text;

            root.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.BossAppeared += OnAppeared;
            GameEvents.BossHealthChanged += OnHealthChanged;
            GameEvents.BossDefeated += OnDefeated;
        }

        private void OnDisable()
        {
            GameEvents.BossAppeared -= OnAppeared;
            GameEvents.BossHealthChanged -= OnHealthChanged;
            GameEvents.BossDefeated -= OnDefeated;
        }

        private void Update()
        {
            shown = Mathf.Lerp(shown, target, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime));
            fill.localScale = new Vector3(shown, 1f, 1f);
        }

        private void OnAppeared(string bossName, int maxHealth)
        {
            nameLabel.text = bossName;
            target = shown = 1f;
            root.SetActive(true);
        }

        private void OnHealthChanged(int current, int max)
        {
            target = max <= 0 ? 0f : (float)current / max;
        }

        private void OnDefeated()
        {
            root.SetActive(false);
        }

        private static RectTransform CreateImage(string objectName, RectTransform parent, string hex, float padding)
        {
            GameObject go = new GameObject(objectName, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            float inset = objectName == "Back" ? 0f : padding;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);

            Image image = go.AddComponent<Image>();
            image.raycastTarget = false;
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                image.color = color;
            }
            return rect;
        }
    }
}