using UnityEngine;
using CoderGoHappy.Events;
using CoderGoHappy.Scene;
using CoderGoHappy.Inventory;
using CoderGoHappy.Interaction;
using CoderGoHappy.Puzzle;
using UnityEngine.SceneManagement;

namespace CoderGoHappy.Core
{
    /// <summary>
    /// Central game orchestrator - manages initialization, game state, and system coordination
    /// Singleton MonoBehaviour - should be in DontDestroyOnLoad scene
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<GameManager>();
                }
                return instance;
            }
        }
        
        #endregion
        
        #region Fields
        
        /// <summary>
        /// Reference to the game state data
        /// </summary>
        private GameStateData gameState;
        
        /// <summary>
        /// Reference to SceneController
        /// </summary>
        [SerializeField] private SceneController sceneController;
        
        /// <summary>
        /// Reference to InventorySystem
        /// </summary>
        [SerializeField] private InventorySystem inventorySystem;
        
        /// <summary>
        /// Reference to HotspotManager
        /// </summary>
        [SerializeField] private HotspotManager hotspotManager;
        
        /// <summary>
        /// Reference to PuzzleSystem
        /// </summary>
        [SerializeField] private PuzzleSystem puzzleSystem;
        
        /// <summary>
        /// PlayerPrefs key for save data
        /// </summary>
        private const string SAVE_KEY = "CoderGoHappy_SaveData";
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Subscribe to Unity's scene loaded event (works for ALL scene loads)
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Debug.Log("[GameManager] Awake - Singleton established");
        }
        
        private void Start()
        {
            InitializeSystems();
            LoadGameState();
            
            // Subscribe to scene transition complete event (for SceneController transitions)
            EventManager.Instance.Subscribe(GameEvents.SceneTransitionComplete, OnSceneTransitionComplete);
            
            Debug.Log("[GameManager] All systems initialized successfully");
            TestStart();
        }

        async void TestStart()
        {
            await System.Threading.Tasks.Task.Delay(100); // Wait a frame for all systems to initialize
            SceneManager.LoadScene(1);
        }
        
        /// <summary>
        /// Called when any scene is loaded (Unity built-in event)
        /// </summary>
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[GameManager] Scene loaded: {scene.name}, re-initializing systems...");
            InitializeSystems();
        }
        
        /// <summary>
        /// Called when scene transition completes (SceneController event)
        /// </summary>
        private void OnSceneTransitionComplete(object sceneName)
        {
            Debug.Log($"[GameManager] Scene transition complete: {sceneName}");
            // InitializeSystems already called by OnSceneLoaded, no need to call again
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from Unity's scene loaded event
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            // Unsubscribe from custom events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe(GameEvents.SceneTransitionComplete, OnSceneTransitionComplete);
            }
            
            // Auto-save on quit
            SaveGame();
        }
        
        private void OnApplicationQuit()
        {
            // Ensure save on application quit
            SaveGame();
        }
        
        #endregion
        
        #region Initialization Methods
        
        /// <summary>
        /// Initialize all game systems in correct dependency order
        /// </summary>
        public void InitializeSystems()
        {
            Debug.Log("[GameManager] Initializing systems...");
            
            // Phase 1: Foundation - EventManager (already initialized via singleton)
            if (EventManager.Instance == null)
            {
                Debug.LogError("[GameManager] EventManager failed to initialize!");
                return;
            }
            
            // Phase 2: Get game state singleton
            gameState = GameStateData.Instance;
            
            // Phase 3: Find or validate domain managers
            // These should be assigned in Inspector or found in scene
            if (sceneController == null)
                sceneController = FindFirstObjectByType<SceneController>();
            
            if (inventorySystem == null)
                inventorySystem = FindFirstObjectByType<InventorySystem>();
            
            if (hotspotManager == null)
                hotspotManager = FindFirstObjectByType<HotspotManager>();
            
            if (puzzleSystem == null)
                puzzleSystem = FindFirstObjectByType<PuzzleSystem>();
            
            // Validate all systems found
            if (sceneController == null)
                Debug.LogWarning("[GameManager] SceneController not found - scene transitions may not work");
            
            if (inventorySystem == null)
                Debug.LogWarning("[GameManager] InventorySystem not found - inventory may not work");
            
            if (hotspotManager == null)
                Debug.LogWarning("[GameManager] HotspotManager not found - hotspots may not work");
            
            if (puzzleSystem == null)
                Debug.LogWarning("[GameManager] PuzzleSystem not found - puzzles may not work");
            
            Debug.Log("[GameManager] Systems initialized");
        }
        
        /// <summary>
        /// Load game state from PlayerPrefs
        /// </summary>
        private void LoadGameState()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                bool success = gameState.Deserialize(json);
                
                if (success)
                {
                    Debug.Log("[GameManager] Game state loaded successfully");
                    EventManager.Instance.Publish(GameEvents.GameLoaded);
                }
                else
                {
                    Debug.LogWarning("[GameManager] Failed to load game state, starting fresh");
                }
            }
            else
            {
                Debug.Log("[GameManager] No save data found, starting new game");
            }
        }
        
        #endregion
        
        #region Game Control Methods
        
        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            Time.timeScale = 0f;
            EventManager.Instance.Publish(GameEvents.GamePaused);
            Debug.Log("[GameManager] Game paused");
        }
        
        /// <summary>
        /// Resume the game
        /// </summary>
        public void ResumeGame()
        {
            Time.timeScale = 1f;
            EventManager.Instance.Publish(GameEvents.GameResumed);
            Debug.Log("[GameManager] Game resumed");
        }
        
        /// <summary>
        /// Quit to main menu
        /// </summary>
        public void QuitToMainMenu()
        {
            SaveGame();
            
            if (sceneController != null)
            {
                sceneController.LoadScene("MainMenu");
            }
            else
            {
                Debug.LogError("[GameManager] Cannot quit to main menu - SceneController not found");
            }
        }
        
        /// <summary>
        /// Restart current level
        /// </summary>
        public void RestartLevel()
        {
            // Clear level-specific state but keep inventory
            // TODO: Implement level restart logic
            
            Debug.Log("[GameManager] Level restarted");
        }
        
        #endregion
        
        #region Save/Load Methods
        
        /// <summary>
        /// Save game state to PlayerPrefs
        /// </summary>
        public void SaveGame()
        {
            string json = gameState.Serialize();
            
            if (!string.IsNullOrEmpty(json))
            {
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();
                
                EventManager.Instance.Publish(GameEvents.GameSaved);
                Debug.Log("[GameManager] Game saved successfully");
            }
            else
            {
                Debug.LogError("[GameManager] Failed to save game - serialization failed");
            }
        }
        
        /// <summary>
        /// Load game from save data
        /// </summary>
        /// <returns>True if save exists and loaded successfully</returns>
        public bool LoadGame()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                LoadGameState();
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if save data exists
        /// </summary>
        /// <returns>True if save exists</returns>
        public bool HasSaveData()
        {
            return PlayerPrefs.HasKey(SAVE_KEY);
        }
        
        /// <summary>
        /// Delete save data (new game)
        /// </summary>
        /// 
        
        public void DeleteSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            gameState.Reset();
            
            Debug.Log("[GameManager] Save data deleted, game state reset");
        }

        [ContextMenu("XOA DATA")]

        public void DeleteAllData(){
            PlayerPrefs.DeleteAll();
            gameState.Reset();
        }
        
        #endregion
        
        #region Public Accessors
        
        /// <summary>
        /// Get reference to game state data
        /// </summary>
        public GameStateData GameState => gameState;
        
        /// <summary>
        /// Get reference to SceneController
        /// </summary>
        public SceneController SceneController => sceneController;
        
        /// <summary>
        /// Get reference to InventorySystem
        /// </summary>
        public InventorySystem InventorySystem => inventorySystem;
        
        /// <summary>
        /// Get reference to HotspotManager
        /// </summary>
        public HotspotManager HotspotManager => hotspotManager;
        
        /// <summary>
        /// Get reference to PuzzleSystem
        /// </summary>
        public PuzzleSystem PuzzleSystem => puzzleSystem;
        
        #endregion
    }
}
