using UnityEngine;
using CoderGoHappy.Inventory;
using CoderGoHappy.Events;
using CoderGoHappy.Core;

namespace CoderGoHappy.Interaction
{
    /// <summary>
    /// SmartHotspot — overrides Examine text based on whether player has a specific item.
    ///
    /// Usage:
    ///   1. Add HotspotComponent (Type = Examine, examineText = "", successEventName = "hotspot_examine_<ID>")
    ///   2. Add this script to the same GameObject.
    ///   3. Fill in requiredItemID, textWithoutItem, textWithItem.
    ///
    /// Example: Server Rack
    ///   - Without flashlight → "Quá tối. Cần đèn pin."
    ///   - With flashlight    → "THỨ TỰ: 1 → 3 → 2"
    /// </summary>
    [RequireComponent(typeof(HotspotComponent))]
    public class SmartHotspot : MonoBehaviour
    {
        [Header("Conditional Examine")]
        [Tooltip("itemID that must be in inventory to reveal the clue")]
        [SerializeField] private string requiredItemID = "";

        [Tooltip("Text shown when player does NOT have the required item")]
        [TextArea(2, 4)]
        [SerializeField] private string textWithoutItem = "Cần một vật phẩm đặc biệt.";

        [Tooltip("Text / clue shown when player HAS the required item")]
        [TextArea(2, 4)]
        [SerializeField] private string textWithItem = "Manh mối: ...";

        private HotspotComponent hotspot;
        private InventorySystem inventorySystem;
        private string listenEventName;

        private void Awake()
        {
            hotspot = GetComponent<HotspotComponent>();
        }

        private void Start()
        {
            inventorySystem = FindFirstObjectByType<InventorySystem>();

            // Listen to the examine trigger fired by HotspotComponent via successEventName
            if (hotspot != null)
            {
                listenEventName = "hotspot_examine_" + hotspot.hotspotID;
                EventManager.Instance?.Subscribe(listenEventName, OnExamineTriggered);
            }
        }

        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(listenEventName))
                EventManager.Instance?.Unsubscribe(listenEventName, OnExamineTriggered);
        }

        private void OnExamineTriggered(object data)
        {
            bool hasItem = !string.IsNullOrEmpty(requiredItemID)
                           && inventorySystem != null
                           && inventorySystem.HasItem(requiredItemID);

            string msg = hasItem ? textWithItem : textWithoutItem;
            EventManager.Instance?.Publish("ShowDialogue", msg);
        }
    }
}
