using UnityEngine;
using System;
using CoderGoHappy.Data;

namespace CoderGoHappy.Puzzle
{
    /// <summary>
    /// PuzzleType defines the type of puzzle logic to execute
    /// </summary>
    public enum PuzzleType
    {
        ButtonSequence,  // Player must click buttons in correct order
        CodeInput,       // Player must enter numeric code
        ColorMatch       // Player must match colors/patterns
    }

    /// <summary>
    /// ScriptableObject that defines puzzle configuration and solution data.
    /// Created via Assets -> Create -> Coder Go Happy -> Puzzle Config.
    /// Referenced by HotspotComponent (type = Puzzle) and PuzzleSystem.
    /// </summary>
    [CreateAssetMenu(fileName = "Puzzle_", menuName = "Coder Go Happy/Puzzle Config", order = 2)]
    public class PuzzleConfig : ScriptableObject
    {
        #region Inspector Fields

        /// <summary>
        /// Unique identifier for this puzzle.
        /// Auto-generated from asset name in OnValidate.
        /// Used as key in GameStateData.solvedPuzzleIDs.
        /// </summary>
        [Header("Puzzle Identity")]
        [Tooltip("Unique ID for this puzzle (auto-generated from asset name)")]
        public string puzzleID = "";

        /// <summary>
        /// Display name shown in UI
        /// </summary>
        [Tooltip("Display name shown in puzzle UI")]
        public string puzzleName = "New Puzzle";

        /// <summary>
        /// Description or hint text for the puzzle
        /// </summary>
        [TextArea(3, 5)]
        [Tooltip("Description/hint shown to player")]
        public string description = "Solve this puzzle to progress.";

        /// <summary>
        /// Type of puzzle determines which concrete puzzle class to instantiate
        /// </summary>
        [Header("Puzzle Configuration")]
        [Tooltip("Type of puzzle logic to use")]
        public PuzzleType puzzleType = PuzzleType.ButtonSequence;

        /// <summary>
        /// Difficulty level (1-5). Used for hint system or UI display.
        /// </summary>
        [Range(1, 5)]
        [Tooltip("Difficulty level (1 = easy, 5 = hard)")]
        public int difficulty = 1;

        /// <summary>
        /// Solution data varies by puzzle type:
        /// - ButtonSequence: comma-separated button indices (e.g., "0,2,1,3")
        /// - CodeInput: numeric code as string (e.g., "1234")
        /// - ColorMatch: comma-separated color names or indices (e.g., "Red,Blue,Green")
        /// </summary>
        [Header("Solution Data")]
        [Tooltip("Solution format depends on puzzle type - see tooltip")]
        public string solution = "";

        /// <summary>
        /// Max attempts allowed before puzzle resets (0 = unlimited)
        /// </summary>
        [Tooltip("Max attempts before reset (0 = unlimited)")]
        public int maxAttempts = 0;

        /// <summary>
        /// Time limit in seconds (0 = no limit)
        /// </summary>
        [Tooltip("Time limit in seconds (0 = no time limit)")]
        public float timeLimit = 0f;

        /// <summary>
        /// Optional ItemData reference that gets added to inventory when puzzle is solved
        /// </summary>
        [Header("Rewards")]
        [Tooltip("Item rewarded when puzzle is solved (optional)")]
        public ItemData rewardItem = null;

        /// <summary>
        /// Event name to publish when puzzle is solved (e.g., "UnlockDoor_Puzzle01")
        /// Leave empty if puzzle completion is tracked only in GameState
        /// </summary>
        [Tooltip("Custom event to publish on solve (optional)")]
        public string successEvent = "";

        #endregion

        #region Helper Methods

        /// <summary>
        /// Parse solution string for ButtonSequence puzzle type.
        /// Returns array of button indices.
        /// </summary>
        public int[] GetButtonSequenceSolution()
        {
            if (string.IsNullOrEmpty(solution))
                return new int[0];

            try
            {
                string[] parts = solution.Split(',');
                int[] sequence = new int[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    sequence[i] = int.Parse(parts[i].Trim());
                }
                return sequence;
            }
            catch (Exception e)
            {
                Debug.LogError($"[PuzzleConfig] Failed to parse ButtonSequence solution '{solution}': {e.Message}");
                return new int[0];
            }
        }

        /// <summary>
        /// Get solution for CodeInput puzzle (returns string as-is)
        /// </summary>
        public string GetCodeInputSolution()
        {
            return solution;
        }

        /// <summary>
        /// Parse solution string for ColorMatch puzzle type.
        /// Returns array of color names.
        /// </summary>
        public string[] GetColorMatchSolution()
        {
            if (string.IsNullOrEmpty(solution))
                return new string[0];

            string[] colors = solution.Split(',');
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = colors[i].Trim();
            }
            return colors;
        }

        /// <summary>
        /// Validate configuration in Inspector
        /// </summary>
        private void OnValidate()
        {
            // Auto-generate puzzleID from asset name
            if (string.IsNullOrEmpty(puzzleID) || puzzleID != name)
            {
                puzzleID = name;
            }

            // Validate solution format based on puzzle type
            switch (puzzleType)
            {
                case PuzzleType.ButtonSequence:
                    // Validate it's comma-separated integers
                    if (!string.IsNullOrEmpty(solution))
                    {
                        string[] parts = solution.Split(',');
                        foreach (string part in parts)
                        {
                            if (!int.TryParse(part.Trim(), out _))
                            {
                                Debug.LogWarning($"[PuzzleConfig] {name}: ButtonSequence solution should be comma-separated integers (e.g., '0,2,1,3')");
                                break;
                            }
                        }
                    }
                    break;

                case PuzzleType.CodeInput:
                    // Validate it's numeric (could be any length)
                    if (!string.IsNullOrEmpty(solution))
                    {
                        if (!int.TryParse(solution, out _))
                        {
                            Debug.LogWarning($"[PuzzleConfig] {name}: CodeInput solution should be numeric (e.g., '1234')");
                        }
                    }
                    break;

                case PuzzleType.ColorMatch:
                    // Just check it has comma-separated values
                    if (!string.IsNullOrEmpty(solution) && !solution.Contains(","))
                    {
                        Debug.LogWarning($"[PuzzleConfig] {name}: ColorMatch solution should be comma-separated colors (e.g., 'Red,Blue,Green')");
                    }
                    break;
            }
        }

        #endregion
    }
}
