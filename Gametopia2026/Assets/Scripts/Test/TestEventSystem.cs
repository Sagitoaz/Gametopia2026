using UnityEngine;
using CoderGoHappy.Events;
using CoderGoHappy.Core;

public class TestEventSystem : MonoBehaviour
{
    void Start()
    {
        // Subscribe to test event
        EventManager.Instance.Subscribe("TestEvent", OnTestEventReceived);
        
        // Publish test event after 1 second
        Invoke("PublishTestEvent", 1f);
    }

    void PublishTestEvent()
    {
        Debug.Log("[TEST] Publishing TestEvent");
        EventManager.Instance.Publish("TestEvent", "Hello from Event System!");
    }

    void OnTestEventReceived(object data)
    {
        Debug.Log($"[TEST] Event received! Data: {data}");
    }

    void OnDestroy()
    {
        EventManager.Instance?.Unsubscribe("TestEvent", OnTestEventReceived);
    }
}