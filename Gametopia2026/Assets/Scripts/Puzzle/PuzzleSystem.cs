using UnityEngine;
using System.Collections.Generic;
using CoderGoHappy.Events;
using CoderGoHappy.Core;

namespace CoderGoHappy.Puzzle
{
    /// <summary>
    /// PuzzleSystem manages all puzzle instances in the game.
    /// Listens for ShowPuzzle/HidePuzzle events from EventManager.
    /// Coordinates puzzle lifecycle: show/hide/reset.
    /// Maintains registry of all available puzzles in scene.
    /// Should be attached to a persistent GameObject (same as GameManager).
    /// </summary>
    public class PuzzleSystem : MonoBehaviour
    {
        #region Singleton

        private static PuzzleSystem instance;
        public static PuzzleSystem Instance => instance;

        #endregion

        #region Inspector Fields

        /// <summary>
        /// All puzzle instances available in the scene.
        /// Assign in Inspector or auto-discover in Awake.
        /// </summary>
        [Header("Puzzle Registry")]
        [Tooltip("All puzzle instances in the scene (can be auto-discovered)")]
        public PuzzleBase[] puzzles;

        /// <summary>
        /// Auto-discover puzzles in scene on Awake?
        /// If true, will find all PuzzleBase components in scene.
        /// </summary>
        [Tooltip("Auto-find all PuzzleBase components in scene on start")]
        public bool autoDiscoverPuzzles = true;

        #endregion

        #region State

        /// <summary>
        /// Dictionary mapping puzzleID to PuzzleBase instance for fast lookup
        /// </summary>
        private Dictionary<string, PuzzleBase> puzzleRegistry = new Dictionary<string, PuzzleBase>();

        /// <summary>
        /// Currently active puzzle (null if none)
        /// </summary>
        private PuzzleBase currentPuzzle = null;

        /// <summary>
        /// Reference to EventManager (cached)
        /// </summary>
        private EventManager eventManager;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton setup
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"[PuzzleSystem] Duplicate instance found - destroying {gameObject.name}");
                Destroy(gameObject);
                return;
            }

            instance = this;

            // Cache EventManager
            eventManager = EventManager.Instance;

            // Auto-discover puzzles if enabled
            if (autoDiscoverPuzzles)
            {
                DiscoverPuzzles();
            }

            // Register puzzles
            RegisterPuzzles();

            Debug.Log($"[PuzzleSystem] Initialized with {puzzleRegistry.Count} puzzles");
        }

        private void OnEnable()
        {
            // Subscribe to events
            if (eventManager != null)
            {
                eventManager.Subscribe(GameEvents.ShowPuzzle, OnShowPuzzleEvent);
                eventManager.Subscribe(GameEvents.HidePuzzle, OnHidePuzzleEvent);
                eventManager.Subscribe(GameEvents.PuzzleSolved, OnPuzzleSolvedEvent);
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            if (eventManager != null)
            {
                eventManager.Unsubscribe(GameEvents.ShowPuzzle, OnShowPuzzleEvent);
                eventManager.Unsubscribe(GameEvents.HidePuzzle, OnHidePuzzleEvent);
                eventManager.Unsubscribe(GameEvents.PuzzleSolved, OnPuzzleSolvedEvent);
            }
        }

        #endregion

        #region Puzzle Discovery & Registration

        /// <summary>
        /// Auto-discover all PuzzleBase components in scene
        /// </summary>
        private void DiscoverPuzzles()
        {
            Debug.Log($"[PuzzleSystem] Auto-discovering puzzles in scene...");

            PuzzleBase[] foundPuzzles = FindObjectsByType<PuzzleBase>(FindObjectsSortMode.None);

            if (foundPuzzles.Length > 0)
            {
                puzzles = foundPuzzles;
                Debug.Log($"[PuzzleSystem] Discovered {foundPuzzles.Length} puzzle(s)");
            }
            else
            {
                Debug.LogWarning($"[PuzzleSystem] No puzzles found in scene");
            }
        }

        /// <summary>
        /// Register all puzzles in the registry dictionary
        /// </summary>
        private void RegisterPuzzles()
        {
            puzzleRegistry.Clear();

            if (puzzles == null || puzzles.Length == 0)
            {
                Debug.LogWarning($"[PuzzleSystem] No puzzles to register");
                return;
            }

            foreach (PuzzleBase puzzle in puzzles)
            {
                if (puzzle == null)
                    continue;

                if (puzzle.Config == null)
                {
                    Debug.LogError($"[PuzzleSystem] Puzzle {puzzle.gameObject.name} has no PuzzleConfig assigned - skipping");
                    continue;
                }

                string puzzleID = puzzle.Config.puzzleID;

                if (puzzleRegistry.ContainsKey(puzzleID))
                {
                    Debug.LogError($"[PuzzleSystem] Duplicate puzzleID '{puzzleID}' - skipping {puzzle.gameObject.name}");
                    continue;
                }

                puzzleRegistry[puzzleID] = puzzle;
                Debug.Log($"[PuzzleSystem] Registered puzzle: {puzzleID} ({puzzle.GetType().Name})");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show a puzzle by its puzzleID.
        /// Hides current puzzle if one is active.
        /// </summary>
        /// <param name="puzzleID">ID from PuzzleConfig</param>
        public void ShowPuzzle(string puzzleID)
        {
            if (string.IsNullOrEmpty(puzzleID))
            {
                Debug.LogError($"[PuzzleSystem] Cannot show puzzle - null or empty puzzleID");
                return;
            }

            Debug.Log($"[PuzzleSystem] ShowPuzzle request: {puzzleID}");

            // Find puzzle in registry
            if (!puzzleRegistry.TryGetValue(puzzleID, out PuzzleBase puzzle))
            {
                Debug.LogError($"[PuzzleSystem] Puzzle not found: {puzzleID}");
                return;
            }

            // Hide current puzzle if any
            if (currentPuzzle != null && currentPuzzle.IsActive)
            {
                Debug.Log($"[PuzzleSystem] Hiding current puzzle: {currentPuzzle.Config.puzzleID}");
                currentPuzzle.HidePuzzle();
            }

            // Show new puzzle
            currentPuzzle = puzzle;
            currentPuzzle.ShowPuzzle();
        }

        /// <summary>
        /// Hide the currently active puzzle
        /// </summary>
        public void HideCurrentPuzzle()
        {
            if (currentPuzzle != null && currentPuzzle.IsActive)
            {
                Debug.Log($"[PuzzleSystem] Hiding current puzzle: {currentPuzzle.Config.puzzleID}");
                currentPuzzle.HidePuzzle();
                currentPuzzle = null;
            }
        }

        /// <summary>
        /// Hide a specific puzzle by ID
        /// </summary>
        /// <param name="puzzleID">ID from PuzzleConfig</param>
        public void HidePuzzle(string puzzleID)
        {
            if (string.IsNullOrEmpty(puzzleID))
            {
                Debug.LogError($"[PuzzleSystem] Cannot hide puzzle - null or empty puzzleID");
                return;
            }

            Debug.Log($"[PuzzleSystem] HidePuzzle request: {puzzleID}");

            // Find puzzle in registry
            if (!puzzleRegistry.TryGetValue(puzzleID, out PuzzleBase puzzle))
            {
                Debug.LogError($"[PuzzleSystem] Puzzle not found: {puzzleID}");
                return;
            }

            // Hide if active
            if (puzzle.IsActive)
            {
                puzzle.HidePuzzle();

                // Clear current if it was this puzzle
                if (currentPuzzle == puzzle)
                {
                    currentPuzzle = null;
                }
            }
        }

        /// <summary>
        /// Get a puzzle instance by ID
        /// </summary>
        /// <param name="puzzleID">ID from PuzzleConfig</param>
        /// <returns>PuzzleBase instance or null if not found</returns>
        public PuzzleBase GetPuzzle(string puzzleID)
        {
            puzzleRegistry.TryGetValue(puzzleID, out PuzzleBase puzzle);
            return puzzle;
        }

        /// <summary>
        /// Check if a puzzle is solved (from GameState)
        /// </summary>
        /// <param name="puzzleID">ID from PuzzleConfig</param>
        /// <returns>True if puzzle was solved</returns>
        public bool IsPuzzleSolved(string puzzleID)
        {
            return GameStateData.Instance.IsPuzzleSolved(puzzleID);
        }

        /// <summary>
        /// Reset a puzzle to its initial state
        /// </summary>
        /// <param name="puzzleID">ID from PuzzleConfig</param>
        public void ResetPuzzle(string puzzleID)
        {
            if (!puzzleRegistry.TryGetValue(puzzleID, out PuzzleBase puzzle))
            {
                Debug.LogError($"[PuzzleSystem] Cannot reset puzzle - not found: {puzzleID}");
                return;
            }

            Debug.Log($"[PuzzleSystem] Resetting puzzle: {puzzleID}");
            puzzle.ResetPuzzle();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handle ShowPuzzle event from EventManager.
        /// Published by HotspotComponent when type = Puzzle.
        /// </summary>
        private void OnShowPuzzleEvent(object data)
        {
            if (data is string puzzleID)
            {
                ShowPuzzle(puzzleID);
            }
            else
            {
                Debug.LogError($"[PuzzleSystem] ShowPuzzle event data is not string: {data?.GetType()}");
            }
        }

        /// <summary>
        /// Handle HidePuzzle event from EventManager
        /// </summary>
        private void OnHidePuzzleEvent(object data)
        {
            if (data is string puzzleID)
            {
                HidePuzzle(puzzleID);
            }
            else
            {
                // If no specific ID, hide current
                HideCurrentPuzzle();
            }
        }

        /// <summary>
        /// Handle PuzzleSolved event from EventManager
        /// </summary>
        private void OnPuzzleSolvedEvent(object data)
        {
            string puzzleID = data as string;
            Debug.Log($"[PuzzleSystem] Puzzle solved: {puzzleID}");

            // Could trigger additional logic here (achievements, analytics, etc.)
        }

        #endregion

        #region Public Accessors

        /// <summary>
        /// Get currently active puzzle
        /// </summary>
        public PuzzleBase CurrentPuzzle => currentPuzzle;

        /// <summary>
        /// Get count of registered puzzles
        /// </summary>
        public int PuzzleCount => puzzleRegistry.Count;

        /// <summary>
        /// Get all registered puzzle IDs
        /// </summary>
        public string[] GetAllPuzzleIDs()
        {
            string[] ids = new string[puzzleRegistry.Keys.Count];
            puzzleRegistry.Keys.CopyTo(ids, 0);
            return ids;
        }

        #endregion

        #region Debug

        /// <summary>
        /// Draw debug info in Scene view
        /// </summary>
        private void OnDrawGizmos()
        {
            // Optional: Draw info about registered puzzles
        }

        #endregion
    }
}
