using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using CoderGoHappy.Core;
using CoderGoHappy.Events;

namespace CoderGoHappy.Scene
{
    /// <summary>
    /// Manages scene loading, transitions, and scene state persistence
    /// MonoBehaviour - should exist in each scene or as singleton
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// Dictionary of scene states (scene name → SceneState)
        /// </summary>
        private Dictionary<string, SceneState> sceneStates = new Dictionary<string, SceneState>();
        
        /// <summary>
        /// Current scene name
        /// </summary>
        private string currentSceneName;
        
        /// <summary>
        /// Canvas group for fade effect (assign in Inspector)
        /// </summary>
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        
        /// <summary>
        /// Default fade duration in seconds
        /// </summary>
        [SerializeField] private float defaultFadeDuration = 0.5f;
        
        /// <summary>
        /// Is a scene transition currently in progress?
        /// </summary>
        private bool isTransitioning = false;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Start()
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            
            // Mark current scene as visited
            GetSceneState(currentSceneName).MarkVisited();
            
            // Restore scene state if revisiting
            RestoreSceneState(currentSceneName);
            
            Debug.Log($"[SceneController] Initialized in scene: {currentSceneName}");
        }
        
        #endregion
        
        #region Scene Loading Methods
        
        /// <summary>
        /// Load a scene by name
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneController] Cannot load scene with null or empty name");
                return;
            }
            
            if (isTransitioning)
            {
                Debug.LogWarning("[SceneController] Scene transition already in progress, ignoring request");
                return;
            }
            
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        /// <summary>
        /// Transition to a scene with fade effect
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <param name="fadeDuration">Fade duration (optional, uses default if not specified)</param>
        public void TransitionToScene(string sceneName, float fadeDuration = -1f)
        {
            if (fadeDuration < 0)
                fadeDuration = defaultFadeDuration;
            
            StartCoroutine(TransitionWithFade(sceneName, fadeDuration));
        }

        public void TransitionToScene(string sceneName){
            TransitionToScene(sceneName, 0.5f);
        }
        
        /// <summary>
        /// Coroutine for async scene loading
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            isTransitioning = true;
            
            // Publish transition start event
            EventManager.Instance.Publish(GameEvents.SceneTransitionStart, sceneName);
            
            // Save current scene state before transitioning
            SaveCurrentSceneState();
            
            // Start async scene load
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            
            // Wait for scene to be ready
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            
            // Activate the scene
            asyncLoad.allowSceneActivation = true;
            
            // Wait for scene to fully load
            yield return asyncLoad;

            
            
            // Update current scene name
            currentSceneName = sceneName;
            
            // Mark new scene as visited
            GetSceneState(currentSceneName).MarkVisited();
            
            // Restore scene state if revisiting
            RestoreSceneState(currentSceneName);

            
            
            // Publish transition complete event
            EventManager.Instance.Publish(GameEvents.SceneTransitionComplete, sceneName);
            
            isTransitioning = false;
            
            Debug.Log($"[SceneController] Scene loaded: {sceneName}");
        }
        
        /// <summary>
        /// Coroutine for scene transition with fade effect
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <param name="fadeDuration">Fade duration</param>
        private IEnumerator TransitionWithFade(string sceneName, float fadeDuration)
        {
            if (fadeCanvasGroup == null)
            {
                Debug.LogWarning("[SceneController] FadeCanvasGroup not assigned, loading without fade");
                LoadScene(sceneName);
                yield break;
            }
            
            isTransitioning = true;
            
            // Fade out
            yield return FadeOut(fadeDuration);
            
            // Load scene
            yield return LoadSceneAsync(sceneName);
            
            // Fade in
            yield return FadeIn(fadeDuration);
            
            isTransitioning = false;
        }
        
        /// <summary>
        /// Navigate to the next scene in current level
        /// </summary>
        public void NavigateToNextScene()
        {
            // TODO: Implement scene progression logic based on level design
            Debug.LogWarning("[SceneController] NavigateToNextScene not yet implemented");
        }
        
        /// <summary>
        /// Navigate to the previous scene
        /// </summary>
        public void NavigateToPreviousScene()
        {
            // TODO: Implement backwards navigation
            Debug.LogWarning("[SceneController] NavigateToPreviousScene not yet implemented");
        }
        
        #endregion
        
        #region Scene State Methods
        
        /// <summary>
        /// Save current scene state
        /// </summary>
        public void SaveCurrentSceneState()
        {
            if (string.IsNullOrEmpty(currentSceneName))
                return;
            
            SceneState state = GetSceneState(currentSceneName);
            
            // TODO: Collect state from active hotspots, puzzles, etc.
            // For now, scene state is managed by individual components
            
            Debug.Log($"[SceneController] Saved state for scene: {currentSceneName}");
        }
        
        /// <summary>
        /// Restore scene state for a given scene
        /// </summary>
        /// <param name="sceneName">Name of the scene to restore</param>
        public void RestoreSceneState(string sceneName)
        {
            SceneState state = GetSceneState(sceneName);
            
            if (!state.visited)
            {
                // First visit, nothing to restore
                return;
            }
            
            // TODO: Apply state to scene objects
            // - Disable collected item hotspots
            // - Mark puzzles as solved
            // - etc.
            
            Debug.Log($"[SceneController] Restored state for scene: {sceneName}");
        }
        
        /// <summary>
        /// Get scene state for a given scene (creates if doesn't exist)
        /// </summary>
        /// <param name="sceneName">Name of the scene</param>
        /// <returns>SceneState object</returns>
        public SceneState GetSceneState(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            }
            
            if (!sceneStates.ContainsKey(sceneName))
            {
                sceneStates[sceneName] = new SceneState(sceneName);
            }
            
            return sceneStates[sceneName];
        }
        
        /// <summary>
        /// Clear scene state (for restart functionality)
        /// </summary>
        /// <param name="sceneName">Name of the scene to clear</param>
        public void ClearSceneState(string sceneName)
        {
            if (sceneStates.ContainsKey(sceneName))
            {
                sceneStates.Remove(sceneName);
                Debug.Log($"[SceneController] Cleared state for scene: {sceneName}");
            }
        }
        
        #endregion
        
        #region Transition Effects
        
        /// <summary>
        /// Fade out (to black)
        /// </summary>
        /// <param name="duration">Fade duration in seconds</param>
        private IEnumerator FadeOut(float duration)
        {
            if (fadeCanvasGroup == null)
                yield break;
            
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.DOFade(1f, duration);
            
            yield return new WaitForSeconds(duration);
        }
        
        /// <summary>
        /// Fade in (from black)
        /// </summary>
        /// <param name="duration">Fade duration in seconds</param>
        private IEnumerator FadeIn(float duration)
        {
            if (fadeCanvasGroup == null)
                yield break;
            
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.DOFade(0f, duration);
            
            yield return new WaitForSeconds(duration);
        }
        
        #endregion
        
        #region Persistence Methods
        
        /// <summary>
        /// Serialize all scene states to JSON
        /// </summary>
        /// <returns>JSON string containing all scene states</returns>
        public string SerializeSceneStates()
        {
            // Create a wrapper object for serialization
            SceneStatesWrapper wrapper = new SceneStatesWrapper();
            wrapper.sceneStatesList = new List<SceneState>();
            
            foreach (var state in sceneStates.Values)
            {
                wrapper.sceneStatesList.Add(state);
            }
            
            return JsonUtility.ToJson(wrapper, prettyPrint: false);
        }
        
        /// <summary>
        /// Deserialize scene states from JSON
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        public void DeserializeSceneStates(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;
            
            try
            {
                SceneStatesWrapper wrapper = JsonUtility.FromJson<SceneStatesWrapper>(json);
                
                sceneStates.Clear();
                
                foreach (var state in wrapper.sceneStatesList)
                {
                    sceneStates[state.sceneName] = state;
                }
                
                Debug.Log($"[SceneController] Loaded {sceneStates.Count} scene states");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SceneController] Failed to deserialize scene states: {e.Message}");
            }
        }
        
        #endregion
        
        #region Helper Classes
        
        /// <summary>
        /// Wrapper class for serializing scene states dictionary
        /// (Unity JsonUtility doesn't support dictionaries directly)
        /// </summary>
        [System.Serializable]
        private class SceneStatesWrapper
        {
            public List<SceneState> sceneStatesList;
        }
        
        #endregion
        
        #region Public Accessors
        
        /// <summary>
        /// Get current scene name
        /// </summary>
        public string CurrentSceneName => currentSceneName;
        
        /// <summary>
        /// Is a scene transition in progress?
        /// </summary>
        public bool IsTransitioning => isTransitioning;
        
        #endregion
    }
}
