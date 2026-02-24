# 05 - Hotspot Setup & Interaction

## Overview
Hướng dẫn setup **HotspotManager** và **HotspotComponent** cho các interactive objects trong game. Point-and-click interaction system với custom bounds detection.

**Prerequisites:**
- Đã hoàn thành [04-inventory-setup.md](04-inventory-setup.md)
- Level01 scene đã có sprites và backgrounds

---

## Architecture Overview

```
Level01 Scene
├── HotspotManager (GameObject) - Singleton manager
│   └── HotspotManager (script)
├── Hotspots (Container)
│   ├── Hotspot_Keyboard (Interactive item)
│   │   ├── HotspotComponent (script)
│   │   └── SpriteRenderer
│   ├── Hotspot_Door (Navigation)
│   │   ├── HotspotComponent (script)
│   │   └── SpriteRenderer
│   └── Hotspot_Puzzle (Puzzle trigger)
│       ├── HotspotComponent (script)
│       └── SpriteRenderer
```

**Hotspot Types:**
1. **Pickup** - Collect item vào inventory
2. **ItemUse** - Use item từ inventory với hotspot
3. **Navigation** - Chuyển scene
4. **Puzzle** - Trigger puzzle UI
5. **Examine** - Show description dialog

---

## Step 1: Create HotspotManager

### 1.1. Create GameObject

1. Right-click **Hierarchy** → **Create Empty**
2. Rename: `HotspotManager`
3. Reset Transform: (0, 0, 0)

### 1.2. Add HotspotManager Script

1. Select **HotspotManager**
2. **Inspector → Add Component**
3. Search: **Hotspot Manager**
4. Component added ✅

### 1.3. No Inspector Configuration Needed

HotspotManager tự động:
- Registry tất cả HotspotComponents trong scene
- Detect mouse hover/click
- Coordinate với InventorySystem cho item drops
- Visualize hotspot bounds (Scene view only)

---

## Step 2: Create Hotspots Container

### 2.1. Organize Hotspots

1. Right-click **Hierarchy** → **Create Empty**
2. Rename: `Hotspots`
3. Reset Transform: (0, 0, 0)

**Purpose:** Group tất cả hotspot GameObjects để dễ organize.

---

## Step 3: Create First Hotspot (Pickup Type)

### 3.1. Example: Keyboard Item

1. Right-click **Hotspots** → **Create Empty**
2. Rename: `Hotspot_Keyboard`
3. Set Transform Position: (theo vị trí trong scene, e.g., (-2, 0, 0))

### 3.2. Add SpriteRenderer

1. Select **Hotspot_Keyboard**
2. **Add Component → Sprite Renderer**
3. Assign sprite:
   - **Sprite**: Import keyboard sprite vào `Assets/Sprites/Items/`
   - Assign sprite vào SpriteRenderer
   - **Sorting Layer**: Default (hoặc tạo custom layer)
   - **Order in Layer**: 1 (trên background)

### 3.3. Add HotspotComponent Script

1. **Add Component → Hotspot Component**
2. Component added ✅

---

## Step 4: Configure HotspotComponent (Pickup Type)

### 4.1. Basic Settings

Select **Hotspot_Keyboard**, trong **HotspotComponent** Inspector:

| Field | Value | Mô tả |
|-------|-------|-------|
| **hotspotID** | "keyboard_01" | Unique ID |
| **hotspotType** | Pickup | Loại interaction |
| **description** | "A wireless keyboard" | Text hiện khi examine |

### 4.2. Custom Bounds Configuration

HotspotComponent dùng **Rect bounds** thay vì Collider2D:

**autoCalculateBounds**: Tích ✅ (auto-detect từ SpriteRenderer)

Khi tích, bounds sẽ tự động calculate từ sprite size.

**Custom Bounds** (nếu muốn override):
- **Bỏ tích** autoCalculateBounds
- Set **customBounds**:
  - **X**: -0.5 (left offset từ GameObject position)
  - **Y**: -0.5 (bottom offset)
  - **Width**: 1.0 (sprite width trong world units)
  - **Height**: 1.0 (sprite height)

> **Tip:** Dùng auto-calculate cho simple sprites, custom cho irregular shapes.

### 4.3. Pickup Type Specific Settings

**collectibleItem** (ItemData ScriptableObject):
1. Tạo ItemData asset (xem [07-scriptableobject-creation.md](07-scriptableobject-creation.md))
2. Drag ItemData asset vào field này

**Hoặc assign bằng code path:**
- Để field null → HotspotComponent sẽ log warning
- Assign asset từ `Assets/Resources/Items/`

### 4.4. Visual Feedback Settings

**highlightSprite** (optional):
- Import highlight version của sprite (e.g., brighter/outlined version)
- Assign vào field → Sprite swap khi hover

**normalSprite** (auto-assigned):
- Automatically assigned từ SpriteRenderer's sprite trong Awake()

---

## Step 5: Create Navigation Hotspot

### 5.1. Example: Door to Next Room

1. Create GameObject: `Hotspot_Door`
2. Add SpriteRenderer → Assign door sprite
3. Add HotspotComponent

### 5.2. Configure Navigation Type

**HotspotComponent settings:**

| Field | Value |
|-------|-------|
| **hotspotID** | "door_to_level02" |
| **hotspotType** | Navigation |
| **description** | "Door to next room" |
| **targetSceneName** | "Level02" |
| **spawnPositionInTargetScene** | (0, 0, 0) |

**targetSceneName**: Tên scene trong Build Settings (chính xác!)

**spawnPositionInTargetScene**: Vị trí player/camera spawn khi vào scene mới

---

## Step 6: Create ItemUse Hotspot

### 6.1. Example: USB Port (Needs USB Drive)

1. Create GameObject: `Hotspot_USBPort`
2. Add SpriteRenderer → Assign usb port sprite
3. Add HotspotComponent

### 6.2. Configure ItemUse Type

**HotspotComponent settings:**

| Field | Value |
|-------|-------|
| **hotspotID** | "usb_port_01" |
| **hotspotType** | ItemUse |
| **description** | "USB Port - needs a USB drive" |
| **requiredItem** | (Drag USBDrive ItemData asset) |
| **successEvent** | "UnlockComputer" |

**requiredItem**: ItemData asset player phải có trong inventory để interact

**successEvent**: Event name sẽ publish khi use đúng item (optional)

---

## Step 7: Create Puzzle Hotspot

### 7.1. Example: Computer Terminal

1. Create GameObject: `Hotspot_Computer`
2. Add SpriteRenderer → Assign computer sprite
3. Add HotspotComponent

### 7.2. Configure Puzzle Type

**HotspotComponent settings:**

| Field | Value |
|-------|-------|
| **hotspotID** | "computer_puzzle_01" |
| **hotspotType** | Puzzle |
| **description** | "Computer terminal - requires password" |
| **puzzleID** | "Puzzle_Computer01" |

**puzzleID**: Reference đến PuzzleConfig ScriptableObject (xem [06-puzzle-setup.md](06-puzzle-setup.md))

Khi click, sẽ publish `ShowPuzzle` event với puzzleID.

---

## Step 8: Create Examine Hotspot

### 8.1. Example: Poster (Info only)

1. Create GameObject: `Hotspot_Poster`
2. Add SpriteRenderer → Assign poster sprite
3. Add HotspotComponent

### 8.2. Configure Examine Type

**HotspotComponent settings:**

| Field | Value |
|-------|-------|
| **hotspotID** | "poster_01" |
| **hotspotType** | Examine |
| **description** | "A motivational poster: 'Code Happy, Be Happy!'" |

Khi click, chỉ hiện description (hoặc custom dialog UI nếu có).

---

## Step 9: Visualize Hotspot Bounds (Scene View)

### 9.1. Scene View Debug

Khi select HotspotComponent trong Hierarchy:

**OnDrawGizmosSelected** sẽ vẽ:
- **Cyan wireframe**: Custom bounds rectangle
- **White label**: hotspotID text

**Verify:**
1. Select Hotspot_Keyboard
2. Scene view sẽ show cyan box around sprite
3. Adjust customBounds nếu không khớp

### 9.2. Runtime Debug (Game View)

HotspotManager có **OnDrawGizmos** để visualize ALL hotspots:

**Gizmo colors:**
- **Yellow wireframe**: Normal hotspots
- **Green wireframe**: Currently hovered hotspot

**Enable Gizmos:**
1. **Game view → Gizmos button** (top-right)
2. Tích checkbox để show gizmos in Game view

---

## Step 10: Link HotspotManager to GameManager

### 10.1. Automatic Linking

GameManager tự động find HotspotManager:
```csharp
if (hotspotManager == null)
    hotspotManager = FindFirstObjectByType<HotspotManager>();
```

### 10.2. Manual Assignment (Optional)

1. Mở PersistentScene
2. Select GameManager
3. Trong GameManager component, field **Hotspot Manager**:
   - Drag HotspotManager từ Level01 (nếu prefab shared)
   - Hoặc để null (auto-find)

---

## Step 11: Test Hotspot Interaction

### 11.1. Test Pickup

1. Press **Play**
2. Move mouse over Hotspot_Keyboard
3. **Expected:**
   - Sprite swaps to highlightSprite (nếu assigned)
   - DOTween pulse animation (scale 1.0 → 1.1 loop)
4. Click hotspot
5. **Expected:**
   - Item added to inventory
   - Hotspot fades out (alpha 1.0 → 0, then destroy)
   - Console: `[HotspotComponent] Pickup action: keyboard_01`

### 11.2. Test Navigation

1. Click door hotspot
2. **Expected:**
   - SceneController transitions to targetScene
   - Player spawns at spawnPosition

### 11.3. Test ItemUse

1. Collect required item vào inventory
2. Select item (click trong inventory)
3. Click ItemUse hotspot
4. **Expected:**
   - Item consumed from inventory
   - successEvent published
   - Console: `[HotspotComponent] Item use success`

### 11.4. Test Puzzle

1. Click puzzle hotspot
2. **Expected:**
   - PuzzleSystem shows puzzle UI
   - EventManager publishes "ShowPuzzle"

---

## Step 12: Scene State Persistence

### 12.1. Understanding Hotspot State

HotspotComponent tự động save state:

**CheckSceneState() trong Start():**
```csharp
SceneState state = GameStateData.Instance.GetSceneState(sceneName);
if (state != null && state.disabledHotspotIDs.Contains(hotspotID))
{
    SetActive(false); // Disable hotspot
}
```

**Example flow:**
1. Player collects keyboard trong Level01
2. hotspotID "keyboard_01" được add vào `sceneState.disabledHotspotIDs`
3. Player quay lại Level01
4. Hotspot_Keyboard sẽ disabled (không hiện)

### 12.2. Manual Reset (Debug)

Nếu muốn reset collected items:

```csharp
GameStateData.Instance.GetSceneState("Level01").disabledHotspotIDs.Clear();
```

---

## Step 13: Create Hotspot Prefab Variants

### 13.1. Why Prefabs?

- Reuse configuration
- Batch updates
- Consistent behavior

### 13.2. Create Prefab

1. Select **Hotspot_Keyboard**
2. Drag vào `Assets/Prefabs/Hotspots/`
3. Prefab created: `Hotspot_Keyboard.prefab`

### 13.3. Prefab Workflow

**For new hotspots:**
1. Duplicate prefab
2. Rename (e.g., `Hotspot_Mouse`)
3. Override sprite, hotspotID, collectibleItem
4. Place in scene

---

## Step 14: Advanced - Custom Bounds Editing

### 14.1. Irregular Shapes

Nếu sprite có irregular shape (e.g., L-shaped object):

**Option A: Bỏ autoCalculateBounds, manual set Rect:**
- X/Y: offset từ GameObject position
- Width/Height: actual clickable area

**Option B: Multiple Hotspots:**
- Create 2 HotspotComponents với different bounds
- Same hotspotID (cùng trigger)

### 14.2. Offset Bounds from Center

Example: Hotspot chỉ ở upper half của sprite:

```
customBounds:
  X: -0.5
  Y: 0      ← Start from center (không phải -0.5)
  Width: 1.0
  Height: 0.5  ← Chỉ 1/2 height
```

---

## Step 15: Hotspot Events Integration

### 15.1. Listening to Hotspot Events

HotspotComponent publishes events via EventManager:

**Events:**
- `ItemCollected`: Khi pickup item
- `HotspotTriggered`: Khi bất kỳ hotspot clicked
- Custom events từ `successEvent` field

**Example listener:**

```csharp
using CoderGoHappy.Events;

public class QuestManager : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.Instance.Subscribe("UnlockComputer", OnComputerUnlocked);
    }

    void OnDisable()
    {
        EventManager.Instance?.Unsubscribe("UnlockComputer", OnComputerUnlocked);
    }

    void OnComputerUnlocked(object data)
    {
        Debug.Log("[Quest] Computer unlocked! Progress updated.");
        // Update quest state, show UI, etc.
    }
}
```

---

## Troubleshooting

### Issue: Hotspot không detect click

**Nguyên nhân:** Bounds không overlap với mouse position

**Giải pháp:**
1. Enable Gizmos trong Game view để visualize bounds
2. Verify customBounds khớp với sprite visible area
3. Check Scene view debug - cyan box phải cover sprite
4. Nếu dùng autoCalculateBounds, verify SpriteRenderer có sprite assigned

### Issue: Multiple hotspots trigger cùng lúc

**Nguyên nhân:** Overlapping bounds, HotspotManager chọn hotspot cuối trong list

**Giải pháp:**
1. Adjust bounds để không overlap
2. Hoặc dùng Sprite Mask để define click area
3. HotspotManager iterate back-to-front (painter's algorithm) - hotspot trên cùng (render last) sẽ prioritize

### Issue: Hotspot không fade out sau khi pickup

**Nguyên nhân:** DOTween sequence error hoặc GameObject bị destroy trước animation complete

**Giải pháp:**
1. Check Console có DOTween error không
2. Verify `spriteRenderer.DOFade(0f, 0.5f).OnComplete(...)` có callback
3. Check GameObject không bị destroy bởi code khác

### Issue: Hotspot re-appears sau khi collected

**Nguyên nhân:** Scene state không được save

**Giải pháp:**
1. Verify `GameStateData.Instance.AddDisabledHotspot()` được call
2. Check SceneController save scene state trước khi transition
3. Console log `CheckSceneState()` để verify disabled list

---

## Performance Tips

### 1. Limit Hotspot Count per Scene

- < 20 hotspots: Excellent performance
- 20-50 hotspots: Good (no noticeable lag)
- \> 50 hotspots: Consider optimizing (spatial partitioning)

### 2. Disable Inactive Hotspots

Nếu hotspot chỉ active sau trigger:
```csharp
SetActive(false); // Completely disable, không check hover
```

### 3. Use Sprite Atlases

Bundle hotspot sprites vào 1 atlas để reduce draw calls.

---

## Next Steps

✅ Hotspot system setup hoàn tất!

**Tiếp theo:**
- [06-puzzle-setup.md](06-puzzle-setup.md) - Setup Puzzle System và UI
- [07-scriptableobject-creation.md](07-scriptableobject-creation.md) - Create ItemData và PuzzleConfig assets

---

## Summary Checklist

- [ ] HotspotManager GameObject created và initialized
- [ ] Test hotspots created (Pickup, Navigation, ItemUse, Puzzle, Examine)
- [ ] Custom bounds configured và verified (Scene view gizmos)
- [ ] Sprite swap/highlight working khi hover
- [ ] Pickup hotspot adds item to inventory
- [ ] Navigation hotspot transitions to target scene
- [ ] Scene state persistence working (collected items stay gone)
- [ ] HotspotManager linked to GameManager

**Nếu tất cả OK → Ready for [06-puzzle-setup.md](06-puzzle-setup.md)** 🧩
