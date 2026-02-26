using UnityEngine;
using CoderGoHappy.Core;
using CoderGoHappy.Events;

namespace CoderGoHappy.Level
{
    /// <summary>
    /// Manages level state, completion conditions, and progression
    /// Singleton - persists across scenes within a level
    /// Attach to LevelManager GameObject in first scene of each level
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        #region Singleton

        private static LevelManager instance;
        public static LevelManager Instance => instance;

        #endregion

        #region Inspector Fields

        [Header("Level Configuration")]
        [Tooltip("Level number (1, 2, or 3)")]
        [SerializeField] private int levelNumber = 1;

        [Tooltip("Required puzzles to complete level (puzzleIDs)")]
        [SerializeField] private string[] requiredPuzzles;

        [Tooltip("Required items to collect for level completion")]
        [SerializeField] private string[] requiredItems;

        [Tooltip("Total MiniBugs available in this level")]
        [SerializeField] private int totalMiniBugsInLevel = 10;

        [Tooltip("Scene name to load when level is complete")]
        [SerializeField] private string nextLevelSceneName;

        [Header("Character State")]
        [Tooltip("Character GameObject to trigger happy state")]
        [SerializeField] private GameObject characterObject;

        [Tooltip("Happy state animation trigger name (optional)")]
        [SerializeField] private string happyAnimationTrigger = "Happy";

        #endregion

        #region State

        private bool levelCompleted = false;
        private int bugsCollectedThisRun = 0;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton pattern (level-specific, not DontDestroyOnLoad)
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"[LevelManager] Duplicate instance in Level {levelNumber}, destroying...");
                Destroy(gameObject);
                return;
            }

            instance = this;

            Debug.Log($"[LevelManager] Level {levelNumber} manager initialized");
        }

        private void Start()
        {
            // Subscribe to events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Subscribe(GameEvents.PuzzleSolved, OnPuzzleSolved);
                EventManager.Instance.Subscribe(GameEvents.ItemCollected, OnItemCollected);
                EventManager.Instance.Subscribe(GameEvents.MiniBugCollected, OnMiniBugCollected);
            }

            // Check if level already completed
            CheckLevelCompletion();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe(GameEvents.PuzzleSolved, OnPuzzleSolved);
                EventManager.Instance.Unsubscribe(GameEvents.ItemCollected, OnItemCollected);
                EventManager.Instance.Unsubscribe(GameEvents.MiniBugCollected, OnMiniBugCollected);
            }

            // Clear singleton if this instance
            if (instance == this)
            {
                instance = null;
            }
        }

        #endregion

        #region Event Handlers

        private void OnPuzzleSolved(object data)
        {
            Debug.Log($"[LevelManager] Puzzle solved, checking level completion...");
            CheckLevelCompletion();
        }

        private void OnItemCollected(object data)
        {
            Debug.Log($"[LevelManager] Item collected, checking level completion...");
            CheckLevelCompletion();
        }

        private void OnMiniBugCollected(object data)
        {
            bugsCollectedThisRun++;
            Debug.Log($"[LevelManager] MiniBug collected! Total in run: {bugsCollectedThisRun}/{totalMiniBugsInLevel}");

            // Publish event for UI update
            EventManager.Instance?.Publish("BugCounterUpdate", new BugCountData
            {
                collected = GameStateData.Instance.miniBugsCollected,
                total = totalMiniBugsInLevel
            });
        }

        #endregion

        #region Completion Logic

        /// <summary>
        /// Check if all level completion conditions are met
        /// </summary>
        private void CheckLevelCompletion()
        {
            if (levelCompleted)
                return;

            // Check required puzzles
            bool allPuzzlesSolved = CheckRequiredPuzzles();

            // Check required items
            bool allItemsCollected = CheckRequiredItems();

            // Level complete if all conditions met
            if (allPuzzlesSolved && allItemsCollected)
            {
                CompleteLevelmethod();
            }
        }

        /// <summary>
        /// Check if all required puzzles are solved
        /// </summary>
        private bool CheckRequiredPuzzles()
        {
            if (requiredPuzzles == null || requiredPuzzles.Length == 0)
            {
                return true; // No required puzzles
            }

            foreach (string puzzleID in requiredPuzzles)
            {
                if (!GameStateData.Instance.IsPuzzleSolved(puzzleID))
                {
                    Debug.Log($"[LevelManager] Puzzle '{puzzleID}' not yet solved");
                    return false;
                }
            }

            Debug.Log($"[LevelManager] All required puzzles solved!");
            return true;
        }

        /// <summary>
        /// Check if all required items are collected
        /// </summary>
        private bool CheckRequiredItems()
        {
            if (requiredItems == null || requiredItems.Length == 0)
            {
                return true; // No required items
            }

            foreach (string itemID in requiredItems)
            {
                if (!GameStateData.Instance.HasItem(itemID))
                {
                    Debug.Log($"[LevelManager] Item '{itemID}' not yet collected");
                    return false;
                }
            }

            Debug.Log($"[LevelManager] All required items collected!");
            return true;
        }

        /// <summary>
        /// Trigger level completion
        /// </summary>
        private void CompleteLevelmethod()
        {
            if (levelCompleted)
                return;

            levelCompleted = true;

            Debug.Log($"[LevelManager] ★★★ LEVEL {levelNumber} COMPLETE! ★★★");

            // Update game state
            if (GameStateData.Instance != null)
            {
                if (GameStateData.Instance.currentLevel < levelNumber + 1)
                {
                    GameStateData.Instance.currentLevel = levelNumber + 1;
                }
            }

            // Trigger character happy state
            TriggerCharacterHappyState();

            // Publish level complete event
            EventManager.Instance?.Publish(GameEvents.LevelComplete, levelNumber);

            // Auto-save game
            GameManager.Instance?.SaveGame();

            // Transition to next level after delay
            if (!string.IsNullOrEmpty(nextLevelSceneName))
            {
                Invoke(nameof(LoadNextLevel), 3f); // 3 second delay for celebration
            }
        }

        /// <summary>
        /// Trigger character happy animation/state
        /// </summary>
        private void TriggerCharacterHappyState()
        {
            if (characterObject == null)
            {
                Debug.LogWarning("[LevelManager] Character object not assigned!");
                return;
            }

            // Try to trigger animation
            Animator animator = characterObject.GetComponent<Animator>();
            if (animator != null && !string.IsNullOrEmpty(happyAnimationTrigger))
            {
                animator.SetTrigger(happyAnimationTrigger);
                Debug.Log($"[LevelManager] Character happy animation triggered: {happyAnimationTrigger}");
            }

            // Could add particle effects, sound, etc. here
            Debug.Log("[LevelManager] Character entered happy state!");
        }

        /// <summary>
        /// Load next level scene
        /// </summary>
        private void LoadNextLevel()
        {
            if (string.IsNullOrEmpty(nextLevelSceneName))
            {
                Debug.LogWarning("[LevelManager] No next level scene configured!");
                return;
            }

            Debug.Log($"[LevelManager] Loading next level: {nextLevelSceneName}");

            // Use SceneController for transition
            CoderGoHappy.Scene.SceneController sceneController = UnityEngine.Object.FindFirstObjectByType<CoderGoHappy.Scene.SceneController>();
            if (sceneController != null)
            {
                sceneController.TransitionToScene(nextLevelSceneName, 0);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelSceneName);
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Get current level number
        /// </summary>
        public int GetLevelNumber() => levelNumber;

        /// <summary>
        /// Check if level is completed
        /// </summary>
        public bool IsLevelCompleted() => levelCompleted;

        /// <summary>
        /// Get MiniBug collection progress
        /// </summary>
        public (int collected, int total) GetMiniBugProgress()
        {
            return (GameStateData.Instance.miniBugsCollected, totalMiniBugsInLevel);
        }

        /// <summary>
        /// Force level completion (debug/testing)
        /// </summary>
        [ContextMenu("Force Complete Level")]
        public void ForceCompleteLevel()
        {
            Debug.Log("[LevelManager] DEBUG: Forcing level completion!");
            CompleteLevelmethod();
        }

        #endregion
    }

    /// <summary>
    /// Data structure for bug counter events
    /// </summary>
    public struct BugCountData
    {
        public int collected;
        public int total;
    }
}
