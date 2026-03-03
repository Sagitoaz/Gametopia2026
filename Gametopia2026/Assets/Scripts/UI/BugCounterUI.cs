using UnityEngine;
using TMPro;
using DG.Tweening;
using CoderGoHappy.Core;
using CoderGoHappy.Events;
using CoderGoHappy.Level;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

        [Tooltip("Success image to show when all bugs collected")]
        [SerializeField] private Image successImage;

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

        [Header("Success Image Settings")]
        [Tooltip("Enable success image animation")]
        [SerializeField] private bool enableSuccessImage = true;

        [Tooltip("Success image scale animation duration")]
        [SerializeField] private float successImageScaleDuration = 0.6f;

        [Tooltip("Success image rotation amount (degrees)")]
        [SerializeField] private float successImageRotation = 360f;

        [Tooltip("Success image final scale")]
        [SerializeField] private float successImageFinalScale = 1f;

        [Tooltip("Success image delay before showing")]
        [SerializeField] private float successImageDelay = 0.2f;

        #endregion

        #region State

        private int currentCollected = 0;
        private int currentTotal = 10;
        private bool isAnimating = false;
        private bool hasShownSuccessImage = false;

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

            // Hide success image initially
            if (successImage != null)
            {
                CanvasGroup canvasGroup = successImage.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = successImage.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0f;
                successImage.transform.localScale = Vector3.zero;
                successImage.gameObject.SetActive(false);
            }
        }

        private IEnumerator Start()
        {
            // Subscribe to events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Subscribe(GameEvents.MiniBugCollected, OnMiniBugCollected);
                EventManager.Instance.Subscribe("BugCounterUpdate", OnBugCounterUpdate);
            }

            // Wait 1 frame so LevelManager.Awake() and Start() in the new scene have all run
            yield return null;

            RefreshDisplay(false);
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

        /// <summary>
        /// Read current global bug count from GameStateData and update the display.
        /// Uses GameStateData.miniBugsCollected (global total across all scenes).
        /// </summary>
        private void RefreshDisplay(bool animate)
        {
            if (GameStateData.Instance == null)
            {
                Debug.LogWarning("[BugCounterUI] RefreshDisplay: GameStateData.Instance is null!");
                return;
            }

            int collected = GameStateData.Instance.miniBugsCollected;
            int total = LevelManager.Instance != null
                ? LevelManager.Instance.GetMiniBugProgress().total
                : currentTotal;

            UpdateDisplay(collected, total, animate);
        }

        private void OnMiniBugCollected(object data)
        {
            RefreshDisplay(true);
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

                    // Show success image when all bugs collected
                    if (!hasShownSuccessImage && enableSuccessImage)
                    {
                        ShowSuccessImage();
                    }
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

        /// <summary>
        /// Show success image with smooth DOTween animation
        /// </summary>
        private void ShowSuccessImage()
        {
            if (successImage == null || hasShownSuccessImage)
                return;

            hasShownSuccessImage = true;

            // Activate the image
            successImage.gameObject.SetActive(true);

            // Get or add CanvasGroup for fading
            CanvasGroup canvasGroup = successImage.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = successImage.gameObject.AddComponent<CanvasGroup>();
            }

            // Reset initial state
            canvasGroup.alpha = 0f;
            successImage.transform.localScale = Vector3.zero;
            successImage.transform.rotation = Quaternion.identity;

            // Kill any existing animations
            successImage.transform.DOKill();
            canvasGroup.DOKill();

            // Create animation sequence
            Sequence successSequence = DOTween.Sequence();

            // Fade in
            successSequence.Append(canvasGroup.DOFade(1f, successImageScaleDuration * 0.5f)
                .SetEase(Ease.OutQuad));

            // Scale up with bounce
            successSequence.Join(successImage.transform.DOScale(successImageFinalScale, successImageScaleDuration)
                .SetEase(Ease.OutBack));

            // Rotate (if rotation is set)
            if (Mathf.Abs(successImageRotation) > 0.1f)
            {
                successSequence.Join(successImage.transform.DORotate(
                    new Vector3(0, 0, successImageRotation), 
                    successImageScaleDuration, 
                    RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad));
            }

            // Add a slight punch scale at the end for extra juice
            successSequence.Append(successImage.transform.DOPunchScale(
                Vector3.one * 0.1f, 
                0.3f, 
                5, 
                0.5f));

            // Add continuous gentle pulse animation
            successSequence.OnComplete(() => {
                successImage.transform.DOScale(
                    successImageFinalScale * 1.05f, 
                    1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            });

            // Set delay before starting
            successSequence.SetDelay(successImageDelay);

            Debug.Log("[BugCounterUI] Success image displayed with animation!");
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
