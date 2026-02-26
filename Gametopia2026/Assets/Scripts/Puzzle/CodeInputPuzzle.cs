using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace CoderGoHappy.Puzzle
{
    /// <summary>
    /// CodeInput puzzle: Player must enter a numeric code.
    /// Solution format in PuzzleConfig: "1234" (numeric string).
    /// Validates input when submit button is clicked.
    /// Supports optional hint display.
    /// </summary>
    public class CodeInputPuzzle : PuzzleBase
    {
        #region Inspector Fields

        /// <summary>
        /// TMP_InputField where player types the code
        /// </summary>
        [Header("Code Input Settings")]
        [Tooltip("TMP_InputField for player to enter code")]
        public TMP_InputField codeInputField;

        /// <summary>
        /// Button to submit the code
        /// </summary>
        [Tooltip("Submit button")]
        public Button submitButton;

        /// <summary>
        /// Optional button to clear input
        /// </summary>
        [Tooltip("Optional clear button")]
        public Button clearButton;

        /// <summary>
        /// Optional text to show hints or feedback
        /// </summary>
        [Tooltip("Optional text for hints/feedback")]
        public TextMeshProUGUI feedbackText;

        /// <summary>
        /// Optional text to show description from PuzzleConfig
        /// </summary>
        [Tooltip("Optional text to display puzzle description")]
        public TextMeshProUGUI descriptionText;

        /// <summary>
        /// Placeholder text for empty input field
        /// </summary>
        [Header("UI Settings")]
        [Tooltip("Placeholder text for input field")]
        public string placeholderText = "Enter Code...";

        /// <summary>
        /// Color for correct feedback
        /// </summary>
        [Tooltip("Color for success feedback")]
        public Color correctColor = Color.green;

        /// <summary>
        /// Color for incorrect feedback
        /// </summary>
        [Tooltip("Color for error feedback")]
        public Color incorrectColor = Color.red;

        /// <summary>
        /// Color for normal state
        /// </summary>
        [Tooltip("Normal color")]
        public Color normalColor = Color.white;

        #endregion

        #region State

        /// <summary>
        /// Correct solution (parsed from config)
        /// </summary>
        private string correctCode;

        #endregion

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            // Setup submit button
            if (submitButton != null)
            {
                submitButton.onClick.AddListener(OnSubmitClicked);
            }

            // Setup clear button
            if (clearButton != null)
            {
                clearButton.onClick.AddListener(OnClearClicked);
            }

            // Setup input field validation (numeric only)
            if (codeInputField != null)
            {
                codeInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                codeInputField.onEndEdit.AddListener(OnInputEndEdit);

                // Set placeholder
                if (codeInputField.placeholder != null)
                {
                    TextMeshProUGUI placeholderComponent = codeInputField.placeholder.GetComponent<TextMeshProUGUI>();
                    if (placeholderComponent != null)
                    {
                        placeholderComponent.text = placeholderText;
                    }
                }
            }
        }

        protected override void SetupPuzzleUI()
        {
            // Parse solution from config
            correctCode = config.GetCodeInputSolution();

            if (string.IsNullOrEmpty(correctCode))
            {
                Debug.LogError($"[CodeInputPuzzle] No valid solution in config: {config.puzzleID}");
                return;
            }

            Debug.Log($"[CodeInputPuzzle] Code length: {correctCode.Length}");

            // Clear input field
            if (codeInputField != null)
            {
                codeInputField.text = "";
                codeInputField.interactable = true;

                // Reset input field color
                Image inputImage = codeInputField.GetComponent<Image>();
                if (inputImage != null)
                {
                    inputImage.color = normalColor;
                }
            }

            // Clear feedback
            if (feedbackText != null)
            {
                feedbackText.text = "";
            }

            // Show description if available
            if (descriptionText != null)
            {
                descriptionText.text = config.description;
            }

            // Enable buttons
            SetButtonsInteractable(true);
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Called when submit button is clicked
        /// </summary>
        private void OnSubmitClicked()
        {
            if (!isActive || isSolved)
                return;

            if (codeInputField == null)
                return;

            string playerCode = codeInputField.text.Trim();

            if (string.IsNullOrEmpty(playerCode))
            {
                ShowFeedback("Please enter a code", incorrectColor);
                return;
            }

            Debug.Log($"[CodeInputPuzzle] Submitting code: {playerCode}");

            // Submit through base class
            SubmitAnswer(playerCode);
        }

        /// <summary>
        /// Called when clear button is clicked
        /// </summary>
        private void OnClearClicked()
        {
            if (!isActive || isSolved)
                return;

            if (codeInputField != null)
            {
                codeInputField.text = "";
            }

            if (feedbackText != null)
            {
                feedbackText.text = "";
            }

            ResetInputFieldColor();
        }

        /// <summary>
        /// Called when player finishes editing input (presses Enter)
        /// </summary>
        private void OnInputEndEdit(string input)
        {
            // Auto-submit on Enter key
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSubmitClicked();
            }
        }

        #endregion

        #region Validation (Abstract Implementation)

        protected override bool ValidatePlayerInput(object playerInput)
        {
            if (!(playerInput is string playerCode))
            {
                Debug.LogError($"[CodeInputPuzzle] Invalid input type - expected string");
                return false;
            }

            // Simple string comparison (case-sensitive)
            bool isCorrect = playerCode == correctCode;

            Debug.Log($"[CodeInputPuzzle] Validating '{playerCode}' vs '{correctCode}': {isCorrect}");

            return isCorrect;
        }

        protected override void ResetPuzzleState()
        {
            if (codeInputField != null)
            {
                codeInputField.text = "";
                codeInputField.interactable = true;
            }

            if (feedbackText != null)
            {
                feedbackText.text = "";
            }

            ResetInputFieldColor();
            SetButtonsInteractable(true);
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// Show feedback message with color
        /// </summary>
        private void ShowFeedback(string message, Color color)
        {
            if (feedbackText == null)
                return;

            feedbackText.text = message;
            feedbackText.color = color;

            // Pulse animation
            feedbackText.transform.DOScale(1.1f, 0.2f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    feedbackText.transform.DOScale(1f, 0.2f).SetEase(Ease.InQuad);
                });
        }

        /// <summary>
        /// Flash input field with color
        /// </summary>
        private void FlashInputField(Color color)
        {
            if (codeInputField == null)
                return;

            Image inputImage = codeInputField.GetComponent<Image>();
            if (inputImage != null)
            {
                inputImage.DOColor(color, 0.2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => inputImage.color = normalColor);
            }

            // Shake animation
            codeInputField.transform.DOShakePosition(0.5f, strength: 10f, vibrato: 20)
                .OnComplete(() => codeInputField.transform.localPosition = Vector3.zero);
        }

        /// <summary>
        /// Reset input field to normal color
        /// </summary>
        private void ResetInputFieldColor()
        {
            if (codeInputField == null)
                return;

            Image inputImage = codeInputField.GetComponent<Image>();
            if (inputImage != null)
            {
                inputImage.color = normalColor;
            }
        }

        /// <summary>
        /// Enable/disable buttons
        /// </summary>
        private void SetButtonsInteractable(bool interactable)
        {
            if (submitButton != null)
            {
                submitButton.interactable = interactable;
            }

            if (clearButton != null)
            {
                clearButton.interactable = interactable;
            }
        }

        #endregion

        #region Overrides

        protected override void OnPuzzleSolved()
        {
            // Show success feedback
            ShowFeedback("Correct!", correctColor);

            // Flash green
            if (codeInputField != null)
            {
                Image inputImage = codeInputField.GetComponent<Image>();
                if (inputImage != null)
                {
                    inputImage.color = correctColor;
                }
            }

            // Disable input
            if (codeInputField != null)
            {
                codeInputField.interactable = false;
            }

            SetButtonsInteractable(false);

            base.OnPuzzleSolved();
        }

        protected override void OnPuzzleFailed()
        {
            // Show failure feedback
            ShowFeedback("Incorrect code", incorrectColor);

            // Flash red
            FlashInputField(incorrectColor);

            // Clear input after delay
            DOVirtual.DelayedCall(1f, () => {
                if (codeInputField != null)
                {
                    codeInputField.text = "";
                }
            });

            base.OnPuzzleFailed();
        }

        protected override void PlaySuccessFeedback()
        {
            base.PlaySuccessFeedback();
            // Additional success animation can be added here
        }

        protected override void PlayFailureFeedback()
        {
            base.PlayFailureFeedback();
            // Additional failure animation can be added here
        }

        #endregion
    }
}
