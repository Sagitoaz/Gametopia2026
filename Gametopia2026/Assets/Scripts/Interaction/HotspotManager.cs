using System.Collections.Generic;
using UnityEngine;
using CoderGoHappy.Core;
using CoderGoHappy.Events;
using CoderGoHappy.Data;
using CoderGoHappy.Inventory;

namespace CoderGoHappy.Interaction
{
    /// <summary>
    /// Manages hotspot detection, registration, and interaction coordination
    /// MonoBehaviour - attach to GameManager or dedicated HotspotManager GameObject
    /// </summary>
    public class HotspotManager : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// List of active hotspots in current scene
        /// </summary>
        private List<HotspotComponent> activeHotspots = new List<HotspotComponent>();
        
        /// <summary>
        /// Currently hovered hotspot
        /// </summary>
        private HotspotComponent hoveredHotspot = null;
        
        /// <summary>
        /// Main camera for screen-to-world conversion
        /// </summary>
        private Camera mainCamera;
        
        /// <summary>
        /// Reference to InventorySystem for item validation
        /// </summary>
        [SerializeField] private InventorySystem inventorySystem;
        
        /// <summary>
        /// Reference to InventoryUI for drag-drop detection
        /// </summary>
        [SerializeField] private InventoryUI inventoryUI;
        
        /// <summary>
        /// Debug mode - visualize hotspot bounds
        /// </summary>
        [SerializeField] private bool debugMode = false;
        
        /// <summary>
        /// Layer mask for hotspot detection (optional optimization)
        /// </summary>
        [SerializeField] private LayerMask hotspotLayer = -1;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            mainCamera = Camera.main;
            
            if (mainCamera == null)
                Debug.LogError("[HotspotManager] Main camera not found!", this);
            
            // Find systems if not assigned
            if (inventorySystem == null)
                inventorySystem = FindFirstObjectByType<InventorySystem>();
            
            if (inventoryUI == null)
                inventoryUI = FindFirstObjectByType<InventoryUI>();
        }
        
        private void Update()
        {
            // Check hover state every frame
            CheckHover();
            
            // Handle mouse clicks
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
        }
        
        private void OnEnable()
        {
            // Subscribe to scene transition events to clear hotspots
            EventManager.Instance.Subscribe(GameEvents.SceneTransitionStart, OnSceneTransitionStart);
        }
        
        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.SceneTransitionStart, OnSceneTransitionStart);
        }
        
        #endregion
        
        #region Registration Methods
        
        /// <summary>
        /// Register a hotspot (called by HotspotComponent on Enable)
        /// </summary>
        /// <param name="hotspot">HotspotComponent to register</param>
        public void RegisterHotspot(HotspotComponent hotspot)
        {
            if (hotspot == null)
                return;
            
            if (!activeHotspots.Contains(hotspot))
            {
                activeHotspots.Add(hotspot);
                
                if (debugMode)
                    Debug.Log($"[HotspotManager] Registered hotspot: {hotspot.name} (Total: {activeHotspots.Count})");
            }
        }
        
        /// <summary>
        /// Unregister a hotspot (called by HotspotComponent on Disable)
        /// </summary>
        /// <param name="hotspot">HotspotComponent to unregister</param>
        public void UnregisterHotspot(HotspotComponent hotspot)
        {
            if (hotspot == null)
                return;
            
            if (activeHotspots.Contains(hotspot))
            {
                // Clear hover state if this is the hovered hotspot
                if (hoveredHotspot == hotspot)
                {
                    hoveredHotspot.OnHoverExit();
                    hoveredHotspot = null;
                }
                
                activeHotspots.Remove(hotspot);
                
                if (debugMode)
                    Debug.Log($"[HotspotManager] Unregistered hotspot: {hotspot.name} (Total: {activeHotspots.Count})");
            }
        }
        
        /// <summary>
        /// Clear all registered hotspots
        /// </summary>
        public void ClearAllHotspots()
        {
            // Notify all hotspots of hover exit
            if (hoveredHotspot != null)
            {
                hoveredHotspot.OnHoverExit();
                hoveredHotspot = null;
            }
            
            activeHotspots.Clear();
            
            if (debugMode)
                Debug.Log("[HotspotManager] Cleared all hotspots");
        }
        
        #endregion
        
        #region Detection Methods
        
        /// <summary>
        /// Check which hotspot is under mouse (called every frame)
        /// </summary>
        private void CheckHover()
        {
            if (mainCamera == null)
                return;
            
            // Get mouse position in world space
            Vector2 mouseWorldPos = ScreenToWorldPoint(Input.mousePosition);
            
            // Find hotspot at this position
            HotspotComponent hotspotAtMouse = GetHotspotAtPosition(mouseWorldPos);
            
            // Update hover state
            if (hotspotAtMouse != hoveredHotspot)
            {
                // Exit previous hotspot
                if (hoveredHotspot != null)
                {
                    hoveredHotspot.OnHoverExit();
                }
                
                // Enter new hotspot
                hoveredHotspot = hotspotAtMouse;
                
                if (hoveredHotspot != null)
                {
                    hoveredHotspot.OnHoverEnter();
                }
            }
        }
        
        /// <summary>
        /// Get hotspot at specific world position
        /// </summary>
        /// <param name="worldPosition">World position to check</param>
        /// <returns>HotspotComponent at position, or null</returns>
        public HotspotComponent GetHotspotAtPosition(Vector2 worldPosition)
        {
            // Iterate through active hotspots (front to back for painter's algorithm)
            // Note: Could optimize with spatial partitioning if many hotspots
            for (int i = activeHotspots.Count - 1; i >= 0; i--)
            {
                if (activeHotspots[i] == null)
                    continue;
                
                if (!activeHotspots[i].IsActive)
                    continue;
                
                // Check if point is within hotspot bounds
                if (activeHotspots[i].IsPointInBounds(worldPosition))
                {
                    return activeHotspots[i];
                }
            }
            
            return null;
        }
        
        #endregion
        
        #region Interaction Handling
        
        /// <summary>
        /// Handle mouse click interaction
        /// </summary>
        private void HandleClick()
        {
            // Check if currently dragging from inventory
            if (inventoryUI != null && inventoryUI.IsDragging)
            {
                // Don't process click while dragging
                return;
            }
            
            // If hovering a hotspot, trigger its click action
            if (hoveredHotspot != null)
            {
                hoveredHotspot.OnClicked();
                
                // Publish event for tracking
                EventManager.Instance.Publish(GameEvents.HotspotTriggered, hoveredHotspot);
            }
        }
        
        /// <summary>
        /// Handle item drop from inventory (called by InventoryUI or externally)
        /// </summary>
        /// <param name="droppedItem">Item that was dropped</param>
        /// <param name="dropPosition">Screen position where item was dropped</param>
        /// <returns>True if item was successfully used, false otherwise</returns>
        public bool HandleItemDrop(ItemData droppedItem, Vector2 dropPosition)
        {
            if (droppedItem == null)
                return false;
            
            // Convert screen position to world position
            Vector2 worldPos = ScreenToWorldPoint(dropPosition);
            
            // Find hotspot at drop position
            HotspotComponent hotspot = GetHotspotAtPosition(worldPos);
            
            if (hotspot != null)
            {
                // Attempt to use item on hotspot
                bool success = hotspot.TryUseItem(droppedItem);
                
                if (success && debugMode)
                {
                    Debug.Log($"[HotspotManager] Successfully used {droppedItem.itemName} on {hotspot.name}");
                }
                
                return success;
            }
            
            if (debugMode)
                Debug.Log($"[HotspotManager] No hotspot at drop position");
            
            return false;
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Convert screen position to world position
        /// </summary>
        /// <param name="screenPosition">Screen position (e.g., mouse position)</param>
        /// <returns>World position</returns>
        public Vector2 ScreenToWorldPoint(Vector2 screenPosition)
        {
            if (mainCamera == null)
                return Vector2.zero;
            
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
            return new Vector2(worldPos.x, worldPos.y);
        }
        
        /// <summary>
        /// Get all active hotspots (read-only)
        /// </summary>
        /// <returns>List of active hotspots</returns>
        public List<HotspotComponent> GetActiveHotspots()
        {
            return new List<HotspotComponent>(activeHotspots);
        }
        
        #endregion
        
        #region Event Handlers
        
        /// <summary>
        /// Handle scene transition start - clear hotspots
        /// </summary>
        /// <param name="data">Scene name (unused)</param>
        private void OnSceneTransitionStart(object data)
        {
            ClearAllHotspots();
        }
        
        #endregion
        
        #region Debug Visualization
        
        private void OnDrawGizmos()
        {
            if (!debugMode || activeHotspots == null)
                return;
            
            // Draw all hotspot bounds
            Gizmos.color = Color.yellow;
            
            foreach (var hotspot in activeHotspots)
            {
                if (hotspot == null)
                    continue;
                
                // Draw hotspot bounds as wireframe box
                Rect bounds = hotspot.GetBounds();
                Vector3 center = new Vector3(bounds.center.x, bounds.center.y, 0f);
                Vector3 size = new Vector3(bounds.width, bounds.height, 0.1f);
                
                Gizmos.DrawWireCube(center, size);
            }
            
            // Draw hovered hotspot in different color
            if (hoveredHotspot != null)
            {
                Gizmos.color = Color.green;
                Rect bounds = hoveredHotspot.GetBounds();
                Vector3 center = new Vector3(bounds.center.x, bounds.center.y, 0f);
                Vector3 size = new Vector3(bounds.width, bounds.height, 0.1f);
                
                Gizmos.DrawWireCube(center, size);
            }
        }
        
        #endregion
        
        #region Public Accessors
        
        /// <summary>
        /// Get currently hovered hotspot
        /// </summary>
        public HotspotComponent HoveredHotspot => hoveredHotspot;
        
        #endregion
    }
}
