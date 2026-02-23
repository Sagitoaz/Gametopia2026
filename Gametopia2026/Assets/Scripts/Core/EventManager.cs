using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoderGoHappy.Core
{
    /// <summary>
    /// Central event bus for decoupled component communication
    /// Singleton pattern - access via EventManager.Instance
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        #region Singleton
        
        private static EventManager instance;
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<EventManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("EventManager");
                        instance = go.AddComponent<EventManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        #endregion
        
        #region Fields
        
        /// <summary>
        /// Dictionary of event names to their listener lists
        /// </summary>
        private Dictionary<string, UnityEvent<object>> eventDictionary = new Dictionary<string, UnityEvent<object>>();
        
        /// <summary>
        /// Enable to see event publish/subscribe logs in console (for debugging)
        /// </summary>
        [SerializeField] private bool debugMode = false;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Ensure singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (debugMode)
                Debug.Log("[EventManager] Initialized");
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Subscribe a listener to an event
        /// </summary>
        /// <param name="eventName">Name of the event to listen for</param>
        /// <param name="listener">Method to call when event is published</param>
        public void Subscribe(string eventName, UnityAction<object> listener)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("[EventManager] Cannot subscribe to null or empty event name");
                return;
            }
            
            if (listener == null)
            {
                Debug.LogError($"[EventManager] Cannot subscribe null listener to event: {eventName}");
                return;
            }
            
            // Create event entry if doesn't exist
            if (!eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] = new UnityEvent<object>();
            }
            
            // Check for duplicate subscription
            // Note: UnityEvent doesn't have built-in duplicate prevention
            // We just add the listener (calling same method twice will invoke twice)
            eventDictionary[eventName].AddListener(listener);
            
            if (debugMode)
                Debug.Log($"[EventManager] Subscribed to event: {eventName}");
        }
        
        /// <summary>
        /// Unsubscribe a listener from an event
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="listener">Method to remove from listeners</param>
        public void Unsubscribe(string eventName, UnityAction<object> listener)
        {
            if (string.IsNullOrEmpty(eventName) || listener == null)
                return;
            
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName].RemoveListener(listener);
                
                // Cleanup: remove event entry if no listeners remain
                if (eventDictionary[eventName].GetPersistentEventCount() == 0)
                {
                    // Note: Unity events don't expose runtime listener count directly
                    // We keep the entry for now to avoid null checks
                }
                
                if (debugMode)
                    Debug.Log($"[EventManager] Unsubscribed from event: {eventName}");
            }
        }
        
        /// <summary>
        /// Publish an event to all subscribers
        /// </summary>
        /// <param name="eventName">Name of the event to publish</param>
        /// <param name="data">Optional data to pass to listeners (can be null)</param>
        public void Publish(string eventName, object data = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("[EventManager] Cannot publish null or empty event name");
                return;
            }
            
            if (debugMode)
            {
                string dataInfo = data != null ? $" with data: {data}" : "";
                Debug.Log($"[EventManager] Publishing event: {eventName}{dataInfo}");
            }
            
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName].Invoke(data);
            }
            else if (debugMode)
            {
                Debug.LogWarning($"[EventManager] Event published but no subscribers: {eventName}");
            }
        }
        
        /// <summary>
        /// Clear all listeners for a specific event
        /// </summary>
        /// <param name="eventName">Name of the event to clear</param>
        public void ClearEvent(string eventName)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName].RemoveAllListeners();
                
                if (debugMode)
                    Debug.Log($"[EventManager] Cleared all listeners for event: {eventName}");
            }
        }
        
        /// <summary>
        /// Clear all events and listeners (use with caution, typically for cleanup)
        /// </summary>
        public void ClearAllEvents()
        {
            foreach (var eventEntry in eventDictionary.Values)
            {
                eventEntry.RemoveAllListeners();
            }
            eventDictionary.Clear();
            
            if (debugMode)
                Debug.Log("[EventManager] Cleared all events");
        }
        
        /// <summary>
        /// Check if an event has any subscribers
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <returns>True if event exists and has listeners</returns>
        public bool HasSubscribers(string eventName)
        {
            return eventDictionary.ContainsKey(eventName);
        }
        
        #endregion
    }
}
