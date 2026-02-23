using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoderGoHappy.Scene
{
    /// <summary>
    /// Data structure representing the state of a single scene
    /// Serializable for save/load persistence
    /// </summary>
    [Serializable]
    public class SceneState
    {
        #region Fields
        
        /// <summary>
        /// Name of the scene this state represents
        /// </summary>
        public string sceneName;
        
        /// <summary>
        /// List of collected item IDs in this scene
        /// </summary>
        public List<string> collectedItemIDs = new List<string>();
        
        /// <summary>
        /// List of solved puzzle IDs in this scene
        /// </summary>
        public List<string> solvedPuzzleIDs = new List<string>();
        
        /// <summary>
        /// List of disabled hotspot IDs (e.g., picked up items)
        /// </summary>
        public List<string> disabledHotspotIDs = new List<string>();
        
        /// <summary>
        /// Has this scene been visited before?
        /// </summary>
        public bool visited = false;
        
        /// <summary>
        /// Last visit timestamp
        /// </summary>
        public string lastVisitTime;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Create a new scene state
        /// </summary>
        /// <param name="sceneName">Name of the scene</param>
        public SceneState(string sceneName)
        {
            this.sceneName = sceneName;
            this.visited = false;
            this.lastVisitTime = "";
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Mark an item as collected in this scene
        /// </summary>
        /// <param name="itemID">Item ID to mark as collected</param>
        public void MarkItemCollected(string itemID)
        {
            if (!collectedItemIDs.Contains(itemID))
            {
                collectedItemIDs.Add(itemID);
            }
        }
        
        /// <summary>
        /// Mark a puzzle as solved in this scene
        /// </summary>
        /// <param name="puzzleID">Puzzle ID to mark as solved</param>
        public void MarkPuzzleSolved(string puzzleID)
        {
            if (!solvedPuzzleIDs.Contains(puzzleID))
            {
                solvedPuzzleIDs.Add(puzzleID);
            }
        }
        
        /// <summary>
        /// Disable a hotspot (e.g., after pickup or solve)
        /// </summary>
        /// <param name="hotspotID">Hotspot ID to disable</param>
        public void DisableHotspot(string hotspotID)
        {
            if (!disabledHotspotIDs.Contains(hotspotID))
            {
                disabledHotspotIDs.Add(hotspotID);
            }
        }
        
        /// <summary>
        /// Check if an item has been collected in this scene
        /// </summary>
        /// <param name="itemID">Item ID to check</param>
        /// <returns>True if item is in collected list</returns>
        public bool IsItemCollected(string itemID)
        {
            return collectedItemIDs.Contains(itemID);
        }
        
        /// <summary>
        /// Check if a puzzle has been solved in this scene
        /// </summary>
        /// <param name="puzzleID">Puzzle ID to check</param>
        /// <returns>True if puzzle is in solved list</returns>
        public bool IsPuzzleSolved(string puzzleID)
        {
            return solvedPuzzleIDs.Contains(puzzleID);
        }
        
        /// <summary>
        /// Check if a hotspot is disabled
        /// </summary>
        /// <param name="hotspotID">Hotspot ID to check</param>
        /// <returns>True if hotspot is disabled</returns>
        public bool IsHotspotDisabled(string hotspotID)
        {
            return disabledHotspotIDs.Contains(hotspotID);
        }
        
        /// <summary>
        /// Mark scene as visited
        /// </summary>
        public void MarkVisited()
        {
            visited = true;
            lastVisitTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        /// <summary>
        /// Serialize this scene state to JSON
        /// </summary>
        /// <returns>JSON string representation</returns>
        public string Serialize()
        {
            return JsonUtility.ToJson(this, prettyPrint: false);
        }
        
        /// <summary>
        /// Deserialize scene state from JSON
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>SceneState object or null if failed</returns>
        public static SceneState Deserialize(string json)
        {
            try
            {
                return JsonUtility.FromJson<SceneState>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SceneState] Failed to deserialize: {e.Message}");
                return null;
            }
        }
        
        #endregion
    }
}
