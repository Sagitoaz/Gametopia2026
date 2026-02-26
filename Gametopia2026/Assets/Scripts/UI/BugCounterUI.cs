using UnityEngine;
using TMPro;
using DG.Tweening;
using CoderGoHappy.Core;
using CoderGoHappy.Events;
using CoderGoHappy.Level;
using UnityEngine.UI;

namespace CoderGoHappy.UI
{
    /// <summary>
    /// UI component to display MiniBug collection progress
    /// Shows "Bugs: X/10" text with juice animations
    /// Attach to UI Text in Canvas
    /// </summary>
    public class BugCounterUI : MonoBehaviour
    {
        #region Inspector Fields

        [Header("UI References")]
        [Tooltip("TextMeshProUGUI component to display bug count")]
        [SerializeField] private TextMeshProUGUI bugCountText;

        [Tooltip("Optional icon/sprite for bug")]
        [SerializeField] private Image bugIcon;

        [Header("Visual Settings")]
        [Tooltip("Text format (use {0} for collected, {1} for total)")]
        [SerializeField] private string textFormat = "🐛 {0}/{1}";

        [Tooltip("Color when all bugs collected")]
        [SerializeField] private Color completedColor = Color.yellow;

        [Tooltip("Normal color")]
        [SerializeField] private Color normalColor = Color.white;

        [Header("Animation Settings")]
        [Tooltip("Scale pulse when bug collected")]
        [SerializeField] private float pulseScale = 1.3f;

        [Tooltip("Pulse animation duration")]
        [SerializeField] private float pulseDuration = 0.3f;

        #endregion

        #region State

        private int currentCollected = 0;
        private int currentTotal = 10;
        private bool isAnimating = false;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Validate references
            if (bugCountText == null)
            {
                bugCountText = GetComponent<TextMeshProUGUI>();
                if (bugCountText == null)
                {
                    Debug.LogError("[BugCounterUI] TextMeshProUGUI component not found!", this);
                }
            }
        }

        private void Start()
        {
            // Subscribe to events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Subscribe(GameEvents.MiniBugCollected, OnMiniBugCollected);
                EventManager.Instance.Subscribe("BugCounterUpdate", OnBugCounterUpdate);
            }

            // Initialize display from LevelManager
            if (LevelManager.Instance != null)
            {
                var (collected, total) = LevelManager.Instance.GetMiniBugProgress();
                UpdateDisplay(collected, total, false);
            }
            else
            {
                // Fallback: use GameStateData
                UpdateDisplay(0, 10, false);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe(GameEvents.MiniBugCollected, OnMiniBugCollected);
                EventManager.Instance.Unsubscribe("BugCounterUpdate", OnBugCounterUpdate);
            }
        }

        #endregion

        #region Event Handlers

        private void OnMiniBugCollected(object data)
        {
            // Get updated count (data might be ItemData or null)
            if (LevelManager.Instance != null)
            {
                var (collected, total) = LevelManager.Instance.GetMiniBugProgress();
                UpdateDisplay(collected, total, true);
            }
        }

        private void OnBugCounterUpdate(object data)
        {
            if (data is BugCountData bugData)
            {
                UpdateDisplay(bugData.collected, bugData.total, true);
            }
        }

        #endregion

        #region Display Update

        /// <summary>
        /// Update bug counter display
        /// </summary>
        /// <param name="collected">Number collected</param>
        /// <param name="total">Total available</param>
        /// <param name="animate">Play animation?</param>
        private void UpdateDisplay(int collected, int total, bool animate)
        {
            currentCollected = collected;
            currentTotal = total;

            // Update text
            if (bugCountText != null)
            {
                bugCountText.text = string.Format(textFormat, collected, total);

                // Change color if all collected
                if (collected >= total)
                {
                    bugCountText.color = completedColor;
                }
                else
                {
                    bugCountText.color = normalColor;
                }
            }

            // Play animation if requested
            if (animate && !isAnimating)
            {
                PlayCollectionAnimation();
            }

            Debug.Log($"[BugCounterUI] Updated: {collected}/{total}");
        }

        /// <summary>
        /// Animate when bug is collected
        /// </summary>
        private void PlayCollectionAnimation()
        {
            if (bugCountText == null || isAnimating)
                return;

            isAnimating = true;

            // Kill existing animations
            bugCountText.transform.DOKill();
            if (bugIcon != null)
            {
                bugIcon.transform.DOKill();
            }

            // Scale pulse on text
            bugCountText.transform.DOPunchScale(Vector3.one * (pulseScale - 1f), pulseDuration, 5, 0.5f)
                .OnComplete(() => {
                    isAnimating = false;
                    bugCountText.transform.localScale = Vector3.one;
                });

            // Rotate icon (if exists)
            if (bugIcon != null)
            {
                bugIcon.transform.DORotate(new Vector3(0, 0, 360f), pulseDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => {
                        bugIcon.transform.rotation = Quaternion.identity;
                    });
            }

            // Flash color
            if (bugCountText != null)
            {
                Color originalColor = bugCountText.color;
                bugCountText.DOColor(Color.green, pulseDuration * 0.5f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => {
                        bugCountText.color = originalColor;
                    });
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Manually update display (for testing)
        /// </summary>
        public void SetBugCount(int collected, int total)
        {
            UpdateDisplay(collected, total, false);
        }

        /// <summary>
        /// Get current bug count
        /// </summary>
        public (int collected, int total) GetBugCount()
        {
            return (currentCollected, currentTotal);
        }

        #endregion
    }
}
