using UnityEngine;
using CoderGoHappy.Events;
using CoderGoHappy.Core;

/// <summary>
/// ActivateOnEvent — listens for a named event and activates/deactivates GameObjects.
///
/// Usage examples:
///   - Old PC (ItemUse) publishes "old_pc_opened"
///     → ActivateOnEvent on same GameObject activates Hotspot_USBKeycard
///   - Card Reader publishes "card_accepted"
///     → ActivateOnEvent activates Hotspot_Mainframe
/// </summary>
public class ActivateOnEvent : MonoBehaviour
{
    [Header("Event")]
    [Tooltip("Name of the event to listen for (must match successEventName in HotspotComponent)")]
    [SerializeField] private string eventName = "";

    [Header("Actions")]
    [Tooltip("GameObjects to SetActive(true) when event fires")]
    [SerializeField] private GameObject[] objectsToActivate;

    [Tooltip("GameObjects to SetActive(false) when event fires")]
    [SerializeField] private GameObject[] objectsToDeactivate;

    [Tooltip("Run once only? (unsubscribes after first trigger)")]
    [SerializeField] private bool runOnce = true;

    private bool hasFired = false;

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(eventName))
            EventManager.Instance?.Subscribe(eventName, OnEventReceived);
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(eventName))
            EventManager.Instance?.Unsubscribe(eventName, OnEventReceived);
    }

    private void OnEventReceived(object data)
    {
        if (runOnce && hasFired) return;
        hasFired = true;

        foreach (var obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"[ActivateOnEvent] Activated: {obj.name}");
            }
        }

        foreach (var obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log($"[ActivateOnEvent] Deactivated: {obj.name}");
            }
        }

        // Unsubscribe if run-once
        if (runOnce)
            EventManager.Instance?.Unsubscribe(eventName, OnEventReceived);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(eventName))
            Debug.LogWarning($"[ActivateOnEvent] '{gameObject.name}' has no eventName set!", this);
    }
#endif
}
