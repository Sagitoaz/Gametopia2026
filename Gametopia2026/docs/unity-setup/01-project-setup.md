# 01 - Project Setup Guide

## Overview
Hướng dẫn cài đặt ban đầu cho dự án "Coder Go Happy" - một point-and-click puzzle game được xây dựng với Unity 2D URP.

**Yêu cầu:**
- Unity 2022.3 LTS trở lên
- DOTween (Free or Pro)
- Universal Render Pipeline (URP) package

---

## Step 1: Import DOTween Package

DOTween được sử dụng rộng rãi trong project cho animations (fade transitions, scale effects, pulses, shakes).

### 1.1. Download DOTween
1. Mở **Unity Asset Store** trong Unity Editor
2. Tìm kiếm "DOTween"
3. Download và Import **DOTween (HOTween v2)** by Demigiant
4. Hoặc download từ: https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

### 1.2. Import vào Project
1. **Window → Package Manager**
2. Chuyển sang tab **My Assets**
3. Tìm **DOTween**, click **Download** rồi **Import**
4. Import popup sẽ hiện lên - click **Import** để import tất cả files

### 1.3. Setup DOTween
1. Sau khi import, popup **DOTween Utility Panel** sẽ xuất hiện
2. Click **Setup DOTween** để tạo DOTween Settings
3. Kiểm tra modules được enable:
   - ✅ Default Modules (đã enable sẵn)
   - ✅ Physics2D (quan trọng cho 2D game)
   - ✅ UI (cần cho InventoryUI, PuzzleUI animations)

> **Lưu ý:** Nếu popup không xuất hiện, vào **Tools → Demigiant → DOTween Utility Panel**

---

## Step 2: Verify Universal Render Pipeline (URP)

Project "Coder Go Happy" sử dụng URP 2D cho rendering optimization.

### 2.1. Check Package Manager
1. **Window → Package Manager**
2. Chuyển sang **Unity Registry**
3. Tìm **Universal RP** trong danh sách
4. Verify version: **12.x hoặc cao hơn** (tương thích Unity 2022.3+)

### 2.2. Check Project Settings
1. **Edit → Project Settings → Graphics**
2. Kiểm tra **Scriptable Render Pipeline Settings**:
   - Phải reference một **UniversalRenderPipelineAsset** (thường ở `Assets/Settings/URP-2D-Asset`)
   - Nếu chưa có, tạo mới:
     - Right-click trong Project window → **Create → Rendering → URP Asset (with 2D Renderer)**
     - Assign vào Graphics settings

### 2.3. Configure Quality Settings
1. **Edit → Project Settings → Quality**
2. Đảm bảo các Quality levels đều reference URP asset
3. Set **Default Quality Level** = **Medium** hoặc **High**

---

## Step 3: Folder Structure Setup

Project đã có structure như sau (verify bằng cách check Project window):

```
Assets/
├── Prefabs/              # Game object prefabs
│   ├── Core/            # GameManager, EventManager, etc.
│   ├── UI/              # InventoryUI, PuzzleUI, etc.
│   └── Hotspots/        # Hotspot component prefabs
│
├── Resources/           # Runtime-loadable assets
│   ├── Items/           # ItemData ScriptableObjects
│   ├── Puzzles/         # PuzzleConfig ScriptableObjects
│   └── Prefabs/         # Dynamically loaded prefabs
│
├── Scenes/              # Game scenes
│   ├── Persistent/      # DontDestroyOnLoad scene (GameManager)
│   └── Levels/          # Game level scenes
│
├── Scripts/             # C# source code (ĐÃ TẠO)
│   ├── Core/            # GameManager, GameStateData
│   ├── Events/          # EventManager, GameEvents
│   ├── Scene/           # SceneController, SceneState
│   ├── Data/            # ItemData ScriptableObject
│   ├── Inventory/       # InventorySystem, InventoryUI
│   ├── Interaction/     # HotspotManager, HotspotComponent
│   └── Puzzle/          # PuzzleSystem, PuzzleBase, concrete puzzles
│
├── Sprites/             # 2D artwork
│   ├── Characters/      # Character sprites
│   ├── Items/           # Item sprites (inventory + world)
│   ├── UI/              # UI elements, buttons, panels
│   └── Backgrounds/     # Scene backgrounds
│
└── Audio/               # Sound effects and music
    ├── SFX/             # Sound effects
    └── Music/           # Background music
```

### 3.1. Create Missing Folders

Nếu các folders chưa tồn tại, tạo chúng:

1. Right-click trong **Project window** → **Create → Folder**
2. Tạo các folders theo structure trên
3. Quan trọng nhất là folders **Resources** (cần cho runtime loading):
   ```
   Assets/Resources/Items/
   Assets/Resources/Puzzles/
   Assets/Resources/Prefabs/
   ```

> **Why Resources folder?**  
> - `InventorySystem.LoadItemsFromResources()` cần `Resources/Items/` để load ItemData
> - `PuzzleSystem` có thể cần load PuzzleConfig từ `Resources/Puzzles/`

---

## Step 4: Scene Setup

### 4.1. Create Persistent Scene

Scene này chứa các GameObject persistent (DontDestroyOnLoad) như GameManager.

1. **File → New Scene**
2. Chọn template: **2D (URP)**
3. Save as `Assets/Scenes/Persistent/PersistentScene.unity`
4. Xóa Main Camera (sẽ dùng camera của từng level)

### 4.2. Create First Level Scene

1. **File → New Scene**
2. Chọn template: **2D (URP)**
3. Save as `Assets/Scenes/Levels/Level01.unity`
4. Keep Main Camera, set:
   - **Position**: (0, 0, -10)
   - **Size** (Orthographic): 5 hoặc theo design
   - **Background**: Color hoặc Skybox

### 4.3. Build Settings

1. **File → Build Settings**
2. Click **Add Open Scenes** để add cả 2 scenes:
   - ✅ `Scenes/Persistent/PersistentScene` (index 0)
   - ✅ `Scenes/Levels/Level01` (index 1)
3. **Note Build Index** - sẽ dùng trong `SceneController`

---

## Step 5: Input System (Optional - Mouse Click)

Game "Coder Go Happy" chủ yếu dùng mouse clicks, không cần Input System mới.

### 5.1. Verify Input Settings

1. **Edit → Project Settings → Player**
2. **Other Settings → Active Input Handling**:
   - Chọn **Input Manager (Old)** hoặc **Both**
   - **KHÔNG** chọn chỉ "Input System Package" vì code dùng `Input.GetMouseButtonDown()`

### 5.2. Mouse Cursor (Optional)

Nếu muốn custom cursor:
1. Import cursor sprite vào `Assets/Sprites/UI/`
2. Set **Texture Type** = **Cursor**
3. Code để set cursor sẽ thêm sau trong `GameManager` hoặc custom script

---

## Step 6: Canvas Setup (UI System)

### 6.1. Create Main Canvas

1. Trong **Level01 scene**, click **GameObject → UI → Canvas**
2. Rename thành `MainCanvas`
3. Configure Canvas:
   - **Render Mode**: Screen Space - Overlay
   - **Canvas Scaler** component:
     - **UI Scale Mode**: Scale With Screen Size
     - **Reference Resolution**: 1920 x 1080 (hoặc theo design)
     - **Match**: 0.5 (width/height balance)

### 6.2. Add Event System

Canvas sẽ tự động tạo **EventSystem** GameObject.

**Verify EventSystem có:**
- ✅ EventSystem component
- ✅ Standalone Input Module component

> **Important:** Chỉ có 1 EventSystem trong scene. Nếu load multiple scenes, cần destroy duplicates.

---

## Step 7: Project Settings - General

### 7.1. Player Settings

**Edit → Project Settings → Player:**

- **Company Name**: Tên team/developer của bạn
- **Product Name**: "Coder Go Happy"
- **Default Icon**: (Import logo sprite, set làm icon)

**Resolution and Presentation:**
- **Default Screen Width**: 1920
- **Default Screen Height**: 1080
- **Fullscreen Mode**: Windowed (hoặc theo preference)

### 7.2. Time Settings

**Edit → Project Settings → Time:**

- **Fixed Timestep**: 0.02 (default OK)
- **Time Scale**: 1 (sẽ change thành 0 khi pause game)

### 7.3. Tags and Layers

**Edit → Project Settings → Tags and Layers:**

Tạo các **Layers** sau (optional nhưng useful):
- `UI` (layer 5 - built-in)
- `Hotspot` (layer 8)
- `Item` (layer 9)
- `Background` (layer 10)

**Note:** HotspotComponent KHÔNG dùng Collider2D, nên layers này chỉ để organize visually.

---

## Step 8: Verify Script Compilation

### 8.1. Check Console

1. Mở **Window → General → Console** (Ctrl+Shift+C)
2. Clear console: click **Clear** button
3. Kiểm tra:
   - ✅ **No compilation errors** (0 red icons)
   - ⚠️ Có thể có warnings (vàng) - OK nếu không critical

### 8.2. Verify Assemblies

Scripts của bạn nên compile thành các assemblies:
- `Assembly-CSharp.dll` (default assembly)

Nếu muốn tối ưu compile time, có thể tạo assembly definitions sau:
- `Assets/Scripts/Core.asmdef`
- `Assets/Scripts/Inventory.asmdef`
- v.v.

**Nhưng KHÔNG bắt buộc cho project này.**

---

## Step 9: Initial Test

### 9.1. Create Temporary Test Script

Tạo script đơn giản để test DOTween:

```csharp
using UnityEngine;
using DG.Tweening;

public class TestDOTween : MonoBehaviour
{
    void Start()
    {
        Debug.Log("DOTween Test - Pulsing...");
        transform.DOScale(1.5f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
}
```

1. Tạo file `Assets/Scripts/TestDOTween.cs` với code trên
2. Trong **Level01 scene**, create Empty GameObject
3. Add component **TestDOTween**
4. Press **Play** → GameObject sẽ pulse (scale lên xuống)
5. Nếu chạy OK → DOTween đã setup đúng! ✅
6. Xóa test script và GameObject sau khi test xong

---

## Step 10: Save Project

1. **File → Save Project** (Ctrl+S)
2. Đảm bảo tất cả scenes đã save:
   - `Scenes/Persistent/PersistentScene.unity`
   - `Scenes/Levels/Level01.unity`

---

## Troubleshooting

### Issue: DOTween không compile

**Giải pháp:**
1. Xóa folder `Assets/Demigiant/DOTween/`
2. Re-import DOTween từ Asset Store
3. Trong DOTween Setup Utility Panel, click **Setup DOTween** lại

### Issue: URP rendering sai màu/tối

**Giải pháp:**
1. Check **Project Settings → Graphics** → phải có URP asset assigned
2. Check Camera component → **Rendering → Renderer** phải là **Universal RP**
3. Nếu vẫn lỗi, tạo URP asset mới: **Create → Rendering → URP Asset (with 2D Renderer)**

### Issue: Canvas UI không hiện

**Giải pháp:**
1. Check Canvas **Render Mode** = Screen Space - Overlay
2. Check EventSystem có trong scene
3. Check UI elements có **Canvas Renderer** component
4. Check Camera có tag **MainCamera**

---

## Next Steps

✅ Project setup hoàn tất!

**Tiếp theo:**
- [02-core-systems.md](02-core-systems.md) - Setup GameManager, EventManager, persistent systems
- [03-scene-setup.md](03-scene-setup.md) - Setup SceneController và scene transitions

---

## Summary Checklist

Trước khi chuyển sang guide tiếp theo, verify:

- [ ] DOTween imported và setup xong
- [ ] URP configured trong Project Settings
- [ ] Folder structure đã tạo (đặc biệt Resources folders)
- [ ] PersistentScene và Level01 scene đã tạo
- [ ] Canvas + EventSystem đã setup trong Level01
- [ ] Build Settings có 2 scenes (PersistentScene index 0, Level01 index 1)
- [ ] Console không có compilation errors
- [ ] Project đã Save

**Nếu tất cả đã OK → Chuyển sang [02-core-systems.md](02-core-systems.md)** 🚀
