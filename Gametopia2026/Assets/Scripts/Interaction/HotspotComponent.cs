using UnityEngine;
using DG.Tweening;
using CoderGoHappy.Core;
using CoderGoHappy.Events;
using CoderGoHappy.Data;
using CoderGoHappy.Scene;
using CoderGoHappy.Inventory;

namespace CoderGoHappy.Interaction
{
    /// <summary>
    /// Individual hotspot component - handles specific interaction logic
    /// MonoBehaviour - attach to interactive game objects
    /// </summary>
    public class HotspotComponent : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// Unique ID for this hotspot (for save state)
        /// </summary>
        [Header("Hotspot Identity")]
        [Tooltip("Unique ID for this hotspot (e.g., 'keyboard_pickup', 'door_to_lab')")]
        public string hotspotID;
        
        /// <summary>
        /// Type of hotspot interaction
        /// </summary>
        [Header("Hotspot Type")]
        [SerializeField] private HotspotType hotspotType = HotspotType.Pickup;
        
        /// <summary>
        /// Is this hotspot currently active?
        /// </summary>
        [SerializeField] private bool isActive = true;
        
        /// <summary>
        /// Custom bounds for hotspot detection (Manual Rect)
        /// </summary>
        [Header("Detection Bounds")]
        [Tooltip("Custom rectangular bounds for mouse detection (in world space)")]
        [SerializeField] private Rect customBounds = new Rect(-0.5f, -0.5f, 1f, 1f);
        
        /// <summary>
        /// Auto-calculate bounds from sprite?
        /// </summary>
        [SerializeField] private bool autoCalculateBounds = true;
        
        /// <summary>
        /// Sprite renderer for this hotspot (optional)
        /// </summary>
        private SpriteRenderer spriteRenderer;
        
        /// <summary>
        /// Item to pickup (for Pickup type)
        /// </summary>
        [Header("Pickup Settings")]
        [SerializeField] private ItemData itemToPickup;
        
        /// <summary>
        /// Disable hotspot after pickup?
        /// </summary>
        [SerializeField] private bool disableAfterPickup = true;
        
        /// <summary>
        /// Required item for interaction (for ItemUse type)
        /// </summary>
        [Header("Item Use Settings")]
        [SerializeField] private ItemData requiredItem;
        
        /// <summary>
        /// Event name to publish on successful item use
        /// </summary>
        [SerializeField] private string successEventName;
        
        /// <summary>
        /// Scene to navigate to (for Navigation type)
        /// </summary>
        [Header("Navigation Settings")]
        [SerializeField] private string targetSceneName;
        
        /// <summary>
        /// Puzzle config to show (for Puzzle type)
        /// </summary>
        [Header("Puzzle Settings")]
        [SerializeField] private string puzzleID;
        
        /// <summary>
        /// Highlight sprite (shown on hover)
        /// </summary>
        [Header("Visual Feedback")]
        [SerializeField] private Sprite highlightSprite;
        
        /// <summary>
        /// Normal sprite (default state)
        /// </summary>
        [SerializeField] private Sprite normalSprite;
        
        /// <summary>
        /// Highlight scale multiplier
        /// </summary>
        [SerializeField] private float highlightScale = 1.1f;
        
        /// <summary>
        /// Pulse animation on hover?
        /// </summary>
        [SerializeField] private bool pulseOnHover = true;
        
        /// <summary>
        /// Cursor texture on hover (optional)
        /// </summary>
        [SerializeField] private Texture2D hoverCursor;
        
        /// <summary>
        /// Reference to HotspotManager
        /// </summary>
        private HotspotManager hotspotManager;
        
        /// <summary>
        /// Reference to InventorySystem
        /// </summary>
        private InventorySystem inventorySystem;
        
        /// <summary>
        /// Reference to SceneController
        /// </summary>
        private SceneController sceneController;
        
        /// <summary>
        /// Is currently hovered?
        /// </summary>
        private bool isHovered = false;
        
        /// <summary>
        /// Pulse tween reference
        /// </summary>
        private Tween pulseTween;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Auto-generate hotspot ID if empty
            if (string.IsNullOrEmpty(hotspotID))
            {
                hotspotID = $"{gameObject.name}_{GetInstanceID()}";
            }
            
            // Store normal sprite
            if (normalSprite == null && spriteRenderer != null)
            {
                normalSprite = spriteRenderer.sprite;
            }
        }
        
        private void Start()
        {
            // Find required systems
            hotspotManager = FindFirstObjectByType<HotspotManager>();
            inventorySystem = FindFirstObjectByType<InventorySystem>();
            sceneController = FindFirstObjectByType<SceneController>();
            
            // Auto-calculate bounds if enabled
            if (autoCalculateBounds && spriteRenderer != null)
            {
                CalculateBoundsFromSprite();
            }
            
            // Check if this hotspot should be disabled based on scene state
            CheckSceneState();
        }
        
        private void OnEnable()
        {
            // Register with HotspotManager
            if (hotspotManager != null)
            {
                hotspotManager.RegisterHotspot(this);
            }
        }
        
        private void OnDisable()
        {
            // Unregister from HotspotManager
            if (hotspotManager != null)
            {
                hotspotManager.UnregisterHotspot(this);
            }
            
            // Kill any active tweens
            if (pulseTween != null)
            {
                pulseTween.Kill();
            }
        }
        
        #endregion
        
        #region Initialization
        
        /// <summary>
        /// Calculate bounds from sprite renderer
        /// </summary>
        private void CalculateBoundsFromSprite()
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null)
                return;
            
            Bounds spriteBounds = spriteRenderer.bounds;
            
            customBounds = new Rect(
                spriteBounds.min.x,
                spriteBounds.min.y,
                spriteBounds.size.x,
                spriteBounds.size.y
            );
        }
        
        /// <summary>
        /// Check scene state to determine if hotspot should be active
        /// </summary>
        private void CheckSceneState()
        {
            if (sceneController == null)
                return;
            
            SceneState state = sceneController.GetSceneState(sceneController.CurrentSceneName);
            
            if (state != null)
            {
                // Check if this hotspot is disabled in scene state
                if (state.IsHotspotDisabled(hotspotID))
                {
                    SetActive(false);
                }
            }
        }
        
        #endregion
        
        #region Bounds Detection
        
        /// <summary>
        /// Check if a point is within this hotspot's bounds
        /// </summary>
        /// <param name="worldPoint">World position to check</param>
        /// <returns>True if point is within bounds</returns>
        public bool IsPointInBounds(Vector2 worldPoint)
        {
            // Apply transform position offset to bounds
            Rect worldBounds = new Rect(
                customBounds.x + transform.position.x,
                customBounds.y + transform.position.y,
                customBounds.width,
                customBounds.height
            );
            
            return worldBounds.Contains(worldPoint);
        }
        
        /// <summary>
        /// Get hotspot bounds in world space
        /// </summary>
        /// <returns>World-space bounds rectangle</returns>
        public Rect GetBounds()
        {
            return new Rect(
                customBounds.x + transform.position.x,
                customBounds.y + transform.position.y,
                customBounds.width,
                customBounds.height
            );
        }
        
        /// <summary>
        /// Set custom bounds manually
        /// </summary>
        /// <param name="bounds">New bounds rectangle</param>
        public void SetBounds(Rect bounds)
        {
            customBounds = bounds;
        }
        
        #endregion
        
        #region Interaction Methods
        
        /// <summary>
        /// Called when mouse enters hotspot bounds
        /// </summary>
        public void OnHoverEnter()
        {
            if (!isActive)
                return;
            
            isHovered = true;
            
            // Visual feedback
            ShowHighlight();
            
            // Change cursor
            if (hoverCursor != null)
            {
                Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
            }
        }
        
        /// <summary>
        /// Called when mouse exits hotspot bounds
        /// </summary>
        public void OnHoverExit()
        {
            isHovered = false;
            
            // Visual feedback
            HideHighlight();
            
            // Reset cursor
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        
        /// <summary>
        /// Called when hotspot is clicked
        /// </summary>
        public void OnClicked()
        {
            if (!isActive)
                return;
            
            // Execute action based on hotspot type
            switch (hotspotType)
            {
                case HotspotType.Pickup:
                    ExecutePickupAction();
                    break;
                
                case HotspotType.Navigation:
                    ExecuteNavigationAction();
                    break;
                
                case HotspotType.Puzzle:
                    ExecutePuzzleAction();
                    break;
                
                case HotspotType.ItemUse:
                    // Item use requires drag-drop, not click
                    Debug.Log($"[HotspotComponent] {hotspotID}: Item use requires dragging item from inventory");
                    break;
                
                case HotspotType.Examine:
                    ExecuteExamineAction();
                    break;
            }
        }
        
        /// <summary>
        /// Try to use an item on this hotspot (from drag-drop)
        /// </summary>
        /// <param name="usedItem">Item being used</param>
        /// <returns>True if item was correct and action executed</returns>
        public bool TryUseItem(ItemData usedItem)
        {
            if (!isActive)
                return false;
            
            if (hotspotType != HotspotType.ItemUse)
            {
                Debug.Log($"[HotspotComponent] {hotspotID}: Not an ItemUse hotspot");
                return false;
            }
            
            // Validate item
            if (usedItem == null || requiredItem == null)
                return false;
            
            if (usedItem.itemID != requiredItem.itemID)
            {
                Debug.Log($"[HotspotComponent] {hotspotID}: Wrong item (need {requiredItem.itemName})");
                return false;
            }
            
            // Correct item! Execute success action
            ExecuteItemUseAction(usedItem);
            return true;
        }
        
        #endregion
        
        #region Action Methods
        
        /// <summary>
        /// Execute pickup action
        /// </summary>
        private void ExecutePickupAction()
        {
            if (itemToPickup == null)
            {
                Debug.LogWarning($"[HotspotComponent] {hotspotID}: No item assigned for pickup");
                return;
            }
            
            if (inventorySystem == null)
            {
                Debug.LogError($"[HotspotComponent] {hotspotID}: InventorySystem not found");
                return;
            }
            
            // Add item to inventory
            bool added = inventorySystem.AddItem(itemToPickup);
            
            if (added)
            {
                Debug.Log($"[HotspotComponent] {hotspotID}: Picked up {itemToPickup.itemName}");
                
                // Save state
                if (sceneController != null)
                {
                    SceneState state = sceneController.GetSceneState(sceneController.CurrentSceneName);
                    state.DisableHotspot(hotspotID);
                }
                
                // Disable hotspot
                if (disableAfterPickup)
                {
                    SetActive(false);
                    
                    // Fade out animation
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.DOFade(0f, 0.3f).OnComplete(() => gameObject.SetActive(false));
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
        
        /// <summary>
        /// Execute item use action (when correct item is used)
        /// </summary>
        /// <param name="usedItem">Item that was used</param>
        private void ExecuteItemUseAction(ItemData usedItem)
        {
            Debug.Log($"[HotspotComponent] {hotspotID}: Used {usedItem.itemName} successfully!");
            
            // Remove item from inventory
            if (inventorySystem != null)
            {
                inventorySystem.RemoveItem(usedItem);
            }
            
            // Publish custom success event if specified
            if (!string.IsNullOrEmpty(successEventName))
            {
                EventManager.Instance.Publish(successEventName, this);
            }
            
            // Save state
            if (sceneController != null)
            {
                SceneState state = sceneController.GetSceneState(sceneController.CurrentSceneName);
                state.DisableHotspot(hotspotID);
            }
            
            // Disable hotspot after successful use
            SetActive(false);
        }
        
        /// <summary>
        /// Execute navigation action
        /// </summary>
        private void ExecuteNavigationAction()
        {
            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogWarning($"[HotspotComponent] {hotspotID}: No target scene assigned");
                return;
            }
            
            if (sceneController == null)
            {
                Debug.LogError($"[HotspotComponent] {hotspotID}: SceneController not found");
                return;
            }
            
            Debug.Log($"[HotspotComponent] {hotspotID}: Navigating to {targetSceneName}");
            
            // Load target scene with fade transition
            sceneController.TransitionToScene(targetSceneName);
        }
        
        /// <summary>
        /// Execute puzzle action
        /// </summary>
        private void ExecutePuzzleAction()
        {
            if (string.IsNullOrEmpty(puzzleID))
            {
                Debug.LogWarning($"[HotspotComponent] {hotspotID}: No puzzle ID assigned");
                return;
            }
            
            Debug.Log($"[HotspotComponent] {hotspotID}: Showing puzzle {puzzleID}");
            
            // Publish event to show puzzle (PuzzleSystem will handle this in Day 4)
            EventManager.Instance.Publish(GameEvents.ShowPuzzle, puzzleID);
        }
        
        /// <summary>
        /// Execute examine action (show description/dialogue)
        /// </summary>
        private void ExecuteExamineAction()
        {
            // TODO: Implement dialogue/description system
            Debug.Log($"[HotspotComponent] {hotspotID}: Examine action (not yet implemented)");
        }
        
        #endregion
        
        #region Visual Methods
        
        /// <summary>
        /// Show highlight visual
        /// </summary>
        private void ShowHighlight()
        {
            // Change sprite if highlight sprite is assigned
            if (highlightSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = highlightSprite;
            }
            
            // Pulse animation
            if (pulseOnHover && spriteRenderer != null)
            {
                pulseTween?.Kill();
                pulseTween = transform.DOScale(highlightScale, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
        
        /// <summary>
        /// Hide highlight visual
        /// </summary>
        private void HideHighlight()
        {
            // Restore normal sprite
            if (normalSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
            
            // Stop pulse animation
            if (pulseTween != null)
            {
                pulseTween.Kill();
                transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad);
            }
        }
        
        #endregion
        
        #region State Methods
        
        /// <summary>
        /// Set hotspot active state
        /// </summary>
        /// <param name="active">New active state</param>
        public void SetActive(bool active)
        {
            isActive = active;
            
            // Exit hover state if deactivating while hovered
            if (!active && isHovered)
            {
                OnHoverExit();
            }
        }
        
        /// <summary>
        /// Is this hotspot currently active?
        /// </summary>
        public bool IsActive => isActive;
        
        #endregion
        
        #region Debug Visualization
        
        private void OnDrawGizmosSelected()
        {
            // Draw custom bounds in editor
            Gizmos.color = Color.cyan;
            
            Rect worldBounds = new Rect(
                customBounds.x + transform.position.x,
                customBounds.y + transform.position.y,
                customBounds.width,
                customBounds.height
            );
            
            Vector3 center = new Vector3(worldBounds.center.x, worldBounds.center.y, transform.position.z);
            Vector3 size = new Vector3(worldBounds.width, worldBounds.height, 0.1f);
            
            Gizmos.DrawWireCube(center, size);
            
            // Draw label
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(center, $"{hotspotID}\n({hotspotType})");
            #endif
        }
        
        #endregion
    }
}
