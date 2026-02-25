# 03 - Scene Setup & Transitions

## Overview
Hướng dẫn setup **SceneController** cho mỗi scene và configure scene transitions với DOTween fade effects.

**Prerequisites:**
- Đã hoàn thành [02-core-systems.md](02-core-systems.md)
- GameManager đang chạy trong PersistentScene

---

## Architecture Overview

```
Level01 Scene
├── Main Camera
├── SceneController (GameObject)
│   └── SceneController (script)
├── Canvas (UI)
└── GameObjects (backgrounds, sprites, hotspots...)
```

**SceneController responsibilities:**
- Load scenes asynchronously với fade transitions
- Save/restore scene state (per-scene data)
- Manage scene-specific data (visitedScenes, collectedItems in scene)

---

## Step 1: Setup SceneController in Level01

### 1.1. Open Level01 Scene

1. Double-click `Assets/Scenes/Levels/Level01.unity`
2. Scene sẽ load trong Hierarchy

### 1.2. Create SceneController GameObject

1. Right-click **Hierarchy** → **Create Empty**
2. Rename thành `SceneController`
3. Reset Transform: (0, 0, 0)

### 1.3. Add SceneController Component

1. Select **SceneController** GameObject
2. **Inspector → Add Component**
3. Search: **Scene Controller**
4. Component sẽ được add

---

## Step 2: Configure Fade Overlay

SceneController cần một **UI Image** để fade in/out transitions.

### 2.1. Create Fade Panel

1. Right-click **Canvas** trong Hierarchy → **UI → Image**
2. Rename thành `FadeOverlay`
3. Configure Image:
   - **Color**: Black (RGB: 0, 0, 0)
   - **Alpha**: 0 (hoàn toàn transparent lúc start)
   - **Raycast Target**: **TÍCH** (để block input khi fade)

### 2.2. Add CanvasGroup Component

**QUAN TRỌNG:** FadeOverlay cần **CanvasGroup** để SceneController control fade effect!

1. Vẫn select **FadeOverlay**
2. **Inspector → Add Component**
3. Search: **Canvas Group**
4. Configure **CanvasGroup**:
   - **Alpha**: 0 (transparent ban đầu)
   - **Interactable**: BỎ TÍCH ☐
   - **Block Raycasts**: TÍCH ☑ (block input khi fade)

### 2.3. Configure RectTransform

Select **FadeOverlay**, trong Inspector:

**Rect Transform:**
- Click **Anchor Presets** → Chọn **Stretch Both** (Alt+Shift+Click)
- **Left, Right, Top, Bottom** = 0 (full screen)

**Canvas Renderer:**
- **Cull Transparent Mesh**: Tích (optional optimization)

### 2.4. Move to Top of Canvas

Drag **FadeOverlay** lên đầu tiên trong Canvas children (để render trên cùng):

```
Canvas
├── FadeOverlay   ← Phải ở đầu!
├── InventoryUI
└── PuzzleUI
```

**Components Summary của FadeOverlay:**
- ✅ RectTransform (Stretch Both)
- ✅ Image (Black, Alpha 0, Raycast Target ON)
- ✅ **CanvasGroup** (Alpha 0, Block Raycasts ON) ← REQUIRED!

---

## Step 3: Assign Fade Canvas Group to SceneController

### 3.1. Inspector Assignment

1. Select **SceneController** GameObject
2. Trong **SceneController component**, tìm field **Fade Canvas Group**
3. **Drag FadeOverlay** từ Hierarchy vào field đó

**Kết quả:** Fade Canvas Group field sẽ reference FadeOverlay's CanvasGroup component.

### 3.2. Configure Fade Settings

Trong **SceneController component**, các settings sau đã có default values tốt:

| Field | Default Value | Mô tả |
|-------|---------------|-------|
| **Default Fade Duration** | 0.5f | Thời gian fade (giây) |
| **Fade Canvas Group** | (assigned) | CanvasGroup để fade |

**Thường không cần đổi**, nhưng có thể tweak:
- **Faster**: 0.3f (snappy transitions)
- **Slower**: 1.0f (cinematic transitions)

---

## Step 4: Setup Scene State Persistence

### 4.1. Understanding SceneState

Mỗi scene có **SceneState** riêng được lưu trong **GameStateData**:

```csharp
public class SceneState
{
    public string sceneName;           // e.g., "Level01"
    public bool visited;               // Scene đã visit chưa?
    public List<string> collectedItemIDs;     // Items đã lấy trong scene này
    public List<string> disabledHotspotIDs;   // Hotspots đã disable
}
```

**Auto-save khi nào?**
- Khi `TransitionToScene()` được gọi (trước khi chuyển scene)
- Khi `GameManager.SaveGame()` được gọi

**Auto-load khi nào?**
- Khi scene load xong (trong `SceneController.Start()`)

### 4.2. No Manual Configuration Needed

SceneController tự động:
1. Save current scene state khi transition
2. Load scene state khi scene loads
3. Publish events: `SceneTransitionStart`, `SceneTransitionComplete`

---

## Step 5: Link SceneController to GameManager

### 5.1. Automatic Linking

GameManager tự động find SceneController:
```csharp
if (sceneController == null)
    sceneController = FindFirstObjectByType<SceneController>();
```

**Nhưng best practice là assign manually** để tránh FindObject overhead.

### 5.2. Manual Assignment (Recommended)

**Option A: Assign in PersistentScene (if using prefab variant)**

1. Mở **PersistentScene**
2. Select **GameManager** GameObject
3. Trong **GameManager component**, field **Scene Controller**:
   - Drag SceneController **prefab** từ Project window
   - Hoặc để **null** và sẽ auto-find

**Option B: Assign at Runtime**

Để GameManager tự động find - không cần làm gì!

---

## Step 6: Test Scene Transition

### 6.1. Create Test Button (Temporary)

Tạo UI Button để test transition:

1. Trong **Canvas**, right-click → **UI → Button**
2. Rename thành `TestTransitionButton`
3. Position: Anywhere visible (e.g., bottom-right corner)
4. Button Text: "Go to Level02"

### 6.2. Create Test Script

**File:** `Assets/Scripts/TestSceneTransition.cs`

```csharp
using UnityEngine;
using CoderGoHappy.Scene;

public class TestSceneTransition : MonoBehaviour
{
    public void OnButtonClick()
    {
        SceneController sceneController = FindFirstObjectByType<SceneController>();
        
        if (sceneController != null)
        {
            Debug.Log("[TEST] Transitioning to Level02...");
            sceneController.TransitionToScene("Level02", Vector3.zero);
        }
        else
        {
            Debug.LogError("[TEST] SceneController not found!");
        }
    }
}
```

### 6.3. Wire Button

1. Create empty GameObject trong scene, rename `TestTransitionHandler`
2. Add component **TestSceneTransition**
3. Select **TestTransitionButton**
4. Trong **Button component**, scroll to **On Click ()**:
   - Click **+** để add event
   - Drag **TestTransitionHandler** vào Object field
   - Function: **TestSceneTransition → OnButtonClick**

### 6.4. Create Level02 Scene (Quick)

1. Duplicate **Level01** scene → Save as `Level02.unity` trong `Assets/Scenes/Levels/`
2. Add **Level02** vào **Build Settings** (index 2)

### 6.5. Test Transition

1. Press **Play** (make sure PersistentScene + Level01 loaded)
2. Click **TestTransitionButton**
3. **Expected behavior:**
   - FadeOverlay fades to black (0.5s)
   - Loading progresses
   - Level02 loads
   - FadeOverlay fades to transparent
   - Console shows: `[SceneController] Transition to Level02 complete`

4. **Delete test button and script** sau khi test xong

---

## Step 7: Configure Camera Setup

### 7.1. Main Camera Tag

Mỗi level scene cần có **Main Camera** với tag "MainCamera":

1. Select **Main Camera** trong Level01
2. **Inspector → Tag** → chọn **MainCamera**

### 7.2. Camera Settings (2D Game)

**Camera component:**
- **Projection**: Orthographic
- **Size**: 5 (hoặc adjust theo screen design)
- **Clipping Planes**: Near = 0.3, Far = 1000
- **Clear Flags**: Solid Color hoặc Skybox
- **Background**: Chọn màu phù hợp (e.g., dark blue #1A1A2E)

**Transform:**
- **Position**: (0, 0, -10) - Negative Z để camera nhìn vào scene

### 7.3. Audio Listener

Camera mặc định có **Audio Listener** component - **giữ nguyên**.

**Note:** Chỉ có 1 Audio Listener active mỗi lúc. Nếu load multiple scenes, sẽ có warning - có thể ignore hoặc disable extra listeners.

---

## Step 8: Scene Naming Convention

### 8.1. Build Settings Scene List

Organize scenes theo thứ tự:

| Index | Scene Name | Path | Purpose |
|-------|------------|------|---------|
| 0 | PersistentScene | Scenes/Persistent/ | GameManager, persistent systems |
| 1 | Level01 | Scenes/Levels/ | First gameplay level |
| 2 | Level02 | Scenes/Levels/ | Second level |
| ... | ... | ... | ... |

### 8.2. Scene Name vs Build Index

SceneController hỗ trợ load bằng:
- **Scene Name** (string): `"Level01"` ← Recommended
- **Build Index** (int): `1`

**Prefer scene names** vì dễ đọc và maintain hơn.

---

## Step 9: Multi-Scene Setup (Advanced)

### 9.1. Additive Scene Loading

Nếu muốn load UI scene separately:

```csharp
SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
```

**Use case:**
- UI scene chứa Canvas shared across levels
- Puzzle scenes load additive khi cần

### 9.2. SceneController with Additive Scenes

SceneController mặc định dùng `LoadSceneMode.Single` (unload previous scene).

**Nếu cần additive**, modify code trong `SceneController.cs`:

```csharp
// Change:
asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

// To:
asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
```

---

## Step 10: Event Integration

### 10.1. Scene Transition Events

SceneController publishes 2 events:

**1. SceneTransitionStart**
```csharp
EventManager.Instance.Publish(GameEvents.SceneTransitionStart, nextSceneName);
```

**2. SceneTransitionComplete**
```csharp
EventManager.Instance.Publish(GameEvents.SceneTransitionComplete, sceneName);
```

### 10.2. Subscribe to Scene Events (Example)

**Use case:** Pause music khi transition, resume khi complete

```csharp
using CoderGoHappy.Events;

public class MusicManager : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvents.SceneTransitionStart, OnTransitionStart);
        EventManager.Instance.Subscribe(GameEvents.SceneTransitionComplete, OnTransitionComplete);
    }

    void OnDisable()
    {
        EventManager.Instance?.Unsubscribe(GameEvents.SceneTransitionStart, OnTransitionStart);
        EventManager.Instance?.Unsubscribe(GameEvents.SceneTransitionComplete, OnTransitionComplete);
    }

    void OnTransitionStart(object data)
    {
        // Fade out music
        AudioSource.volume = 0f;
    }

    void OnTransitionComplete(object data)
    {
        // Fade in music
        AudioSource.volume = 1f;
    }
}
```

---

## Step 11: Create SceneController Prefab

### 11.1. Why Prefab?

Để reuse SceneController config trong tất cả level scenes.

### 11.2. Create Prefab

1. Select **SceneController** GameObject trong Level01
2. Drag vào `Assets/Prefabs/Core/`
3. Prefab created: `SceneController.prefab`

### 11.3. Use in Other Scenes

Khi tạo Level02, Level03, etc.:
1. Drag `SceneController.prefab` vào scene
2. Assign FadeOverlay của scene đó vào prefab instance
3. Prefab instance sẽ override chỉ Fade Canvas Group field

---

## Troubleshooting

### Issue: Scene không fade, load ngay lập tức

**Nguyên nhân:** FadeOverlay không được assign hoặc thiếu CanvasGroup

**Giải pháp:**
1. Check SceneController component → Fade Canvas Group field có assigned không
2. Verify FadeOverlay có **CanvasGroup** component (REQUIRED!)
3. Check FadeOverlay cũng có Image component (Black, Alpha 0)
4. Check Canvas có **Canvas Scaler** component

### Issue: "Scene 'Level02' not found"

**Nguyên nhân:** Scene chưa add vào Build Settings

**Giải pháp:**
1. **File → Build Settings**
2. Drag Level02.unity vào **Scenes In Build** list
3. Verify scene được enable (checkbox tích)

### Issue: FadeOverlay không hiện (vẫn thấy được scene cũ khi transition)

**Nguyên nhân:** FadeOverlay không ở top của Canvas

**Giải pháp:**
1. Trong Hierarchy, drag FadeOverlay lên đầu tiên trong Canvas children
2. Verify **Canvas Renderer → Sorting Order** của FadeOverlay > các UI khác

### Issue: Transition lag/freeze

**Nguyên nhân:** Loading scene quá lớn

**Giải pháp:**
1. Use `allowSceneActivation = false` để control activation timing
2. Load assets asynchronously trong scene
3. Optimize scene (giảm GameObject count, texture size)

---

## Performance Tips

### Optimize Scene Loading

1. **Async Loading** - SceneController đã dùng `LoadSceneAsync` ✅
2. **Preload scenes** - Load scenes ở background:
   ```csharp
   AsyncOperation preload = SceneManager.LoadSceneAsync("Level03", LoadSceneMode.Additive);
   preload.allowSceneActivation = false; // Don't activate yet
   ```
3. **Addressables** - Dùng Addressables system cho large games (advanced)

### Reduce Fade Overhead

- Fade Duration 0.5s là balanced
- Quá nhanh (0.1s) → Jarring
- Quá chậm (2s) → Player impatient

---

## Next Steps

✅ Scene setup hoàn tất!

**Tiếp theo:**
- [04-inventory-setup.md](04-inventory-setup.md) - Setup Inventory System và UI
- [05-hotspot-setup.md](05-hotspot-setup.md) - Setup Hotspots cho interaction

---

## Summary Checklist

- [ ] SceneController GameObject có trong mỗi level scene
- [ ] FadeOverlay UI Image đã tạo trong Canvas
- [ ] FadeOverlay assigned vào SceneController component
- [ ] Main Camera có tag "MainCamera"
- [ ] Test transition (fade in/out) hoạt động
- [ ] All scenes added to Build Settings
- [ ] SceneController prefab created (optional)
- [ ] Console shows SceneTransitionStart/Complete events

**Nếu tất cả OK → Ready for [04-inventory-setup.md](04-inventory-setup.md)** 🎒
