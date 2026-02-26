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
        }

        protected override void SetupPuzzleUI()
        {
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
                return;

            // Update each slot
            for (int i = 0; i < sequenceSlots.Length; i++)
            {
                if (sequenceSlots[i] == null)
                    continue;

                // If player has selected this index
                if (i < playerColorSequence.Count)
                {
                    string colorName = playerColorSequence[i];
                    Color color = GetColorFromName(colorName);
                    sequenceSlots[i].color = color;
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
            for (int i = 0; i < colorNames.Length; i++)
            {
                if (colorNames[i].Equals(colorName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (i < colors.Length)
                    {
                        return colors[i];
                    }
                }
            }

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
                    // Shake the Transform instead of Image
                    slot.transform.DOShakePosition(0.5f, strength: 5f, vibrato: 20)
                        .OnComplete(() => {
                            if (slot != null)
                            {
                                slot.transform.localPosition = Vector3.zero;
                            }
                        });
                }
            }
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
            // Flash error feedback
            FlashErrorFeedback();

            // Disable buttons temporarily
            SetButtonsInteractable(false);

            // Clear sequence after delay
            DOVirtual.DelayedCall(1f, () => {
                playerColorSequence.Clear();
                UpdateSequenceDisplay();
                UpdateProgressText();
                SetButtonsInteractable(true);
            });

            base.OnPuzzleFailed();
        }

        protected override void OnPuzzleSolved()
        {
            // Disable buttons
            SetButtonsInteractable(false);

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
    }
}
