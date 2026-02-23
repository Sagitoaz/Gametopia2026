using UnityEngine;
using CoderGoHappy.Events;
using CoderGoHappy.Scene;
// NOTE: These namespaces will be added in Day 2-4
// using CoderGoHappy.Inventory;
// using CoderGoHappy.Interaction;
// using CoderGoHappy.Puzzle;

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
        
        // NOTE: These references will be enabled in Day 2-4 when classes are created
        // /// <summary>
        // /// Reference to InventorySystem
        // /// </summary>
        // [SerializeField] private InventorySystem inventorySystem;
        // 
        // /// <summary>
        // /// Reference to HotspotManager
        // /// </summary>
        // [SerializeField] private HotspotManager hotspotManager;
        // 
        // /// <summary>
        // /// Reference to PuzzleSystem
        // /// </summary>
        // [SerializeField] private PuzzleSystem puzzleSystem;
        
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
            
            Debug.Log("[GameManager] Awake - Singleton established");
        }
        
        private void Start()
        {
            InitializeSystems();
            LoadGameState();
            
            Debug.Log("[GameManager] All systems initialized successfully");
        }
        
        private void OnDestroy()
        {
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
        private void InitializeSystems()
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
            
            // NOTE: Day 2-4 systems will be initialized here when created
            // if (inventorySystem == null)
            //     inventorySystem = FindFirstObjectByType<InventorySystem>();
            // 
            // if (hotspotManager == null)
            //     hotspotManager = FindFirstObjectByType<HotspotManager>();
            // 
            // if (puzzleSystem == null)
            //     puzzleSystem = FindFirstObjectByType<PuzzleSystem>();
            
            // Validate all systems found
            if (sceneController == null)
                Debug.LogWarning("[GameManager] SceneController not found - scene transitions may not work");
            
            // NOTE: Validation for Day 2-4 systems will be added when classes exist
            // if (inventorySystem == null)
            //     Debug.LogWarning("[GameManager] InventorySystem not found - inventory may not work");
            // 
            // if (hotspotManager == null)
            //     Debug.LogWarning("[GameManager] HotspotManager not found - hotspots may not work");
            // 
            // if (puzzleSystem == null)
            //     Debug.LogWarning("[GameManager] PuzzleSystem not found - puzzles may not work");
            
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
        public void DeleteSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            gameState.Reset();
            
            Debug.Log("[GameManager] Save data deleted, game state reset");
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
        
        // NOTE: These accessors will be enabled in Day 2-4
        // /// <summary>
        // /// Get reference to InventorySystem
        // /// </summary>
        // public InventorySystem InventorySystem => inventorySystem;
        // 
        // /// <summary>
        // /// Get reference to HotspotManager
        // /// </summary>
        // public HotspotManager HotspotManager => hotspotManager;
        // 
        // /// <summary>
        // /// Get reference to PuzzleSystem
        // /// </summary>
        // public PuzzleSystem PuzzleSystem => puzzleSystem;
        
        #endregion
    }
}
