using UnityEngine;

namespace CoderGoHappy.Data
{
    /// <summary>
    /// ScriptableObject that defines an inventory item
    /// Create instances via: Assets > Create > Coder Go Happy > Item Data
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Coder Go Happy/Item Data", order = 1)]
    public class ItemData : ScriptableObject
    {
        #region Fields
        
        /// <summary>
        /// Unique identifier for this item
        /// </summary>
        [Header("Item Identity")]
        [Tooltip("Unique ID for this item (e.g., 'keyboard', 'mouse', 'usb_drive')")]
        public string itemID;
        
        /// <summary>
        /// Display name for the item
        /// </summary>
        [Tooltip("Name shown in UI (e.g., 'Keyboard', 'USB Drive')")]
        public string itemName;
        
        /// <summary>
        /// Description of the item
        /// </summary>
        [TextArea(2, 4)]
        [Tooltip("Item description for tooltips")]
        public string description;
        
        /// <summary>
        /// Sprite displayed in inventory
        /// </summary>
        [Header("Visuals")]
        [Tooltip("Item sprite shown in inventory UI")]
        public Sprite sprite;
        
        /// <summary>
        /// Sprite displayed in game world (on hotspot)
        /// </summary>
        [Tooltip("Optional: sprite shown in world (if different from inventory sprite)")]
        public Sprite worldSprite;
        
        /// <summary>
        /// Is this item a MiniBug collectible?
        /// </summary>
        [Header("Item Type")]
        [Tooltip("Check if this is a MiniBug collectible")]
        public bool isMiniBug = false;
        
        /// <summary>
        /// Can this item be used/combined with other items?
        /// </summary>
        [Tooltip("Can this item be used on hotspots?")]
        public bool isUsable = true;
        
        #endregion
        
        #region Validation
        
        /// <summary>
        /// Validate item data in editor
        /// </summary>
        private void OnValidate()
        {
            // Auto-generate itemID from asset name if empty
            if (string.IsNullOrEmpty(itemID))
            {
                itemID = name.ToLower().Replace(" ", "_");
            }
            
            // Warn if critical fields are missing
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning($"[ItemData] '{name}' is missing itemName", this);
            }
            
            if (sprite == null)
            {
                Debug.LogWarning($"[ItemData] '{name}' is missing sprite", this);
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Get string representation for debugging
        /// </summary>
        /// <returns>Item info string</returns>
        public override string ToString()
        {
            return $"ItemData: {itemName} (ID: {itemID})";
        }
        
        #endregion
    }
}
