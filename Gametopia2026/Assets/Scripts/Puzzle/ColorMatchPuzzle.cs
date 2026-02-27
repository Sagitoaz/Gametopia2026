using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

namespace CoderGoHappy.Puzzle
{
    /// <summary>
    /// ColorMatch puzzle: Player must select colors in the correct sequence.
    /// Solution format in PuzzleConfig: "Red,Blue,Green,Yellow" (comma-separated color names).
    /// Color buttons allow player to build a sequence.
    /// Auto-submits when player selects enough colors.
    /// </summary>
    public class ColorMatchPuzzle : PuzzleBase
    {
        #region Inspector Fields

        /// <summary>
        /// Array of color buttons available for selection.
        /// Each button should have a unique color assigned.
        /// </summary>
        [Header("Color Match Settings")]
        [Tooltip("Array of color buttons for selection")]
        public Button[] colorButtons;

        /// <summary>
        /// Names corresponding to each color button (must match order of colorButtons array).
        /// Used for matching solution.
        /// </summary>
        [Tooltip("Color names matching each button (e.g., 'Red', 'Blue', 'Green')")]
        public string[] colorNames;

        /// <summary>
        /// Colors for each button (must match order of colorButtons array).
        /// Used for visual display.
        /// </summary>
        [Tooltip("Colors for each button (must match order)")]
        public Color[] colors;

        /// <summary>
        /// UI slots to display player's selected sequence.
        /// Should be at least as many as longest possible solution.
        /// </summary>
        [Tooltip("UI Image slots to show selected sequence")]
        public Image[] sequenceSlots;

        /// <summary>
        /// Optional text to show sequence progress
        /// </summary>
        [Tooltip("Optional text to show progress (e.g., '3/5')")]
        public TextMeshProUGUI progressText;

        /// <summary>
        /// Optional button to clear current sequence
        /// </summary>
        [Tooltip("Optional clear button")]
        public Button clearButton;

        /// <summary>
        /// Optional text to show description
        /// </summary>
        [Tooltip("Optional description text")]
        public TextMeshProUGUI descriptionText;

        /// <summary>
        /// Color for empty sequence slots
        /// </summary>
        [Header("Visual Settings")]
        [Tooltip("Color for empty sequence slots")]
        public Color emptySlotColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

        [Header("Notification Settings")]
        [Tooltip("Panel to show notifications (requires CanvasGroup)")]
        public RectTransform notificationPanel;

        [Tooltip("Text to display notification message")]
        public TextMeshProUGUI notificationText;

        [Tooltip("Icon for correct answer")]
        public Image correctIcon;

        [Tooltip("Icon for wrong answer")]
        public Image wrongIcon;

        [Tooltip("Color for correct notification background")]
        public Color correctColor = new Color(0.2f, 0.8f, 0.2f, 1f);

        [Tooltip("Color for wrong notification background")]
        public Color wrongColor = new Color(0.9f, 0.2f, 0.2f, 1f);

        [Tooltip("Duration to show notification")]
        public float notificationDuration = 1.5f;

        private CanvasGroup notificationCanvasGroup;
        private Image notificationBackground;

        #endregion

        #region State

        /// <summary>
        /// Player's current selected color sequence
        /// </summary>
        private List<string> playerColorSequence = new List<string>();

        /// <summary>
        /// Correct solution sequence (parsed from config)
        /// </summary>
        private string[] solutionSequence;

        /// <summary>
        /// Track delayed call for clearing sequence after fail (to prevent race conditions)
        /// </summary>
        private Tween clearSequenceTween;

        /// <summary>
        /// Track delayed call for layout rebuild
        /// </summary>
        private Tween layoutRebuildTween;

        #endregion

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            // Validate arrays
            if (colorButtons != null && colorNames != null && colors != null)
            {
                if (colorButtons.Length != colorNames.Length || colorButtons.Length != colors.Length)
                {
                    Debug.LogError($"[ColorMatchPuzzle] colorButtons, colorNames, and colors arrays must have same length!");
                }
            }

            // Setup color button listeners
            if (colorButtons != null && colorNames != null)
            {
                for (int i = 0; i < colorButtons.Length && i < colorNames.Length; i++)
                {
                    int colorIndex = i; // Capture for closure
                    colorButtons[i].onClick.AddListener(() => OnColorButtonClicked(colorIndex));

                    // Set button color
                    if (i < colors.Length)
                    {
                        Image btnImage = colorButtons[i].GetComponent<Image>();
                        if (btnImage != null)
                        {
                            btnImage.color = colors[i];
                        }
                    }
                }
            }

            // Setup clear button
            if (clearButton != null)
            {
                clearButton.onClick.AddListener(ClearSequence);
            }

            // Initialize sequence slots
            InitializeSequenceSlots();

            // Initialize notification system
            InitializeNotification();
        }

        protected override void SetupPuzzleUI()
        {
            // Kill any pending delayed calls from previous failure
            // (base.OnPuzzleFailed -> ShowPuzzle -> SetupPuzzleUI path)
            clearSequenceTween?.Kill();
            clearSequenceTween = null;
            layoutRebuildTween?.Kill();
            layoutRebuildTween = null;
            KillAllSlotTweens();

            // Parse solution from config
            solutionSequence = config.GetColorMatchSolution();

            if (solutionSequence.Length == 0)
            {
                Debug.LogError($"[ColorMatchPuzzle] No valid solution in config: {config.puzzleID}");
                return;
            }

            Debug.Log($"[ColorMatchPuzzle] Solution: {string.Join(", ", solutionSequence)}");

            // Clear player sequence
            playerColorSequence.Clear();

            // Reset sequence slots
            UpdateSequenceDisplay();

            // Force rebuild all LayoutGroups after frame delay (fixes slots being collapsed after SetActive)
            StartCoroutine(ForceRebuildLayoutsDelayed());

            // Show description
            if (descriptionText != null)
            {
                descriptionText.text = config.description;
            }

            // Update progress
            UpdateProgressText();

            // Enable buttons
            SetButtonsInteractable(true);
        }

        /// <summary>
        /// Force rebuild all LayoutGroups in the puzzle UI hierarchy with delay
        /// </summary>
        private System.Collections.IEnumerator ForceRebuildLayoutsDelayed()
        {
            // Wait for end of frame so all GameObjects are fully active
            yield return new WaitForEndOfFrame();
            
            // Rebuild all LayoutGroups in children
            var layoutGroups = GetComponentsInChildren<UnityEngine.UI.LayoutGroup>(true);
            foreach (var layout in layoutGroups)
            {
                if (layout != null && layout.gameObject.activeInHierarchy)
                {
                    layout.enabled = false;
                    layout.enabled = true;
                    UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
                }
            }
            
            // Also rebuild Canvas
            Canvas.ForceUpdateCanvases();
            
            Debug.Log("[ColorMatchPuzzle] LayoutGroups rebuilt");
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Called when player clicks a color button
        /// </summary>
        private void OnColorButtonClicked(int colorIndex)
        {
            if (!isActive || isSolved)
                return;

            if (colorIndex < 0 || colorIndex >= colorNames.Length)
                return;

            // Cancel any pending clear from previous failure to prevent wiping new input
            clearSequenceTween?.Kill();
            clearSequenceTween = null;
            layoutRebuildTween?.Kill();
            layoutRebuildTween = null;

            string colorName = colorNames[colorIndex];
            Debug.Log($"[ColorMatchPuzzle] Color selected: {colorName}");

            // Add to player sequence
            playerColorSequence.Add(colorName);

            // Visual feedback
            PlayColorButtonFeedback(colorIndex);

            // Update UI
            UpdateSequenceDisplay();
            UpdateProgressText();

            // Check if sequence is complete
            if (playerColorSequence.Count >= solutionSequence.Length)
            {
                // Convert List to array and submit
                SubmitAnswer(playerColorSequence.ToArray());
            }
        }

        /// <summary>
        /// Clear current sequence
        /// </summary>
        private void ClearSequence()
        {
            if (!isActive || isSolved)
                return;

            Debug.Log($"[ColorMatchPuzzle] Clearing sequence");

            playerColorSequence.Clear();
            UpdateSequenceDisplay();
            UpdateProgressText();
        }

        #endregion

        #region Validation (Abstract Implementation)

        protected override bool ValidatePlayerInput(object playerInput)
        {
            if (!(playerInput is string[] colorSequence))
            {
                Debug.LogError($"[ColorMatchPuzzle] Invalid input type - expected string[]");
                return false;
            }

            // Check if length matches
            if (colorSequence.Length != solutionSequence.Length)
            {
                Debug.Log($"[ColorMatchPuzzle] Length mismatch: {colorSequence.Length} vs {solutionSequence.Length}");
                return false;
            }

            // Compare sequences (case-insensitive)
            for (int i = 0; i < colorSequence.Length; i++)
            {
                if (!colorSequence[i].Equals(solutionSequence[i], System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"[ColorMatchPuzzle] Mismatch at index {i}: {colorSequence[i]} vs {solutionSequence[i]}");
                    return false;
                }
            }

            return true;
        }

        protected override void ResetPuzzleState()
        {
            // Kill any pending delayed calls
            clearSequenceTween?.Kill();
            layoutRebuildTween?.Kill();
            KillAllSlotTweens();
            
            playerColorSequence.Clear();
            UpdateSequenceDisplay();
            UpdateProgressText();
            SetButtonsInteractable(true);
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// Initialize sequence slot visuals
        /// </summary>
        private void InitializeSequenceSlots()
        {
            if (sequenceSlots == null)
                return;

            foreach (Image slot in sequenceSlots)
            {
                if (slot != null)
                {
                    slot.color = emptySlotColor;
                }
            }
        }

        /// <summary>
        /// Update sequence slot display to show player's selections
        /// </summary>
        private void UpdateSequenceDisplay()
        {
            if (sequenceSlots == null)
            {
                Debug.LogWarning("[ColorMatchPuzzle] sequenceSlots is NULL!");
                return;
            }

            Debug.Log($"[ColorMatchPuzzle] UpdateSequenceDisplay: {sequenceSlots.Length} slots, {playerColorSequence.Count} selected");

            // Update each slot
            for (int i = 0; i < sequenceSlots.Length; i++)
            {
                if (sequenceSlots[i] == null)
                {
                    Debug.LogWarning($"[ColorMatchPuzzle] sequenceSlots[{i}] is NULL!");
                    continue;
                }

                // If player has selected this index
                if (i < playerColorSequence.Count)
                {
                    string colorName = playerColorSequence[i];
                    Color color = GetColorFromName(colorName);
                    sequenceSlots[i].color = color;
                    Debug.Log($"[ColorMatchPuzzle] Slot[{i}] set to {colorName} = {color}");
                }
                else
                {
                    sequenceSlots[i].color = emptySlotColor;
                }
            }
        }

        /// <summary>
        /// Get Color from color name
        /// </summary>
        private Color GetColorFromName(string colorName)
        {
            if (colorNames == null || colors == null)
            {
                Debug.LogError("[ColorMatchPuzzle] colorNames or colors array is NULL!");
                return Color.white;
            }

            for (int i = 0; i < colorNames.Length; i++)
            {
                if (colorNames[i].Equals(colorName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (i < colors.Length)
                    {
                        return colors[i];
                    }
                    else
                    {
                        Debug.LogWarning($"[ColorMatchPuzzle] Color index {i} out of range for colors array (length={colors.Length})");
                    }
                }
            }

            Debug.LogWarning($"[ColorMatchPuzzle] Color name '{colorName}' not found in colorNames array!");
            return Color.white; // Default
        }

        /// <summary>
        /// Play animation when color button is clicked
        /// </summary>
        private void PlayColorButtonFeedback(int colorIndex)
        {
            if (colorIndex < 0 || colorIndex >= colorButtons.Length)
                return;

            Button btn = colorButtons[colorIndex];

            // Scale pulse
            btn.transform.DOScale(1.2f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    btn.transform.DOScale(1f, 0.1f).SetEase(Ease.InQuad);
                });

            // Flash brightness
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null && colorIndex < colors.Length)
            {
                Color originalColor = colors[colorIndex];
                Color brightColor = originalColor * 1.5f;
                brightColor.a = 1f;

                btnImage.color = brightColor;
                btnImage.DOColor(originalColor, 0.3f);
            }
        }

        /// <summary>
        /// Flash error feedback on sequence slots
        /// </summary>
        private void FlashErrorFeedback()
        {
            if (sequenceSlots == null)
                return;

            foreach (Image slot in sequenceSlots)
            {
                if (slot != null && slot.color != emptySlotColor)
                {
                    // Kill any existing tween on this slot first
                    slot.transform.DOKill();
                    
                    // Save original position before shaking (LayoutGroup manages position)
                    Vector3 originalPosition = slot.transform.localPosition;
                    
                    // Shake the Transform instead of Image
                    slot.transform.DOShakePosition(0.5f, strength: 5f, vibrato: 20)
                        .OnComplete(() => {
                            if (slot != null)
                            {
                                // Restore to original position (not Vector3.zero which breaks LayoutGroup)
                                slot.transform.localPosition = originalPosition;
                            }
                        });
                }
            }
            
            // Kill any existing layout rebuild delayed call
            layoutRebuildTween?.Kill();
            
            // Force rebuild LayoutGroup after shake animation completes
            layoutRebuildTween = DOVirtual.DelayedCall(0.6f, () => {
                StartCoroutine(ForceRebuildLayoutsDelayed());
            });
        }

        /// <summary>
        /// Update progress text
        /// </summary>
        private void UpdateProgressText()
        {
            if (progressText != null && solutionSequence != null)
            {
                progressText.text = $"{playerColorSequence.Count}/{solutionSequence.Length}";
            }
        }

        /// <summary>
        /// Enable/disable buttons
        /// </summary>
        private void SetButtonsInteractable(bool interactable)
        {
            if (colorButtons != null)
            {
                foreach (Button btn in colorButtons)
                {
                    if (btn != null)
                    {
                        btn.interactable = interactable;
                    }
                }
            }

            if (clearButton != null)
            {
                clearButton.interactable = interactable;
            }
        }

        #endregion

        #region Overrides

        protected override void OnPuzzleFailed()
        {
            // Kill any previous delayed calls to prevent race conditions
            clearSequenceTween?.Kill();
            layoutRebuildTween?.Kill();
            
            // Kill any existing slot tweens
            KillAllSlotTweens();

            // Flash error feedback
            FlashErrorFeedback();

            // Disable buttons temporarily
            SetButtonsInteractable(false);

            // Show wrong notification
            ShowNotification(false, "Sai rồi! Thử lại nhé");

            // Clear sequence after notification
            clearSequenceTween = DOVirtual.DelayedCall(notificationDuration + 0.5f, () => {
                playerColorSequence.Clear();
                UpdateSequenceDisplay();
                UpdateProgressText();
                SetButtonsInteractable(true);
            });

            base.OnPuzzleFailed();
        }

        /// <summary>
        /// Kill all active tweens on sequence slots
        /// </summary>
        private void KillAllSlotTweens()
        {
            if (sequenceSlots == null) return;
            
            foreach (Image slot in sequenceSlots)
            {
                if (slot != null)
                {
                    slot.transform.DOKill();
                    slot.DOKill();
                }
            }
        }

        protected override void OnPuzzleSolved()
        {
            // Disable buttons
            SetButtonsInteractable(false);

            // Show correct notification
            ShowNotification(true, "Chính xác! Tuyệt vời!");

            // Celebrate animation (pulse all correct slots)
            if (sequenceSlots != null)
            {
                for (int i = 0; i < playerColorSequence.Count && i < sequenceSlots.Length; i++)
                {
                    if (sequenceSlots[i] != null)
                    {
                        sequenceSlots[i].transform.DOScale(1.2f, 0.3f)
                            .SetEase(Ease.OutQuad)
                            .SetLoops(2, LoopType.Yoyo);
                    }
                }
            }

            base.OnPuzzleSolved();
        }

        #endregion

        #region Notification System

        /// <summary>
        /// Initialize notification panel
        /// </summary>
        private void InitializeNotification()
        {
            if (notificationPanel != null)
            {
                // Get or add CanvasGroup
                notificationCanvasGroup = notificationPanel.GetComponent<CanvasGroup>();
                if (notificationCanvasGroup == null)
                {
                    notificationCanvasGroup = notificationPanel.gameObject.AddComponent<CanvasGroup>();
                }

                // Get background Image
                notificationBackground = notificationPanel.GetComponent<Image>();

                // Hide initially
                notificationCanvasGroup.alpha = 0f;
                notificationPanel.localScale = Vector3.zero;
                notificationPanel.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Show notification with animation
        /// </summary>
        /// <param name="isCorrect">True for correct, false for wrong</param>
        /// <param name="message">Message to display</param>
        private void ShowNotification(bool isCorrect, string message)
        {
            if (notificationPanel == null)
            {
                Debug.LogWarning("[ColorMatchPuzzle] Notification panel not assigned!");
                return;
            }

            // Kill any existing notification tweens
            notificationPanel.DOKill();
            if (notificationCanvasGroup != null)
                notificationCanvasGroup.DOKill();

            // Setup notification content
            if (notificationText != null)
            {
                notificationText.text = message;
            }

            // Set background color
            if (notificationBackground != null)
            {
                notificationBackground.color = isCorrect ? correctColor : wrongColor;
            }

            // Show/hide icons
            if (correctIcon != null)
            {
                correctIcon.gameObject.SetActive(isCorrect);
            }
            if (wrongIcon != null)
            {
                wrongIcon.gameObject.SetActive(!isCorrect);
            }

            // Activate panel
            notificationPanel.gameObject.SetActive(true);

            // Create animation sequence
            Sequence showSequence = DOTween.Sequence();

            // Pop-in animation
            showSequence.Append(notificationPanel.DOScale(Vector3.one * 1.1f, 0.3f).SetEase(Ease.OutBack));
            showSequence.Join(notificationCanvasGroup.DOFade(1f, 0.2f));
            showSequence.Append(notificationPanel.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutQuad));

            // Add shake for wrong answer
            if (!isCorrect)
            {
                showSequence.Append(notificationPanel.DOShakeRotation(0.3f, new Vector3(0, 0, 10), 10, 90));
            }
            else
            {
                // Pulse effect for correct
                showSequence.Append(notificationPanel.DOScale(Vector3.one * 1.05f, 0.15f).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo));
            }

            // Wait
            showSequence.AppendInterval(notificationDuration - 0.8f);

            // Fade out animation
            showSequence.Append(notificationPanel.DOScale(Vector3.one * 0.8f, 0.2f).SetEase(Ease.InBack));
            showSequence.Join(notificationCanvasGroup.DOFade(0f, 0.2f));

            // Hide panel when done
            showSequence.OnComplete(() => {
                notificationPanel.gameObject.SetActive(false);
                notificationPanel.localScale = Vector3.zero;
            });

            showSequence.Play();
        }

        #endregion
    }
}
