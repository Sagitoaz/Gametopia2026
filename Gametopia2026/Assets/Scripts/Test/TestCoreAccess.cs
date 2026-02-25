using UnityEngine;
using CoderGoHappy.Core;
using CoderGoHappy.Events;

public class TestCoreAccess : MonoBehaviour
{
    void Start()
    {
        // Test GameManager access
        if (GameManager.Instance != null)
            Debug.Log("✓ GameManager accessible");
        else
            Debug.LogError("✗ GameManager null!");

        // Test EventManager access
        if (EventManager.Instance != null)
            Debug.Log("✓ EventManager accessible");
        else
            Debug.LogError("✗ EventManager null!");

        // Test GameStateData access
        if (GameStateData.Instance != null)
            Debug.Log("✓ GameStateData accessible");
        else
            Debug.LogError("✗ GameStateData null!");
    }
}