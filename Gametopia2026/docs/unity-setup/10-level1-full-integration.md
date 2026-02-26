# 10 - Level 1 Full Integration & Level 2 Structure Setup

## Overview
Hướng dẫn setup **toàn bộ Level 1** end-to-end trong Unity Editor và tạo cấu trúc **Level 2**.  
Đây là bước tổng hợp sau khi đã code xong Day 5 — kết nối tất cả hệ thống lại với nhau.

**Mục tiêu:**
- Level 1 chơi được từ đầu đến cuối (3 scenes)
- Tất cả items, puzzles, hotspots, MiniBugs được tạo và gán
- LevelManager + BugCounterUI hoạt động
- Character happy state khi hoàn thành level
- Level 2 scenes tạo sẵn cấu trúc

**Prerequisites:**
- Đã hoàn thành ALL guides 01-09
- Đã có code: Core, Inventory, Interaction, Puzzle, Level, UI

---

## PHẦN A: TẠO SCENES CHO LEVEL 1

### Step 1: Tách Level 1 thành 3 Scenes

Hiện tại bạn có `Level01.unity`. Cần tách thành 3 scenes:

1. **File → Save Scene As...**
   - Save `Level01.unity` thành `Level01_Scene1.unity` (trong `Assets/Scenes/Levels/`)
   
2. **Duplicate scene:**
   - Trong Project window, select `Level01_Scene1.unity`
   - **Ctrl+D** để duplicate → rename thành `Level01_Scene2.unity`
   - **Ctrl+D** lần nữa → rename thành `Level01_Scene3.unity`

3. **Xóa file cũ** `Level01.unity` (nếu không cần nữa)

4. **Update Build Settings:**
   - **File → Build Settings**
   - Kéo các scene vào theo thứ tự:
     ```
     0: Scenes/Persistent/PersistentScenes
     1: Scenes/Levels/Level01_Scene1
     2: Scenes/Levels/Level01_Scene2
     3: Scenes/Levels/Level01_Scene3
     4: Scenes/Levels/Level02 (sẽ tách sau)
     ```

### Step 2: Setup Hierarchy cho mỗi Scene Level 1

**Mở Level01_Scene1.unity**, tạo hierarchy sau:

```
Level01_Scene1 (Scene)
├── Main Camera
│   └── (Camera component, Orthographic, Size = 5.4)
├── SceneController          ← Drag prefab từ Prefabs/Core/
├── LevelManager (Empty)     ← Chỉ Scene1 mới có!
├── Canvas (UI)
│   ├── FadeOverlay           ← Image fullscreen, black, CanvasGroup
│   ├── InventoryPanel        ← Setup theo guide 04
│   ├── BugCounterUI (Empty)  ← Text TMP ở góc phải trên
│   └── PuzzlePanel (Hidden)  ← Puzzle UI, SetActive(false)
├── Background               ← Sprite lớn (background art)
├── Hotspots (Empty Parent)
│   ├── Hotspot_Flashlight    ← Pickup type
│   ├── Hotspot_Screwdriver   ← Pickup type  
│   ├── Hotspot_ServerRack    ← Examine type
│   ├── Hotspot_OldPC         ← ItemUse type (cần tua vít)
│   ├── Hotspot_NavToScene2   ← Navigation type
│   └── Hotspot_Puzzle1       ← Puzzle type
├── MiniBugs (Empty Parent)
│   ├── MiniBug_01            ← Pickup type (isMiniBug)
│   ├── MiniBug_02
│   └── MiniBug_03            ← (3-4 bugs mỗi scene)
├── Character (Empty)
│   └── CoderSprite           ← Character sad sprite
└── InteractiveObjects
    └── (các objects trang trí có thể click)
```

**Level01_Scene2 và Scene3**: Tương tự nhưng **KHÔNG có LevelManager** (chỉ Scene1 có).

---

## PHẦN B: TẠO SCRIPTABLEOBJECT ASSETS

### Step 3: Tạo ItemData cho Level 1

**Tổ chức thư mục:**
```
Assets/Resources/Items/
├── Essential/          ← Items cần thiết để qua level
│   ├── Item_Flashlight.asset
│   ├── Item_Screwdriver.asset
│   └── Item_USBKeycard.asset
├── MiniBugs/           ← 10 MiniBug items
│   ├── MiniBug_L1_01.asset
│   ├── MiniBug_L1_02.asset
│   ├── ...
│   └── MiniBug_L1_10.asset
└── Optional/           ← Items phụ
    ├── Item_CodeSnippet1.asset
    └── Item_CodeSnippet2.asset
```

#### 3.1 Tạo Essential Items

**Cho mỗi item, làm:**

1. Right-click `Assets/Resources/Items/Essential/` → **Create → Coder Go Happy → Item Data**
   - Nếu không thấy menu này: Right-click → **Create → ScriptableObject** → tìm **ItemData**
2. Rename asset theo bảng dưới
3. Cấu hình trong Inspector:

| Asset Name | itemID | itemName | description | isConsumable | isMiniBug |
|-----------|--------|----------|-------------|-------------|-----------|
| Item_Flashlight | flashlight | Đèn Pin | Soi sáng các góc tối để tìm mã số | false | false |
| Item_Screwdriver | screwdriver | Tua Vít | Mở nắp thiết bị để lấy linh kiện | true | false |
| Item_USBKeycard | usb_keycard | USB Keycard | Thẻ từ để truy cập hệ thống | true | false |

**⚠️ Quan trọng:** 
- `itemID` phải KHỚP với string bạn dùng trong hotspot `requiredItemID`
- `isConsumable = true` nếu item biến mất sau khi sử dụng
- Gán **itemSprite** (icon cho inventory) và **worldSprite** (sprite hiển thị trong scene)

#### 3.2 Tạo MiniBug Items

1. Right-click `Assets/Resources/Items/MiniBugs/` → **Create → Coder Go Happy → Item Data**
2. Tạo 10 assets, **MiniBug_L1_01** đến **MiniBug_L1_10**

**Cấu hình GIỐNG NHAU cho tất cả MiniBugs:**

| Field | Value |
|-------|-------|
| itemID | minibug_l1_01 (tăng dần) |
| itemName | Mini Bug |
| description | Một con bọ phần mềm đang trốn! |
| isConsumable | true |
| **isMiniBug** | **true** ← QUAN TRỌNG! |
| itemSprite | (sprite con bug xanh lá) |

**⚠️ isMiniBug = true** là điều kiện để LevelManager đếm bug. Nếu quên check → bug không được đếm!

### Step 4: Tạo PuzzleConfig cho Level 1

**Tổ chức:**
```
Assets/Resources/Puzzles/
└── Level01/
    ├── Puzzle_ComputerLogin.asset     ← CodeInput type
    ├── Puzzle_NetworkFix.asset        ← ButtonSequence type  
    └── Puzzle_FinalCode.asset         ← ColorMatch type
```

1. Right-click `Assets/Resources/Puzzles/Level01/` → **Create → Coder Go Happy → Puzzle Config**
2. Cấu hình:

| Asset | puzzleID | puzzleType | solution | difficulty | maxAttempts |
|-------|----------|-----------|----------|-----------|-------------|
| Puzzle_ComputerLogin | puzzle_computer_login | CodeInput | "1337" | 1 | 5 |
| Puzzle_NetworkFix | puzzle_network_fix | ButtonSequence | "2,0,3,1" | 2 | 3 |
| Puzzle_FinalCode | puzzle_final_code | ColorMatch | "Red,Blue,Green,Yellow" | 3 | 3 |

**Solution format theo loại puzzle:**
- **CodeInput**: Chuỗi số, ví dụ "1337", "4269"
- **ButtonSequence**: Index buttons cách nhau bởi dấu phẩy, ví dụ "2,0,3,1"
- **ColorMatch**: Tên màu cách bởi dấu phẩy, ví dụ "Red,Blue,Green"

**Tùy chọn thêm:**
- `timeLimit`: Thời gian giới hạn (0 = không giới hạn)
- `rewardItemID`: Item nhận được khi solve (ví dụ: "usb_keycard")
- `successEventName`: Event tùy chỉnh khi solve

---

## PHẦN C: SETUP HOTSPOTS TRONG LEVEL 1

### Step 5: Setup Hotspots Scene 1 (Server Room Hallway)

#### 5.1 Hotspot Pickup - Flashlight

1. Tạo **Empty GameObject** trong `Hotspots/` → rename `Hotspot_Flashlight`
2. Add Component → **HotspotComponent**
3. Inspector config:

| Field | Value |
|-------|-------|
| Hotspot ID | "hotspot_flashlight_s1" |
| Hotspot Type | **Pickup** |
| Item To Give | (drag Item_Flashlight.asset vào đây) |
| Hover Text | "Nhặt đèn pin" |
| Interaction Sprite | (sprite đèn pin trong scene) |
| Use Custom Bounds | false (auto from SpriteRenderer) |
| Disable After Use | true |

4. Add **SpriteRenderer** → gán sprite đèn pin
5. Đặt vị trí: nơi đèn pin nằm trong background (ví dụ trên kệ server rack)

#### 5.2 Hotspot ItemUse - Old PC (cần tua vít)

1. Tạo `Hotspot_OldPC` trong `Hotspots/`
2. Add Component → **HotspotComponent**
3. Config:

| Field | Value |
|-------|-------|
| Hotspot ID | "hotspot_oldpc_s1" |
| Hotspot Type | **ItemUse** |
| Required Item ID | "screwdriver" |
| Item To Give | (item bên trong PC, ví dụ: Item_GraphicsCard) |
| Hover Text | "Mở nắp PC cũ" |
| Fail Text | "Cần tua vít để mở!" |
| Disable After Use | true |

#### 5.3 Hotspot Navigation - Sang Scene 2

1. Tạo `Hotspot_NavToScene2` → ĐẶT ở RÌA PHẢI màn hình
2. Add Component → **HotspotComponent**
3. Config:

| Field | Value |
|-------|-------|
| Hotspot ID | "nav_to_scene2" |
| Hotspot Type | **Navigation** |
| Target Scene Name | "Level01_Scene2" |
| Target Scene Index | 0 (hoặc để 0 nếu dùng scene name) |
| Hover Text | "→ Góc Kỹ Thuật" |
| Use Custom Bounds | true |
| Custom Bounds | x=8, y=0, width=2, height=10 (rìa phải) |
| Disable After Use | false |

**💡 Tip**: Navigation hotspots thường dùng Custom Bounds để tạo vùng click lớn ở rìa màn hình. Không cần SpriteRenderer — chỉ cần vùng click.

#### 5.4 Hotspot Puzzle - Computer Login

1. Tạo `Hotspot_Puzzle1`
2. Add Component → **HotspotComponent**
3. Config:

| Field | Value |
|-------|-------|
| Hotspot ID | "hotspot_puzzle_login_s1" |
| Hotspot Type | **Puzzle** |
| Puzzle Config | (drag Puzzle_ComputerLogin.asset) |
| Hover Text | "Đăng nhập máy tính" |
| Disable After Use | true |

#### 5.5 Hotspot Examine

1. Tạo `Hotspot_ServerRack`
2. Config:

| Field | Value |
|-------|-------|
| Hotspot ID | "examine_serverrack_s1" |
| Hotspot Type | **Examine** |
| Examine Text | "Tủ Server cũ kỹ. Có ghi chú dán: '&&' means AND, '||' means OR" |
| Hover Text | "Xem tủ Server" |
| Disable After Use | false |

### Step 6: Setup Hotspots Scene 2 (Tech Corner)

Mở **Level01_Scene2.unity**, tạo các hotspots tương tự:

| Hotspot | Type | Mô tả |
|---------|------|--------|
| Hotspot_Screwdriver | Pickup | Nhặt tua vít trên bàn |
| Hotspot_CableBox | ItemUse | Dùng đèn pin soi vào hộp cáp → thấy mã số |
| Hotspot_NavToScene1 | Navigation | ← Quay lại Scene 1 |
| Hotspot_NavToScene3 | Navigation | → Sang Scene 3 |
| Hotspot_Puzzle2 | Puzzle | Puzzle sửa mạng (NetworkFix) |
| MiniBug_04 → MiniBug_07 | Pickup (isMiniBug) | 4 bugs ẩn |

### Step 7: Setup Hotspots Scene 3 (Central Electrical Cabinet)

Mở **Level01_Scene3.unity**:

| Hotspot | Type | Mô tả |
|---------|------|--------|
| Hotspot_USBKeycard | Pickup | USB keycard trên bảng điện |
| Hotspot_MainPanel | ItemUse | Cắm USB vào panel chính |
| Hotspot_NavToScene2 | Navigation | ← Quay lại Scene 2 |
| Hotspot_FinalPuzzle | Puzzle | Puzzle mã cuối cùng (FinalCode) |
| MiniBug_08 → MiniBug_10 | Pickup (isMiniBug) | 3 bugs ẩn |

---

## PHẦN D: SETUP MINIBUGS

### Step 8: Tạo MiniBug Hotspots

Mỗi MiniBug là 1 **HotspotComponent** với type **Pickup**.

**Cho mỗi MiniBug:**

1. Tạo Empty GameObject con trong `MiniBugs/` parent
2. Rename: `MiniBug_01`, `MiniBug_02`, etc.
3. Add **SpriteRenderer** → gán sprite bug xanh lá (nhỏ, khó thấy)
4. Add **HotspotComponent**
5. Config:

| Field | Value |
|-------|-------|
| Hotspot ID | "minibug_01" (tăng dần) |
| Hotspot Type | **Pickup** |
| Item To Give | (drag MiniBug_L1_01.asset) |
| Hover Text | "Bắt bug!" |
| Disable After Use | true |

6. **ĐẶT VỊ TRÍ ẨN**: Đặt bug ở các vị trí khó thấy:
   - Trên dây cáp (theo theme Level 1)
   - Góc tối của tủ server
   - Dưới bàn, sau màn hình
   - Trên trần nhà, gần ống thông gió

**Phân bổ 10 bugs / 3 scenes:**
- Scene 1: 3 bugs
- Scene 2: 4 bugs  
- Scene 3: 3 bugs

**⚠️ Nhắc lại**: ItemData của MiniBug PHẢI có `isMiniBug = true`, nếu không LevelManager không đếm!

---

## PHẦN E: SETUP LEVELMANAGER

### Step 9: Cấu hình LevelManager (CHỈ trong Scene 1)

1. Mở **Level01_Scene1**
2. Select **LevelManager** GameObject
3. Add Component → **Level Manager** (script LevelManager.cs)
4. Inspector config:

| Field | Value |
|-------|-------|
| Level Number | 1 |
| Required Puzzles (size = 3) | |
| → Element 0 | "puzzle_computer_login" |
| → Element 1 | "puzzle_network_fix" |
| → Element 2 | "puzzle_final_code" |
| Required Items (size = 0) | (để trống nếu không bắt buộc collect item) |
| Total Mini Bugs In Level | 10 |
| Next Level Scene Name | "Level02_Scene1" |
| Character Object | (drag Character GameObject vào đây) |
| Happy Animation Trigger | "Happy" |

**⚠️ Lưu ý quan trọng:**
- `puzzleID` trong Required Puzzles phải **KHỚP CHÍNH XÁC** với `puzzleID` trong PuzzleConfig assets
- LevelManager chỉ có trong **Scene đầu tiên** của level. Các scene khác không cần
- LevelManager KHÔNG phải DontDestroyOnLoad — nó sống trong scene

### Step 10: Cấu hình BugCounterUI

1. Trong **Level01_Scene1**, mở Canvas
2. Tạo **Empty GameObject** con → rename `BugCounterUI`
3. Chỉnh **Rect Transform**:
   - Anchor: Top Right (Alt+click preset)
   - Pivot: (1, 1)
   - Pos X: -20, Pos Y: -20
   - Width: 200, Height: 50
4. Add child **TextMeshPro - Text (UI)** → rename `BugCountText`
   - Text: "🐛 0/10"
   - Font Size: 28
   - Alignment: Right, Middle
   - Color: White
5. Select `BugCounterUI` parent
6. Add Component → **Bug Counter UI** (script BugCounterUI.cs)
7. Inspector:

| Field | Value |
|-------|-------|
| Bug Count Text | (drag BugCountText TMP vào) |
| Bug Icon | (optional, để null nếu chưa có sprite) |
| Text Format | "🐛 {0}/{1}" |
| Completed Color | Yellow (hoặc Gold) |
| Normal Color | White |
| Pulse Scale | 1.3 |
| Pulse Duration | 0.3 |

**⚠️ Lưu ý:** BugCounterUI cần nằm trong MỌI scene của level (hoặc trong Canvas persistent). Nếu chỉ có ở Scene1, khi chuyển sang Scene2 sẽ mất UI bug counter.

**Giải pháp cho multi-scene:**
- **Option A**: Copy BugCounterUI vào Canvas của TỪNG scene (Scene1, Scene2, Scene3)
- **Option B**: Đặt BugCounterUI trong PersistentScene Canvas (nếu có UI persistent)

→ **Khuyến nghị Option A** cho đơn giản — mỗi scene có BugCounterUI riêng, nó tự sync từ LevelManager/GameStateData khi Start()

---

## PHẦN F: SETUP PUZZLES UI

### Step 11: Tạo Puzzle UI Panels

Mỗi scene có puzzle cần 1 **PuzzlePanel** trong Canvas.

#### 11.1 CodeInput Puzzle (Scene 1 - Computer Login)

1. Canvas → Create Empty → rename `PuzzlePanel_Login`
2. Chỉnh Rect Transform: Full stretch (all zeros)
3. Add Image component → Color: Black, Alpha = 180 (dim background)
4. Tạo children:
   - **Title** (TextMeshPro): "ĐĂNG NHẬP HỆ THỐNG"
   - **InputField** (TMP_InputField): Content Type = Integer Number
   - **SubmitButton** (Button - TMP): Text = "Xác Nhận"
   - **CloseButton** (Button - TMP): Text = "X" (góc phải trên)
   - **AttemptsText** (TextMeshPro): "Lần thử: 0/5"
5. Add Component → **CodeInputPuzzle**
6. Gán references:
   - Puzzle Config → drag `Puzzle_ComputerLogin.asset`
   - Input Field → drag TMP_InputField
   - Submit Button → drag SubmitButton
   - Close Button → drag CloseButton
   - Feedback Text → drag AttemptsText
7. **SetActive(false)** panel này! (puzzle chỉ mở khi click hotspot)

#### 11.2 ButtonSequence Puzzle (Scene 2 - Network Fix)

1. Tương tự, tạo `PuzzlePanel_Network`
2. Tạo 4 buttons (Button 0, 1, 2, 3) với icons/labels khác nhau
3. Add **ButtonSequencePuzzle** component
4. Gán Puzzle Config → `Puzzle_NetworkFix.asset`
5. Gán 4 buttons vào array `sequenceButtons`
6. **SetActive(false)**

#### 11.3 ColorMatch Puzzle (Scene 3 - Final Code)

1. Tạo `PuzzlePanel_FinalCode`
2. Tạo color selection buttons (Red, Blue, Green, Yellow)
3. Tạo display slots (4 slots hiển thị màu đã chọn)
4. Add **ColorMatchPuzzle** component
5. Gán Puzzle Config → `Puzzle_FinalCode.asset`
6. **SetActive(false)**

---

## PHẦN G: SETUP CHARACTER

### Step 12: Tạo Character GameObject

1. Trong Level01_Scene1, tạo Empty → rename `Character`
2. Add child + SpriteRenderer → gán sprite "Coder Sad"
3. Đặt vị trí phù hợp (góc dưới trái thường gặp)

#### 12.1 Setup Animator (nếu có animation)

1. Select Character
2. Add Component → **Animator**
3. Tạo **Animator Controller**:
   - **Window → Animation → Animator**
   - Create controller: `Assets/Animation/CoderAnimator.controller`
4. Trong Animator window:
   - Create State: **Idle_Sad** (default)
   - Create State: **Happy**
   - Create Trigger parameter: **"Happy"**
   - Transition: Idle_Sad → Happy (Condition: Happy trigger)
5. Gán animation clips cho mỗi state

#### 12.2 Nếu CHƯA có animation (dùng sprite swap)

Nếu chưa có animation, bạn có thể skip Animator. LevelManager sẽ chỉ log warning.

**Cách thay thế đơn giản:**
1. KHÔNG add Animator
2. Thay vào đó, tạo script đơn giản swap sprite khi nhận event:

```
Khi LevelComplete event → Character SpriteRenderer.sprite = happySprite
```

Hoặc chỉ cần gán `characterObject` trong LevelManager và nó sẽ try GetComponent<Animator>(), nếu không có thì log warning nhưng level vẫn complete bình thường.

---

## PHẦN H: SETUP LEVEL 2 SCENES

### Step 13: Tạo cấu trúc Level 2

1. **Tách Level02.unity thành 4 scenes:**
   - `Level02_Scene1.unity` (Reception Desk)
   - `Level02_Scene2.unity` (Dev Room)
   - `Level02_Scene3.unity` (Meeting Room)
   - `Level02_Scene4.unity` (Storage)

2. **Duplicate Level01_Scene1** làm template:
   - Copy và rename
   - Xóa các hotspots/items cũ
   - Giữ lại: Camera, Canvas structure, SceneController, FadeOverlay

3. **Mỗi scene Level 2 cần:**
   - SceneController (prefab)
   - Canvas với FadeOverlay + InventoryPanel + BugCounterUI
   - Hotspots parent (trống, sẽ thêm sau)
   - MiniBugs parent (trống)
   - Background placeholder

4. **Update Build Settings** thêm scenes mới:
   ```
   5: Scenes/Levels/Level02_Scene1
   6: Scenes/Levels/Level02_Scene2
   7: Scenes/Levels/Level02_Scene3
   8: Scenes/Levels/Level02_Scene4
   ```

5. **Xóa Level02.unity cũ** sau khi tách xong

### Step 14: Navigation giữa scenes Level 2

Tạo navigation hotspots cho flow:
```
Scene1 ←→ Scene2 ←→ Scene3 ←→ Scene4
```

Mỗi scene cần hotspot Navigation trỏ sang scenes kề nhau.

---

## PHẦN I: WIRING & TESTING

### Step 15: Kết nối SceneController References

Trong mỗi scene Level 1 (Scene1, Scene2, Scene3):

1. Select **SceneController**
2. Inspector → gán **Fade Overlay** (Image + CanvasGroup)
3. Kiểm tra các settings:
   - Scene Name: (tên scene hiện tại)
   - Fade Duration: 0.5
   - Auto Find Game Manager: true

### Step 16: Kết nối HotspotManager

Trong mỗi scene Level 1:

1. Tạo Empty → rename `HotspotManager`
2. Add Component → **Hotspot Manager**
3. Config → nó sẽ auto-discover HotspotComponent con

**⚠️ Hoặc**: Nếu HotspotManager đã là singleton, chỉ cần 1 instance trong PersistentScene.

### Step 17: Kết nối PuzzleSystem

1. Tạo Empty → rename `PuzzleSystem` (trong mỗi scene có puzzle)
2. Add Component → **Puzzle System**
3. PuzzleSystem sẽ auto-discover puzzle scripts trong scene

### Step 18: Full Integration Test

**Chạy test từ PersistentScene:**

1. **File → Build Settings** → kiểm tra PersistentScene ở index 0
2. Mở PersistentScene
3. **Play** 

**Test Checklist:**

#### Scene Navigation
- [ ] Từ Scene 1, click navigation hotspot → chuyển sang Scene 2 (có fade)
- [ ] Từ Scene 2, quay lại Scene 1
- [ ] Từ Scene 2, sang Scene 3
- [ ] Inventory vẫn giữ items khi chuyển scene

#### Item Pickup
- [ ] Click flashlight → item vào inventory
- [ ] Click screwdriver → item vào inventory
- [ ] Hotspot biến mất sau khi nhặt (disableAfterUse)

#### Item Use
- [ ] Chọn screwdriver trong inventory
- [ ] Click Old PC → screwdriver được sử dụng
- [ ] Click Old PC khi chưa có screwdriver → hiện fail text

#### Puzzle Flow
- [ ] Click computer → Puzzle panel mở ra
- [ ] Nhập đúng mã "1337" → Puzzle solved, panel đóng
- [ ] Nhập sai → Feedback lỗi, giảm attempts

#### MiniBug Collection
- [ ] Click MiniBug → bug counter tăng (🐛 1/10)
- [ ] Animation pulse khi collect
- [ ] Counter sync đúng khi chuyển scene

#### Level Completion
- [ ] Solve tất cả 3 puzzles
- [ ] Character chuyển sang happy state (hoặc log message)
- [ ] "LEVEL 1 COMPLETE!" log xuất hiện
- [ ] Auto-transition sang Level 2 sau 3 giây

#### Save/Load
- [ ] Nhấn Save (qua GameManager) → dữ liệu lưu
- [ ] Restart game → load lại → chỉ ItemData tham chiếu không bị lỗi

---

## PHẦN J: COMMON ISSUES & FIXES

### Issue 1: LevelManager không đếm MiniBugs
**Nguyên nhân**: ItemData.isMiniBug = false
**Fix**: Kiểm tra TẤT CẢ MiniBug assets → isMiniBug phải = true

### Issue 2: Puzzle không trigger từ Hotspot
**Nguyên nhân**: PuzzleConfig chưa gán vào HotspotComponent
**Fix**: Select hotspot → Puzzle Config field → drag PuzzleConfig asset

### Issue 3: Scene transition không hoạt động
**Nguyên nhân**: Scene chưa add vào Build Settings
**Fix**: File → Build Settings → Add Open Scenes cho TẤT CẢ scenes

### Issue 4: BugCounterUI hiện 0/10 mãi
**Nguyên nhân**: LevelManager instance null (vì LevelManager chỉ ở Scene1)
**Fix**: LevelManager singleton persist qua scenes TRONG cùng level. Nếu bạn dùng LoadScene (không phải additive), LevelManager sẽ bị destroy. Đảm bảo chuyển scene dùng SceneController (additive loading).

### Issue 5: Required Puzzles không match
**Nguyên nhân**: puzzleID trong LevelManager ≠ puzzleID trong PuzzleConfig
**Fix**: Kiểm tra CHÍNH XÁC string match (case-sensitive!)

### Issue 6: Character happy state không trigger
**Nguyên nhân**: characterObject chưa gán trong Inspector
**Fix**: Drag Character GameObject vào LevelManager → Character Object field  

### Issue 7: Inventory mất khi chuyển scene
**Nguyên nhân**: InventorySystem ở scene bị unload
**Fix**: InventorySystem phải ở PersistentScene (DontDestroyOnLoad) hoặc dùng GameStateData để persist

---

## Summary Checklist

### Assets Created
- [ ] 3 Essential ItemData assets (flashlight, screwdriver, usb_keycard)
- [ ] 10 MiniBug ItemData assets (isMiniBug = true)
- [ ] 3 PuzzleConfig assets (ComputerLogin, NetworkFix, FinalCode)
- [ ] Code snippet / optional items (nếu cần)

### Scenes Setup
- [ ] Level01_Scene1 — với LevelManager + full hotspots
- [ ] Level01_Scene2 — hotspots + puzzles
- [ ] Level01_Scene3 — hotspots + final puzzle
- [ ] Level02_Scene1 through Scene4 — structure only (empty)
- [ ] Tất cả scenes trong Build Settings

### UI Setup
- [ ] BugCounterUI trong mỗi scene Level 1
- [ ] Puzzle panels trong scenes có puzzle
- [ ] InventoryPanel (nên ở PersistentScene hoặc copy mỗi scene)
- [ ] FadeOverlay hoạt động

### Components Wired
- [ ] LevelManager — required puzzles, totalMiniBugs, character ref
- [ ] HotspotManager — trong mỗi scene
- [ ] PuzzleSystem — trong scenes có puzzle
- [ ] SceneController — fade overlay gán
- [ ] All hotspots — đúng type, đúng references

### Integration Tests
- [ ] Full Level 1 flow pass
- [ ] MiniBug counter hoạt động
- [ ] Level completion trigger
- [ ] Scene transitions smooth
- [ ] Save/Load không crash

---

**Guide Status**: COMPLETE  
**Next Steps**: Setup Level 2 content (Day 6 - Puzzle content và Level 2 scenes)  
**Document**: 10-level1-full-integration.md
