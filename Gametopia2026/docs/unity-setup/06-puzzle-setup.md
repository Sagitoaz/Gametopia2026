# 06 - Puzzle System Setup

## Overview
Hướng dẫn setup **PuzzleSystem** và 3 loại puzzle UI: **ButtonSequence**, **CodeInput**, **ColorMatch**. Mỗi puzzle type có UI riêng với interaction logic khác nhau.

**Prerequisites:**
- Đã hoàn thành [05-hotspot-setup.md](05-hotspot-setup.md)
- Canvas đã có trong Level01 scene

---

## Architecture Overview

```
Canvas
├── PuzzleSystem (GameObject)
│   └── PuzzleSystem (script) - Manager
├── ButtonSequencePuzzle (Panel)
│   ├── ButtonSequencePuzzle (script)
│   ├── PuzzleUI (background, buttons)
│   └── Button[] (4-6 buttons cho sequence)
├── CodeInputPuzzle (Panel)
│   ├── CodeInputPuzzle (script)
│   ├── InputField (nhập code)
│   └── Submit/Clear buttons
└── ColorMatchPuzzle (Panel)
    ├── ColorMatchPuzzle (script)
    ├── Color buttons (Red, Blue, Green, Yellow...)
    └── Sequence slots (hiển thị selected colors)
```

**3 Puzzle Types:**
1. **ButtonSequence**: Click buttons theo đúng thứ tự
2. **CodeInput**: Nhập mã số (numeric password)
3. **ColorMatch**: Chọn colors theo đúng sequence

---

## Step 1: Create PuzzleSystem GameObject

### 1.1. Create GameObject

1. Right-click **Canvas** → **Create Empty**
2. Rename: `PuzzleSystem`
3. Position: (0, 0, 0)

### 1.2. Add PuzzleSystem Script

1. Select **PuzzleSystem**
2. **Add Component → Puzzle System**
3. Component added ✅

### 1.3. Configure PuzzleSystem

**Inspector fields:**

| Field | Value | Mô tả |
|-------|-------|-------|
| **autoDiscoverPuzzles** | Tích ✅ | Auto-find all PuzzleBase components |
| **puzzles** | (empty array) | Sẽ auto-populate khi Play |

**Auto-discovery:** PuzzleSystem sẽ tự động find tất cả ButtonSequencePuzzle, CodeInputPuzzle, ColorMatchPuzzle trong scene.

---

## Step 2: Setup ButtonSequence Puzzle UI

### 2.1. Create Puzzle Panel

1. Right-click **Canvas** → **UI → Panel**
2. Rename: `ButtonSequencePuzzle`
3. **RectTransform:**
   - **Anchor**: Center
   - **Width**: 600
   - **Height**: 400
   - **Position**: (0, 0, 0) - center screen

### 2.2. Configure Background

**Image component:**
- **Color**: RGBA: 20, 20, 40, 240 (dark blue semi-transparent)
- **Raycast Target**: Tích ✅ (block clicks behind)

### 2.3. Add ButtonSequencePuzzle Script

1. Select **ButtonSequencePuzzle** panel
2. **Add Component → Button Sequence Puzzle**

### 2.4. Create Buttons Container

1. Right-click **ButtonSequencePuzzle** → **Create Empty**
2. Rename: `ButtonsContainer`
3. **Add Component → Grid Layout Group**

**Grid Layout Group settings:**
- **Cell Size**: 100 x 100
- **Spacing**: 20 x 20
- **Constraint**: Fixed Column Count = 4
- **Child Alignment**: Middle Center

### 2.5. Create Puzzle Buttons (4 buttons)

1. Right-click **ButtonsContainer** → **UI → Button**
2. Rename: `Button_0`
3. Duplicate 3 lần → `Button_1`, `Button_2`, `Button_3`

**Configure each button:**
- **Width x Height**: 100 x 100
- **Image Color**: Different colors (e.g., Red, Blue, Green, Yellow)
- **Button Text**: Number hoặc symbol (e.g., "1", "2", "3", "4")

### 2.6. Create Progress Text (Optional)

1. Right-click **ButtonSequencePuzzle** → **UI → Text**
2. Rename: `ProgressText`
3. Configure:
   - **Text**: "0/4" (sẽ update runtime)
   - **Font Size**: 24
   - **Alignment**: Center
   - **Position**: Top of panel (e.g., Y = 150)

### 2.7. Create Clear Button (Optional)

1. Right-click **ButtonSequencePuzzle** → **UI → Button**
2. Rename: `ClearButton`
3. Configure:
   - **Text**: "Clear"
   - **Position**: Bottom-left (e.g., (-200, -150))

### 2.8. Assign References to Script

Select **ButtonSequencePuzzle** panel, trong **ButtonSequencePuzzle component**:

| Field | Assign To |
|-------|-----------|
| **config** | (Sẽ assign PuzzleConfig asset sau - [07-scriptableobject-creation.md](07-scriptableobject-creation.md)) |
| **puzzleUI** | ButtonSequencePuzzle panel itself |
| **puzzleButtons** | Drag Button_0, Button_1, Button_2, Button_3 array (size = 4) |
| **sequenceProgressText** | ProgressText |
| **clearButton** | ClearButton |
| **normalColor** | White (255, 255, 255) |
| **clickedColor** | Green (0, 255, 0) |
| **errorColor** | Red (255, 0, 0) |

### 2.9. Hide Panel by Default

1. Select **ButtonSequencePuzzle** panel
2. **Uncheck** GameObject enable checkbox (hide lúc start)

---

## Step 3: Setup CodeInput Puzzle UI

### 3.1. Create Puzzle Panel

1. Right-click **Canvas** → **UI → Panel**
2. Rename: `CodeInputPuzzle`
3. **RectTransform:**
   - **Anchor**: Center
   - **Size**: 500 x 350

### 3.2. Add CodeInputPuzzle Script

1. Select **CodeInputPuzzle**
2. **Add Component → Code Input Puzzle**

### 3.3. Create InputField

1. Right-click **CodeInputPuzzle** → **UI → Input Field**
2. Rename: `CodeInputField`
3. Configure:
   - **Position**: (0, 50) - upper-middle
   - **Width x Height**: 300 x 60
   - **Placeholder Text**: "Enter Code..."
   - **Content Type**: Integer Number (numeric only)
   - **Font Size**: 32
   - **Alignment**: Center

### 3.4. Create Submit Button

1. Right-click **CodeInputPuzzle** → **UI → Button**
2. Rename: `SubmitButton`
3. Configure:
   - **Text**: "Submit"
   - **Position**: (0, -50)
   - **Size**: 150 x 50

### 3.5. Create Clear Button

1. Duplicate **SubmitButton**
2. Rename: `ClearButton`
3. **Text**: "Clear"
4. **Position**: (0, -120)

### 3.6. Create Feedback Text

1. Right-click **CodeInputPuzzle** → **UI → Text**
2. Rename: `FeedbackText`
3. Configure:
   - **Text**: "" (empty)
   - **Font Size**: 20
   - **Color**: Red (sẽ change runtime)
   - **Position**: (0, -20) - below input field

### 3.7. Create Description Text (Optional)

1. Right-click **CodeInputPuzzle** → **UI → Text**
2. Rename: `DescriptionText`
3. Configure:
   - **Text**: "Enter the 4-digit password"
   - **Font Size**: 18
   - **Position**: (0, 140) - top

### 3.8. Assign References

Select **CodeInputPuzzle**, trong **CodeInputPuzzle component**:

| Field | Assign To |
|-------|-----------|
| **config** | (PuzzleConfig asset - assign sau) |
| **puzzleUI** | CodeInputPuzzle panel |
| **codeInputField** | CodeInputField |
| **submitButton** | SubmitButton |
| **clearButton** | ClearButton |
| **feedbackText** | FeedbackText |
| **descriptionText** | DescriptionText |
| **correctColor** | Green (0, 255, 0) |
| **incorrectColor** | Red (255, 0, 0) |
| **normalColor** | White |

### 3.9. Hide Panel

Uncheck **CodeInputPuzzle** GameObject để hide.

---

## Step 4: Setup ColorMatch Puzzle UI

### 4.1. Create Puzzle Panel

1. Right-click **Canvas** → **UI → Panel**
2. Rename: `ColorMatchPuzzle`
3. **Size**: 700 x 450

### 4.2. Add ColorMatchPuzzle Script

1. Select **ColorMatchPuzzle**
2. **Add Component → Color Match Puzzle**

### 4.3. Create Color Buttons Container

1. Right-click **ColorMatchPuzzle** → **Create Empty**
2. Rename: `ColorButtonsContainer`
3. **Add Component → Horizontal Layout Group**
4. Settings:
   - **Spacing**: 20
   - **Child Alignment**: Middle Center
   - **Child Force Expand**: Width + Height

### 4.4. Create Color Buttons (4 colors)

1. Right-click **ColorButtonsContainer** → **UI → Button**
2. Rename: `ColorButton_Red`
3. Configure:
   - **Image Color**: Red (255, 0, 0)
   - **Size**: 100 x 100

Duplicate for other colors:
- `ColorButton_Blue` - Color: Blue (0, 0, 255)
- `ColorButton_Green` - Color: Green (0, 255, 0)
- `ColorButton_Yellow` - Color: Yellow (255, 255, 0)

### 4.5. Create Sequence Slots Container

1. Right-click **ColorMatchPuzzle** → **Create Empty**
2. Rename: `SequenceSlotsContainer`
3. **Add Component → Horizontal Layout Group**
4. **Position**: (0, 80) - upper area
5. **Spacing**: 15

### 4.6. Create Sequence Slots (5 slots)

1. Right-click **SequenceSlotsContainer** → **UI → Image**
2. Rename: `SequenceSlot_0`
3. Configure:
   - **Color**: Gray (128, 128, 128, 100) - semi-transparent
   - **Size**: 80 x 80

Duplicate 4 lần → `SequenceSlot_0` to `SequenceSlot_4` (total 5 slots)

### 4.7. Create Progress Text

1. Right-click **ColorMatchPuzzle** → **UI → Text**
2. Rename: `ProgressText`
3. **Text**: "0/5"
4. **Position**: (0, 150)

### 4.8. Create Clear Button

1. Right-click **ColorMatchPuzzle** → **UI → Button**
2. Rename: `ClearButton`
3. **Text**: "Clear"
4. **Position**: (0, -150)

### 4.9. Assign References

Select **ColorMatchPuzzle**, trong **ColorMatchPuzzle component**:

**Arrays (important - order matters!):**

| Field | Assign (in order) |
|-------|------------------|
| **colorButtons** | [ColorButton_Red, ColorButton_Blue, ColorButton_Green, ColorButton_Yellow] |
| **colorNames** | ["Red", "Blue", "Green", "Yellow"] (type manually) |
| **colors** | [Red(255,0,0), Blue(0,0,255), Green(0,255,0), Yellow(255,255,0)] |
| **sequenceSlots** | [SequenceSlot_0, SequenceSlot_1, ..., SequenceSlot_4] |

**Other fields:**

| Field | Assign To |
|-------|-----------|
| **config** | (PuzzleConfig - assign sau) |
| **puzzleUI** | ColorMatchPuzzle panel |
| **progressText** | ProgressText |
| **clearButton** | ClearButton |
| **emptySlotColor** | Gray (128, 128, 128, 100) |

### 4.10. Hide Panel

Uncheck **ColorMatchPuzzle** GameObject.

---

## Step 5: Register Puzzles in PuzzleSystem

### 5.1. Auto-Discovery (Recommended)

PuzzleSystem tự động find puzzles khi Play:

```csharp
// PuzzleSystem.Awake()
if (autoDiscoverPuzzles)
{
    PuzzleBase[] foundPuzzles = FindObjectsByType<PuzzleBase>(FindObjectsSortMode.None);
    puzzles = foundPuzzles;
}
```

**Không cần manual assignment!** ✅

### 5.2. Manual Assignment (Optional)

Nếu muốn kiểm soát thứ tự:

1. Select **PuzzleSystem** GameObject
2. Trong **PuzzleSystem component**, **puzzles** array:
   - Size = 3
   - Element 0 = ButtonSequencePuzzle
   - Element 1 = CodeInputPuzzle
   - Element 2 = ColorMatchPuzzle

---

## Step 6: Link PuzzleSystem to GameManager

### 6.1. Auto-Linking

GameManager tự động find:
```csharp
if (puzzleSystem == null)
    puzzleSystem = FindFirstObjectByType<PuzzleSystem>();
```

### 6.2. Verify Linking

Press **Play**, check Console:
```
[PuzzleSystem] Initialized with 3 puzzles
[PuzzleSystem] Registered puzzle: Puzzle_ButtonSeq01 (ButtonSequencePuzzle)
```

---

## Step 7: Create PuzzleConfig Assets

**Xem guide:** [07-scriptableobject-creation.md](07-scriptableobject-creation.md) để tạo PuzzleConfig ScriptableObjects.

**Quick create:**

1. Right-click `Assets/Resources/Puzzles/` → **Create → Coder Go Happy → Puzzle Config**
2. Rename: `Puzzle_ButtonSeq01`
3. Configure:
   - **puzzleID**: "Puzzle_ButtonSeq01" (auto)
   - **puzzleName**: "Button Sequence Test"
   - **puzzleType**: ButtonSequence
   - **solution**: "0,2,1,3" (click buttons: 0 → 2 → 1 → 3)
   - **difficulty**: 2

Repeat cho CodeInput và ColorMatch puzzles.

---

## Step 8: Assign PuzzleConfig to Puzzle Scripts

### 8.1. ButtonSequencePuzzle

1. Select **ButtonSequencePuzzle** panel
2. Trong **ButtonSequencePuzzle component**, field **config**:
   - Drag `Puzzle_ButtonSeq01` asset từ `Assets/Resources/Puzzles/`

### 8.2. CodeInputPuzzle

1. Select **CodeInputPuzzle** panel
2. Assign **config** → `Puzzle_CodeInput01`

### 8.3. ColorMatchPuzzle

1. Select **ColorMatchPuzzle** panel
2. Assign **config** → `Puzzle_ColorMatch01`

---

## Step 9: Test Puzzle System

### 9.1. Trigger Puzzle via Hotspot

**Prerequisite:** Đã tạo Puzzle-type Hotspot (xem [05-hotspot-setup.md](05-hotspot-setup.md))

1. Create Hotspot với type = Puzzle
2. Set **puzzleID** = "Puzzle_ButtonSeq01"
3. Press **Play**
4. Click hotspot
5. **Expected:**
   - ButtonSequencePuzzle panel shows (fades in)
   - Buttons clickable
   - Click theo sequence "0,2,1,3"
   - Nếu đúng → Puzzle solved, panel fades out
   - Nếu sai → Buttons flash red, sequence resets

### 9.2. Test via Script (Debug)

Create test script: `Assets/Scripts/TestPuzzle.cs`

```csharp
using UnityEngine;
using CoderGoHappy.Puzzle;

public class TestPuzzle : MonoBehaviour
{
    void Start()
    {
        Invoke("ShowTestPuzzle", 2f);
    }

    void ShowTestPuzzle()
    {
        PuzzleSystem.Instance.ShowPuzzle("Puzzle_ButtonSeq01");
    }
}
```

Attach vào any GameObject, Press Play → Puzzle shows after 2 seconds.

---

## Step 10: Configure Puzzle Rewards

### 10.1. PuzzleConfig Reward Item

Trong PuzzleConfig asset:

**Field: rewardItem** (optional)
- Drag ItemData asset (e.g., "KeyItem")
- Khi puzzle solved → Item tự động add vào inventory

### 10.2. Success Event

**Field: successEvent** (optional)
- Type event name: "UnlockDoor_Puzzle01"
- Khi puzzle solved → EventManager publishes event

**Use case:** Trigger door unlock, enable new hotspot, etc.

---

## Step 11: Puzzle Timer & Attempts

### 11.1. Configure Time Limit

Trong PuzzleConfig:

**Field: timeLimit**
- 0 = No time limit ✅
- > 0 = Seconds before auto-fail (e.g., 60)

### 11.2. Configure Max Attempts

**Field: maxAttempts**
- 0 = Unlimited ✅
- > 0 = Max wrong attempts before reset (e.g., 3)

---

## Step 12: Styling Puzzles (Optional)

### 12.1. Import Puzzle UI Sprites

1. Import sprites vào `Assets/Sprites/UI/Puzzles/`:
   - `puzzle_background.png`
   - `button_normal.png`
   - `button_pressed.png`

### 12.2. Apply to Panels

- **ButtonSequencePuzzle** background → `puzzle_background`
- **Button_0, 1, 2, 3** → `button_normal` sprite

### 12.3. Fonts

Assign custom font cho InputField, ProgressText, etc.

---

## Step 13: Puzzle Events Integration

### 13.1. Subscribe to Puzzle Events

```csharp
using CoderGoHappy.Events;

public class AchievementManager : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvents.PuzzleSolved, OnPuzzleSolved);
        EventManager.Instance.Subscribe(GameEvents.PuzzleFailed, OnPuzzleFailed);
    }

    void OnDisable()
    {
        EventManager.Instance?.Unsubscribe(GameEvents.PuzzleSolved, OnPuzzleSolved);
        EventManager.Instance?.Unsubscribe(GameEvents.PuzzleFailed, OnPuzzleFailed);
    }

    void OnPuzzleSolved(object data)
    {
        string puzzleID = data as string;
        Debug.Log($"[Achievement] Puzzle solved: {puzzleID}");
        // Unlock achievement, update stats, etc.
    }

    void OnPuzzleFailed(object data)
    {
        // Track failures, analytics, etc.
    }
}
```

---

## Troubleshooting

### Issue: Puzzle panel không hiện khi click hotspot

**Nguyên nhân:** PuzzleSystem không find được puzzle hoặc puzzleID mismatch

**Giải pháp:**
1. Verify PuzzleConfig.puzzleID khớp với Hotspot.puzzleID
2. Check PuzzleSystem đã register puzzle (Console log)
3. Verify PuzzleConfig assigned vào puzzle script component

### Issue: Buttons không click được

**Nguyên nhân:** EventSystem thiếu hoặc panel block raycasts

**Giải pháp:**
1. Check EventSystem có trong scene
2. Verify buttons có **Button** component
3. Check panel background không block raycasts (nếu không cần block, bỏ tích Raycast Target)

### Issue: Solution validation fail với đúng answer

**Nguyên nhân:** Solution format sai

**Giải pháp:**
- **ButtonSequence**: "0,2,1,3" (comma-separated, no spaces)
- **CodeInput**: "1234" (string numeric, not int list)
- **ColorMatch**: "Red,Blue,Green" (exact color names, case-insensitive)

### Issue: Puzzle không auto-hide sau khi solve

**Nguyên nhân:** Auto-hide code issue hoặc DOTween error

**Giải pháp:**
1. Check Console có error
2. Verify `Invoke(nameof(HidePuzzle), 1.5f)` được call
3. Manual call `HidePuzzle()` in OnPuzzleSolved() override

---

## Performance Tips

### 1. Pooling Puzzle Panels

Nếu puzzles xuất hiện frequently:
- Disable/enable panels thay vì destroy
- Reuse buttons/UI elements

### 2. Optimize Button Count

- ButtonSequence: 4-6 buttons optimal
- ColorMatch: 4-6 colors optimal
- Quá nhiều → UI cluttered, input slow

### 3. Cache References

PuzzleBase đã cache:
- EventManager instance ✅
- SpriteRenderer, Image components ✅

---

## Next Steps

✅ Puzzle System setup hoàn tất!

**Tiếp theo:**
- [07-scriptableobject-creation.md](07-scriptableobject-creation.md) - Create ItemData và PuzzleConfig assets chi tiết
- [08-testing-guide.md](08-testing-guide.md) - Test tất cả systems integration

---

## Summary Checklist

- [ ] PuzzleSystem GameObject created và initialized
- [ ] 3 puzzle UI panels created: ButtonSequence, CodeInput, ColorMatch
- [ ] All UI references assigned to respective puzzle scripts
- [ ] PuzzleConfig assets created và assigned
- [ ] Test puzzle triggered từ hotspot
- [ ] Solution validation working correctly
- [ ] Puzzle panel shows/hides với fade transitions
- [ ] Reward items added to inventory khi puzzle solved
- [ ] Events published (PuzzleSolved, PuzzleFailed)

**Nếu tất cả OK → Ready for [07-scriptableobject-creation.md](07-scriptableobject-creation.md)** 🎨
