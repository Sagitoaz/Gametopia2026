# 02 - Core Systems Setup

## Overview
Hướng dẫn setup các core systems: **GameManager**, **EventManager**, **GameStateData** trong **PersistentScene**. Các systems này sẽ persist xuyên suốt game (DontDestroyOnLoad).

**Prerequisites:**
- Đã hoàn thành [01-project-setup.md](01-project-setup.md)
- PersistentScene đã được tạo

---

## Architecture Overview

```
PersistentScene (DontDestroyOnLoad)
└── GameManager (GameObject)
    ├── GameManager (script) - Orchestrator
    ├── EventManager (script) - Pub/Sub event bus  
    └── GameStateData (script) - Save/Load data singleton
```

**Core responsibilities:**
- **GameManager**: Initialize systems, coordinate managers, save/load game
- **EventManager**: Publish/Subscribe event system (decoupled communication)
- **GameStateData**: Centralized save data (items, puzzles, scenes)

---

## Step 1: Setup PersistentScene

### 1.1. Open PersistentScene

1. Double-click `Assets/Scenes/Persistent/PersistentScene.unity`
2. Scene sẽ mở và hiện trong Hierarchy

### 1.2. Create GameManager GameObject

1. Right-click **Hierarchy** → **Create Empty**
2. Rename thành `GameManager`
3. Reset Transform:
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (1, 1, 1)

> **Note:** GameObject này sẽ chứa tất cả core components.

---

## Step 2: Add Core Scripts to GameManager

### 2.1. Add GameManager Component

1. Select **GameManager** GameObject trong Hierarchy
2. **Inspector → Add Component**
3. Search và add: **GameManager** (từ `CoderGoHappy.Core` namespace)

### 2.2. Add EventManager Component

1. Vẫn đang select **GameManager** GameObject
2. **Inspector → Add Component**
3. Search và add: **Event Manager**

### 2.3. Add GameStateData Component

1. Vẫn đang select **GameManager** GameObject
2. **Inspector → Add Component**
3. Search và add: **Game State Data**

**Kết quả:** GameObject GameManager sẽ có 4 components:
- ✅ Transform
- ✅ GameManager
- ✅ EventManager
- ✅ GameStateData

---

## Step 3: Configure GameManager Component

### 3.1. Inspector Fields

Select GameManager GameObject, check **GameManager component** trong Inspector.

**Bạn sẽ thấy các fields sau:**

#### Scene Controller (SceneController)
- **Chưa cần assign ngay** - sẽ auto-find trong Level scenes
- Hoặc assign sau khi tạo SceneController prefab (guide 03)

#### Inventory System (InventorySystem)
- **Chưa cần assign ngay** - sẽ auto-find trong Level scenes
- Hoặc assign sau khi tạo InventorySystem (guide 04)

#### Hotspot Manager (HotspotManager)
- **Chưa cần assign ngay** - sẽ auto-find
- Hoặc assign sau khi tạo HotspotManager (guide 05)

#### Puzzle System (PuzzleSystem)
- **Chưa cần assign ngay** - sẽ auto-find
- Hoặc assign sau khi tạo PuzzleSystem (guide 06)

> **Auto-Find Logic:**  
> `GameManager.InitializeSystems()` sẽ tự động gọi `FindFirstObjectByType<T>()` nếu fields null.  
> **Best Practice:** Assign manually trong Inspector để tránh FindObject overhead.

### 3.2. Mark as DontDestroyOnLoad

**GameManager component tự động gọi `DontDestroyOnLoad(gameObject)` trong `Awake()`.**

Verify code (đã có sẵn trong `GameManager.cs`):
```csharp
if (instance == null)
{
    instance = this;
    DontDestroyOnLoad(gameObject);
}
```

**Không cần làm gì thêm!** ✅

---

## Step 4: Configure EventManager

### 4.1. Singleton Verification

EventManager cũng là Singleton pattern.

**Verify trong Inspector:**
- Component đã attached vào GameManager GameObject
- Không có warning hoặc error trong Console

### 4.2. Test Event System (Optional Debug Script)

Tạo temporary script để test EventManager:

**File:** `Assets/Scripts/TestEventSystem.cs`

```csharp
using UnityEngine;
using CoderGoHappy.Events;

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
```

**To Test:**
1. Attach `TestEventSystem` script vào GameManager GameObject
2. Press **Play**
3. Check Console:
   ```
   [TEST] Publishing TestEvent
   [EventManager] Published: TestEvent
   [TEST] Event received! Data: Hello from Event System!
   ```
4. Nếu thấy messages trên → EventManager hoạt động! ✅
5. **Remove TestEventSystem component** sau khi test xong

---

## Step 5: Configure GameStateData

### 5.1. Default Values

GameStateData không có Inspector fields cần config.

**Internal state (runtime only):**
- `collectedItemIDs`: List<string> - empty khi start
- `solvedPuzzleIDs`: List<string> - empty khi start
- `sceneStates`: Dictionary<string, SceneState> - empty khi start

### 5.2. Verify Singleton

GameStateData cũng là Singleton:
```csharp
public static GameStateData Instance
```

**Press Play và check Console** - không nên có error về duplicate instances.

### 5.3. PlayerPrefs Save Key

GameStateData lưu vào PlayerPrefs với key: `"CoderGoHappy_GameState"`

**Xem save data location:**
- **Windows:** Registry key `HKCU\Software\[CompanyName]\[ProductName]`
- **Mac:** `~/Library/Preferences/unity.[CompanyName].[ProductName].plist`
- **Linux:** `~/.config/unity3d/[CompanyName]/[ProductName]/prefs`

**Debug Tip:** Để clear save data, dùng code:
```csharp
PlayerPrefs.DeleteKey("CoderGoHappy_GameState");
PlayerPrefs.Save();
```

---

## Step 6: Verify Initialization Flow

### 6.1. Expected Startup Sequence

Khi press **Play** (với PersistentScene + Level01 loaded):

1. **GameManager.Awake():**
   - Sets up Singleton
   - Calls `DontDestroyOnLoad(gameObject)`
   
2. **EventManager.Awake():**
   - Initializes event dictionary
   - Marks `DontDestroyOnLoad`

3. **GameStateData.Awake():**
   - Initializes data structures
   - Marks `DontDestroyOnLoad`

4. **GameManager.Start():**
   - Calls `InitializeSystems()`:
     - Validates EventManager instance
     - Validates GameStateData instance
     - Finds/validates domain managers (SceneController, InventorySystem, etc.)
   - Calls `LoadGameState()` from PlayerPrefs (nếu có save data)

### 6.2. Console Debug Logs

Press **Play** trong PersistentScene, Console sẽ show:

```
[EventManager] Initialized
[GameStateData] Initialized
[GameManager] Starting initialization...
[GameManager] GameStateData validated: OK
[GameManager] EventManager validated: OK
[GameManager] WARNING: SceneController not found - scene transitions may not work
[GameManager] WARNING: InventorySystem not found - inventory may not work
[GameManager] WARNING: HotspotManager not found - hotspots may not work
[GameManager] WARNING: PuzzleSystem not found - puzzles may not work
[GameManager] Systems initialized
[GameManager] No save data found, starting new game
```

**Warnings là NORMAL** tại thời điểm này - vì chưa tạo các domain managers.

---

## Step 7: Create GameManager Prefab (Optional but Recommended)

### 7.1. Why Create Prefab?

- Dễ dàng re-use trong multiple scenes
- Update prefab sẽ propagate changes
- Backup configuration

### 7.2. Create Prefab

1. Select **GameManager** GameObject trong Hierarchy
2. Drag vào folder `Assets/Prefabs/Core/`
3. Prefab sẽ được tạo: `GameManager.prefab`
4. GameObject trong Hierarchy sẽ chuyển màu xanh (prefab instance)

### 7.3. Prefab Variant (Optional)

Nếu muốn có variants cho different configurations (e.g., TestGameManager):
1. Right-click `GameManager.prefab`
2. **Create → Prefab Variant**
3. Rename thành `TestGameManager.prefab`

---

## Step 8: Multi-Scene Setup (Additive Loading)

### 8.1. Persistent + Level Pattern

Game "Coder Go Happy" dùng pattern:
- **PersistentScene** (index 0): Luôn loaded, chứa GameManager
- **Level scenes** (index 1+): Load additive hoặc single

### 8.2. Load Both Scenes at Startup

**Option A: Load in Build Settings** (Simple - for development)

1. **File → Build Settings**
2. Make sure PersistentScene ở index 0
3. **Play button** sẽ load scene đầu tiên (PersistentScene)
4. Để test với Level01, dùng `SceneController.LoadScene()` sau

**Option B: Load Additive via Code** (Advanced)

Create script `BootstrapLoader.cs`:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    void Start()
    {
        // Load Level01 additively
        if (SceneManager.sceneCount == 1) // Only PersistentScene loaded
        {
            SceneManager.LoadScene("Level01", LoadSceneMode.Additive);
        }
    }
}
```

Attach vào GameManager GameObject nếu muốn dùng pattern này.

---

## Step 9: Testing Core Systems

### 9.1. Test Singleton Access

Create test script: `Assets/Scripts/TestCoreAccess.cs`

```csharp
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
```

**Usage:**
1. Create empty GameObject trong Level01 scene
2. Attach TestCoreAccess script
3. Play → All 3 messages "✓ accessible" sẽ hiện
4. Remove script sau khi test xong

### 9.2. Test Save/Load

Press **Play**, mở **Console**, nhập commands (cần Console Pro hoặc dùng custom UI):

**Via Code (Runtime Inspector or Debug Menu):**

```csharp
// Add test item
GameStateData.Instance.AddCollectedItem("test_item_01");

// Save game
GameManager.Instance.SaveGame();

// Stop Play, then Play again

// Check if item persists
bool hasItem = GameStateData.Instance.HasCollectedItem("test_item_01");
Debug.Log($"Item persists: {hasItem}"); // Should be true
```

---

## Step 10: Performance Verification

### 10.1. Check Profiler

1. **Window → Analysis → Profiler** (Ctrl+7)
2. Press **Play**
3. Check **CPU Usage** tab:
   - `GameManager.InitializeSystems()` chỉ chạy 1 lần ở Start
   - `EventManager.Publish()` overhead phải < 0.1ms
   - Không có spike lớn trong first frame

### 10.2. Memory Check

1. Trong Profiler, tab **Memory**
2. Press **Play** → Take Sample
3. Check:
   - GameManager: ~1 KB
   - EventManager: ~2 KB (event dictionary)
   - GameStateData: ~5-10 KB (depends on save data size)

**Nếu thấy megabytes → có memory leak!**

---

## Troubleshooting

### Issue: "NullReferenceException: GameManager.Instance is null"

**Nguyên nhân:** GameManager chưa được initialized trước khi access

**Giải pháp:**
1. Verify GameManager GameObject có trong PersistentScene
2. Check Script Execution Order:
   - **Edit → Project Settings → Script Execution Order**
   - Set `GameManager` = **-100** (execute trước các script khác)
   - Set `EventManager` = **-90**
   - Set `GameStateData` = **-80**

### Issue: "Multiple EventManager instances"

**Nguyên nhân:** EventManager bị duplicate khi load scene

**Giải pháp:**
1. Verify chỉ có 1 GameManager GameObject trong Hierarchy khi Play
2. Nếu thấy duplicate, check code:
   ```csharp
   // GameManager.Awake() phải có:
   if (instance != null && instance != this)
   {
       Destroy(gameObject);
       return;
   }
   ```

### Issue: Save data không persist

**Nguyên nhân:** PlayerPrefs không được save

**Giải pháp:**
1. Thêm `PlayerPrefs.Save()` sau `PlayerPrefs.SetString()`:
   ```csharp
   PlayerPrefs.SetString(SAVE_KEY, json);
   PlayerPrefs.Save(); // Force save to disk
   ```
2. Check Console có error về JSON serialization không

---

## Next Steps

✅ Core Systems setup hoàn tất!

**Tiếp theo:**
- [03-scene-setup.md](03-scene-setup.md) - Setup SceneController và scene transitions
- [04-inventory-setup.md](04-inventory-setup.md) - Setup InventorySystem và UI

---

## Summary Checklist

- [ ] PersistentScene có GameManager GameObject
- [ ] GameManager có 3 components: GameManager, EventManager, GameStateData
- [ ] Press Play → Console shows initialization logs (no errors)
- [ ] GameManager persists khi load scene khác (DontDestroyOnLoad)
- [ ] EventManager test passed (publish/subscribe works)
- [ ] GameStateData singleton accessible
- [ ] GameManager prefab created (optional)
- [ ] Script Execution Order configured (optional nhưng recommended)

**Nếu tất cả OK → Ready for [03-scene-setup.md](03-scene-setup.md)** 🎮
