using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

namespace CoderGoHappy.Puzzle
{
    /// <summary>
    /// ButtonSequence puzzle: Player must click buttons in the correct order.
    /// Solution format in PuzzleConfig: "0,2,1,3" (comma-separated button indices).
    /// Auto-submits when player clicks enough buttons.
    /// Provides visual feedback (scale animation) on each button click.
    /// </summary>
    public class ButtonSequencePuzzle : PuzzleBase
    {
        #region Inspector Fields

        /// <summary>
        /// Array of UI buttons for the puzzle.
        /// Assign in Inspector - order determines button index (0, 1, 2, 3...).
        /// </summary>
        [Header("Button Sequence Settings")]
        [Tooltip("Array of buttons - order is important (index 0, 1, 2...)")]
        public Button[] puzzleButtons;

        /// <summary>
        /// Optional text to show current sequence length
        /// </summary>
        [Tooltip("Optional UI Text to show sequence progress (e.g., '2/4')")]
        public TextMeshProUGUI sequenceProgressText;

        /// <summary>
        /// Optional button to manually clear current sequence
        /// </summary>
        [Tooltip("Optional clear button to reset current sequence")]
        public Button clearButton;

        /// <summary>
        /// Color for normal button state
        /// </summary>
        [Header("Visual Feedback")]
        [Tooltip("Normal button color")]
        public Color normalColor = Color.white;

        /// <summary>
        /// Color when button is clicked (temporary flash)
        /// </summary>
        [Tooltip("Color flash when button is clicked")]
        public Color clickedColor = Color.green;

        /// <summary>
        /// Color when sequence is wrong
        /// </summary>
        [Tooltip("Color flash when sequence is wrong")]
        public Color errorColor = Color.red;

        #endregion

        #region State

        /// <summary>
        /// Player's current clicked sequence (button indices)
        /// </summary>
        private List<int> playerSequence = new List<int>();

        /// <summary>
        /// Correct solution sequence (parsed from config)
        /// </summary>
        private int[] solutionSequence;

        #endregion

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            // Setup button listeners
            if (puzzleButtons != null)
            {
                for (int i = 0; i < puzzleButtons.Length; i++)
                {
                    int buttonIndex = i; // Capture for closure
                    puzzleButtons[i].onClick.AddListener(() => OnButtonClicked(buttonIndex));
                }
            }

            // Setup clear button
            if (clearButton != null)
            {
                clearButton.onClick.AddListener(ClearSequence);
            }
        }

        protected override void SetupPuzzleUI()
        {
            // Parse solution from config
            solutionSequence = config.GetButtonSequenceSolution();

            if (solutionSequence.Length == 0)
            {
                Debug.LogError($"[ButtonSequencePuzzle] No valid solution in config: {config.puzzleID}");
                return;
            }

            Debug.Log($"[ButtonSequencePuzzle] Solution length: {solutionSequence.Length}");

            // Reset visuals
            ResetButtonColors();

            // Clear player sequence
            playerSequence.Clear();

            // Update progress text
            UpdateProgressText();

            // Enable buttons
            SetButtonsInteractable(true);
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Called when player clicks a puzzle button
        /// </summary>
        private void OnButtonClicked(int buttonIndex)
        {
            if (!isActive || isSolved)
                return;

            Debug.Log($"[ButtonSequencePuzzle] Button {buttonIndex} clicked");

            // Add to player sequence
            playerSequence.Add(buttonIndex);

            // Visual feedback
            PlayButtonClickFeedback(buttonIndex);

            // Update progress
            UpdateProgressText();

            // Check if sequence is complete
            if (playerSequence.Count >= solutionSequence.Length)
            {
                // Convert List to array and submit
                SubmitAnswer(playerSequence.ToArray());
            }
        }

        /// <summary>
        /// Clear current sequence without submitting
        /// </summary>
        private void ClearSequence()
        {
            if (!isActive || isSolved)
                return;

            Debug.Log($"[ButtonSequencePuzzle] Clearing sequence");

            playerSequence.Clear();
            UpdateProgressText();
            ResetButtonColors();
        }

        #endregion

        #region Validation (Abstract Implementation)

        protected override bool ValidatePlayerInput(object playerInput)
        {
            if (!(playerInput is int[] sequence))
            {
                Debug.LogError($"[ButtonSequencePuzzle] Invalid input type - expected int[]");
                return false;
            }

            // Check if length matches
            if (sequence.Length != solutionSequence.Length)
            {
                Debug.Log($"[ButtonSequencePuzzle] Length mismatch: {sequence.Length} vs {solutionSequence.Length}");
                return false;
            }

            // Compare sequences
            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] != solutionSequence[i])
                {
                    Debug.Log($"[ButtonSequencePuzzle] Mismatch at index {i}: {sequence[i]} vs {solutionSequence[i]}");
                    return false;
                }
            }

            return true;
        }

        protected override void ResetPuzzleState()
        {
            playerSequence.Clear();
            UpdateProgressText();
            ResetButtonColors();
            SetButtonsInteractable(true);
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// Play animation when button is clicked
        /// </summary>
        private void PlayButtonClickFeedback(int buttonIndex)
        {
            if (buttonIndex < 0 || buttonIndex >= puzzleButtons.Length)
                return;

            Button btn = puzzleButtons[buttonIndex];
            Image btnImage = btn.GetComponent<Image>();

            if (btnImage != null)
            {
                // Flash color
                Color originalColor = btnImage.color;
                btnImage.color = clickedColor;

                DOTween.Sequence()
                    .Append(btnImage.DOColor(originalColor, 0.3f));
            }

            // Scale pulse
            btn.transform.DOScale(1.2f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    btn.transform.DOScale(1f, 0.1f).SetEase(Ease.InQuad);
                });
        }

        /// <summary>
        /// Reset all button colors to normal
        /// </summary>
        private void ResetButtonColors()
        {
            if (puzzleButtons == null)
                return;

            foreach (Button btn in puzzleButtons)
            {
                Image btnImage = btn.GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = normalColor;
                }
            }
        }

        /// <summary>
        /// Flash all buttons with error color
        /// </summary>
        private void FlashErrorFeedback()
        {
            if (puzzleButtons == null)
                return;

            foreach (Button btn in puzzleButtons)
            {
                Image btnImage = btn.GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.DOColor(errorColor, 0.2f)
                        .SetLoops(2, LoopType.Yoyo)
                        .OnComplete(() => btnImage.color = normalColor);
                }
            }
        }

        /// <summary>
        /// Update sequence progress text (e.g., "2/4")
        /// </summary>
        private void UpdateProgressText()
        {
            if (sequenceProgressText != null)
            {
                sequenceProgressText.text = $"{playerSequence.Count}/{solutionSequence.Length}";
            }
        }

        /// <summary>
        /// Enable/disable button interaction
        /// </summary>
        private void SetButtonsInteractable(bool interactable)
        {
            if (puzzleButtons == null)
                return;

            foreach (Button btn in puzzleButtons)
            {
                btn.interactable = interactable;
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
                playerSequence.Clear();
                UpdateProgressText();
                ResetButtonColors();
                SetButtonsInteractable(true);
            });

            base.OnPuzzleFailed();
        }

        protected override void OnPuzzleSolved()
        {
            // Disable buttons
            SetButtonsInteractable(false);

            base.OnPuzzleSolved();
        }

        #endregion
    }
}
