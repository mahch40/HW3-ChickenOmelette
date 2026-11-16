using TMPro;
using UnityEngine;

namespace ChickenOmelette.Core
{
    public class Hud : MonoBehaviour
    {
        [SerializeField, Min(10f)] private float fontSize = 40f;
        [Tooltip("Distance of the labels from the screen corners (reference pixels).")]
        [SerializeField] private Vector2 margin = new Vector2(40f, 30f);

        private TMP_Text scoreLabel;
        private TMP_Text livesLabel;
        private TMP_Text missilesLabel;
        private TMP_Text powerLabel;
        private TMP_Text roundLabel;

        private int level = 1;
        private int round;

        private void Awake()
        {
            scoreLabel = CreateLabel("Score", new Vector2(0f, 1f), TextAlignmentOptions.TopLeft, "#FFD54F");
            livesLabel = CreateLabel("Lives", new Vector2(1f, 1f), TextAlignmentOptions.TopRight, "#FF8A80");
            roundLabel = CreateLabel("Round", new Vector2(0.5f, 1f), TextAlignmentOptions.Top, "#FFFFFF");
            missilesLabel = CreateLabel("Missiles", new Vector2(0f, 0f), TextAlignmentOptions.BottomLeft, "#80D8FF");
            powerLabel = CreateLabel("Power", new Vector2(1f, 0f), TextAlignmentOptions.BottomRight, "#B9F6CA");

            ShowScore(0);
            ShowLives(0, 0);
            ShowMissiles(0);
            ShowPower(1);
            ShowRound();
        }

        private void OnEnable()
        {
            GameEvents.ScoreChanged += ShowScore;
            GameEvents.LivesChanged += ShowLives;
            GameEvents.MissileCountChanged += ShowMissiles;
            GameEvents.PowerLevelChanged += ShowPower;
            GameEvents.LevelStarted += OnLevelStarted;
            GameEvents.RoundStarted += OnRoundStarted;
        }

        private void OnDisable()
        {
            GameEvents.ScoreChanged -= ShowScore;
            GameEvents.LivesChanged -= ShowLives;
            GameEvents.MissileCountChanged -= ShowMissiles;
            GameEvents.PowerLevelChanged -= ShowPower;
            GameEvents.LevelStarted -= OnLevelStarted;
            GameEvents.RoundStarted -= OnRoundStarted;
        }

        private void ShowScore(int score)
        {
            scoreLabel.text = $"SCORE {score:000000}";
        }

        private void ShowLives(int current, int max)
        {
            livesLabel.text = $"LIVES {current}/{max}";
        }

        private void ShowMissiles(int count)
        {
            missilesLabel.text = $"MISSILES {count}";
        }

        private void ShowPower(int powerLevel)
        {
            powerLabel.text = $"POWER {powerLevel}";
        }

        private void OnLevelStarted(int levelNumber)
        {
            level = levelNumber;
            round = 0;
            ShowRound();
        }

        private void OnRoundStarted(int levelNumber, int roundNumber)
        {
            level = levelNumber;
            round = roundNumber;
            ShowRound();
        }

        private void ShowRound()
        {
            int total = 0;
            GameManager manager = GameManager.Instance;
            if (manager != null && manager.CurrentLevel != null)
            {
                total = manager.CurrentLevel.RoundCount;
            }

            if (round <= 0 || total <= 0)
            {
                roundLabel.text = $"LEVEL {level}";
            }
            else
            {
                roundLabel.text = $"LEVEL {level}   ROUND {round}/{total}";
            }
        }

        private TMP_Text CreateLabel(string labelName, Vector2 anchor, TextAlignmentOptions alignment, string hexColor)
        {
            GameObject labelObject = new GameObject(labelName, typeof(RectTransform));
            labelObject.transform.SetParent(transform, false);

            RectTransform rect = (RectTransform)labelObject.transform;
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.sizeDelta = new Vector2(700f, 70f);

            float x = anchor.x == 0f ? margin.x : (anchor.x == 1f ? -margin.x : 0f);
            float y = anchor.y == 0f ? margin.y : -margin.y;
            rect.anchoredPosition = new Vector2(x, y);

            TextMeshProUGUI text = labelObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.fontStyle = FontStyles.Bold;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.raycastTarget = false;

            if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                text.color = color;
            }

            return text;
        }
    }
}