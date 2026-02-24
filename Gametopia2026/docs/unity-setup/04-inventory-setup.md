# 04 - Inventory System Setup

## Overview
Hướng dẫn setup **InventorySystem** và **InventoryUI** với drag-drop interaction, inventory slots, và tooltip system.

**Prerequisites:**
- Đã hoàn thành [03-scene-setup.md](03-scene-setup.md)
- Canvas đã setup trong Level01 scene

---

## Architecture Overview

```
Canvas
└── InventoryPanel (GameObject)
    ├── InventorySystem (script) - Logic backend
    ├── InventoryUI (script) - UI controller
    ├── Background (Image)
    ├── SlotsContainer (GridLayoutGroup)
    │   ├── Slot_0 (InventorySlot)
    │   ├── Slot_1 (InventorySlot)
    │   └── ... (up to 20 slots)
    ├── TooltipPanel (Panel)
    │   ├── TooltipText (Text)
    │   └── Background
    └── DraggedItemIcon (Image) - Follows cursor when dragging
```

---

## Step 1: Create InventoryPanel Container

### 1.1. Create Panel GameObject

1. Right-click **Canvas** → **UI → Panel**
2. Rename thành `InventoryPanel`

### 1.2. Configure RectTransform

Position InventoryPanel ở bottom của screen:

**Anchor**: Bottom-Center
- Min: (0.5, 0)
- Max: (0.5, 0)
- Pivot: (0.5, 0)

**Size:**
- Width: 1000
- Height: 150

**Position:**
- X: 0
- Y: 10 (cách bottom 10 pixels)

### 1.3. Configure Background Image

**Image component:**
- **Color**: Semi-transparent dark (e.g., RGBA: 0, 0, 0, 180)
- **Sprite**: (optional) import inventory background sprite
- **Image Type**: Sliced (nếu dùng 9-slice sprite)

---

## Step 2: Add InventorySystem & InventoryUI Scripts

### 2.1. Add InventorySystem Component

1. Select **InventoryPanel**
2. **Inspector → Add Component**
3. Search: **Inventory System**
4. Component added ✅

### 2.2. Add InventoryUI Component

1. Vẫn select **InventoryPanel**
2. **Add Component**
3. Search: **Inventory UI**
4. Component added ✅

**InventoryPanel GameObject sẽ có:**
- ✅ RectTransform
- ✅ Image (background)
- ✅ InventorySystem
- ✅ InventoryUI

---

## Step 3: Create Inventory Slots

### 3.1. Create SlotsContainer

1. Right-click **InventoryPanel** → **Create Empty**
2. Rename: `SlotsContainer`
3. **Add Component → Grid Layout Group**

### 3.2. Configure GridLayoutGroup

**Grid Layout Group settings:**

| Field | Value | Mô tả |
|-------|-------|-------|
| **Cell Size** | 80 x 80 | Slot size (pixels) |
| **Spacing** | 10 x 10 | Gap between slots |
| **Start Corner** | Upper Left | Layout direction |
| **Start Axis** | Horizontal | Fill rows first |
| **Child Alignment** | Middle Center | Center slots |
| **Constraint** | Fixed Column Count | Lock columns |
| **Constraint Count** | 10 | 10 slots per row (2 rows total) |

### 3.3. Configure RectTransform

**SlotsContainer RectTransform:**
- **Anchor**: Stretch-Stretch
- **Left/Right/Top/Bottom**: 10 (padding)

---

## Step 4: Create Slot Prefab

### 4.1. Create First Slot

1. Right-click **SlotsContainer** → **UI → Image**
2. Rename: `Slot_0`

### 4.2. Configure Slot RectTransform

**Width/Height:** 80 x 80 (match GridLayoutGroup Cell Size)

### 4.3. Configure Slot Background

**Image component:**
- **Color**: Dark gray (e.g., RGBA: 60, 60, 60, 255)
- **Sprite**: (optional) import slot frame sprite

### 4.4. Add InventorySlot Script

1. Select **Slot_0**
2. **Add Component → Inventory Slot**

### 4.5. Configure InventorySlot Component

**Inspector fields:**

| Field | Value | Assign |
|-------|-------|--------|
| **itemIcon** | (ต้อง assign) | Create child Image (step 4.6) |
| **slotIndex** | 0 | Auto-set by InventoryUI |
| **slotBackground** | (optional) | Reference to Slot_0's Image |

### 4.6. Create ItemIcon Child

1. Right-click **Slot_0** → **UI → Image**
2. Rename: `ItemIcon`
3. Configure:
   - **Anchor**: Stretch-Stretch
   - **Left/Right/Top/Bottom**: 5 (padding inside slot)
   - **Color**: White (sẽ tint sprite)
   - **Sprite**: None (empty lúc start)
   - **Preserve Aspect**: Tích ✅
   - **Raycast Target**: **BỎ TÍCH** (để không block slot click)

### 4.7. Assign ItemIcon to InventorySlot

1. Select **Slot_0**
2. Drag **ItemIcon** vào field **Item Icon** trong InventorySlot component

---

## Step 5: Duplicate Slots (Create 20 slots total)

### 5.1. Duplicate Slot_0

1. Select **Slot_0**
2. **Ctrl+D** (duplicate) 19 lần
3. Rename manually:
   - Slot_0, Slot_1, Slot_2, ..., Slot_19

**GridLayoutGroup sẽ tự động arrange slots thành 2 rows x 10 columns.**

### 5.2. Auto-Assign Slot Indices (via Script - Optional)

Nếu không muốn manual assign, InventoryUI sẽ tự động assign `slotIndex` trong `Awake()`:

```csharp
for (int i = 0; i < inventorySlots.Length; i++)
{
    inventorySlots[i].Initialize(i);
}
```

**Không cần làm gì thêm!** ✅

---

## Step 6: Create Slot Prefab for Reuse

### 6.1. Save as Prefab

1. Select **Slot_0** trong Hierarchy
2. Drag vào `Assets/Prefabs/UI/`
3. Prefab created: `InventorySlot.prefab`

### 6.2. Replace Existing Slots with Prefab (Optional)

1. Delete Slot_1 đến Slot_19
2. Drag `InventorySlot.prefab` vào **SlotsContainer** 19 lần
3. GridLayoutGroup auto-arrange

**Hoặc giữ nguyên manual slots** - đều OK!

---

## Step 7: Create TooltipPanel

### 7.1. Create Tooltip Background

1. Right-click **InventoryPanel** → **UI → Panel**
2. Rename: `TooltipPanel`
3. **RectTransform:**
   - **Width**: 300
   - **Height**: 100
   - **Position**: Hover above slots (e.g., Y = 100)

### 7.2. Configure Tooltip Image

**Image component:**
- **Color**: RGBA: 0, 0, 0, 220 (dark semi-transparent)
- **Sprite**: (optional) tooltip background sprite

### 7.3. Create Tooltip Text

1. Right-click **TooltipPanel** → **UI → Text**
2. Rename: `TooltipText`
3. Configure:
   - **Anchor**: Stretch-Stretch
   - **Padding**: 10 on all sides
   - **Font Size**: 16
   - **Color**: White
   - **Alignment**: Center-Middle
   - **Horizontal/Vertical Overflow**: Wrap

### 7.4. Hide Tooltip by Default

1. Select **TooltipPanel**
2. **Inspector** → **Uncheck** checkbox bên cạnh tên GameObject
3. Tooltip sẽ hidden lúc start ✅

---

## Step 8: Create DraggedItemIcon

### 8.1. Create Image for Dragged Item

1. Right-click **InventoryPanel** → **UI → Image**
2. Rename: `DraggedItemIcon`
3. Configure:
   - **RectTransform:**
     - Width: 80
     - Height: 80
   - **Image:**
     - **Color**: White (with alpha = 180 for transparency)
     - **Sprite**: None (will be set dynamically)
     - **Raycast Target**: **BỎ TÍCH** (không block clicks)
     - **Preserve Aspect**: Tích ✅

### 8.2. Hide by Default

1. **Uncheck** GameObject để hide
2. InventoryUI sẽ show/hide khi drag/drop

---

## Step 9: Wire UI References to InventoryUI

### 9.1. Assign All Slots

1. Select **InventoryPanel**
2. Trong **InventoryUI component**, tìm **Inventory Slots** array
3. Set **Size**: 20
4. Drag từng slot (Slot_0 đến Slot_19) vào array:
   - Element 0 = Slot_0
   - Element 1 = Slot_1
   - ...
   - Element 19 = Slot_19

**Tip:** Có thể drag tất cả cùng lúc:
1. Select tất cả slots trong SlotsContainer (Shift+Click)
2. Drag vào array field → Unity tự populate

### 9.2. Assign Other References

**InventoryUI component fields:**

| Field | Assign To | GameObject Path |
|-------|-----------|-----------------|
| **draggedItemIcon** | DraggedItemIcon | InventoryPanel/DraggedItemIcon |
| **tooltipPanel** | TooltipPanel | InventoryPanel/TooltipPanel |
| **tooltipText** | TooltipText | InventoryPanel/TooltipPanel/TooltipText |

---

## Step 10: Wire InventorySystem References

### 10.1. No Inspector Fields Required

InventorySystem **không có** Inspector fields cần assign!

Tất cả logic internal:
- `items`: List<ItemData> - runtime data
- `selectedItem`: ItemData - runtime data
- MaxInventorySlots: const int = 20 - hardcoded

### 10.2. Verify Auto-Initialization

InventorySystem tự động:
1. Load items từ `Resources/Items/` folder (nếu có)
2. Subscribe to EventManager
3. Publish events khi items change

---

## Step 11: Link InventorySystem to GameManager

### 11.1. Automatic Linking

GameManager tự động find InventorySystem:
```csharp
if (inventorySystem == null)
    inventorySystem = FindFirstObjectByType<InventorySystem>();
```

### 11.2. Manual Assignment (Optional)

1. Mở **PersistentScene**
2. Select **GameManager**
3. Trong **GameManager component**, field **Inventory System**:
   - Để **null** (sẽ auto-find)
   - Hoặc drag InventoryPanel từ Level01 scene (nếu shared prefab)

---

## Step 12: Test Inventory System

### 12.1. Create Test Item (ScriptableObject)

Tạm thời tạo 1 ItemData để test:

1. Right-click `Assets/Resources/Items/` → **Create → Coder Go Happy → Item Data**
2. Rename: `TestItem_Keyboard`
3. Configure:
   - **Item ID**: "test_keyboard" (auto-generated)
   - **Item Name**: "Keyboard"
   - **Description**: "A test keyboard item"
   - **Sprite**: (import test sprite hoặc dùng Unity default)
   - **Is Mini Bug**: Unchecked

### 12.2. Test Adding Item (Runtime)

Create test script: `Assets/Scripts/TestInventory.cs`

```csharp
using UnityEngine;
using CoderGoHappy.Inventory;
using CoderGoHappy.Data;

public class TestInventory : MonoBehaviour
{
    void Start()
    {
        // Wait 1 second then add test item
        Invoke("AddTestItem", 1f);
    }

    void AddTestItem()
    {
        // Load item from Resources
        ItemData item = Resources.Load<ItemData>("Items/TestItem_Keyboard");
        
        if (item != null)
        {
            InventorySystem inventorySystem = FindFirstObjectByType<InventorySystem>();
            
            if (inventorySystem != null)
            {
                bool success = inventorySystem.AddItem(item);
                Debug.Log($"[TEST] Add item: {success}");
            }
        }
        else
        {
            Debug.LogError("[TEST] TestItem_Keyboard not found in Resources/Items/");
        }
    }
}
```

**Test:**
1. Attach script vào any GameObject trong Level01
2. Press **Play**
3. After 1 second, keyboard item sẽ xuất hiện trong inventory slot đầu tiên
4. Hover mouse lên slot → Tooltip hiện "Keyboard"
5. Click slot → Item được select (highlight)
6. Drag item → Icon follows cursor
7. Drop → Item returns to slot (hoặc swap nếu drop vào slot khác)

**Xóa test script sau khi test xong.**

---

## Step 13: Configure Drag-Drop Settings

### 13.1. Canvas Raycast Settings

InventoryUI drag-drop cần EventSystem:

1. Verify **EventSystem** có trong scene
2. Check **Standalone Input Module** component có enabled

### 13.2. Drag Threshold (Optional)

Nếu drag quá sensitive hoặc không responsive:

**Edit → Project Settings → Input Manager → Drag Threshold**
- Default: 10 pixels
- Increase nếu quá sensitive (e.g., 20)
- Decrease nếu không responsive (e.g., 5)

---

## Step 14: Styling (Optional)

### 14.1. Import UI Sprites

1. Import inventory UI sprites vào `Assets/Sprites/UI/Inventory/`:
   - `slot_background.png`
   - `slot_highlight.png`
   - `tooltip_background.png`

2. Set **Texture Type** = **Sprite (2D and UI)**
3. Set **Sprite Mode** = **Single**

### 14.2. Apply Sprites

**Slot Background:**
- Select all Slot GameObjects (Slot_0 to Slot_19)
- Trong Image component, assign `slot_background` sprite

**Tooltip Background:**
- Select TooltipPanel
- Assign `tooltip_background` sprite

### 14.3. Font (Optional)

Nếu muốn custom font:
1. Import .ttf font vào `Assets/Fonts/`
2. Select TooltipText
3. **Font** field → Assign custom font

---

## Step 15: Performance Optimization

### 15.1. Object Pooling (Advanced - Optional)

Nếu có nhiều items spawn/despawn:

Create **InventorySlotPool** script để pool slots thay vì create/destroy.

**Nhưng với 20 static slots → KHÔNG CẦN object pooling.**

### 15.2. Disable Raycast on Hidden Elements

Verify:
- ItemIcon: **Raycast Target** = OFF ✅
- DraggedItemIcon: **Raycast Target** = OFF ✅
- Hidden panels: Disable GameObject thay vì chỉ set alpha = 0

---

## Troubleshooting

### Issue: Slots không click được

**Nguyên nhân:** EventSystem thiếu hoặc Raycast blocked

**Giải pháp:**
1. Check EventSystem có trong scene
2. Verify Slot GameObject có **Image** component (cần để receive clicks)
3. Check **Raycast Target** của slot Image = **ON**
4. Check không có GameObject nào che phủ slots (check Hierarchy order)

### Issue: Drag không hoạt động

**Nguyên nhân:** IBeginDragHandler/IDragHandler/IEndDragHandler không receive events

**Giải pháp:**
1. Verify InventoryUI attached đúng GameObject
2. Check Canvas có **Graphic Raycaster** component
3. Check slots có **Image** component và Raycast Target enabled

### Issue: Tooltip không hiện

**Nguyên nhân:** Reference chưa assign hoặc TooltipPanel không enable

**Giải pháp:**
1. Check InventoryUI → tooltipPanel, tooltipText assigned đúng
2. Verify TooltipPanel có **Canvas Group** component (nếu dùng alpha fade)
3. Check code `ShowTooltip()` có được call khi hover

### Issue: Items không persist khi chuyển scene

**Nguyên nhân:** GameStateData chưa save hoặc InventorySystem không load từ GameState

**Giải pháp:**
1. Check GameManager.SaveGame() được call trước khi transition
2. Verify InventorySystem.LoadFromGameState() được call trong Start()
3. Check Console có error về JSON serialization không

---

## Next Steps

✅ Inventory System setup hoàn tất!

**Tiếp theo:**
- [05-hotspot-setup.md](05-hotspot-setup.md) - Setup Hotspots cho interaction
- [07-scriptableobject-creation.md](07-scriptableobject-creation.md) - Create ItemData assets

---

## Summary Checklist

- [ ] InventoryPanel created với InventorySystem + InventoryUI scripts
- [ ] 20 InventorySlots created và arranged trong GridLayoutGroup
- [ ] TooltipPanel + TooltipText configured
- [ ] DraggedItemIcon created
- [ ] All UI references assigned to InventoryUI component
- [ ] Test item added successfully và hiện trong slot
- [ ] Drag-drop functionality tested và working
- [ ] Tooltip hiện khi hover
- [ ] Item selection (highlight) working

**Nếu tất cả OK → Ready for [05-hotspot-setup.md](05-hotspot-setup.md)** 🖱️
