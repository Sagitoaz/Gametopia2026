using System.Collections.Generic;
using UnityEngine;
using CoderGoHappy.Core;
using CoderGoHappy.Events;
using CoderGoHappy.Data;

namespace CoderGoHappy.Inventory
{
    /// <summary>
    /// Manages inventory item collection, selection, and validation logic
    /// MonoBehaviour - attach to GameManager or dedicated InventorySystem GameObject
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// List of collected items
        /// </summary>
        private List<ItemData> collectedItems = new List<ItemData>();
        
        /// <summary>
        /// Currently selected item (for usage)
        /// </summary>
        private ItemData selectedItem = null;
        
        /// <summary>
        /// Maximum number of inventory slots
        /// </summary>
        [SerializeField] private int maxInventorySlots = 20;
        
        /// <summary>
        /// Debug mode - log inventory operations
        /// </summary>
        [SerializeField] private bool debugMode = false;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Start()
        {
            // Load items from game state if available
            LoadFromGameState();
            
            if (debugMode)
                Debug.Log($"[InventorySystem] Initialized with {collectedItems.Count} items");
        }
        
        #endregion
        
        #region Item Management
        
        /// <summary>
        /// Add an item to inventory
        /// </summary>
        /// <param name="item">ItemData to add</param>
        /// <returns>True if added successfully, false if inventory full</returns>
        public bool AddItem(ItemData item)
        {
            if (item == null)
            {
                Debug.LogError("[InventorySystem] Cannot add null item");
                return false;
            }
            
            // Check if inventory is full
            if (IsInventoryFull())
            {
                Debug.LogWarning($"[InventorySystem] Inventory full! Cannot add {item.itemName}");
                return false;
            }
            
            // Check if item already exists (prevent duplicates)
            if (HasItem(item.itemID))
            {
                Debug.LogWarning($"[InventorySystem] Item {item.itemName} already in inventory");
                return false;
            }
            
            // Add item to collection
            collectedItems.Add(item);
            
            // Update game state
            GameStateData.Instance.AddCollectedItem(item.itemID);
            
            // Publish event
            EventManager.Instance.Publish(GameEvents.ItemCollected, item);
            EventManager.Instance.Publish(GameEvents.InventoryUpdated);
            
            // Track MiniBugs separately
            if (item.isMiniBug)
            {
                GameStateData.Instance.miniBugsCollected++;
                EventManager.Instance.Publish(GameEvents.MiniBugCollected, item);
            }
            
            if (debugMode)
                Debug.Log($"[InventorySystem] Added item: {item.itemName} ({collectedItems.Count}/{maxInventorySlots})");
            
            return true;
        }
        
        /// <summary>
        /// Remove an item from inventory (when used)
        /// </summary>
        /// <param name="item">ItemData to remove</param>
        public void RemoveItem(ItemData item)
        {
            if (item == null)
            {
                Debug.LogError("[InventorySystem] Cannot remove null item");
                return;
            }
            
            if (!collectedItems.Contains(item))
            {
                Debug.LogWarning($"[InventorySystem] Cannot remove {item.itemName} - not in inventory");
                return;
            }
            
            // Deselect if this is the selected item
            if (selectedItem == item)
            {
                DeselectItem();
            }
            
            // Remove from collection
            collectedItems.Remove(item);
            
            // Update game state
            GameStateData.Instance.collectedItemIDs.Remove(item.itemID);
            
            // Publish events
            EventManager.Instance.Publish(GameEvents.ItemUsed, item);
            EventManager.Instance.Publish(GameEvents.InventoryUpdated);
            
            if (debugMode)
                Debug.Log($"[InventorySystem] Removed item: {item.itemName} ({collectedItems.Count}/{maxInventorySlots})");
        }
        
        /// <summary>
        /// Check if inventory has a specific item
        /// </summary>
        /// <param name="itemID">Item ID to check</param>
        /// <returns>True if item is in inventory</returns>
        public bool HasItem(string itemID)
        {
            return collectedItems.Exists(item => item.itemID == itemID);
        }
        
        /// <summary>
        /// Get item by ID
        /// </summary>
        /// <param name="itemID">Item ID to find</param>
        /// <returns>ItemData reference or null if not found</returns>
        public ItemData GetItem(string itemID)
        {
            return collectedItems.Find(item => item.itemID == itemID);
        }
        
        #endregion
        
        #region Selection Methods
        
        /// <summary>
        /// Select an item for usage
        /// </summary>
        /// <param name="item">ItemData to select</param>
        public void SelectItem(ItemData item)
        {
            if (item == null)
            {
                Debug.LogWarning("[InventorySystem] Cannot select null item");
                return;
            }
            
            if (!collectedItems.Contains(item))
            {
                Debug.LogWarning($"[InventorySystem] Cannot select {item.itemName} - not in inventory");
                return;
            }
            
            // Set selected item
            selectedItem = item;
            
            // Publish event
            EventManager.Instance.Publish(GameEvents.ItemSelected, item);
            
            if (debugMode)
                Debug.Log($"[InventorySystem] Selected item: {item.itemName}");
        }
        
        /// <summary>
        /// Deselect current item
        /// </summary>
        public void DeselectItem()
        {
            if (selectedItem == null)
                return;
            
            ItemData previousItem = selectedItem;
            selectedItem = null;
            
            // Publish event
            EventManager.Instance.Publish(GameEvents.ItemDeselected, previousItem);
            
            if (debugMode)
                Debug.Log($"[InventorySystem] Deselected item: {previousItem.itemName}");
        }
        
        /// <summary>
        /// Get currently selected item
        /// </summary>
        /// <returns>Selected ItemData or null</returns>
        public ItemData GetSelectedItem()
        {
            return selectedItem;
        }
        
        #endregion
        
        #region Validation Methods
        
        /// <summary>
        /// Check if item can be used on a specific hotspot
        /// Note: Actual validation logic is in HotspotComponent
        /// </summary>
        /// <param name="item">Item to validate</param>
        /// <returns>True if item is usable</returns>
        public bool CanUseItem(ItemData item)
        {
            if (item == null)
                return false;
            
            return item.isUsable;
        }
        
        /// <summary>
        /// Check if inventory is full
        /// </summary>
        /// <returns>True if at max capacity</returns>
        public bool IsInventoryFull()
        {
            return collectedItems.Count >= maxInventorySlots;
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Get current item count
        /// </summary>
        /// <returns>Number of items in inventory</returns>
        public int GetItemCount()
        {
            return collectedItems.Count;
        }
        
        /// <summary>
        /// Clear all items from inventory
        /// </summary>
        public void ClearInventory()
        {
            collectedItems.Clear();
            selectedItem = null;
            
            EventManager.Instance.Publish(GameEvents.InventoryUpdated);
            
            if (debugMode)
                Debug.Log("[InventorySystem] Inventory cleared");
        }
        
        /// <summary>
        /// Get all items (copy of list)
        /// </summary>
        /// <returns>List of all collected items</returns>
        public List<ItemData> GetAllItems()
        {
            return new List<ItemData>(collectedItems);
        }
        
        /// <summary>
        /// Get max inventory slots
        /// </summary>
        /// <returns>Maximum number of slots</returns>
        public int GetMaxSlots()
        {
            return maxInventorySlots;
        }
        
        #endregion
        
        #region Persistence
        
        /// <summary>
        /// Load items from GameStateData
        /// </summary>
        private void LoadFromGameState()
        {
            // Note: This requires ItemData assets to be in Resources folder
            // We'll load items by ID from GameStateData
            List<string> savedItemIDs = GameStateData.Instance.collectedItemIDs;
            
            foreach (string itemID in savedItemIDs)
            {
                // Load ItemData from Resources
                ItemData item = Resources.Load<ItemData>($"Items/{itemID}");
                
                if (item != null)
                {
                    collectedItems.Add(item);
                }
                else
                {
                    Debug.LogWarning($"[InventorySystem] Could not load item from Resources: Items/{itemID}");
                }
            }
            
            if (collectedItems.Count > 0)
            {
                EventManager.Instance.Publish(GameEvents.InventoryUpdated);
                
                if (debugMode)
                    Debug.Log($"[InventorySystem] Loaded {collectedItems.Count} items from save data");
            }
        }
        
        #endregion
        
        #region Public Accessors
        
        /// <summary>
        /// Get reference to collected items (read-only)
        /// </summary>
        public IReadOnlyList<ItemData> CollectedItems => collectedItems.AsReadOnly();
        
        /// <summary>
        /// Get currently selected item (read-only)
        /// </summary>
        public ItemData SelectedItem => selectedItem;
        
        #endregion
    }
}
