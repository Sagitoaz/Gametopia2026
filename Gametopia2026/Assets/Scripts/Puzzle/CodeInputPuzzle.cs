using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace CoderGoHappy.Puzzle
{
    /// <summary>
    /// CodeInput puzzle: Player enters a numeric code, then confirms.
    /// Supports TWO input modes — whichever is assigned in the Inspector:
    ///   (A) Keyboard mode  : assign a TMP_InputField. Player types normally.
    ///   (B) Numpad mode    : assign numberButtons[]. Player taps on-screen buttons.
    ///   Both modes can be used at the same time.
    ///
    /// Solution format in PuzzleConfig: "1234" (numeric string, any length).
    ///
    /// Keyboard hierarchy:
    ///   PuzzlePanel
    ///   ├── InputField  (TMP_InputField — Text Area > Placeholder + Text)
    ///   ├── AttemptsText
    ///   ├── FeedbackText
    ///   ├── ClearButton  (optional)
    ///   └── ConfirmButton
    /// </summary>
    public class CodeInputPuzzle : PuzzleBase
    {
        #region Inspector Fields

        // ---- (A) Keyboard mode ----
        [Header("Keyboard Input Mode")]
        [Tooltip("TMP_InputField for typing on keyboard. If assigned, keyboard mode is active.")]
        public TMP_InputField inputField;

        [Tooltip("Optional clear button — clears the input field")]
        public Button clearButton;

        // ---- (B) Numpad button mode ----
        [Header("Numpad Button Mode (optional)")]
        [Tooltip("Numpad buttons 0-9. Each must have a child TMP with its digit text.")]
        public Button[] numberButtons;

        [Tooltip("Backspace / delete button for numpad mode")]
        public Button deleteButton;

        [Tooltip("Optional TextMeshProUGUI to mirror numpad input (ignored in keyboard mode)")]
        public TextMeshProUGUI inputDisplayText;

        // ---- Shared ----
        [Header("Shared UI")]
        [Tooltip("Confirm / submit button")]
        public Button confirmButton;

        [Tooltip("Remaining attempts label")]
        public TextMeshProUGUI attemptsText;

        [Tooltip("Feedback label (shows ✓ / ✕ after submit)")]
        public TextMeshProUGUI feedbackText;

        [Tooltip("Puzzle description label")]
        public TextMeshProUGUI descriptionText;

        [Header("Colors")]
        public Color correctColor   = new Color(0.2f, 0.9f, 0.3f);
        public Color incorrectColor = new Color(0.95f, 0.25f, 0.25f);
        public Color normalColor    = Color.white;

        [Header("Settings")]
        [Tooltip("Max digits for numpad mode. 0 = auto from solution length. Ignored in keyboard mode.")]
        public int maxInputLength = 0;

        #endregion

        #region State

        private string correctCode  = "";
        private string currentInput = "";

        #endregion

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            // ---- Keyboard mode wiring ----
            if (inputField != null)
            {
                // onSubmit fires ONLY when user presses Enter/Return (reliable, unlike onEndEdit)
                inputField.onSubmit.AddListener(_ => OnConfirmClicked());
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            }

            if (clearButton != null)
                clearButton.onClick.AddListener(OnClearClicked);

            // ---- Numpad mode wiring ----
            if (numberButtons != null)
            {
                foreach (Button btn in numberButtons)
                {
                    if (btn == null) continue;
                    TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();
                    string digit = label != null ? label.text.Trim() : "";
                    btn.onClick.AddListener(() => OnDigitPressed(digit));
                }
            }

            if (deleteButton  != null) deleteButton.onClick.AddListener(OnDeletePressed);

            // ---- Shared ----
            if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        protected override void SetupPuzzleUI()
        {
            correctCode = config.GetCodeInputSolution();
            if (string.IsNullOrEmpty(correctCode))
            {
                Debug.LogError($"[CodeInputPuzzle] No valid solution in config: {config.puzzleID}");
                return;
            }

            // Auto max-length from solution if not set in Inspector
            if (maxInputLength <= 0)
                maxInputLength = correctCode.Length;

            currentInput = "";

            // Clear the input field (keyboard mode)
            if (inputField != null)
            {
                inputField.text = "";
                inputField.interactable = true;
            }

            RefreshDisplay();
            ClearFeedback();
            RefreshAttemptsText();

            if (descriptionText != null)
                descriptionText.text = config.description;

            SetAllInteractable(true);
            Debug.Log($"[CodeInputPuzzle] Ready — solution length: {correctCode.Length}");
        }

        #endregion

        #region Input Handling

        // Called by numpad buttons
        private void OnDigitPressed(string digit)
        {
            if (!isActive || isSolved) return;
            if (maxInputLength > 0 && currentInput.Length >= maxInputLength) return;
            if (string.IsNullOrEmpty(digit) || digit.Length != 1 || !char.IsDigit(digit[0])) return;

            currentInput += digit;
            RefreshDisplay();
            if (inputDisplayText != null)
                inputDisplayText.transform.DOScale(1.05f, 0.07f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => inputDisplayText.transform.DOScale(1f, 0.07f));
        }

        // Called by delete / backspace button (numpad mode)
        private void OnDeletePressed()
        {
            if (!isActive || isSolved) return;
            if (currentInput.Length == 0) return;
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            RefreshDisplay();
            ClearFeedback();
        }

        // Called by clear button (keyboard mode)
        private void OnClearClicked()
        {
            if (!isActive || isSolved) return;
            currentInput = "";
            if (inputField != null) inputField.text = "";
            RefreshDisplay();
            ClearFeedback();
        }

        // Called by confirm button OR Enter key (onSubmit)
        private void OnConfirmClicked()
        {
            if (!isActive || isSolved) return;

            // Keyboard mode: read from TMP_InputField
            if (inputField != null)
                currentInput = inputField.text.Trim();

            if (string.IsNullOrEmpty(currentInput))
            {
                ShowFeedback("Vui lòng nhập mã!", incorrectColor);
                return;
            }

            Debug.Log($"[CodeInputPuzzle] Submitting: '{currentInput}'");
            SubmitAnswer(currentInput);
        }

        #endregion

        #region Validation (Abstract Implementation)

        protected override bool ValidatePlayerInput(object playerInput)
        {
            if (!(playerInput is string code))
            {
                Debug.LogError("[CodeInputPuzzle] Invalid input type — expected string");
                return false;
            }
            bool correct = code == correctCode;
            Debug.Log($"[CodeInputPuzzle] '{code}' vs '{correctCode}' → {correct}");
            return correct;
        }

        protected override void ResetPuzzleState()
        {
            currentInput = "";
            if (inputField != null)
            {
                inputField.text = "";
                inputField.interactable = true;
            }
            RefreshDisplay();
            ClearFeedback();
            RefreshAttemptsText();
            SetAllInteractable(true);
        }

        #endregion

        #region Visual Helpers

        private void RefreshDisplay()
        {
            // Keyboard mode: TMP_InputField shows its own text, nothing to mirror
            if (inputField != null) return;

            // Numpad mode: mirror currentInput onto display text
            if (inputDisplayText == null) return;
            if (maxInputLength > 0)
            {
                string blanks = new string('—', maxInputLength - currentInput.Length);
                inputDisplayText.text = currentInput + blanks;
            }
            else
            {
                inputDisplayText.text = currentInput.Length > 0 ? currentInput : "—";
            }
        }

        private void RefreshAttemptsText()
        {
            if (attemptsText == null) return;
            attemptsText.text = config.maxAttempts > 0
                ? $"Còn {config.maxAttempts - currentAttempts} lần thử"
                : "";
        }

        private void ShowFeedback(string message, Color color)
        {
            if (feedbackText == null) return;
            feedbackText.text  = message;
            feedbackText.color = color;
            feedbackText.transform.localScale = Vector3.one;
            feedbackText.transform.DOScale(1.1f, 0.15f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => feedbackText.transform.DOScale(1f, 0.15f));
        }

        private void ClearFeedback()
        {
            if (feedbackText != null) feedbackText.text = "";
        }

        private void ShakeDisplay()
        {
            // Shake whichever display is active
            Transform target = inputField != null
                ? inputField.transform
                : inputDisplayText != null ? inputDisplayText.transform : null;
            if (target == null) return;
            target.DOShakePosition(0.4f, strength: 8f, vibrato: 20)
                .OnComplete(() => target.localPosition = Vector3.zero);
        }

        private void SetAllInteractable(bool value)
        {
            if (inputField  != null) inputField.interactable  = value;
            if (clearButton != null) clearButton.interactable = value;
            if (numberButtons != null)
                foreach (var btn in numberButtons)
                    if (btn != null) btn.interactable = value;
            if (deleteButton  != null) deleteButton.interactable  = value;
            if (confirmButton != null) confirmButton.interactable = value;
        }

        #endregion

        #region Overrides

        protected override void OnPuzzleSolved()
        {
            ShowFeedback("✓ Chính xác!", correctColor);
            // Flash whichever display is active
            if (inputField != null)
            {
                var img = inputField.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    img.DOColor(correctColor, 0.2f).SetLoops(2, LoopType.Yoyo)
                        .OnComplete(() => img.color = normalColor);
            }
            else if (inputDisplayText != null)
            {
                Color orig = inputDisplayText.color;
                inputDisplayText.DOColor(correctColor, 0.2f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => inputDisplayText.color = orig);
            }
            SetAllInteractable(false);
            base.OnPuzzleSolved();
        }

        protected override void OnPuzzleFailed()
        {
            int remaining = config.maxAttempts > 0
                ? config.maxAttempts - (currentAttempts + 1)
                : -1;
            string msg = remaining > 0
                ? $"✕ Sai mã! Còn {remaining} lần thử"
                : "✕ Sai mã!";
            ShowFeedback(msg, incorrectColor);
            ShakeDisplay();
            SetAllInteractable(false);

            DOVirtual.DelayedCall(0.9f, () =>
            {
                if (!isSolved)
                {
                    currentInput = "";
                    if (inputField != null) inputField.text = "";
                    RefreshDisplay();
                    RefreshAttemptsText();
                    SetAllInteractable(true);
                    // Re-focus keyboard input field after reset
                    if (inputField != null)
                        inputField.ActivateInputField();
                }
            });

            base.OnPuzzleFailed();
        }

        #endregion
    }
}
