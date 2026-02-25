using UnityEngine;
using CoderGoHappy.Core;

/// <summary>
/// Test script for Save/Load functionality
/// Attach this to any GameObject in the scene
/// Press Space to save, L to load
/// </summary>
public class TestSaveLoad : MonoBehaviour
{
    void Update()
    {
        // Press SPACE to trigger save
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestSave();
        }

        // Press L to trigger load
        if (Input.GetKeyDown(KeyCode.L))
        {
            TestLoad();
        }

        // Press C to clear save data
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearSave();
        }

        // Press T to test add item
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestAddItem();
        }
    }

    void TestSave()
    {
        Debug.Log("=== TEST SAVE ===");
        GameManager.Instance.SaveGame();
        Debug.Log("✓ Save triggered (check above logs for result)");
    }

    void TestLoad()
    {
        Debug.Log("=== TEST LOAD ===");
        
        // Check if item persists
        bool hasItem = GameStateData.Instance.HasItem("test_item_01");
        Debug.Log($"Item 'test_item_01' exists: {hasItem}");
        
        int itemCount = GameStateData.Instance.collectedItemIDs.Count;
        Debug.Log($"Total collected items: {itemCount}");
        
        // Print all items
        if (itemCount > 0)
        {
            Debug.Log("Collected items:");
            foreach (string itemID in GameStateData.Instance.collectedItemIDs)
            {
                Debug.Log($"  - {itemID}");
            }
        }
    }

    void ClearSave()
    {
        Debug.Log("=== CLEAR SAVE DATA ===");
        PlayerPrefs.DeleteKey("CoderGoHappy_GameState");
        PlayerPrefs.Save();
        Debug.Log("✓ Save data cleared! Stop and restart Play mode to start fresh.");
    }

    void TestAddItem()
    {
        Debug.Log("=== TEST ADD ITEM ===");
        GameStateData.Instance.AddCollectedItem("test_item_01");
        Debug.Log("✓ Added 'test_item_01' to collected items");
        Debug.Log($"Total items now: {GameStateData.Instance.collectedItemIDs.Count}");
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 150), 
            "Save/Load Test Controls:\n" +
            "T - Add test item\n" +
            "SPACE - Save game\n" +
            "L - Load game (check items)\n" +
            "C - Clear save data");
    }
}
