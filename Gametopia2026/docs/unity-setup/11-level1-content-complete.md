# 11 — Level 1: "The Legacy System" — Build Guide Hoàn Chỉnh

> **Mục tiêu**: Level 1 chơi được 100% end-to-end: nhặt đồ → giải 2 puzzle → thu thập 10 bug → Coder happy.  
> **Thời gian build**: ~2–3 giờ (bao gồm import sprite + setup Unity).  
> **Sprite source**: `docs/asset-prompts-level1.md` — gen bằng Gemini, lưu đúng tên file.

---

## PHẦN 0: CHUẨN BỊ SPRITE — IMPORT VÀO UNITY

### Bước 0.1: Cấu Trúc Thư Mục Sprite

Tạo cấu trúc thư mục sau trong `Assets/`:

```
Assets/
└── Sprites/
    ├── Backgrounds/          ← BG-01, BG-02, BG-03
    ├── Characters/           ← CHAR-01, CHAR-02
    ├── Items/                ← ITEM-01, ITEM-02, ITEM-03
    ├── MiniBugs/             ← BUG-01 đến BUG-10
    ├── Objects/              ← OBJ-01 đến OBJ-12
    └── UI/                   ← UI-01 đến UI-09
```

Kéo thả từng file sprite vào đúng thư mục.

---

### Bước 0.2: Import Settings Cho Từng Loại Sprite

Chọn từng sprite trong Project window → Inspector → Texture Importer:

#### Backgrounds (BG-01, BG-02, BG-03)

| Setting | Giá trị |
|---|---|
| Texture Type | **Sprite (2D and UI)** |
| Sprite Mode | **Single** |
| Pixels Per Unit | **100** |
| Filter Mode | **Bilinear** |
| Compression | **Normal Quality** |
| Max Size | **2048** |
| Generate Mip Maps | ❌ tắt |

#### Characters (CHAR-01, CHAR-02)

| Setting | Giá trị |
|---|---|
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Single |
| Pixels Per Unit | **100** |
| Pivot | **Bottom** (để đặt character đứng lên sàn) |
| Filter Mode | Bilinear |

#### Items — Inventory (ITEM-01, 02, 03)

| Setting | Giá trị |
|---|---|
| Texture Type | Sprite (2D and UI) |
| Pixels Per Unit | **100** |
| Pivot | **Center** |
| Max Size | **512** |

#### MiniBugs (BUG-01 đến BUG-10)

| Setting | Giá trị |
|---|---|
| Texture Type | Sprite (2D and UI) |
| Pixels Per Unit | **100** |
| Pivot | **Center** |
| Max Size | **256** |

#### World Objects (OBJ-01 đến OBJ-12)

| Setting | Giá trị |
|---|---|
| Texture Type | Sprite (2D and UI) |
| Pixels Per Unit | **100** |
| Pivot | **Bottom** (trừ OBJ-11, OBJ-12: Center) |
| Max Size | **1024** |

#### UI Elements (UI-01 đến UI-09)

| Setting | Giá trị |
|---|---|
| Texture Type | **Sprite (2D and UI)** |
| Sprite Mode | Single |
| Pixels Per Unit | 100 |
| Mesh Type | **Full Rect** (bắt buộc cho UI elements) |
| Filter Mode | **Bilinear** |

> 💡 Nhấn **Apply** sau mỗi thay đổi setting.

---

### Bước 0.3: Bảng Mapping Sprite ↔ Dùng ở đâu

| Prompt ID | Filename | Dùng ở đâu |
|---|---|---|
| BG-01 | `bg_level1_scene1_server_hallway.png` | Scene 1 — Background |
| BG-02 | `bg_level1_scene2_tech_corner.png` | Scene 2 — Background |
| BG-03 | `bg_level1_scene3_central_control.png` | Scene 3 — Background |
| CHAR-01 | `char_coder_sad.png` | Scene 1 — CoderSprite (ban đầu) |
| CHAR-02 | `char_coder_happy.png` | Scene 1 — CoderSprite (khi level complete) |
| ITEM-01 | `item_flashlight.png` | Hotspot pickup + Inventory icon |
| ITEM-02 | `item_screwdriver.png` | Hotspot pickup + Inventory icon |
| ITEM-03 | `item_usb_keycard.png` | Hotspot pickup + Inventory icon |
| BUG-01→10 | `item_minibug_01.png` … `10.png` | MiniBug hotspots + Inventory icon |
| OBJ-01 | `obj_server_rack.png` | Scene 1 — Tủ server (tối) |
| OBJ-02 | `obj_server_rack_lit.png` | Scene 1 — Tủ server (sau soi đèn) |
| OBJ-03 | `obj_old_pc_closed.png` | Scene 2 — PC chưa mở |
| OBJ-04 | `obj_old_pc_open.png` | Scene 2 — PC đã mở |
| OBJ-05 | `obj_terminal.png` | Scene 2 — Terminal puzzle |
| OBJ-06 | `obj_card_reader.png` | Scene 3 — Ổ đọc thẻ (locked) |
| OBJ-07 | `obj_card_reader_accepted.png` | Scene 3 — Ổ đọc thẻ (accepted) |
| OBJ-08 | `obj_mainframe_locked.png` | Scene 3 — Mainframe (locked) |
| OBJ-09 | `obj_mainframe_unlocked.png` | Scene 3 — Mainframe (unlocked) |
| OBJ-10 | `obj_code_note.png` | Scene 2 — Tờ giấy mật mã 1337 |
| OBJ-11 | `obj_nav_arrow_right.png` | Tất cả scenes — mũi tên phải |
| OBJ-12 | `obj_nav_arrow_left.png` | Tất cả scenes — mũi tên trái |
| UI-01 | `ui_inventory_slot_empty.png` | InventorySlot — trống |
| UI-02 | `ui_inventory_slot_selected.png` | InventorySlot — selected |
| UI-03 | `ui_inventory_panel_bg.png` | InventoryPanel — background bar |
| UI-04 | `ui_bug_counter_icon.png` | BugCounterUI — icon con bọ |
| UI-05 | `ui_dialogue_panel_bg.png` | DialoguePopup — panel |
| UI-06 | `ui_btn_ok.png` | DialoguePopup — nút OK |
| UI-07 | `ui_puzzle_btn_normal.png` | Puzzle buttons — idle |
| UI-08 | `ui_puzzle_btn_pressed.png` | Puzzle buttons — pressed |
| UI-09 | `ui_puzzle_btn_correct.png` | Puzzle buttons — correct |

---

### Bước 0.4: Gán Sprite vào ItemData Assets

Sau khi import xong, mở từng asset trong `Assets/Resources/Items/` và drag sprite vào field **Icon**:

| Asset file | Icon sprite |
|---|---|
| `item_flashlight` | `Sprites/Items/item_flashlight.png` |
| `item_screwdriver` | `Sprites/Items/item_screwdriver.png` |
| `item_usb_keycard` | `Sprites/Items/item_usb_keycard.png` |
| `item_minibug_01` | `Sprites/MiniBugs/item_minibug_01.png` |
| `item_minibug_02` | `Sprites/MiniBugs/item_minibug_02.png` |
| `item_minibug_03` | `Sprites/MiniBugs/item_minibug_03.png` |
| `item_minibug_04` | `Sprites/MiniBugs/item_minibug_04.png` |
| `item_minibug_05` | `Sprites/MiniBugs/item_minibug_05.png` |
| `item_minibug_06` | `Sprites/MiniBugs/item_minibug_06.png` |
| `item_minibug_07` | `Sprites/MiniBugs/item_minibug_07.png` |
| `item_minibug_08` | `Sprites/MiniBugs/item_minibug_08.png` |
| `item_minibug_09` | `Sprites/MiniBugs/item_minibug_09.png` |
| `item_minibug_10` | `Sprites/MiniBugs/item_minibug_10.png` |

---

## PHẦN 1: TẠO SCRIPTABLEOBJECT ASSETS

### Bước 1.1: Tạo Items

Trong Project window: **chuột phải → Create → Coder Go Happy → Item Data**  
Lưu vào `Assets/Resources/Items/`

#### Item 1: Đèn Pin

| Field | Giá trị |
|---|---|
| File name | `item_flashlight` |
| itemID | `item_flashlight` |
| itemName | `Đèn Pin` |
| description | `Đèn pin sạc USB sáng rực. Dùng để soi những nơi thiếu ánh sáng.` |
| icon | drag `Sprites/Items/item_flashlight.png` |
| isMiniBug | ❌ false |
| isUsable | ✅ true |

#### Item 2: Tua Vít

| Field | Giá trị |
|---|---|
| File name | `item_screwdriver` |
| itemID | `item_screwdriver` |
| itemName | `Tua Vít` |
| description | `Tua vít Phillips đầu chữ thập. Có thể tháo vít trên thiết bị.` |
| icon | drag `Sprites/Items/item_screwdriver.png` |
| isMiniBug | ❌ false |
| isUsable | ✅ true |

#### Item 3: Thẻ USB Keycard

| Field | Giá trị |
|---|---|
| File name | `item_usb_keycard` |
| itemID | `item_usb_keycard` |
| itemName | `Thẻ USB ADMIN` |
| description | `Thẻ xác thực kỹ thuật số cấp ADMIN. Dùng để mở khóa hệ thống bảo mật.` |
| icon | drag `Sprites/Items/item_usb_keycard.png` |
| isMiniBug | ❌ false |
| isUsable | ✅ true |

#### Items 4–13: MiniBug 01–10

Tạo 10 file, tất cả: `description = "Một con bọ phần mềm đang ẩn trốn!"`, `isUsable = false`, `isMiniBug = true`:

| File name | itemID | itemName | icon |
|---|---|---|---|
| `item_minibug_01` | `item_minibug_01` | `Bug #01` | `Sprites/MiniBugs/item_minibug_01` |
| `item_minibug_02` | `item_minibug_02` | `Bug #02` | `Sprites/MiniBugs/item_minibug_02` |
| `item_minibug_03` | `item_minibug_03` | `Bug #03` | `Sprites/MiniBugs/item_minibug_03` |
| `item_minibug_04` | `item_minibug_04` | `Bug #04` | `Sprites/MiniBugs/item_minibug_04` |
| `item_minibug_05` | `item_minibug_05` | `Bug #05` | `Sprites/MiniBugs/item_minibug_05` |
| `item_minibug_06` | `item_minibug_06` | `Bug #06` | `Sprites/MiniBugs/item_minibug_06` |
| `item_minibug_07` | `item_minibug_07` | `Bug #07` | `Sprites/MiniBugs/item_minibug_07` |
| `item_minibug_08` | `item_minibug_08` | `Bug #08` | `Sprites/MiniBugs/item_minibug_08` |
| `item_minibug_09` | `item_minibug_09` | `Bug #09` | `Sprites/MiniBugs/item_minibug_09` |
| `item_minibug_10` | `item_minibug_10` | `Bug #10` | `Sprites/MiniBugs/item_minibug_10` |

---

### Bước 1.2: Tạo PuzzleConfig Assets

Lưu vào `Assets/Resources/Puzzles/`

#### Puzzle 1: Terminal (Button Sequence)

| Field | Giá trị |
|---|---|
| File name | `puzzle_terminal` |
| puzzleID | `puzzle_terminal` |
| puzzleName | `Terminal Cũ` |
| description | `Nhấn các nút theo đúng thứ tự được ghi trên tờ giấy...` |
| puzzleType | **ButtonSequence** |
| solution | `0,2,1` |
| maxAttempts | `0` (không giới hạn) |
| difficulty | `2` |

> ⚠️ `0,2,1` = Nút `&&` trước, rồi `!`, rồi `||` (zero-indexed)

#### Puzzle 2: Mainframe (Code Input)

| Field | Giá trị |
|---|---|
| File name | `puzzle_mainframe` |
| puzzleID | `puzzle_mainframe` |
| puzzleName | `Mainframe Trung Tâm` |
| description | `Nhập mã số bí mật để truy cập hệ thống.` |
| puzzleType | **CodeInput** |
| solution | `1337` |
| maxAttempts | `3` |
| difficulty | `2` |

---

## PHẦN 2: SETUP SCENE 1 — Hành Lang Server

Mở `Level01_Scene1.unity`.

### Bước 2.1: Tạo Sorting Layers (làm 1 lần, dùng cho cả 3 scene)

**Edit → Project Settings → Tags and Layers → Sorting Layers**, thêm theo thứ tự:

```
0: Default
1: Background
2: Objects
3: Characters
4: Effects
5: UI
```

### Bước 2.2: Hierarchy Scene 1

```
Level01_Scene1
├── Main Camera              (Tag: MainCamera | Orthographic Size: 5.4)
├── [SYSTEMS]
│   ├── GameManager          (GameManager.cs + SceneController.cs)
│   ├── EventManager         (EventManager.cs)
│   ├── InventorySystem      (InventorySystem.cs)
│   ├── HotspotManager       (HotspotManager.cs)
│   └── PuzzleSystem         (PuzzleSystem.cs)
├── [LEVEL]
│   └── LevelManager         (LevelManager.cs — CHỈ Scene 1 mới có)
├── [WORLD]
│   ├── Background
│   ├── ServerRack_Object        ← sprite tối (obj_server_rack)
│   ├── ServerRack_Lit           ← sprite sáng (obj_server_rack_lit) — SIBLING, SetActive FALSE
│   ├── Character
│   ├── Hotspots
│   │   ├── Hotspot_Flashlight
│   │   ├── Hotspot_Screwdriver
│   │   ├── Hotspot_ServerRack
│   │   └── Hotspot_ToScene2
│   └── MiniBugs
│       ├── MiniBug_01
│       ├── MiniBug_02
│       ├── MiniBug_03
│       └── MiniBug_04
└── [UI]
    └── Canvas (Screen Space – Camera | 1920×1080)
        ├── FadeOverlay
        ├── InventoryPanel
        ├── BugCounterUI
        └── DialoguePopup
```

---

### Bước 2.3: Background Scene 1

1. Tạo Empty **`Background`**
2. Add **SpriteRenderer**:

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Backgrounds/bg_level1_scene1_server_hallway` |
| Sorting Layer | **Background** |
| Order in Layer | **0** |
| Color | White |

3. Transform: Position `(0, 0, 0)`, Scale `(1, 1, 1)`

> 💡 Kiểm tra background vừa màn hình: Camera Orthographic Size = 5.4 → màn hình cao = 10.8 units. Sprite 1080px / 100 PPU = 10.8 units. Background sẽ vừa khít. ✅

---

### Bước 2.4: World Objects Scene 1

#### ServerRack_Object + ServerRack_Lit (tủ server — 2 trạng thái SIBLING)

> ⚠️ **QUAN TRỌNG**: `ServerRack_Lit` phải là **sibling** (cùng cấp) với `ServerRack_Object`, **KHÔNG phải child**. Nếu là child, khi deactivate parent thì child cũng bị ẩn theo, khiến cả 2 đều biến mất.

**Nếu bạn đã setup `ServerRack_Lit` là child**: Trong Hierarchy, **drag `ServerRack_Lit` ra ngoài** `ServerRack_Object` để nó đứng cùng cấp trong `[WORLD]`. Sau đó set lại Position và Scale như bảng dưới.

1. Tạo Empty **`ServerRack_Object`** → Add SpriteRenderer:

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_server_rack` |
| Sorting Layer | Objects |
| Order in Layer | **5** |
| Position | `(1.5, -1.2, 0)` — giữa phải, đứng trên sàn |
| Scale | `(0.6, 0.6, 1)` |

2. Tạo **SIBLING** (cùng cấp, KHÔNG trong ServerRack_Object) **`ServerRack_Lit`** → Add SpriteRenderer:

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_server_rack_lit` |
| Sorting Layer | Objects |
| Order in Layer | **6** |
| Position | `(1.5, -1.2, 0)` — **world position giống hệt** ServerRack_Object |
| Scale | `(0.6, 0.6, 1)` — **tự set scale, không inherit nữa** |
| **GameObject Active** | ❌ **FALSE** |

> ✅ Bây giờ deactivate `ServerRack_Object` sẽ không ảnh hưởng đến `ServerRack_Lit`.

---

#### Character (Coder Sad)

1. Tạo Empty **`Character`** → Add SpriteRenderer:

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Characters/char_coder_sad` |
| Sorting Layer | **Characters** |
| Order in Layer | **10** |
| Position | `(-3.5, -2.7, 0)` — góc trái phía dưới, ngồi gần sàn |
| Scale | `(0.6, 0.6, 1)` — nhân vật chiều cao ~3 units |

2. Gán vào **LevelManager → Character Object**: drag `Character` GameObject  
3. LevelManager thêm field `public Sprite happySprite` → drag `char_coder_happy`

Khi Level Complete, LevelManager thực thi:
```csharp
characterObject.GetComponent<SpriteRenderer>().sprite = happySprite;
```

---

### Bước 2.5: Setup Hotspots Scene 1

> 💡 **Quy tắc chung**: Mỗi hotspot = Empty GameObject + SpriteRenderer (nếu có sprite) + HotspotComponent + Collider2D

#### Hotspot_Flashlight (Pickup đèn pin)

1. Tạo Empty **`Hotspot_Flashlight`** → Add SpriteRenderer:

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Items/item_flashlight` |
| Sorting Layer | Objects |
| Order in Layer | **8** |
| Position | `(-3.8, -1.9, 0)` — góc trái, đặt trên bề mặt bàn/sàn của background |
| Scale | `(0.5, 0.5, 1)` |

2. Add **HotspotComponent**:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_flashlight` |
| Hotspot Type | **Pickup** |
| Is Active | ✅ true |
| Auto Calculate Bounds | ✅ true |
| Item To Pickup | drag asset `item_flashlight` |
| Disable After Pickup | ✅ true |
| Pulse On Hover | ✅ true |
| Highlight Scale | `1.2` |

3. Add **CircleCollider2D**: Radius `0.4`

---

#### Hotspot_Screwdriver (Pickup tua vít)

1. Tạo Empty **`Hotspot_Screwdriver`** → Add SpriteRenderer:

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Items/item_screwdriver` |
| Sorting Layer | Objects, Order **8** |
| Position | `(3.2, -2.1, 0)` — góc phải phía dưới |
| Scale | `(0.5, 0.5, 1)` |

2. Add HotspotComponent:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_screwdriver` |
| Hotspot Type | **Pickup** |
| Item To Pickup | drag `item_screwdriver` |
| Disable After Pickup | ✅ true |
| Pulse On Hover | ✅ true |
| Highlight Scale | `1.2` |

3. Add CircleCollider2D: Radius `0.4`

---

#### Hotspot_ServerRack (SmartHotspot — Examine khi chưa có đèn + ItemUse khi có đèn)

Đây là hotspot vô hình chồng lên tủ server, xử lý cả 2 trạng thái.

1. Tạo Empty **`Hotspot_ServerRack`** (không cần SpriteRenderer)
2. Transform: Position `(1.5, 0, 0)` — trùng vị trí tủ server
3. Add **BoxCollider2D**: Size `(2.0, 5.0)` — bao phủ toàn bộ tủ
4. Add **HotspotComponent**:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_server_rack` |
| Hotspot Type | **ItemUse** |
| Is Active | ✅ true |
| Required Item | drag `item_flashlight` |
| Consume Item On Use | ❌ **FALSE** (đèn pin không mất) |
| Success Event Name | `server_rack_revealed` |
| Examine Text | `Khu vực này khá tối. Có gì đó trong tủ nhưng không nhìn thấy rõ.` |

5. Add **SmartHotspot.cs**:

| Field | Giá trị |
|---|---|
| Required Item ID | `item_flashlight` |
| Text Without Item | `Tủ server bị tối hoàn toàn. Cần một nguồn sáng để nhìn bên trong.` |
| Text With Item | `Ánh đèn soi vào trong và bạn thấy một tờ giấy dán trong tủ:\n\n<color=#FF4444><b>THỨ TỰ: 1 → 3 → 2</b></color>\n\nGhi nhớ con số này!` |

6. Add **ActivateOnEvent**:

| Field | Giá trị |
|---|---|
| Event Name | `server_rack_revealed` |
| Objects To Activate | drag `ServerRack_Lit` (**sibling** GameObject, không phải child) |
| Objects To Deactivate | drag `ServerRack_Object` |
| Run Once | ✅ true |

---

#### Hotspot_ToScene2 (Navigation — mũi tên phải)

1. Tạo Empty **`Hotspot_ToScene2`** → Add SpriteRenderer:

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_nav_arrow_right` |
| Sorting Layer | **UI**, Order **20** |
| Position | `(5.2, 0, 0)` — sát cạnh phải màn hình, canh giữa dọc |
| Scale | `(1.0, 1.0, 1)` |

2. Add HotspotComponent:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_to_scene2` |
| Hotspot Type | **Navigation** |
| Target Scene Name | `Level01_Scene2` |

3. Add BoxCollider2D: Size `(0.9, 1.5)`

---

### Bước 2.6: MiniBugs Scene 1 (4 bugs)

**Quy trình cho mỗi bug**: Empty GameObject → SpriteRenderer → HotspotComponent → CircleCollider2D

| GameObject | Sprite | Position | Ghi chú vị trí |
|---|---|---|---|
| MiniBug_01 | `item_minibug_01` | `(-4.5, 0.8, 0)` | Trên dây cáp bên trái |
| MiniBug_02 | `item_minibug_02` | `(0.3, 2.2, 0)` | Trên nóc tủ server giữa |
| MiniBug_03 | `item_minibug_03` | `(3.8, -0.5, 0)` | Trên/quanh dây cáp bên phải |
| MiniBug_04 | `item_minibug_04` | `(-1.8, 2.6, 0)` | Gần trần / đèn LED phía trên |

SpriteRenderer Settings (giống nhau cho cả 4):

| Field | Giá trị |
|---|---|
| Sorting Layer | Objects |
| Order in Layer | **12** |
| Scale | `(0.7, 0.7, 1)` |

HotspotComponent Settings:

| Field | MiniBug_01 | MiniBug_02 | MiniBug_03 | MiniBug_04 |
|---|---|---|---|---|
| Hotspot ID | `minibug_01` | `minibug_02` | `minibug_03` | `minibug_04` |
| Hotspot Type | Pickup | Pickup | Pickup | Pickup |
| Item To Pickup | `item_minibug_01` | `item_minibug_02` | `item_minibug_03` | `item_minibug_04` |
| Disable After Pickup | ✅ | ✅ | ✅ | ✅ |
| Pulse On Hover | ✅ | ✅ | ✅ | ✅ |
| Highlight Scale | `1.4` | `1.4` | `1.4` | `1.4` |

CircleCollider2D: Radius `0.25` (nhỏ hơn item thường — thử thách người chơi)

---

### Bước 2.7: Setup Systems Scene 1

#### GameManager

- Add Component: **GameManager.cs**
- SceneController reference: drag `GameManager` (nếu SceneController trên cùng GameObject) hoặc drag object chứa SceneController
- InventorySystem: drag `InventorySystem`
- HotspotManager: drag `HotspotManager`
- PuzzleSystem: drag `PuzzleSystem`

#### SceneController (thêm vào GameManager hoặc object riêng)

- Add Component: **SceneController.cs**
- Fade Canvas Group: drag **CanvasGroup trên FadeOverlay**
- Default Fade Duration: `0.5`

#### LevelManager (CHỈ Scene 1)

| Field | Giá trị |
|---|---|
| Level Number | `1` |
| Required Puzzles | Size=2 → `puzzle_terminal`, `puzzle_mainframe` |
| Total MiniBugs In Level | `10` |
| Next Level Scene Name | `Level02_Scene1` |
| Character Object | drag `Character` GameObject |
| Happy Sprite | drag `char_coder_happy` |

#### HotspotManager

- Inventory System: drag `InventorySystem`
- Inventory UI: drag `InventoryUI` component trong Canvas

#### PuzzleSystem

- Auto Discover Puzzles: ✅ true

---

### Bước 2.8: Setup UI Scene 1

#### Canvas

| Field | Giá trị |
|---|---|
| Render Mode | **Screen Space - Camera** |
| Render Camera | drag **Main Camera** |
| Plane Distance | `1` |
| UI Scale Mode | **Scale With Screen Size** |
| Reference Resolution | `1920 × 1080` |
| Screen Match Mode | **Match Width Or Height**, Match = `0.5` |

#### FadeOverlay

1. **UI → Image** tên `FadeOverlay`
2. Rect Transform: Anchor Stretch-Stretch, tất cả = 0
3. Image Color: `(0, 0, 0, 0)` — trong suốt
4. Add **CanvasGroup**: Alpha=0, Interactable=false, BlocksRaycasts=false
5. Vị trí trong hierarchy: **cuối cùng** trong Canvas (render trên hết)

#### InventoryPanel

1. **UI → Panel** tên `InventoryPanel`
2. Rect Transform:

| Field | Giá trị |
|---|---|
| Anchor preset | Bottom Center |
| Pivot | `(0.5, 0)` |
| Pos Y | `10` |
| Width | `800` |
| Height | `128` |

3. **Image Component**: Source Image = `ui_inventory_panel_bg`, Color White
4. Add **InventoryUI.cs**

5. Tạo child **`SlotContainer`**:
   - Add **Horizontal Layout Group**
   - Child Alignment: Middle Center
   - Spacing: `8`
   - Padding: Left `20`, Right `20`, Top `8`, Bottom `8`
   - Child Force Expand Width: ❌, Height: ✅

6. Tạo **6 slot con** (Slot_0 → Slot_5) trong SlotContainer:

Mỗi slot:
- UI → Image, Width/Height `104 × 104`
- Image → Source Image: `ui_inventory_slot_empty`
- Add **InventorySlot.cs**:
  - Normal Sprite: drag `ui_inventory_slot_empty`
  - Selected Sprite: drag `ui_inventory_slot_selected`
- Tạo child `ItemIcon` (UI → Image, Width/Height `80 × 80`, Anchor Center, Alpha=0 khi trống)

7. InventoryUI.cs Inspector:
   - Slot Container: drag `SlotContainer`
   - Dragged Item Image: tạo thêm 1 Image `DraggedItemImage` trong Canvas (không trong SlotContainer), SetActive false

#### BugCounterUI

1. **UI → Empty** tên `BugCounterUI`, Anchor Top-Right
2. Rect Transform: Pos X=`-20`, Pos Y=`-20`, Width=`200`, Height=`60`

3. Child **`BugIcon`** (UI → Image):
   - Source Image: `ui_bug_counter_icon`
   - Width/Height: `48 × 48`, Anchor Left-Middle
   - Pos X: `24` (căn trái)

4. Child **`CounterText`** (UI → TextMeshPro — Text):
   - Text: `0/10`
   - Font Size: `32`, Bold
   - Color: White
   - Alignment: Left, Middle

5. Add **BugCounterUI.cs** vào `BugCounterUI`:
   - Bug Count Text: drag `CounterText`
   - Bug Icon Image: drag `BugIcon`
   - Text Format: `{0}/{1}`
   - Normal Color: White `(255,255,255)`
   - Complete Color: Gold `(255,215,0)`

#### DialoguePopup

1. **UI → Empty** tên `DialoguePopup`, Anchor Middle-Center
2. Rect Transform: Width=`720`, Height=`300`, Pos Y=`80`
3. Add **CanvasGroup**: Alpha=`0` (ẩn ban đầu)

4. Child **`PanelBG`** (UI → Image):
   - Source Image: `ui_dialogue_panel_bg`
   - Image Type: **Sliced** (9-slice nếu sprite hỗ trợ) hoặc Simple
   - Color: White
   - Anchor: Stretch toàn popup

5. Child **`MessageText`** (UI → TextMeshPro):
   - Rect Transform: Margin Left/Right `40`, Top `30`, Bottom `75`
   - Font Size: `28`, Alignment: Center+Middle
   - Wrapping: ✅ enabled
   - Overflow: Truncate

6. Child **`CloseButton`** (UI → Button - TextMeshPro):
   - Source Image: `ui_btn_ok`
   - Rect Transform: Anchor Bottom-Center, Width=`160`, Height=`50`, Pos Y=`15`
   - Button label: TMP "OK", Font Size `24`, White, Bold

7. **DialoguePopup.cs** Inspector:
   - Canvas Group: drag CanvasGroup trên root `DialoguePopup`
   - Message Text: drag `MessageText`
   - Close Button: drag `CloseButton`
   - Fade In Duration: `0.25`
   - Auto Close Delay: `0`
   - Block Interaction While Open: ✅ true

---

## PHẦN 3: SETUP SCENE 2 — Góc Kỹ Thuật

Mở `Level01_Scene2.unity`.

### Bước 3.1: Hierarchy Scene 2

```
Level01_Scene2
├── Main Camera               (Orthographic Size 5.4)
├── [SYSTEMS]                 ⚠️ KHÔNG có GameManager / LevelManager
│   ├── HotspotManager
│   └── PuzzleSystem
├── [WORLD]
│   ├── Background            (bg_level1_scene2_tech_corner)
│   ├── Objects
│   │   ├── OldPC_Closed      (obj_old_pc_closed)
│   │   ├── OldPC_Open        (obj_old_pc_open — SetActive FALSE)
│   │   ├── Terminal_Object   (obj_terminal)
│   │   └── CodeNote_Object   (obj_code_note)
│   ├── Hotspots
│   │   ├── Hotspot_CodeNote
│   │   ├── Hotspot_OldPC
│   │   ├── Hotspot_USBKeycard  (SetActive FALSE)
│   │   ├── Hotspot_Terminal
│   │   ├── Hotspot_ToScene1
│   │   └── Hotspot_ToScene3
│   └── MiniBugs
│       ├── MiniBug_05
│       ├── MiniBug_06
│       └── MiniBug_07
└── [UI]
    └── Canvas
        ├── FadeOverlay
        ├── InventoryPanel        (copy y chang từ Scene 1)
        ├── BugCounterUI          (copy từ Scene 1)
        ├── DialoguePopup         (copy từ Scene 1)
        └── PuzzlePanel_Terminal  (SetActive FALSE)
```

> 💡 Copy Canvas từ Scene 1: Trong Hierarchy, chọn Canvas → Ctrl+C → mở Scene 2 → Ctrl+V. Sau đó xóa PuzzlePanel_Mainframe (sẽ tạo ở Scene 3).

---

### Bước 3.2: World Objects Scene 2

#### Background

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Backgrounds/bg_level1_scene2_tech_corner` |
| Sorting Layer | Background, Order 0 |
| Position | `(0, 0, 0)` |

#### OldPC_Closed / OldPC_Open (2 trạng thái chồng nhau)

**OldPC_Closed:**

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_old_pc_closed` |
| Sorting Layer | Objects, Order **5** |
| Position | `(-2.5, -1.5, 0)` — trên bàn, góc trái |
| Scale | `(0.65, 0.65, 1)` |

**OldPC_Open** (cùng position, SetActive FALSE):

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_old_pc_open` |
| Sorting Layer | Objects, Order **5** |
| Position | `(-2.5, -1.5, 0)` |
| Scale | `(0.65, 0.65, 1)` |
| **Active** | ❌ FALSE |

#### Terminal_Object

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_terminal` |
| Sorting Layer | Objects, Order **5** |
| Position | `(2.0, -0.8, 0)` — trên bàn bên phải |
| Scale | `(0.7, 0.7, 1)` |

#### CodeNote_Object

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_code_note` |
| Sorting Layer | Objects, Order **6** |
| Position | `(0.5, 1.8, 0)` — gắn trên corkboard trong background |
| Scale | `(0.5, 0.5, 1)` |

---

### Bước 3.3: Setup Hotspots Scene 2

#### Hotspot_CodeNote

Tạo Empty chồng lên `CodeNote_Object`, BoxCollider2D `(1.0, 1.3)`:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_code_note` |
| Hotspot Type | **Examine** |
| Position | `(0.5, 1.8, 0)` |
| Examine Text | `Tờ giấy có viết bằng bút marker đỏ:\n\n<size=42><b><color=#FF4444>MÃ SỐ: 1337</color></b></size>\n\nNhớ kỹ con số này!` |

---

#### Hotspot_OldPC (ItemUse — Tua Vít)

Tạo Empty chồng lên `OldPC_Closed`, BoxCollider2D `(1.8, 3.3)`:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_old_pc` |
| Hotspot Type | **ItemUse** |
| Position | `(-2.5, -1.5, 0)` |
| Required Item | drag `item_screwdriver` |
| Consume Item On Use | ✅ TRUE |
| Success Event Name | `old_pc_opened` |
| Examine Text | `Vỏ máy tính có 4 vít Phillips. Cần tua vít để mở.` |

Add **ActivateOnEvent**:

| Field | Giá trị |
|---|---|
| Event Name | `old_pc_opened` |
| Objects To Activate | `[OldPC_Open, Hotspot_USBKeycard]` |
| Objects To Deactivate | `[OldPC_Closed, Hotspot_OldPC]` *(self deactivate)* |
| Run Once | ✅ true |

---

#### Hotspot_USBKeycard (Pickup — ban đầu ẩn)

1. Tạo Empty **`Hotspot_USBKeycard`**, **SetActive FALSE**
2. Add SpriteRenderer: `Sprites/Items/item_usb_keycard`, Order **10**
3. Position: `(-2.5, -2.0, 0)` — xuất hiện trong khe PC
4. Scale: `(0.4, 0.4, 1)`

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_usb_keycard` |
| Hotspot Type | **Pickup** |
| Item To Pickup | drag `item_usb_keycard` |
| Disable After Pickup | ✅ true |
| Pulse On Hover | ✅ true |
| Highlight Scale | `1.3` |

CircleCollider2D: Radius `0.3`

---

#### Hotspot_Terminal (Puzzle)

Tạo Empty chồng lên `Terminal_Object`, BoxCollider2D `(2.8, 3.5)`:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_terminal` |
| Hotspot Type | **Puzzle** |
| Position | `(2.0, -0.8, 0)` |
| Puzzle ID | `puzzle_terminal` |
| Examine Text | `Một terminal cũ. Có 3 nút lớn. Thứ tự đúng sẽ mở khóa...` |

---

#### Nav Arrows Scene 2

**Hotspot_ToScene1** (về Scene 1):

- Sprite: `obj_nav_arrow_left`, Position `(-5.2, 0, 0)`, Order 20
- HotspotComponent: Type Navigation, Target `Level01_Scene1`

**Hotspot_ToScene3** (sang Scene 3):

- Sprite: `obj_nav_arrow_right`, Position `(5.2, 0, 0)`, Order 20
- HotspotComponent: Type Navigation, Target `Level01_Scene3`

---

### Bước 3.4: MiniBugs Scene 2 (3 bugs)

| GameObject | Sprite | Position | Vị trí trong background |
|---|---|---|---|
| MiniBug_05 | `item_minibug_05` | `(-2.0, -0.5, 0)` | Trong cốc cà phê trên bàn |
| MiniBug_06 | `item_minibug_06` | `(0.3, -2.0, 0)` | Dưới bàn phím |
| MiniBug_07 | `item_minibug_07` | `(2.0, 1.5, 0)` | Trên nóc terminal |

SpriteRenderer: Order **12**, Scale `(0.6, 0.6, 1)`  
CircleCollider2D: Radius `0.25`

---

### Bước 3.5: PuzzlePanel_Terminal (UI)

> **3 cách đóng panel puzzle** (đã được code thêm vào `PuzzleBase.cs`):
> - **Phím Escape** — thoát ngay lập tức
> - **Nút Close (X)** — nút rõ ràng trong panel
> - **Click ngoài panel** — click vào vùng tối bên ngoài (Overlay)
>
> Trong Editor chỉ cần: thêm **Button** vào Overlay + gán 2 field trong component.

Tạo trong Canvas, **SetActive FALSE** ban đầu:

```
PuzzlePanel_Terminal            (Empty | SetActive FALSE | CanvasGroup alpha=0)
├── Overlay                     (Image | Color 0,0,0,180 | Anchor Stretch | + Button component)
└── PanelContent                (Image | ui_dialogue_panel_bg | 600×400 | Center)
    ├── TitleText               (TMP "TERMINAL CŨ" | Size 32 | Bold | White)
    ├── DescText                (TMP "Nhấn đúng thứ tự..." | Size 20 | White)
    ├── ButtonsContainer        (Empty | HorizontalLayoutGroup | spacing 20)
    │   ├── PuzzleBtn_0         (Image ui_puzzle_btn_normal | 110×110)
    │   │   └── BtnLabel_0      (TMP "&&" | Size 28 | Bold | White | Center)
    │   ├── PuzzleBtn_1         (Image ui_puzzle_btn_normal | 110×110)
    │   │   └── BtnLabel_1      (TMP "||" | Size 28 | Bold | White)
    │   └── PuzzleBtn_2         (Image ui_puzzle_btn_normal | 110×110)
    │       └── BtnLabel_2      (TMP "!" | Size 28 | Bold | White)
    ├── FeedbackText            (TMP "" | Size 22 | Color green)
    └── CloseButton             (Image ui_btn_ok | 140×50 | TMP "ĐÓNG")
```

**Setup Overlay để click ngoài = đóng panel:**
1. Chọn node **Overlay** → Add Component → **Button**
2. Transition: **None** (không cần hiệu ứng hover)
3. Interactable: ✅ (để nhận click)

> ⚠️ Quan trọng: Overlay phải nằm **trước** PanelContent trong Hierarchy (thấp hơn = render sau → nhưng về raycast thì Unity catch theo thứ tự từ trên xuống, Overlay ở trên → nếu click vào PanelContent thì Panel chặn raycast, nếu click ra ngoài Panel thì Overlay nhận)

**Puzzle Button Sprite Swap** — `ButtonSequencePuzzle.cs` cần 3 sprite references:
- Normal Button Sprite → drag `ui_puzzle_btn_normal`
- Pressed Button Sprite → drag `ui_puzzle_btn_pressed`
- Correct Button Sprite → drag `ui_puzzle_btn_correct`

Gán **ButtonSequencePuzzle.cs** vào `PuzzlePanel_Terminal`:

| Field | Giá trị |
|---|---|
| Puzzle Config | drag `puzzle_terminal` |
| Puzzle Buttons | Size=3: drag `PuzzleBtn_0`, `PuzzleBtn_1`, `PuzzleBtn_2` |
| Normal Sprite | drag `ui_puzzle_btn_normal` |
| Pressed Sprite | drag `ui_puzzle_btn_pressed` |
| Correct Sprite | drag `ui_puzzle_btn_correct` |
| **Close Button** | drag `CloseButton` |
| **Background Overlay** | drag `Overlay` |
| Allow Escape To Close | ✅ true |

---

## PHẦN 4: SETUP SCENE 3 — Tủ Điện Trung Tâm

Mở `Level01_Scene3.unity`.

### Bước 4.1: Hierarchy Scene 3

```
Level01_Scene3
├── Main Camera
├── [SYSTEMS]
│   ├── HotspotManager
│   └── PuzzleSystem
├── [WORLD]
│   ├── Background             (bg_level1_scene3_central_control)
│   ├── Objects
│   │   ├── CardReader_Locked  (obj_card_reader)
│   │   ├── CardReader_OK      (obj_card_reader_accepted — SetActive FALSE)
│   │   ├── Mainframe_Locked   (obj_mainframe_locked)
│   │   └── Mainframe_Unlocked (obj_mainframe_unlocked — SetActive FALSE)
│   ├── Hotspots
│   │   ├── Hotspot_CardReader
│   │   ├── Hotspot_Mainframe  (SetActive FALSE)
│   │   └── Hotspot_ToScene2
│   └── MiniBugs
│       ├── MiniBug_08
│       ├── MiniBug_09
│       └── MiniBug_10
└── [UI]
    └── Canvas
        ├── FadeOverlay
        ├── InventoryPanel
        ├── BugCounterUI
        ├── DialoguePopup
        └── PuzzlePanel_Mainframe (SetActive FALSE)
```

---

### Bước 4.2: World Objects Scene 3

#### Background

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Backgrounds/bg_level1_scene3_central_control` |
| Sorting Layer | Background, Order 0 |
| Position | `(0, 0, 0)` |

#### CardReader Objects (2 trạng thái)

**CardReader_Locked:**

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_card_reader` |
| Position | `(-3.5, 0.3, 0)` — trên tường trái |
| Scale | `(0.7, 0.7, 1)` — card reader cao ~2.7 units |
| Sorting Layer | Objects, Order **5** |

**CardReader_OK** (SetActive FALSE):

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_card_reader_accepted` |
| Position | `(-3.5, 0.3, 0)` — cùng vị trí |
| Scale | `(0.7, 0.7, 1)` |
| **Active** | ❌ FALSE |

#### Mainframe Objects (2 trạng thái)

**Mainframe_Locked:**

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_mainframe_locked` |
| Position | `(2.0, 0.2, 0)` — giữa phải màn hình |
| Scale | `(0.75, 0.75, 1)` — mainframe rộng ~4.8 units |
| Sorting Layer | Objects, Order **5** |

**Mainframe_Unlocked** (SetActive FALSE):

| Field | Giá trị |
|---|---|
| Sprite | `Sprites/Objects/obj_mainframe_unlocked` |
| Position | `(2.0, 0.2, 0)` |
| Scale | `(0.75, 0.75, 1)` |
| **Active** | ❌ FALSE |

---

### Bước 4.3: Setup Hotspot_CardReader

Tạo Empty chồng lên CardReader_Locked, BoxCollider2D `(1.5, 2.5)`:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_card_reader` |
| Hotspot Type | **ItemUse** |
| Position | `(-3.5, 0.3, 0)` |
| Required Item | drag `item_usb_keycard` |
| Consume Item On Use | ✅ TRUE |
| Success Event Name | `card_accepted` |
| Examine Text | `Ổ đọc thẻ USB. Đèn đỏ đang nhấp nháy. Cần một thẻ xác thực đúng loại.` |

Add **ActivateOnEvent**:

| Field | Giá trị |
|---|---|
| Event Name | `card_accepted` |
| Objects To Activate | `[CardReader_OK, Hotspot_Mainframe]` |
| Objects To Deactivate | `[CardReader_Locked, Hotspot_CardReader]` |
| Run Once | ✅ true |

---

### Bước 4.4: Setup Hotspot_Mainframe (ban đầu ẩn)

**SetActive FALSE**. Sẽ được ActivateOnEvent bật lên sau khi quẹt thẻ.

Tạo Empty chồng lên Mainframe_Locked, BoxCollider2D `(4.5, 3.5)`:

| Field | Giá trị |
|---|---|
| Hotspot ID | `hotspot_mainframe` |
| Hotspot Type | **Puzzle** |
| Puzzle ID | `puzzle_mainframe` |
| Examine Text | `Màn hình hiện "ACCESS DENIED". Nhập đúng mã số để truy cập.` |

Add **ActivateOnEvent** (swap sprite khi giải puzzle):

| Field | Giá trị |
|---|---|
| Event Name | `puzzle_solved_puzzle_mainframe` |
| Objects To Activate | `[Mainframe_Unlocked]` |
| Objects To Deactivate | `[Mainframe_Locked]` |
| Run Once | ✅ true |

---

### Bước 4.5: PuzzlePanel_Mainframe (CodeInput)

```
PuzzlePanel_Mainframe           (SetActive FALSE | CanvasGroup)
├── Overlay                     (Image | Color 0,0,0,180 | Stretch | + Button component)
└── PanelContent                (Image | ui_dialogue_panel_bg | 560×420 | Center)
    ├── TitleText               (TMP "MAINFRAME TRUNG TÂM" | Size 28 | Bold | White)
    ├── DescText                (TMP "Nhập mã truy cập 4 chữ số:" | Size 20)
    ├── InputDisplay            (Image | Color dark | 280×70 | Center)
    │   └── InputText           (TMP "————" | Size 36 | Center | Monospace)
    ├── NumpadContainer         (Grid Layout Group | 3 col | Cell 80×80 | Spacing 8)
    │   ├── NumBtn_1 … NumBtn_9 (Image ui_puzzle_btn_normal | TMP "1"–"9")
    │   ├── NumBtn_0            (Image ui_puzzle_btn_normal | TMP "0")
    │   └── DeleteBtn           (Image ui_puzzle_btn_normal | TMP "⌫")
    ├── AttemptsText            (TMP "Còn 3 lần thử" | Size 18 | Yellow)
    ├── ConfirmButton           (Image ui_btn_ok | 180×55 | TMP "XÁC NHẬN")
    └── CloseButton             (Image ui_btn_cancel | 100×40 | TMP "✕" | Top-right góc panel)
```

**Setup Overlay để click ngoài = đóng panel:**
1. Chọn node **Overlay** → Add Component → **Button**
2. Transition: **None**
3. Interactable: ✅

**Setup CloseButton (nút X):**
- Anchor: **Top Right** của `PanelContent`
- Position offset: `(240, -20)` so với center của PanelContent
- Size: `50×50` (dạng icon tròn hoặc text "✕")
- Không cần gán `onClick` trong Inspector — `PuzzleBase.cs` tự wire khi ShowPuzzle() chạy

Gán **CodeInputPuzzle.cs**:

| Field | Giá trị |
|---|---|
| Puzzle Config | drag `puzzle_mainframe` |
| Input Text Display | drag `InputText` |
| Attempts Remaining Text | drag `AttemptsText` |
| Number Buttons | drag NumBtn_0 đến NumBtn_9 (10 buttons) |
| Delete Button | drag `DeleteBtn` |
| Confirm Button | drag `ConfirmButton` |
| **Close Button** | drag `CloseButton` |
| **Background Overlay** | drag `Overlay` |
| Allow Escape To Close | ✅ true |

---

### Bước 4.6: Nav Arrow và MiniBugs Scene 3

**Hotspot_ToScene2** (về Scene 2):
- Sprite: `obj_nav_arrow_left`, Position `(-5.2, 0, 0)`
- Type: Navigation, Target `Level01_Scene2`

**MiniBugs Scene 3** (3 bug — khó tìm nhất):

| GameObject | Sprite | Position | Ghi chú |
|---|---|---|---|
| MiniBug_08 | `item_minibug_08` | `(-4.0, 2.0, 0)` | Sau lề màn hình mainframe |
| MiniBug_09 | `item_minibug_09` | `(4.5, -2.3, 0)` | Góc tường phải phía dưới |
| MiniBug_10 | `item_minibug_10` | `(0.8, 2.8, 0)` | Gần trần phía trên mainframe |

Scale: `(0.55, 0.55, 1)` — nhỏ nhất trong cả 3 cảnh

---

## PHẦN 5: BUILD SETTINGS

**File → Build Settings → Add Open Scenes**, sắp xếp:

| Index | Scene Path |
|---|---|
| 0 | `Scenes/Persistent/PersistentScenes` *(nếu có)* |
| 1 | `Scenes/Levels/Level01_Scene1` |
| 2 | `Scenes/Levels/Level01_Scene2` |
| 3 | `Scenes/Levels/Level01_Scene3` |
| 4 | `Scenes/Levels/Level02` |

---

## PHẦN 6: TEST CHECKLIST

### Pre-flight Checks
- [ ] Không có compile error trong Console
- [ ] Tất cả Sorting Layers đã tạo
- [ ] Tất cả sprites đã import đúng (không có icon !)
- [ ] Tất cả ItemData assets đã được gán icon sprite

### Scene 1
- [ ] Background `bg_level1_scene1_server_hallway` hiện đúng, vừa màn hình
- [ ] Character `char_coder_sad` hiện ở góc trái dưới
- [ ] `ServerRack_Object` (tối) hiện, `ServerRack_Lit` ẩn
- [ ] BugCounter hiện `0/10` góc trên phải với icon con bọ
- [ ] Inventory bar hiện 6 ô trống với sprite `ui_inventory_slot_empty`
- [ ] Click đèn pin → icon `item_flashlight` hiện trong slot, ô dùng sprite `ui_inventory_slot_selected` khi chọn
- [ ] Click tua vít → icon vào slot 1
- [ ] Click tủ server (không chọn đèn) → DialoguePopup mở, hiện text "Quá tối..."
- [ ] Chọn đèn pin → click tủ server → Dialogue hiện "THỨ TỰ: 1→3→2", `ServerRack_Lit` xuất hiện thay `ServerRack_Object`
- [ ] Click mũi tên phải → fade → chuyển sang Scene 2

### Scene 2
- [ ] Background `bg_level1_scene2_tech_corner` hiện đúng
- [ ] Inventory vẫn có đèn pin + tua vít sau khi chuyển scene
- [ ] `OldPC_Closed` hiện, `OldPC_Open` ẩn
- [ ] `CodeNote_Object` hiện trên corkboard
- [ ] Click tờ giấy → Dialogue "MÃ SỐ: 1337" với text màu đỏ
- [ ] Click PC (không chọn tua vít) → Dialogue hint "Cần tua vít"
- [ ] Chọn tua vít → click PC → `OldPC_Open` hiện, `OldPC_Closed` ẩn, `Hotspot_USBKeycard` active
- [ ] Icon USB keycard xuất hiện trong khe PC
- [ ] Click USB → vào inventory, tua vít biến mất
- [ ] Click terminal → PuzzlePanel_Terminal mở
- [ ] Nút `&&`,`||`,`!` hiện với sprite `ui_puzzle_btn_normal`
- [ ] Nhấn nút → sprite đổi sang `ui_puzzle_btn_pressed`
- [ ] Nhấn đúng `&&` → `!` → `||` → sprite đổi `ui_puzzle_btn_correct`, "Puzzle Solved!"
- [ ] MiniBug_05 (trong cốc), 06 (dưới bàn phím), 07 (trên terminal) có thể nhặt

### Scene 3
- [ ] Background `bg_level1_scene3_central_control` hiện đúng
- [ ] `CardReader_Locked` (đèn đỏ) hiện, `CardReader_OK` ẩn
- [ ] `Mainframe_Locked` ("ACCESS DENIED") hiện, `Mainframe_Unlocked` ẩn
- [ ] `Hotspot_Mainframe` đang ẩn (không click được)
- [ ] Click card reader (không có USB) → Dialogue hint
- [ ] Chọn USB → click card reader → `CardReader_OK` hiện, `Hotspot_Mainframe` active
- [ ] Click mainframe → PuzzlePanel_Mainframe mở với numpad
- [ ] Nhập `1337` → `ConfirmButton` → "ACCESS GRANTED"
- [ ] `Mainframe_Unlocked` hiện, `Mainframe_Locked` ẩn
- [ ] Nhặt 3 bug → BugCounter `10/10` đổi màu vàng
- [ ] LevelManager phát hiện 2 puzzles solved → Level Complete!
- [ ] Character sprite đổi sang `char_coder_happy` 🎉

---

## PHẦN 7: SCRIPTS CHECKLIST

| Script | Path | Status |
|---|---|---|
| `GameManager.cs` | Scripts/Core/ | ✅ |
| `EventManager.cs` | Scripts/Core/ | ✅ |
| `SceneController.cs` | Scripts/Scene/ | ✅ |
| `HotspotManager.cs` | Scripts/Interaction/ | ✅ |
| `HotspotComponent.cs` | Scripts/Interaction/ | ✅ (đã fix) |
| `SmartHotspot.cs` | Scripts/Interaction/ | ✅ (đã tạo) |
| `ActivateOnEvent.cs` | Scripts/Utilities/ | ✅ (đã tạo) |
| `InventorySystem.cs` | Scripts/Inventory/ | ✅ |
| `InventoryUI.cs` | Scripts/Inventory/ | ✅ |
| `InventorySlot.cs` | Scripts/Inventory/ | ✅ |
| `PuzzleSystem.cs` | Scripts/Puzzle/ | ✅ |
| `ButtonSequencePuzzle.cs` | Scripts/Puzzle/ | ✅ |
| `CodeInputPuzzle.cs` | Scripts/Puzzle/ | ✅ |
| `LevelManager.cs` | Scripts/Level/ | ✅ |
| `BugCounterUI.cs` | Scripts/UI/ | ✅ |
| `DialoguePopup.cs` | Scripts/UI/ | ✅ (đã tạo) |

**Tất cả scripts đã sẵn sàng. Không cần tạo thêm.**
