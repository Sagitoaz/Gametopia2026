using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using CoderGoHappy.Data;

namespace CoderGoHappy.Inventory
{
    /// <summary>
    /// UI component for a single inventory slot
    /// Attach to each slot GameObject in the inventory UI
    /// </summary>
    public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Fields
        
        /// <summary>
        /// Slot index in inventory
        /// </summary>
        [SerializeField] private int slotIndex;
        
        /// <summary>
        /// Item currently in this slot
        /// </summary>
        private ItemData currentItem;
        
        /// <summary>
        /// Image component for item sprite
        /// </summary>
        [Header("UI References")]
        [SerializeField] private Image itemImage;
        
        /// <summary>
        /// Background image for slot
        /// </summary>
        [SerializeField] private Image backgroundImage;
        
        /// <summary>
        /// Highlight overlay for selection
        /// </summary>
        [SerializeField] private GameObject highlightOverlay;
        
        /// <summary>
        /// Color for empty slot
        /// </summary>
        [Header("Visual Settings")]
        [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.3f);
        
        /// <summary>
        /// Color for filled slot
        /// </summary>
        [SerializeField] private Color filledColor = Color.white;
        
        /// <summary>
        /// Highlight scale multiplier
        /// </summary>
        [SerializeField] private float highlightScale = 1.1f;
        
        /// <summary>
        /// Animation duration
        /// </summary>
        [SerializeField] private float animDuration = 0.2f;
        
        /// <summary>
        /// Reference to parent InventoryUI
        /// </summary>
        private InventoryUI inventoryUI;
        
        /// <summary>
        /// Is this slot empty?
        /// </summary>
        private bool isEmpty = true;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Validate references
            if (itemImage == null)
                Debug.LogError($"[InventorySlot] Slot {slotIndex} missing itemImage reference", this);
            
            if (highlightOverlay != null)
                highlightOverlay.SetActive(false);
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Initialize the slot with parent reference
        /// </summary>
        /// <param name="index">Slot index</param>
        /// <param name="parent">Parent InventoryUI</param>
        public void Initialize(int index, InventoryUI parent)
        {
            slotIndex = index;
            inventoryUI = parent;
            ClearSlot();
        }
        
        /// <summary>
        /// Set item in this slot
        /// </summary>
        /// <param name="item">ItemData to display</param>
        public void SetItem(ItemData item)
        {
            currentItem = item;
            
            if (item != null)
            {
                // Show item
                isEmpty = false;
                
                if (itemImage != null)
                {
                    itemImage.sprite = item.sprite;
                    itemImage.color = filledColor;
                    itemImage.enabled = true;
                }
                
                // Animate appearance
                if (itemImage != null)
                {
                    itemImage.transform.localScale = Vector3.zero;
                    itemImage.transform.DOScale(1f, animDuration).SetEase(Ease.OutBack);
                }
            }
            else
            {
                ClearSlot();
            }
        }
        
        /// <summary>
        /// Clear the slot
        /// </summary>
        public void ClearSlot()
        {
            currentItem = null;
            isEmpty = true;
            
            if (itemImage != null)
            {
                itemImage.sprite = null;
                itemImage.color = emptyColor;
                itemImage.enabled = false;
            }
            
            HideHighlight();
        }
        
        /// <summary>
        /// Show highlight on this slot
        /// </summary>
        public void ShowHighlight()
        {
            if (highlightOverlay != null)
            {
                highlightOverlay.SetActive(true);
            }
            
            // Scale animation
            if (itemImage != null && !isEmpty)
            {
                itemImage.transform.DOKill();
                itemImage.transform.DOScale(highlightScale, animDuration).SetEase(Ease.OutQuad);
            }
        }
        
        /// <summary>
        /// Hide highlight
        /// </summary>
        public void HideHighlight()
        {
            if (highlightOverlay != null)
            {
                highlightOverlay.SetActive(false);
            }
            
            // Reset scale
            if (itemImage != null)
            {
                itemImage.transform.DOKill();
                itemImage.transform.DOScale(1f, animDuration).SetEase(Ease.OutQuad);
            }
        }
        
        #endregion
        
        #region Event Handlers (IPointerClickHandler, etc.)
        
        /// <summary>
        /// Handle click on slot
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isEmpty || currentItem == null)
                return;
            
            // Notify parent InventoryUI
            if (inventoryUI != null)
            {
                inventoryUI.OnSlotClicked(slotIndex);
            }
        }
        
        /// <summary>
        /// Handle mouse enter (hover)
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isEmpty || currentItem == null)
                return;
            
            // Show tooltip
            if (inventoryUI != null)
            {
                inventoryUI.ShowItemTooltip(currentItem, transform.position);
            }
            
            // Slight scale feedback
            if (itemImage != null)
            {
                itemImage.transform.DOScale(1.05f, 0.1f).SetEase(Ease.OutQuad);
            }
        }
        
        /// <summary>
        /// Handle mouse exit (hover end)
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            // Hide tooltip
            if (inventoryUI != null)
            {
                inventoryUI.HideItemTooltip();
            }
            
            // Reset scale (unless highlighted)
            if (itemImage != null && (highlightOverlay == null || !highlightOverlay.activeSelf))
            {
                itemImage.transform.DOScale(1f, 0.1f).SetEase(Ease.OutQuad);
            }
        }
        
        #endregion
        
        #region Public Accessors
        
        /// <summary>
        /// Get slot index
        /// </summary>
        public int SlotIndex => slotIndex;
        
        /// <summary>
        /// Get current item in slot
        /// </summary>
        public ItemData CurrentItem => currentItem;
        
        /// <summary>
        /// Is this slot empty?
        /// </summary>
        public bool IsEmpty => isEmpty;
        
        #endregion
    }
}
