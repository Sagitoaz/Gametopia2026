using UnityEngine;
using UnityEngine.UI;
using System;
using CoderGoHappy.Events;
using CoderGoHappy.Core;
using CoderGoHappy.Data;
using CoderGoHappy.Inventory;

namespace CoderGoHappy.Puzzle
{
    /// <summary>
    /// Abstract base class for all puzzle implementations.
    /// Concrete puzzle types (ButtonSequence, CodeInput, ColorMatch) inherit from this.
    /// Handles common logic: timer, attempts, state tracking, event publishing.
    /// Concrete classes must implement: SetupPuzzleUI(), ValidatePlayerInput(), ResetPuzzleState().
    /// </summary>
    public abstract class PuzzleBase : MonoBehaviour
    {
        #region Inspector Fields

        /// <summary>
        /// Configuration data for this puzzle instance
        /// </summary>
        [Header("Puzzle Configuration")]
        [Tooltip("PuzzleConfig ScriptableObject defining this puzzle")]
        public PuzzleConfig config;

        /// <summary>
        /// UI GameObject that contains puzzle interface (will be shown/hidden)
        /// </summary>
        [Tooltip("UI GameObject containing puzzle interface")]
        public GameObject puzzleUI;

        /// <summary>
        /// Optional X/close button inside the puzzle panel
        /// </summary>
        [Header("Close Options")]
        [Tooltip("Optional X/Close button inside the puzzle panel - clicking closes the puzzle")]
        public Button closeButton;

        /// <summary>
        /// Full-screen transparent Button that sits BEHIND the puzzle panel.
        /// Clicking anywhere outside the panel area triggers HidePuzzle().
        /// </summary>
        [Tooltip("Full-screen transparent button behind the panel (click outside to close)")]
        public Button backgroundOverlay;

        /// <summary>
        /// Allow pressing Escape to close puzzle without solving it
        /// </summary>
        [Tooltip("Allow Escape key to close puzzle panel")]
        public bool allowEscapeToClose = true;

        #endregion

        #region State

        /// <summary>
        /// Is puzzle currently active (UI shown, accepting input)?
        /// </summary>
        protected bool isActive = false;

        /// <summary>
        /// Has puzzle been solved?
        /// </summary>
        protected bool isSolved = false;

        /// <summary>
        /// Current number of failed attempts
        /// </summary>
        protected int currentAttempts = 0;

        /// <summary>
        /// Time remaining if puzzle has time limit (seconds)
        /// </summary>
        protected float timeRemaining = 0f;

        /// <summary>
        /// Is timer running?
        /// </summary>
        protected bool timerActive = false;

        /// <summary>
        /// Reference to EventManager (cached)
        /// </summary>
        protected EventManager eventManager;

        /// <summary>
        /// CanvasGroup for show/hide (auto-created if puzzleUI == gameObject)
        /// </summary>
        protected CanvasGroup canvasGroup;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            // Cache EventManager reference
            eventManager = EventManager.Instance;

            // Validate configuration
            if (config == null)
            {
                Debug.LogError($"[PuzzleBase] {gameObject.name}: No PuzzleConfig assigned!");
            }

            // Hide UI initially
            if (puzzleUI != null && puzzleUI != gameObject)
            {
                puzzleUI.SetActive(false);
            }
            else if (puzzleUI == gameObject)
            {
                // If puzzleUI is same as script holder, use CanvasGroup to hide (preserves LayoutGroup)
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                
                // Hide using CanvasGroup (keeps children active, preserves layouts)
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        protected virtual void Update()
        {
            // Close puzzle on Escape key (only when active and not already solved)
            if (isActive && !isSolved && allowEscapeToClose && Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log($"[PuzzleBase] Escape pressed - closing puzzle: {config?.puzzleName}");
                HidePuzzle();
                return;
            }

            // Update timer if active
            if (timerActive && config != null && config.timeLimit > 0)
            {
                timeRemaining -= Time.deltaTime;

                if (timeRemaining <= 0f)
                {
                    OnTimerExpired();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show puzzle UI and initialize puzzle state.
        /// Called by PuzzleSystem when ShowPuzzle event is triggered.
        /// </summary>
        public virtual void ShowPuzzle()
        {
            Debug.Log($"[PuzzleBase] ShowPuzzle() called on {gameObject.name}");
            
            if (config == null)
            {
                Debug.LogError($"[PuzzleBase] Cannot show puzzle - no config assigned!");
                return;
            }

            // Check if already solved (from GameStateData)
            if (GameStateData.Instance.IsPuzzleSolved(config.puzzleID))
            {
                Debug.Log($"[PuzzleBase] Puzzle '{config.puzzleID}' already solved - skipping");
                OnPuzzleAlreadySolved();
                return;
            }

            Debug.Log($"[PuzzleBase] Showing puzzle: {config.puzzleName}");

            // Reset state
            isActive = true;
            isSolved = false;
            currentAttempts = 0;

            // Start timer if configured
            if (config.timeLimit > 0)
            {
                timeRemaining = config.timeLimit;
                timerActive = true;
            }

            // Show UI
            Debug.Log($"[PuzzleBase] puzzleUI = {(puzzleUI != null ? puzzleUI.name : "NULL")}, gameObject = {gameObject.name}, same? {puzzleUI == gameObject}");
            
            if (puzzleUI != null)
            {
                 puzzleUI.SetActive(true);
                if (puzzleUI != gameObject)
                {
                    // Different object - use SetActive
                    puzzleUI.SetActive(true);
                }
                else if (canvasGroup != null)
                {
                    // Same object - use CanvasGroup (preserves LayoutGroups)
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
                
                Debug.Log($"[PuzzleBase] PuzzleUI activated: {puzzleUI.name}, activeInHierarchy: {puzzleUI.activeInHierarchy}");
            }
            else
            {
                Debug.LogWarning($"[PuzzleBase] puzzleUI is NULL! Puzzle UI won't be visible.");
            }

            // Let concrete puzzle set up its specific UI
            SetupPuzzleUI();

            // Wire up background overlay (click outside to close)
            if (backgroundOverlay != null)
            {
                backgroundOverlay.gameObject.SetActive(true);
                backgroundOverlay.onClick.RemoveAllListeners();
                backgroundOverlay.onClick.AddListener(HidePuzzle);
            }

            // Wire up explicit close/X button
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(HidePuzzle);
            }

            // Note: Don't publish ShowPuzzle event here - it would cause recursion
            // with PuzzleSystem.OnShowPuzzleEvent
        }

        /// <summary>
        /// Hide puzzle UI and deactivate puzzle.
        /// Called when player closes puzzle or solves it.
        /// </summary>
        public virtual void HidePuzzle()
        {
            Debug.Log($"[PuzzleBase] Hiding puzzle: {config?.puzzleName}");

            isActive = false;
            timerActive = false;

            // Hide UI
            if (puzzleUI != null)
            {
                 puzzleUI.SetActive(false);
                if (puzzleUI != gameObject)
                {
                    // Different object - use SetActive
                    puzzleUI.SetActive(false);
                }
                else if (canvasGroup != null)
                {
                    // Same object - use CanvasGroup (preserves LayoutGroups)
                    canvasGroup.alpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }

            // Hide background overlay
            if (backgroundOverlay != null)
            {
                backgroundOverlay.gameObject.SetActive(false);
            }

            // Publish event
            if (config != null)
            {
                eventManager?.Publish(GameEvents.HidePuzzle, config.puzzleID);
            }
        }

        /// <summary>
        /// Submit player's input for validation.
        /// Called by concrete puzzle UI when player confirms their answer.
        /// </summary>
        /// <param name="playerInput">Player's answer (format depends on puzzle type)</param>
        public void SubmitAnswer(object playerInput)
        {
            if (!isActive || isSolved)
            {
                Debug.LogWarning($"[PuzzleBase] Cannot submit answer - puzzle not active or already solved");
                return;
            }

            Debug.Log($"[PuzzleBase] Validating answer for: {config.puzzleName}");

            // Let concrete puzzle validate the input
            bool isCorrect = ValidatePlayerInput(playerInput);

            if (isCorrect)
            {
                OnPuzzleSolved();
            }
            else
            {
                OnPuzzleFailed();
            }
        }

        /// <summary>
        /// Reset puzzle to initial state.
        /// Called when player fails max attempts or timer expires.
        /// </summary>
        public virtual void ResetPuzzle()
        {
            Debug.Log($"[PuzzleBase] Resetting puzzle: {config?.puzzleName}");

            currentAttempts = 0;
            
            if (config != null && config.timeLimit > 0)
            {
                timeRemaining = config.timeLimit;
                timerActive = true;
            }

            // Let concrete puzzle reset its specific state
            ResetPuzzleState();
        }

        #endregion

        #region Abstract Methods (Must be implemented by concrete puzzles)

        /// <summary>
        /// Setup puzzle-specific UI elements.
        /// Called when ShowPuzzle() is invoked.
        /// Concrete puzzles should initialize buttons, input fields, etc.
        /// </summary>
        protected abstract void SetupPuzzleUI();

        /// <summary>
        /// Validate player's input against solution.
        /// Called when SubmitAnswer() is invoked.
        /// </summary>
        /// <param name="playerInput">Player's answer (type varies: int[], string, etc.)</param>
        /// <returns>True if answer is correct</returns>
        protected abstract bool ValidatePlayerInput(object playerInput);

        /// <summary>
        /// Reset puzzle-specific state.
        /// Called when ResetPuzzle() is invoked.
        /// Concrete puzzles should clear input fields, reset button states, etc.
        /// </summary>
        protected abstract void ResetPuzzleState();

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when player solves the puzzle correctly
        /// </summary>
        protected virtual void OnPuzzleSolved()
        {
            Debug.Log($"[PuzzleBase] Puzzle solved: {config.puzzleName}");

            isSolved = true;
            isActive = false;
            timerActive = false;

            // Mark as solved in GameState
            GameStateData.Instance.MarkPuzzleSolved(config.puzzleID);

            // Give reward item if configured
            if (config.rewardItem != null)
            {
                Debug.Log($"[PuzzleBase] Rewarding item: {config.rewardItem.itemName}");
                
                // Find InventorySystem and add item
                var inventorySystem = FindFirstObjectByType<InventorySystem>();
                if (inventorySystem != null)
                {
                    inventorySystem.AddItem(config.rewardItem);
                }
            }

            // Publish success event
            eventManager?.Publish(GameEvents.PuzzleSolved, config.puzzleID);

            // Publish custom event if configured
            if (!string.IsNullOrEmpty(config.successEvent))
            {
                eventManager?.Publish(config.successEvent, config.puzzleID);
            }

            // Play success feedback (override in concrete class for custom effects)
            PlaySuccessFeedback();

            // Auto-hide after short delay
            Invoke(nameof(HidePuzzle), 1.5f);
        }

        /// <summary>
        /// Called when player fails to solve the puzzle
        /// </summary>
        protected virtual void OnPuzzleFailed()
        {
            currentAttempts++;

            Debug.Log($"[PuzzleBase] Puzzle failed - Attempt {currentAttempts}/{config.maxAttempts}");

            // Play failure feedback
            PlayFailureFeedback();

            // Check if max attempts reached
            if (config.maxAttempts > 0 && currentAttempts >= config.maxAttempts)
            {
                Debug.Log($"[PuzzleBase] Max attempts reached - resetting puzzle");
                ResetPuzzle();
            }
            

            // Publish event
            eventManager?.Publish(GameEvents.PuzzleFailed, config.puzzleID);

            
        }

        /// <summary>
        /// Called when puzzle was already solved (loaded from save data)
        /// </summary>
        protected virtual void OnPuzzleAlreadySolved()
        {
            // Just close immediately - puzzle is already complete
            Debug.Log($"[PuzzleBase] Puzzle already completed");
        }

        /// <summary>
        /// Called when timer expires
        /// </summary>
        protected virtual void OnTimerExpired()
        {
            Debug.Log($"[PuzzleBase] Timer expired for: {config.puzzleName}");

            timerActive = false;
            timeRemaining = 0f;

            // Treat as failure
            OnPuzzleFailed();
        }

        #endregion

        #region Feedback Methods (Can be overridden for custom effects)

        /// <summary>
        /// Play visual/audio feedback for successful solve.
        /// Override in concrete classes for custom effects.
        /// </summary>
        protected virtual void PlaySuccessFeedback()
        {
            Debug.Log($"[PuzzleBase] ✓ SUCCESS!");
            // TODO: Play success sound, particle effects, etc.
        }

        /// <summary>
        /// Play visual/audio feedback for failed attempt.
        /// Override in concrete classes for custom effects.
        /// </summary>
        protected virtual void PlayFailureFeedback()
        {
            Debug.Log($"[PuzzleBase] ✗ FAILED");
            // TODO: Play error sound, shake effect, etc.
        }

        #endregion

        #region Public Accessors

        /// <summary>
        /// Is puzzle currently active?
        /// </summary>
        public bool IsActive => isActive;

        /// <summary>
        /// Is puzzle solved?
        /// </summary>
        public bool IsSolved => isSolved;

        /// <summary>
        /// Get current attempts count
        /// </summary>
        public int CurrentAttempts => currentAttempts;

        /// <summary>
        /// Get remaining time (0 if no timer)
        /// </summary>
        public float TimeRemaining => timeRemaining;

        /// <summary>
        /// Get puzzle config
        /// </summary>
        public PuzzleConfig Config => config;

        #endregion
    }
}
