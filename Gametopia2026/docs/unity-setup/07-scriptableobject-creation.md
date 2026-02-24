# 07 - ScriptableObject Creation Guide

## Overview
Hướng dẫn tạo **ItemData** và **PuzzleConfig** ScriptableObject assets - data-driven configuration cho items và puzzles trong game.

**Prerequisites:**
- Đã setup code structure từ guides trước
- Đã import sprites vào project

---

## What are ScriptableObjects?

**ScriptableObjects** là Unity data containers:
- Store data independently từ GameObjects
- Share data across scenes
- Lightweight (không cần MonoBehaviour overhead)
- Easy to edit trong Inspector
- Version control friendly (text-based assets)

**Project sử dụng 2 loại:**
1. **ItemData** - Inventory items (keyboard, mouse, USB, etc.)
2. **PuzzleConfig** - Puzzle configurations (solutions, rewards)

---

## Part 1: Creating ItemData Assets

### Step 1.1: Prepare Item Sprites

**Import sprites:**
1. Copy item sprites vào `Assets/Sprites/Items/`
2. Select all sprites → **Inspector:**
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Single
   - **Pixels Per Unit**: 100 (hoặc theo design)
   - **Filter Mode**: Bilinear
   - **Max Size**: 512 hoặc 1024 (based on quality needs)
   - **Compression**: None hoặc Low Quality
3. **Apply**

**Naming convention:**
- `keyboard_sprite.png` - Icon trong inventory
- `keyboard_world.png` - Sprite trong scene world (optional - có thể dùng chung)

---

### Step 2: Create First ItemData

**Create asset:**

1. Right-click `Assets/Resources/Items/` → **Create → Coder Go Happy → Item Data**
2. Rename: `Item_Keyboard`
3. Asset created ✅

**Why Resources folder?**
- `InventorySystem.LoadItemsFromResources()` cần load items từ `Resources/Items/`
- Runtime loading support

---

### Step 3: Configure ItemData Fields

Select `Item_Keyboard` asset, trong Inspector:

#### Item Identity

| Field | Value | Mô tả |
|-------|-------|-------|
| **itemID** | "keyboard" | Auto-generated từ asset name, unique ID |
| **itemName** | "Wireless Keyboard" | Display name trong UI |
| **description** | "A wireless keyboard for the computer. Looks important!" | Tooltip text |

> **itemID** tự động generated trong `OnValidate()` từ asset file name

#### Item Visuals

| Field | Value | Mô tả |
|-------|-------|-------|
| **sprite** | keyboard_sprite | Icon trong inventory slots |
| **worldSprite** | keyboard_world | Sprite trong scene (có thể dùng chung với sprite nếu giống nhau) |

**Sprite assignment:**
1. Click **circle icon** bên phải field
2. Select sprite từ picker window
3. Hoặc drag sprite từ Project window vào field

#### Special Flags

| Field | Value | Mô tả |
|-------|-------|-------|
| **isMiniBug** | Unchecked | Có phải "Mini Bug" item không? (collectible secret) |

**isMiniBug** = true → Item được track riêng cho achievement "Collect all Mini Bugs"

---

### Step 4: Create More ItemData Assets

**Best practice workflow:**

1. Duplicate `Item_Keyboard`:
   - Right-click → **Duplicate** (Ctrl+D)
   - Rename: `Item_Mouse`, `Item_USBDrive`, etc.

2. Configure each asset:
   - Unity auto-updates **itemID** khi rename (via OnValidate)
   - Change **itemName**, **description**
   - Assign appropriate sprites

**Example item set pour "Coder Go Happy":**

| Asset Name | itemID | itemName | Description | isMiniBug |
|------------|--------|----------|-------------|-----------|
| Item_Keyboard | keyboard | Wireless Keyboard | A wireless keyboard... | No |
| Item_Mouse | mouse | Gaming Mouse | High DPI gaming mouse | No |
| Item_USBDrive | usb_drive | USB Drive | 64GB USB drive with data | No |
| Item_Headphones | headphones | Headphones | Noise-cancelling headphones | No |
| Item_CoffeeCup | coffee_cup | Coffee Cup | Empty coffee cup | Yes |
| Item_RubberDuck | rubber_duck | Rubber Duck | Debugging companion | Yes |

---

### Step 5: Organize ItemData Assets

**Folder structure trong Resources/Items/:**

```
Resources/
└── Items/
    ├── Essential/        # Items cần cho progression
    │   ├── Item_Keyboard.asset
    │   ├── Item_Mouse.asset
    │   └── Item_USBDrive.asset
    ├── MiniBugs/         # Collectible mini bugs
    │   ├── Item_CoffeeCup.asset
    │   └── Item_RubberDuck.asset
    └── Optional/         # Optional items
        └── Item_Headphones.asset
```

**Lưu ý:** Subfolder structure không ảnh hưởng loading - `Resources.LoadAll<ItemData>("Items")` sẽ load recursively.

---

### Step 6: Verify ItemData Loading

**Test loading:**

Create test script: `Assets/Scripts/TestItemLoading.cs`

```csharp
using UnityEngine;
using CoderGoHappy.Data;

public class TestItemLoading : MonoBehaviour
{
    void Start()
    {
        // Load all items from Resources/Items/
        ItemData[] items = Resources.LoadAll<ItemData>("Items");
        
        Debug.Log($"[TEST] Loaded {items.Length} items:");
        
        foreach (ItemData item in items)
        {
            Debug.Log($"  - {item.itemID}: {item.itemName} (MiniBug: {item.isMiniBug})");
        }
    }
}
```

**Expected Console output:**
```
[TEST] Loaded 6 items:
  - keyboard: Wireless Keyboard (MiniBug: False)
  - mouse: Gaming Mouse (MiniBug: False)
  - usb_drive: USB Drive (MiniBug: False)
  - headphones: Headphones (MiniBug: False)
  - coffee_cup: Coffee Cup (MiniBug: True)
  - rubber_duck: Rubber Duck (MiniBug: True)
```

---

## Part 2: Creating PuzzleConfig Assets

### Step 7: Create First PuzzleConfig

**Create asset:**

1. Right-click `Assets/Resources/Puzzles/` → **Create → Coder Go Happy → Puzzle Config**
2. Rename: `Puzzle_ComputerLogin`
3. Asset created ✅

---

### Step 8: Configure PuzzleConfig - ButtonSequence Example

Select `Puzzle_ComputerLogin`, configure:

#### Puzzle Identity

| Field | Value |
|-------|-------|
| **puzzleID** | "Puzzle_ComputerLogin" (auto from name) |
| **puzzleName** | "Computer Login" |
| **description** | "Enter the correct button sequence to unlock the computer" |

#### Puzzle Configuration

| Field | Value |
|-------|-------|
| **puzzleType** | ButtonSequence |
| **difficulty** | 2 (1=easy, 5=hard) |

#### Solution Data

**For ButtonSequence:**
- **solution**: `"0,2,1,3"`
- Format: Comma-separated button indices (0-based)
- Meaning: Player must click buttons: **Button 0 → Button 2 → Button 1 → Button 3**

**Attempts & Time:**
- **maxAttempts**: 0 (unlimited) hoặc 3 (max 3 wrong attempts)
- **timeLimit**: 0 (no limit) hoặc 60 (60 seconds)

#### Rewards

| Field | Value |
|-------|-------|
| **rewardItem** | (drag `Item_USBDrive` asset) |
| **successEvent** | "ComputerUnlocked" |

**rewardItem**: Automatically added to inventory khi puzzle solved  
**successEvent**: Custom event name published khi solved (optional)

---

### Step 9: Configure PuzzleConfig - CodeInput Example

**Create new asset:**

1. Duplicate `Puzzle_ComputerLogin`
2. Rename: `Puzzle_SafeCode`

**Configure:**

| Field | Value |
|-------|-------|
| **puzzleID** | "Puzzle_SafeCode" |
| **puzzleName** | "Safe Lock Code" |
| **description** | "Hint: The code is the year Unity was released" |
| **puzzleType** | CodeInput |
| **difficulty** | 3 |
| **solution** | `"2005"` |
| **maxAttempts** | 5 |
| **rewardItem** | Item_KeyCard |
| **successEvent** | "SafeUnlocked" |

**Solution format cho CodeInput:**
- Chuỗi số (string): `"2005"`, `"1234"`, `"999"`
- KHÔNG dùng commas

---

### Step 10: Configure PuzzleConfig - ColorMatch Example

**Create asset:** `Puzzle_ColorSequence`

**Configure:**

| Field | Value |
|-------|-------|
| **puzzleID** | "Puzzle_ColorSequence" |
| **puzzleName** | "Color Memory Puzzle" |
| **description** | "Match the color sequence shown on the terminal" |
| **puzzleType** | ColorMatch |
| **difficulty** | 4 |
| **solution** | `"Red,Blue,Red,Green,Yellow"` |
| **maxAttempts** | 3 |
| **timeLimit** | 30 |
| **rewardItem** | (none) |
| **successEvent** | "TerminalAccess" |

**Solution format cho ColorMatch:**
- Comma-separated color names: `"Red,Blue,Green,Yellow"`
- **Phải khớp** color names trong ColorMatchPuzzle.colorNames array
- **Case-insensitive** (Red = red = RED)

---

### Step 11: Validate Solution Format

PuzzleConfig có **OnValidate()** để tự kiểm tra solution format:

**Console warnings nếu sai format:**

```
[PuzzleConfig] Puzzle_ComputerLogin: ButtonSequence solution should be comma-separated integers (e.g., '0,2,1,3')
```

**Fix:**
1. Check solution field
2. Ensure format đúng theo loại puzzle
3. Warning sẽ mất khi format correct

---

### Step 12: Create Puzzle Asset Collection

**Recommended assets cho một level:**

| Puzzle Name | Type | Solution Example | Difficulty |
|-------------|------|------------------|------------|
| Computer Password | CodeInput | "2026" | 1 |
| Button Lock | ButtonSequence | "0,1,2,3" | 1 |
| Safe Code | CodeInput | "8472" | 2 |
| Color Memory | ColorMatch | "Red,Blue,Green" | 3 |
| Advanced Sequence | ButtonSequence | "3,1,0,2,1" | 4 |
| RGB Code | ColorMatch | "Red,Red,Green,Blue,Green" | 5 |

---

### Step 13: Link PuzzleConfig to Puzzle Scripts

**Assignment:**

1. Open scene với puzzle UI panels (từ [06-puzzle-setup.md](06-puzzle-setup.md))
2. Select **ButtonSequencePuzzle** panel
3. Trong **ButtonSequencePuzzle component**, field **config**:
   - Drag `Puzzle_ComputerLogin` asset
4. Repeat cho other puzzle types

**Result:** Puzzle script sẽ load solution, settings từ PuzzleConfig.

---

### Step 14: Link PuzzleConfig to Hotspots

**Assignment:**

1. Select Hotspot với type = Puzzle
2. Trong **HotspotComponent**, field **puzzleID**:
   - Type: `"Puzzle_ComputerLogin"` (exact match với PuzzleConfig.puzzleID)

**Trigger flow:**
1. Player clicks hotspot
2. HotspotComponent publishes `ShowPuzzle` event với puzzleID
3. PuzzleSystem finds puzzle by ID
4. Shows corresponding puzzle UI
5. Puzzle loads solution từ PuzzleConfig

---

## Part 3: Best Practices

### BP 1: Asset Naming Convention

**ItemData:**
- Prefix: `Item_`
- PascalCase: `Item_KeyboardWireless`, `Item_USBDrive64GB`
- Unique và descriptive

**PuzzleConfig:**
- Prefix: `Puzzle_`
- Include type hint: `Puzzle_CodeInput_Safe`, `Puzzle_ButtonSeq_ComputerLogin`

### BP 2: Version Control

**Git-friendly:**
- ScriptableObjects are **text assets** (.asset files)
- Easy to diff và merge
- Add to version control ✅

**.gitignore:**
- Ignore `.meta` conflicts (Unity regenerates)
- Keep `.asset` files tracked

### BP 3: Organization

**Folder structure:**
```
Resources/
├── Items/
│   ├── Essential/      # Critical path items
│   ├── MiniBugs/       # Collectibles
│   └── Optional/       # Side content
└── Puzzles/
    ├── Level01/        # Per-level puzzles
    ├── Level02/
    └── Shared/         # Puzzles reused across levels
```

### BP 4: Data Validation

**ItemData validation:**
- `OnValidate()` auto-generates itemID từ asset name
- Verify sprites assigned (Console warning if null)

**PuzzleConfig validation:**
- `OnValidate()` checks solution format
- Warns nếu format invalid

**Always check Console** sau khi create/edit assets!

---

## Part 4: Advanced Workflows

### Advanced 1: Batch Create Items via Script (Optional)

**Use case:** Tạo nhiều items cùng lúc

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using CoderGoHappy.Data;

public class ItemDataBatchCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Test Items")]
    static void CreateTestItems()
    {
        string[] itemNames = { "Keyboard", "Mouse", "Monitor" };
        
        foreach (string name in itemNames)
        {
            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = name;
            item.itemID = name.ToLower();
            
            string path = $"Assets/Resources/Items/Item_{name}.asset";
            AssetDatabase.CreateAsset(item, path);
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("Test items created!");
    }
}
#endif
```

**Usage:** **Tools → Create Test Items** trong Unity menu bar

### Advanced 2: Import from JSON/CSV (Optional)

**Use case:** Game designers dùng spreadsheet để design items

1. Export CSV từ Google Sheets
2. Parse CSV trong Unity Editor script
3. Auto-create ItemData/PuzzleConfig assets

**Example CSV format:**
```csv
itemID,itemName,description,isMiniBug
keyboard,Wireless Keyboard,A keyboard...,false
mouse,Gaming Mouse,A mouse...,false
```

---

## Troubleshooting

### Issue: itemID không auto-update khi rename asset

**Nguyên nhân:** OnValidate không được trigger

**Giải pháp:**
1. Select asset trong Project window
2. Modify bất kỳ field nào (e.g., add space vào description)
3. OnValidate sẽ trigger → itemID updates
4. Hoặc manual type itemID = asset name

### Issue: "Resources.LoadAll<ItemData> returns 0 items"

**Nguyên nhân:** Assets không ở trong Resources folder

**Giải pháp:**
1. Verify path: `Assets/Resources/Items/` (chính xác!)
2. Check asset type = ItemData (không phải prefab)
3. Reimport Resources folder: Right-click → Reimport

### Issue: PuzzleConfig solution validation warning

**Nguyên nhân:** Solution format sai

**Giải pháp:**
- **ButtonSequence**: "0,1,2" (integers, comma-separated, no spaces)
- **CodeInput**: "1234" (string số, no commas)
- **ColorMatch**: "Red,Blue,Green" (comma-separated, match colorNames array)

### Issue: Puzzle không load solution

**Nguyên nhân:** PuzzleConfig chưa assigned vào puzzle script

**Giải pháp:**
1. Select puzzle panel (ButtonSequencePuzzle, etc.)
2. Check **config** field có assigned asset không
3. Verify puzzleID trong config khớp với hotspot puzzleID

---

## Next Steps

✅ ScriptableObject assets creation hoàn tất!

**Tiếp theo:**
- [08-testing-guide.md](08-testing-guide.md) - Test toàn bộ systems integration

---

## Summary Checklist

**ItemData:**
- [ ] Sprites imported vào `Assets/Sprites/Items/`
- [ ] At least 5 ItemData assets created trong `Assets/Resources/Items/`
- [ ] itemID auto-generated correctly
- [ ] Sprites assigned (sprite, worldSprite)
- [ ] isMiniBug flag set cho collectibles
- [ ] Test loading: `Resources.LoadAll<ItemData>("Items")` returns assets

**PuzzleConfig:**
- [ ] At least 3 PuzzleConfig assets created (1 cho mỗi type)
- [ ] puzzleID matches asset name
- [ ] Solution formats validated (no warnings)
- [ ] Assigned to puzzle script components
- [ ] Linked to hotspots (puzzleID matches)

**Organization:**
- [ ] Assets organized trong subfolders
- [ ] Naming convention followed
- [ ] Version control added assets

**Nếu tất cả OK → Ready for final testing [08-testing-guide.md](08-testing-guide.md)** ✅
