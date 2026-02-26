# 09 - Day 5: Level Manager & Bug Counter Setup

## Overview
Hướng dẫn setup **LevelManager** (quản lý level completion) và **BugCounterUI** (hiển thị MiniBug collection progress) cho Day 5.

**New Systems:**
- **LevelManager**: Detect level completion, trigger character happy state
- **BugCounterUI**: Display bug collection progress (Bugs: X/10)
- **Mini-Bug Collectibles**: Hidden bugs trong Level 1

**Prerequisites:**
- Đã hoàn thành guides 01-08
- Level 1 scenes đã có trong project

---

## Architecture Overview

```
Level 1 Structure:
├── PersistentScene (GameManager, EventManager, GameStateData)
├── Level01_Scene1 
│   ├── LevelManager (NEW!)
│   ├── Canvas
│   │   └── BugCounterUI (NEW! - hiển thị bug count)
│   ├── Hotspots
│   └── MiniBugs (hidden collectibles)
├── Level01_Scene2
│   ├── MiniBugs
│   └── Hotspots
└── Level01_Scene3
    ├── MiniBugs
    └── Hotspots (final puzzle)
```

---

## Step 1: Create LevelManager GameObject

### 1.1. Create in First Scene of Level 1

1. Open **Level01_Scene1** (first scene of Level 1)
2. Right-click Hierarchy → **Create Empty**
3. Rename: `LevelManager`
4. Reset Transform: (0, 0, 0)

### 1.2. Add LevelManager Component

1. Select **LevelManager**
2. **Inspector → Add Component**
3. Search: **Level Manager**
4. Component will be added

---

## Step 2: Configure LevelManager

### 2.1. Inspector Configuration

Select **LevelManager**, trong **Level Manager component**:

| Field | Value | Description |
|-------|-------|-------------|
| **Level Number** | 1 | Current level (1, 2, hoặc 3) |
| **Required Puzzles** | Array | puzzleIDs player must solve để complete level |
| **Required Items** | Array | itemIDs player must collect |
| **Total Mini Bugs In Level** | 10 | Total bugs available trong Level 1 |
| **Next Level Scene Name** | "Level02_Scene1" | Scene to load khi complete |
| **Character Object** | (assign later) | Character GameObject để trigger happy state |
| **Happy Animation Trigger** | "Happy" | Animation trigger name |

### 2.2. Configure Required Puzzles Example

**Level 1 có 3 puzzles:**
1. "Puzzle_ComputerLogin" (Scene 1)
2. "Puzzle_NetworkFix" (Scene 2)
3. "Puzzle_FinalCode" (Scene 3)

**Inspector setup:**
1. Click **Required Puzzles** → Set **Size = 3**
2. Element 0: "Puzzle_ComputerLogin"
3. Element 1: "Puzzle_NetworkFix"
4. Element 2: "Puzzle_FinalCode"

### 2.3. Configure Required Items (Optional)

Nếu Level 1 yêu cầu collect specific items (ví dụ: USB Drive):

1. Click **Required Items** → Set **Size = 1**
2. Element 0: "Item_USBDrive"

**⚠️ Lưu ý:** Nếu không cần required items → Set Size = 0

---

## Step 3: Create BugCounterUI

### 3.1. Create UI Text

1. Right-click **Canvas** → **UI → Text - TextMeshPro**
2. Rename: `BugCounterUI`
3. Configure **RectTransform:**
   - **Anchor Preset**: **Top-Right**
   - **Pos X**: -100 (offset from right edge)
   - **Pos Y**: -50 (offset from top)
   - **Width**: 150
   - **Height**: 50

### 3.2. Configure TextMeshProUGUI

1. **Text**: "🐛 0/10" (placeholder)
2. **Font Size**: 24
3. **Color**: White
4. **Alignment**: Right-Middle
5. **Horizontal/Vertical Overflow**: Overflow

**Visual Position:**
```
┌─────────────────────────────────┐
│                      🐛 3/10 ← │  Top-right corner
│                                 │
│        Game View                │
│                                 │
│                                 │
└─────────────────────────────────┘
```

### 3.3. Add BugCounterUI Component

1. Select **BugCounterUI** GameObject
2. **Inspector → Add Component**
3. Search: **Bug Counter UI**
4. Component will be added

### 3.4. Configure BugCounterUI Component

| Field | Value | Description |
|-------|-------|-------------|
| **Bug Count Text** | BugCounterUI (self) | Auto-assigned |
| **Bug Icon** | (optional) | Sprite icon for bug |
| **Text Format** | "🐛 {0}/{1}" | {0}=collected, {1}=total |
| **Completed Color** | Yellow (255,255,0) | Color khi all bugs collected |
| **Normal Color** | White | Default color |
| **Pulse Scale** | 1.3 | Scale multiplier khi animate |
| **Pulse Duration** | 0.3 | Animation duration (seconds) |

---

## Step 4: Create Mini-Bug Collectibles

### 4.1. Create ItemData for Mini-Bug

1. Right-click **Assets/Resources/Items** → **Create → Item Data**
2. Rename: `MiniBug_01`
3. Configure:
   - **Item Name**: "Debug Bug"
   - **Item ID**: "MiniBug_01"
   - **Description**: "A mysterious coding bug to collect!"
   - **Sprite**: (bug sprite - create or use placeholder)
   - **Is Mini Bug**: ☑ **TÍCH** (CRITICAL!)

**⚠️ QUAN TRỌNG:** Phải tích **Is Mini Bug** checkbox để system track riêng!

### 4.2. Create Bug Hotspot in Scene

1. Right-click **Hierarchy** → **2D Object → Sprite**
2. Rename: `Hotspot_MiniBug01`
3. Configure:
   - **Position**: Hidden location (behind objects, corners, etc.)
   - **Sprite**: Bug sprite (nhỏ, subtle)
   - **Sorting Layer**: "Interactable" hoặc "Foreground"
   - **Order in Layer**: 5

### 4.3. Add HotspotComponent

1. Select **Hotspot_MiniBug01**
2. **Add Component → Hotspot Component**
3. Configure:
   - **Hotspot Type**: **Pickup**
   - **Hotspot ID**: "Hotspot_MiniBug01"
   - **Collectible Item**: MiniBug_01 (ItemData asset)
   - **Persistent**: ☑ Tích (để không re-appear sau khi collected)

### 4.4. Create 10 MiniBugs

**Level 1 cần 10 bugs total:**
- Scene 1: 3 bugs
- Scene 2: 4 bugs
- Scene 3: 3 bugs

**Workflow:**
1. Duplicate **Hotspot_MiniBug01** → Rename MiniBug02, MiniBug03, etc.
2. Change positions (hidden locations)
3. Create separate ItemData assets: MiniBug_02, MiniBug_03, etc.
4. Assign unique itemIDs: "MiniBug_02", "MiniBug_03"
5. Update hotspotID: "Hotspot_MiniBug02", etc.

**💡 Placement Tips:**
- Behind furniture/objects
- In corners
- Behind characters
- Subtle locations requiring exploration

---

## Step 5: Test Level Completion

### 5.1. Setup Test Scenario

**Required:**
1. LevelManager có requiredPuzzles configured (3 puzzles)
2. PuzzleConfig assets có correct puzzleIDs
3. Hotspots trigger đúng puzzles

### 5.2. Test Workflow

1. **Press Play**
2. Solve all required puzzles:
   - Puzzle_ComputerLogin
   - Puzzle_NetworkFix
   - Puzzle_FinalCode
3. **Console logs:**
   ```
   [LevelManager] Puzzle solved, checking level completion...
   [LevelManager] All required puzzles solved!
   [LevelManager] ★★★ LEVEL 1 COMPLETE! ★★★
   [LevelManager] Character entered happy state!
   ```
4. Level transitions to Level 2 sau 3 seconds

### 5.3. Test Bug Collection

1. Press Play
2. Click MiniBug hotspots (10 bugs)
3. **Observe BugCounterUI:**
   - Text updates: 🐛 1/10 → 2/10 → ... → 10/10
   - Pulse animation mỗi khi collect
   - Color changes to Yellow khi 10/10
4. **Console logs:**
   ```
   [LevelManager] MiniBug collected! Total in run: 1/10
   [BugCounterUI] Updated: 1/10
   ```

---

## Step 6: Integrate Character Happy State

### 6.1. Setup Character with Animator

**If you have character sprite:**
1. Select Character GameObject
2. Add **Animator** component
3. Create Animation Controller: `CharacterAnimator`
4. Add animation states: Idle, Happy
5. Add trigger parameter: "Happy"

### 6.2. Assign to LevelManager

1. Select **LevelManager**
2. Drag **Character GameObject** vào **Character Object** field
3. Set **Happy Animation Trigger** = "Happy"

### 6.3. Test Happy State

1. Press Play
2. Complete level
3. Character animation triggers Happy state

**Without animation:**
- Character happy state still triggers (logged)
- Can add particle effects, sound, etc. in code

---

## Step 7: Debug & Testing Tools

### 7.1. Force Complete Level (Debug)

**Inspector Context Menu:**
1. Select **LevelManager** trong Hierarchy (Play mode)
2. Right-click **Level Manager component**
3. Click **"Force Complete Level"**
4. Level completes immediately ✅

**Usage:** Nhanh test level completion logic

### 7.2. Check Bug Collection Progress

**Runtime inspection:**
1. Play mode → Select **BugCounterUI**
2. Inspector shows current bug count
3. Call `GetBugCount()` trong Console (nếu có debug console)

### 7.3. Verify Required Puzzles

**Console logs:**
```
[LevelManager] Puzzle 'Puzzle_ComputerLogin' not yet solved
[LevelManager] Puzzle 'Puzzle_NetworkFix' solved
...
[LevelManager] All required puzzles solved!
```

---

## Troubleshooting

### Issue: Level không complete sau khi solve puzzles

**Nguyên nhân:** puzzleID mismatch

**Giải pháp:**
1. Check **LevelManager.requiredPuzzles** array
2. Verify PuzzleConfig.puzzleID khớp exact (case-sensitive)
3. Check Console: "Puzzle 'XXX' not yet solved"
4. Ensure PuzzleSystem published PuzzleSolved event

### Issue: BugCounterUI không update

**Nguyên nhân:** Event không published hoặc ItemData không set isMiniBug

**Giải pháp:**
1. Check ItemData → **Is Mini Bug** = ☑ Tích
2. Verify EventManager subscribed trong BugCounterUI.Start()
3. Check Console: "[LevelManager] MiniBug collected!"
4. Ensure Canvas có BugCounterUI component

### Issue: Character happy state không trigger

**Nguyên nhân:** Character object không assigned hoặc Animator thiếu

**Giải pháp:**
1. Assign Character GameObject vào LevelManager.characterObject
2. Check Character có Animator component
3. Verify Animator Controller có "Happy" trigger parameter
4. Check Console: "[LevelManager] Character happy animation triggered"

### Issue: Level transitions ngay lập tức

**Nguyên nhân:** nextLevelSceneName assigned nhưng không có delay

**Giải pháp:**
1. Code có `Invoke(nameof(LoadNextLevel), 3f)` - 3 second delay
2. Check nextLevelSceneName correct trong Build Settings
3. Nếu muốn skip transition → Clear nextLevelSceneName field

---

## Performance Tips

### 1. LevelManager Singleton

- LevelManager **không** persist across scenes (khác GameManager)
- Mỗi level có LevelManager riêng
- Automatically destroyed khi leave level

### 2. Bug Counter Updates

- BugCounterUI chỉ update khi event fired
- Không poll/check mỗi frame
- Efficient với nhiều bugs ✅

### 3. Animation Optimization

- DOTween animations optimized
- Kill existing animations trước khi start new
- No memory leaks ✅

---

## Summary Checklist

- [ ] LevelManager created trong Level01_Scene1
- [ ] LevelManager configured:
  - [ ] Level Number = 1
  - [ ] Required Puzzles array filled (3 puzzles)
  - [ ] Total MiniBugs = 10
  - [ ] Next Level Scene = "Level02_Scene1"
- [ ] BugCounterUI created trong Canvas (top-right)
- [ ] BugCounterUI component configured
- [ ] 10 MiniBug ItemData assets created (isMiniBug = true)
- [ ] 10 MiniBug hotspots placed trong Level 1 scenes
- [ ] Test level completion working
- [ ] Test bug collection working (counter updates)
- [ ] Character happy state configured (if applicable)
- [ ] All Console logs showing correctly

**Nếu tất cả OK → Level 1 completion system ready! Proceed to content creation.** 🎮

---

## Next Steps

**Day 5 Content Tasks:**
1. Place 10 MiniBugs strategically trong Level 1
2. Test full Level 1 playthrough
3. Polish interactions & transitions
4. Create Level 2 Scene 1-2 structure

**Day 6:**
- Level 2 implementation
- More puzzles & content

**Happy coding!** 🚀
