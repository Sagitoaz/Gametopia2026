using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoderGoHappy.Core
{
    /// <summary>
    /// Singleton data class for game state persistence
    /// Contains all data that needs to be saved/loaded
    /// </summary>
    [Serializable]
    public class GameStateData : MonoBehaviour
    {
        #region Singleton
        
        private static GameStateData instance;
        public static GameStateData Instance => instance;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[GameStateData] Duplicate instance detected, destroying...");
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            Debug.Log("[GameStateData] Initialized");
        }
        
        #endregion
        
        #region Fields
        
        /// <summary>
        /// Current level number (1-3)
        /// </summary>
        public int currentLevel = 1;
        
        /// <summary>
        /// Current scene name within the level
        /// </summary>
        public string currentScene = "MainMenu";
        
        /// <summary>
        /// List of collected item IDs
        /// </summary>
        public List<string> collectedItemIDs = new List<string>();
        
        /// <summary>
        /// List of solved puzzle IDs
        /// </summary>
        public List<string> solvedPuzzleIDs = new List<string>();
        
        /// <summary>
        /// Dictionary of scene states (scene name → serialized state)
        /// </summary>
        public Dictionary<string, string> sceneStates = new Dictionary<string, string>();
        
        /// <summary>
        /// Total playtime in seconds
        /// </summary>
        public float totalPlayTime = 0f;
        
        /// <summary>
        /// Number of MiniBugs collected
        /// </summary>
        public int miniBugsCollected = 0;
        
        /// <summary>
        /// Last save timestamp
        /// </summary>
        public string lastSaveTime = "";
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Serialize game state to JSON string
        /// </summary>
        /// <returns>JSON string representation of game state</returns>
        public string Serialize()
        {
            try
            {
                // Update last save time
                lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                
                // Use Unity's JsonUtility for serialization
                string json = JsonUtility.ToJson(this, prettyPrint: true);
                return json;
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameStateData] Failed to serialize: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Deserialize game state from JSON string
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[GameStateData] Cannot deserialize null or empty JSON");
                return false;
            }
            
            try
            {
                // Overwrite this instance with loaded data
                JsonUtility.FromJsonOverwrite(json, this);
                Debug.Log($"[GameStateData] Successfully loaded game state from {lastSaveTime}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameStateData] Failed to deserialize: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Reset game state to default values (new game)
        /// </summary>
        public void Reset()
        {
            currentLevel = 1;
            currentScene = "MainMenu";
            collectedItemIDs.Clear();
            solvedPuzzleIDs.Clear();
            sceneStates.Clear();
            totalPlayTime = 0f;
            miniBugsCollected = 0;
            lastSaveTime = "";
            
            Debug.Log("[GameStateData] Game state reset to defaults");
        }
        
        /// <summary>
        /// Check if an item has been collected
        /// </summary>
        /// <param name="itemID">Item ID to check</param>
        /// <returns>True if item is in collected list</returns>
        public bool HasItem(string itemID)
        {
            return collectedItemIDs.Contains(itemID);
        }
        
        /// <summary>
        /// Check if a puzzle has been solved
        /// </summary>
        /// <param name="puzzleID">Puzzle ID to check</param>
        /// <returns>True if puzzle is in solved list</returns>
        public bool IsPuzzleSolved(string puzzleID)
        {
            return solvedPuzzleIDs.Contains(puzzleID);
        }
        
        /// <summary>
        /// Add an item to collected list
        /// </summary>
        /// <param name="itemID">Item ID to add</param>
        public void AddCollectedItem(string itemID)
        {
            if (!collectedItemIDs.Contains(itemID))
            {
                collectedItemIDs.Add(itemID);
            }
        }
        
        /// <summary>
        /// Mark a puzzle as solved
        /// </summary>
        /// <param name="puzzleID">Puzzle ID to mark as solved</param>
        public void MarkPuzzleSolved(string puzzleID)
        {
            if (!solvedPuzzleIDs.Contains(puzzleID))
            {
                solvedPuzzleIDs.Add(puzzleID);
            }
        }
        
        #endregion
    }
}
