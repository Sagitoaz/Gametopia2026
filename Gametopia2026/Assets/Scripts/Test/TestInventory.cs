using UnityEngine;
using CoderGoHappy.Inventory;
using CoderGoHappy.Data;

public class TestInventory : MonoBehaviour
{
    void Start()
    {
        // Wait 1 second then add test item
        Invoke("AddTestItem", 1f);
    }

    void AddTestItem()
    {
        // Load item from Resources
        ItemData item = Resources.Load<ItemData>("Items/TestItem_Keyboard");
        
        if (item != null)
        {
            InventorySystem inventorySystem = FindFirstObjectByType<InventorySystem>();
            
            if (inventorySystem != null)
            {
                bool success = inventorySystem.AddItem(item);
                Debug.Log($"[TEST] Add item: {success}");
            }
        }
        else
        {
            Debug.LogError("[TEST] TestItem_Keyboard not found in Resources/Items/");
        }
    }
}