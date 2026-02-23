using UnityEngine;

namespace CoderGoHappy.Events
{
    /// <summary>
    /// Central repository for all game event names
    /// Use these constants to ensure consistent event naming across the codebase
    /// </summary>
    public static class GameEvents
    {
        // Inventory Events
        public const string ItemCollected = "ItemCollected";
        public const string ItemUsed = "ItemUsed";
        public const string ItemSelected = "ItemSelected";
        public const string ItemDeselected = "ItemDeselected";
        public const string InventoryUpdated = "InventoryUpdated";
        
        // Puzzle Events
        public const string PuzzleSolved = "PuzzleSolved";
        public const string ShowPuzzle = "ShowPuzzle";
        
        // Scene Events
        public const string SceneTransitionStart = "SceneTransitionStart";
        public const string SceneTransitionComplete = "SceneTransitionComplete";
        
        // Game State Events
        public const string LevelComplete = "LevelComplete";
        public const string GamePaused = "GamePaused";
        public const string GameResumed = "GameResumed";
        public const string GameSaved = "GameSaved";
        public const string GameLoaded = "GameLoaded";
        
        // Interaction Events
        public const string HotspotTriggered = "HotspotTriggered";
        public const string MiniBugCollected = "MiniBugCollected";
    }
}
