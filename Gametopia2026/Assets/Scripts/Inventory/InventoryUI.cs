using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using CoderGoHappy.Core;
using CoderGoHappy.Events;
using CoderGoHappy.Data;

namespace CoderGoHappy.Inventory
{
    /// <summary>
    /// Manages inventory UI display and drag-drop interactions
    /// MonoBehaviour - attach to Inventory Canvas GameObject
    /// </summary>
    public class InventoryUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Fields
        
        /// <summary>
        /// Reference to InventorySystem
        /// </summary>
        [Header("System References")]
        [SerializeField] private InventorySystem inventorySystem;
        
        /// <summary>
        /// List of inventory slots
        /// </summary>
        [Header("Slot References")]
        [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>();
        
        /// <summary>
        /// Container for dynamically created slots
        /// </summary>
        [SerializeField] private Transform slotsContainer;
        
        /// <summary>
        /// Slot prefab for dynamic creation
        /// </summary>
        [SerializeField] private GameObject slotPrefab;
        
        /// <summary>
        /// Tooltip text component
        /// </summary>
        [Header("Tooltip")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshProUGUI tooltipText;
        
        /// <summary>
        /// Dragged item visual
        /// </summary>
        [Header("Drag Visual")]
        [SerializeField] private Image draggedItemImage;
        [SerializeField] private Canvas dragCanvas;
        
        /// <summary>
        /// Currently dragged item
        /// </summary>
        private ItemData draggedItem;
        
        /// <summary>
        /// Slot index being dragged from
        /// </summary>
        private int draggedSlotIndex = -1;
        
        /// <summary>
        /// Is currently dragging?
        /// </summary>
        private bool isDragging = false;
        
        /// <summary>
        /// Currently selected slot index
        /// </summary>
        private int selectedSlotIndex = -1;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Validate references
            if (inventorySystem == null)
            {
                inventorySystem = FindFirstObjectByType<InventorySystem>();
                if (inventorySystem == null)
                    Debug.LogError("[InventoryUI] InventorySystem not found!", this);
            }
            
            // Hide drag visual
            if (draggedItemImage != null)
                draggedItemImage.gameObject.SetActive(false);
            
            // Hide tooltip
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
                
                // CRITICAL: Disable raycasts on tooltip to prevent hover flickering
                // When tooltip appears over cursor, it shouldn't block pointer events
                Image tooltipImage = tooltipPanel.GetComponent<Image>();
                if (tooltipImage != null)
                {
                    tooltipImage.raycastTarget = false;
                }
                
                // Also disable raycasts on all children (like tooltip text)
                Image[] childImages = tooltipPanel.GetComponentsInChildren<Image>(true);
                foreach (Image img in childImages)
                {
                    img.raycastTarget = false;
                }
            }
            
            // Initialize slots
            InitializeSlots();
        }
        
        private void OnEnable()
        {
            // Subscribe to inventory events
            EventManager.Instance.Subscribe(GameEvents.InventoryUpdated, OnInventoryUpdated);
            EventManager.Instance.Subscribe(GameEvents.ItemSelected, OnItemSelected);
            EventManager.Instance.Subscribe(GameEvents.ItemDeselected, OnItemDeselected);
        }
        
        private void OnDisable()
        {
            // Unsubscribe from events
            EventManager.Instance.Unsubscribe(GameEvents.InventoryUpdated, OnInventoryUpdated);
            EventManager.Instance.Unsubscribe(GameEvents.ItemSelected, OnItemSelected);
            EventManager.Instance.Unsubscribe(GameEvents.ItemDeselected, OnItemDeselected);
        }
        
        private void Start()
        {
            // Initial UI refresh
            RefreshUI();
        }
        
        #endregion
        
        #region Initialization
        
        /// <summary>
        /// Initialize inventory slots
        /// </summary>
        private void InitializeSlots()
        {
            if (inventorySlots.Count == 0 && slotPrefab != null && slotsContainer != null)
            {
                // Create slots dynamically
                int maxSlots = inventorySystem != null ? inventorySystem.GetMaxSlots() : 20;
                
                for (int i = 0; i < maxSlots; i++)
                {
                    GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
                    InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                    
                    if (slot != null)
                    {
                        slot.Initialize(i, this);
                        inventorySlots.Add(slot);
                    }
                }
            }
            else
            {
                // Use pre-existing slots
                for (int i = 0; i < inventorySlots.Count; i++)
                {
                    if (inventorySlots[i] != null)
                    {
                        inventorySlots[i].Initialize(i, this);
                    }
                }
            }
        }
        
        #endregion
        
        #region UI Update Methods
        
        /// <summary>
        /// Refresh entire inventory UI
        /// </summary>
        public void RefreshUI()
        {
            if (inventorySystem == null)
                return;
            
            List<ItemData> items = inventorySystem.GetAllItems();
            
            // Update all slots
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i] != null)
                {
                    if (i < items.Count)
                    {
                        inventorySlots[i].SetItem(items[i]);
                    }
                    else
                    {
                        inventorySlots[i].ClearSlot();
                    }
                }
            }
        }
        
        /// <summary>
        /// Update a specific slot
        /// </summary>
        /// <param name="slotIndex">Slot index to update</param>
        /// <param name="item">Item to display</param>
        public void UpdateSlot(int slotIndex, ItemData item)
        {
            if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
                return;
            
            if (inventorySlots[slotIndex] != null)
            {
                inventorySlots[slotIndex].SetItem(item);
            }
        }
        
        /// <summary>
        /// Clear all slots
        /// </summary>
        public void ClearAllSlots()
        {
            foreach (var slot in inventorySlots)
            {
                if (slot != null)
                    slot.ClearSlot();
            }
        }
        
        #endregion
        
        #region Slot Interaction
        
        /// <summary>
        /// Handle slot click (called from InventorySlot)
        /// </summary>
        /// <param name="slotIndex">Index of clicked slot</param>
        public void OnSlotClicked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
                return;
            
            InventorySlot slot = inventorySlots[slotIndex];
            if (slot == null || slot.IsEmpty)
                return;
            
            ItemData item = slot.CurrentItem;
            
            // Toggle selection
            if (selectedSlotIndex == slotIndex)
            {
                // Deselect
                DeselectCurrentSlot();
                inventorySystem.DeselectItem();
            }
            else
            {
                // Select new item
                DeselectCurrentSlot();
                selectedSlotIndex = slotIndex;
                slot.ShowHighlight();
                inventorySystem.SelectItem(item);
            }
        }
        
        /// <summary>
        /// Deselect currently selected slot
        /// </summary>
        private void DeselectCurrentSlot()
        {
            if (selectedSlotIndex >= 0 && selectedSlotIndex < inventorySlots.Count)
            {
                if (inventorySlots[selectedSlotIndex] != null)
                {
                    inventorySlots[selectedSlotIndex].HideHighlight();
                }
            }
            selectedSlotIndex = -1;
        }
        
        #endregion
        
        #region Drag and Drop (Hybrid: Event System + DOTween)
        
        /// <summary>
        /// Begin drag operation
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Find which slot we're dragging from
            GameObject clickedObject = eventData.pointerPress;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();
            
            if (slot == null)
                slot = clickedObject.GetComponentInParent<InventorySlot>();
            
            if (slot == null || slot.IsEmpty)
                return;
            
            // Start drag
            draggedItem = slot.CurrentItem;
            draggedSlotIndex = slot.SlotIndex;
            isDragging = true;
            
            // Show drag visual
            if (draggedItemImage != null && draggedItem != null)
            {
                draggedItemImage.sprite = draggedItem.sprite;
                draggedItemImage.gameObject.SetActive(true);
                draggedItemImage.raycastTarget = false; // Don't block raycasts
                
                // Start with scale animation
                draggedItemImage.transform.localScale = Vector3.one * 0.8f;
                draggedItemImage.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            }
            
            // Hide tooltip
            HideItemTooltip();
        }
        
        /// <summary>
        /// During drag - follow cursor
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || draggedItemImage == null)
                return;
            
            // Smoothly follow cursor using DOTween
            Vector3 targetPos = eventData.position;
            draggedItemImage.transform.DOMove(targetPos, 0.1f).SetEase(Ease.OutQuad);
        }
        
        /// <summary>
        /// End drag - check if dropped on valid target
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;
            
            isDragging = false;
            
            // Hide drag visual
            if (draggedItemImage != null)
            {
                draggedItemImage.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    draggedItemImage.gameObject.SetActive(false);
                });
            }
            
            // Check if dropped on a hotspot (via HotspotManager)
            // Note: HotspotManager will handle this in Day 3
            // For now, we just notify that drag ended
            
            // Reset dragged references
            draggedItem = null;
            draggedSlotIndex = -1;
        }
        
        #endregion
        
        #region Tooltip
        
        /// <summary>
        /// Show item tooltip
        /// </summary>
        /// <param name="item">Item to show tooltip for</param>
        /// <param name="position">Screen position for tooltip</param>
        public void ShowItemTooltip(ItemData item, Vector2 position)
        {
            if (tooltipPanel == null || tooltipText == null || item == null)
                return;
            
            tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
            tooltipPanel.transform.position = position;
            tooltipPanel.SetActive(true);
            
            // Fade in animation
            CanvasGroup canvasGroup = tooltipPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, 0.2f);
            }
        }
        
        /// <summary>
        /// Hide item tooltip
        /// </summary>
        public void HideItemTooltip()
        {
            if (tooltipPanel == null)
                return;
            
            tooltipPanel.SetActive(false);
        }
        
        #endregion
        
        #region Event Handlers
        
        /// <summary>
        /// Handle inventory updated event
        /// </summary>
        /// <param name="data">Event data (unused)</param>
        private void OnInventoryUpdated(object data)
        {
            RefreshUI();
        }
        
        /// <summary>
        /// Handle item selected event
        /// </summary>
        /// <param name="data">Selected ItemData</param>
        private void OnItemSelected(object data)
        {
            ItemData item = data as ItemData;
            if (item == null)
                return;
            
            // Find and highlight the slot with this item
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i] != null && inventorySlots[i].CurrentItem == item)
                {
                    DeselectCurrentSlot();
                    selectedSlotIndex = i;
                    inventorySlots[i].ShowHighlight();
                    break;
                }
            }
        }
        
        /// <summary>
        /// Handle item deselected event
        /// </summary>
        /// <param name="data">Event data (unused)</param>
        private void OnItemDeselected(object data)
        {
            DeselectCurrentSlot();
        }
        
        #endregion
        
        #region Public Accessors
        
        /// <summary>
        /// Get currently dragged item (for HotspotManager)
        /// </summary>
        public ItemData DraggedItem => draggedItem;
        
        /// <summary>
        /// Is currently dragging an item?
        /// </summary>
        public bool IsDragging => isDragging;
        
        #endregion
    }
}
