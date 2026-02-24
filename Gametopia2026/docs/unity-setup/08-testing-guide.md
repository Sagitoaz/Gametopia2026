# 08 - Integration Testing Guide

## Overview
Comprehensive testing guide cho "Coder Go Happy" game - verify tất cả systems hoạt động correctly và integrate với nhau seamlessly.

**Prerequisites:**
- Đã hoàn thành guides 01-07
- All systems đã setup

---

## Testing Workflow Overview

```
1. Core Systems Test (GameManager, EventManager, GameState)
2. Scene Transition Test (SceneController, fade effects)
3. Inventory System Test (add items, drag-drop, persistence)
4. Hotspot Interaction Test (5 types, bounds detection)
5. Puzzle System Test (3 puzzle types, solutions)
6. Integration Test (full gameplay loop)
7. Save/Load Test (persistence across sessions)
8. Performance Test (profiling, memory)
```

---

## Test 1: Core Systems Initialization

### 1.1. Startup Verification

**Steps:**
1. Open **PersistentScene**
2. Press **Play**
3. Check **Console** logs

**Expected Output:**
```
[EventManager] Initialized
[GameStateData] Initialized
[GameManager] Starting initialization...
[GameManager] GameStateData validated: OK
[GameManager] EventManager validated: OK
[GameManager] Systems initialized
[GameManager] No save data found, starting new game
```

**✅ PASS if:**
- No red errors
- All "validated: OK" messages appeared
- GameManager persists khi switch scenes (check Hierarchy)

**❌ FAIL if:**
- NullReferenceException errors
- "Instance is null" errors
- Multiple GameManager instances in Hierarchy

---

### 1.2. Singleton Persistence Test

**Steps:**
1. Press Play trong PersistentScene
2. Load Level01 (via SceneController hoặc manual)
3. Check Hierarchy → **DontDestroyOnLoad** group có GameManager

**Expected:**
- GameManager GameObject trong **DontDestroyOnLoad** root
- Chỉ 1 instance
- EventManager, GameStateData vẫn accessible

**✅ PASS Criteria:**
- Only 1 GameManager across scenes
- Console không có "Instance already exists" warning

---

## Test 2: EventManager Pub/Sub

### 2.1. Event Publishing Test

**Create test script:** `Assets/Scripts/TestEventManager.cs`

```csharp
using UnityEngine;
using CoderGoHappy.Events;

public class TestEventManager : MonoBehaviour
{
    void Start()
    {
        EventManager.Instance.Subscribe("TestEvent", OnTestReceived);
        EventManager.Instance.Publish("TestEvent", "Hello Events!");
    }

    void OnTestReceived(object data)
    {
        Debug.Log($"✓ Event received: {data}");
        Debug.Log("EventManager Pub/Sub TEST PASSED");
    }

    void OnDestroy()
    {
        EventManager.Instance?.Unsubscribe("TestEvent", OnTestReceived);
    }
}
```

**Steps:**
1. Attach script vào any GameObject
2. Press Play
3. Check Console

**✅ PASS:** "✓ Event received: Hello Events!" + "TEST PASSED"  
**❌ FAIL:** No log, error messages

---

## Test 3: Scene Transitions

### 3.1. Fade Transition Test

**Setup:**
1. Ensure PersistentScene (index 0) và Level01 (index 1) trong Build Settings
2. Level01 has SceneController với FadeOverlay assigned

**Steps:**
1. Press Play (PersistentScene loads)
2. Load Level01 via code:
   ```csharp
   SceneController.Instance.TransitionToScene("Level01", Vector3.zero);
   ```
3. Observe Game view

**Expected Behavior:**
- Screen fades to black (0.5s)
- Loading progress
- Level01 loads
- Screen fades from black to clear

**✅ PASS Criteria:**
- Smooth fade in/out (no sudden cut)
- No errors during transition
- Console: `[SceneController] Transition to Level01 complete`

---

### 3.2. Scene State Persistence Test

**Steps:**
1. Load Level01
2. Modify scene state (e.g., collect item)
3. Transition to Level02
4. Transition back to Level01
5. Verify collected item không re-appear

**✅ PASS:** Item đã collected không hiện lại  
**❌ FAIL:** Item re-appears (scene state không save)

---

## Test 4: Inventory System

### 4.1. Add Item Test

**Steps:**
1. Press Play
2. Run code (via test script hoặc hotspot pickup):
   ```csharp
   ItemData item = Resources.Load<ItemData>("Items/Item_Keyboard");
   InventorySystem.Instance.AddItem(item);
   ```
3. Check InventoryUI

**Expected:**
- Item icon appears trong first empty slot
- Slot highlights khi hover
- Console: `[InventorySystem] Item added: keyboard`

**✅ PASS:** Item visible trong inventory  
**❌ FAIL:** Item không hiện, console error

---

### 4.2. Drag-Drop Test

**Steps:**
1. Add 2+ items vào inventory
2. Click-and-hold item icon
3. Drag to another slot
4. Release mouse

**Expected:**
- DraggedItemIcon follows cursor
- Drop vào empty slot → Item moves
- Drop vào occupied slot → Items swap

**✅ PASS:** Drag-drop smooth, no visual glitches  
**❌ FAIL:** Icon không follow cursor, drop không work

---

### 4.3. Tooltip Test

**Steps:**
1. Add item vào inventory
2. Hover mouse over slot
3. Observe tooltip

**Expected:**
- Tooltip panel appears above slot
- Shows item name và description
- Disappears on mouse exit

**✅ PASS:** Tooltip shows correct info  
**❌ FAIL:** Tooltip không hiện hoặc wrong content

---

### 4.4. Inventory Persistence Test

**Steps:**
1. Add 3 items vào inventory
2. Call `GameManager.Instance.SaveGame()`
3. Stop Play mode
4. Press Play again
5. Check inventory

**Expected:**
- All 3 items persist trong inventory
- Console: `[GameManager] Game state loaded successfully`

**✅ PASS:** Items loaded from save  
**❌ FAIL:** Inventory empty after reload

---

## Test 5: Hotspot Interactions

### 5.1. Pickup Hotspot Test

**Setup:**
- Hotspot type = Pickup
- collectibleItem assigned (e.g., Item_Keyboard)

**Steps:**
1. Press Play
2. Hover mouse over hotspot
3. Observe visual feedback
4. Click hotspot

**Expected:**
- **Hover:** Sprite swaps to highlightSprite, pulse animation
- **Click:** Item added to inventory, hotspot fades out
- Console: `[HotspotComponent] Pickup action: keyboard`

**✅ PASS:** Item collected, hotspot disappears  
**❌ FAIL:** No response on click, item không add

---

### 5.2. Navigation Hotspot Test

**Setup:**
- Hotspot type = Navigation
- targetSceneName = "Level02"

**Steps:**
1. Click navigation hotspot (e.g., door)

**Expected:**
- SceneController transitions to Level02
- Player spawns at spawnPositionInTargetScene
- Fade transition plays

**✅ PASS:** Scene changes smoothly  
**❌ FAIL:** Error "Scene not found", no transition

---

### 5.3. ItemUse Hotspot Test

**Setup:**
- Hotspot type = ItemUse
- requiredItem = Item_USBDrive

**Steps:**
1. Add Item_USBDrive vào inventory
2. Select item (click trong inventory)
3. Click ItemUse hotspot

**Expected:**
- Item consumed from inventory
- successEvent published (if configured)
- Console: `[HotspotComponent] Item use success`

**Without required item:**
- Click hotspot → Console: "Required item not selected"

**✅ PASS:** Both scenarios work correctly  
**❌ FAIL:** Item không consumed hoặc wrong item accepted

---

### 5.4. Puzzle Hotspot Test

**Setup:**
- Hotspot type = Puzzle
- puzzleID = "Puzzle_ComputerLogin"

**Steps:**
1. Click puzzle hotspot

**Expected:**
- Puzzle UI panel shows (ButtonSequencePuzzle, etc.)
- Console: `[PuzzleSystem] ShowPuzzle request: Puzzle_ComputerLogin`

**✅ PASS:** Puzzle UI appears  
**❌ FAIL:** Nothing happens, console error

---

### 5.5. Examine Hotspot Test

**Setup:**
- Hotspot type = Examine
- description = "A motivational poster"

**Steps:**
1. Click examine hotspot

**Expected:**
- Description text appears (if UI implemented)
- Console: `[HotspotComponent] Examine action: poster_01`

**✅ PASS:** Description visible/logged  
**❌ FAIL:** No feedback

---

## Test 6: Puzzle System

### 6.1. ButtonSequence Puzzle Test

**Setup:**
- PuzzleConfig: solution = "0,2,1,3"
- 4 buttons trong UI

**Steps:**
1. Click hotspot để show puzzle
2. Click buttons: Button 0 → Button 2 → Button 1 → Button 3
3. Observe feedback

**Expected (correct sequence):**
- Each click: Button flash green, scale pulse
- After 4th click: Puzzle solved, panel fades out
- Reward item added to inventory (if configured)

**Expected (wrong sequence):**
- All buttons flash red
- Sequence resets
- Attempt count increments

**✅ PASS:** Both correct and wrong sequences behave as expected  
**❌ FAIL:** Validation fails, buttons không respond

---

### 6.2. CodeInput Puzzle Test

**Setup:**
- PuzzleConfig: solution = "1234"

**Steps:**
1. Show puzzle
2. Type "1234" trong InputField
3. Click Submit

**Expected (correct code):**
- Feedback text: "Correct!" (green)
- Puzzle solved, panel closes
- Reward added

**Expected (wrong code):**
- Feedback text: "Incorrect code" (red)
- InputField shakes
- Input clears after delay

**✅ PASS:** Validation works for correct and wrong codes  
**❌ FAIL:** Any code accepted hoặc validation error

---

### 6.3. ColorMatch Puzzle Test

**Setup:**
- PuzzleConfig: solution = "Red,Blue,Green"
- colorNames = ["Red", "Blue", "Green", "Yellow"]

**Steps:**
1. Show puzzle
2. Click: Red button → Blue button → Green button
3. Observe sequence slots

**Expected:**
- Each click: Color appears trong sequence slot
- Progress text: "3/3"
- After 3rd click: Auto-submit, puzzle solved

**Expected (wrong sequence):**
- Sequence slots shake/flash red
- Sequence clears
- Retry allowed

**✅ PASS:** Color selection và validation correct  
**❌ FAIL:** Colors không display, validation fails

---

### 6.4. Puzzle Timer Test (Optional)

**Setup:**
- PuzzleConfig: timeLimit = 10 (seconds)

**Steps:**
1. Show puzzle
2. Wait 10 seconds without solving
3. Observe behavior

**Expected:**
- Timer counts down (if UI shows timer)
- At 0 seconds: Puzzle fails, resets
- Console: `[PuzzleBase] Timer expired`

**✅ PASS:** Timer triggers fail  
**❌ FAIL:** Timer doesn't enforce

---

## Test 7: Full Gameplay Loop Integration

### 7.1. Complete Gameplay Flow

**Scenario: "Unlock Computer Puzzle"**

1. **Start:** Player trong Level01
2. **Collect item:** Click Hotspot_Keyboard (Pickup)
   - ✓ Keyboard added to inventory
3. **Collect item 2:** Click Hotspot_Mouse (Pickup)
   - ✓ Mouse added to inventory
4. **Use item:** Select USBDrive, click Hotspot_USBPort (ItemUse)
   - ✓ USB consumed, computer powered on
5. **Trigger puzzle:** Click Hotspot_Computer (Puzzle)
   - ✓ ButtonSequence puzzle shows
6. **Solve puzzle:** Enter correct sequence
   - ✓ Puzzle solved, reward item added
7. **Navigate:** Click Hotspot_Door (Navigation)
   - ✓ Transition to Level02

**✅ PASS:** All 7 steps execute without errors  
**❌ FAIL:** Any step breaks, console errors

---

### 7.2. Cross-Scene Persistence

**Steps:**
1. Collect 3 items trong Level01
2. Transition to Level02
3. Collect 2 items trong Level02
4. Transition back to Level01
5. Check inventory

**Expected:**
- All 5 items persist trong inventory
- Level01 collected items không re-appear
- Level02 items still trong inventory

**✅ PASS:** Full persistence working  
**❌ FAIL:** Items lost hoặc duplicated

---

## Test 8: Save/Load System

### 8.1. Manual Save Test

**Steps:**
1. Play game, collect items, solve puzzles
2. Call `GameManager.Instance.SaveGame()`
3. Check Console: `[GameManager] Game saved successfully`
4. Check PlayerPrefs:
   - **Windows:** Open Registry Editor
   - **Mac:** `~/Library/Preferences/`
   - Key: `CoderGoHappy_GameState`

**✅ PASS:** PlayerPrefs entry exists  
**❌ FAIL:** No save data hoặc error

---

### 8.2. Load Game Test

**Steps:**
1. Press Play (fresh start)
2. Verify GameStateData loads từ PlayerPrefs
3. Check inventory và puzzle states

**Expected:**
- Console: `[GameManager] Game state loaded successfully`
- Inventory populated với saved items
- Solved puzzles stay solved

**✅ PASS:** State restored correctly  
**❌ FAIL:** Fresh state instead of loaded

---

### 8.3. Clear Save Test (Debug)

**Code:**
```csharp
PlayerPrefs.DeleteKey("CoderGoHappy_GameState");
PlayerPrefs.Save();
Debug.Log("Save data cleared");
```

**Usage:** Để reset game state cho testing.

---

## Test 9: Performance Testing

### 9.1. CPU Profiler Test

**Steps:**
1. **Window → Analysis → Profiler** (Ctrl+7)
2. Press Play
3. Interact với game (collect items, solve puzzles)
4. Check **CPU Usage** tab

**Expected Metrics:**
- **Frame time:** < 16ms (60 FPS)
- **GameManager.Update():** < 0.1ms
- **EventManager.Publish():** < 0.05ms
- **No GC.Alloc spikes** (< 5KB per frame)

**✅ PASS:** Metrics trong range  
**❌ FAIL:** Frame drops, high GC

---

### 9.2. Memory Profiler

**Steps:**
1. Profiler → **Memory** tab
2. Take Sample
3. Check **Detailed** view

**Expected:**
- GameManager: ~1-2 KB
- EventManager: ~2-5 KB
- GameStateData: ~10-20 KB (depends on save data)
- Total Managed Heap: < 50 MB (for small game)

**✅ PASS:** No memory leaks  
**❌ FAIL:** Heap grows continuously

---

## Test 10: Edge Cases & Stress Tests

### 10.1. Rapid Event Publishing

**Test:**
```csharp
for (int i = 0; i < 1000; i++)
{
    EventManager.Instance.Publish("StressTest", i);
}
```

**Expected:** No errors, all events processed

---

### 10.2. Inventory Overflow

**Test:** Add 25 items (max = 20)

**Expected:**
- Items 21-25 rejected
- Console: `[InventorySystem] Inventory full: 20/20`

---

### 10.3. Invalid Puzzle Solution

**Test:** PuzzleConfig solution = "invalid,format"

**Expected:**
- OnValidate warning trong Console
- Puzzle refuses to solve (validation fails)

---

## Common Issues & Fixes

### Issue 1: "NullReferenceException: GameManager.Instance is null"

**Fix:**
1. Set Script Execution Order:
   - GameManager = -100
   - EventManager = -90
   - GameStateData = -80
2. Verify GameManager trong PersistentScene

---

### Issue 2: Items không persist khi reload

**Fix:**
1. Check `GameManager.SaveGame()` được call
2. Verify `InventorySystem.LoadFromGameState()` trong Start()
3. PlayerPrefs có data: Check Registry/plist

---

### Issue 3: Puzzle không trigger

**Fix:**
1. Verify puzzleID trong hotspot khớp PuzzleConfig.puzzleID
2. Check PuzzleConfig assigned vào puzzle script
3. Verify PuzzleSystem đã registered puzzle

---

### Issue 4: Drag-drop không work

**Fix:**
1. Verify EventSystem trong scene
2. Check Canvas có Graphic Raycaster
3. Verify InventorySlot có Image component với Raycast Target enabled

---

### Issue 5: Scene transition freeze

**Fix:**
1. Verify target scene trong Build Settings
2. Check FadeOverlay assigned vào SceneController
3. Ensure no infinite loops trong scene load callbacks

---

## Performance Optimization Tips

1. **Object Pooling** - Pool inventory slots, puzzle buttons (nếu cần)
2. **Sprite Atlases** - Bundle sprites để reduce draw calls
3. **Event Cleanup** - Always Unsubscribe trong OnDisable/OnDestroy
4. **Async Loading** - SceneController đã dùng async ✅
5. **Limit FindObjectOfType** - Cache references trong GameManager ✅

---

## Final Checklist

### Core Systems
- [ ] GameManager initializes without errors
- [ ] EventManager pub/sub working
- [ ] GameStateData saves/loads correctly
- [ ] DontDestroyOnLoad persists across scenes

### Scene Management
- [ ] Scene transitions smooth với fade
- [ ] Scene state persists (collected items)
- [ ] Multiple scenes loadable

### Inventory
- [ ] Items add/remove correctly
- [ ] Drag-drop functional
- [ ] Tooltip shows correct info
- [ ] Inventory persists across sessions

### Hotspots
- [ ] All 5 types working (Pickup, ItemUse, Navigation, Puzzle, Examine)
- [ ] Bounds detection accurate
- [ ] Visual feedback (hover, click)
- [ ] State persistence (collected items stay gone)

### Puzzles
- [ ] ButtonSequence validates correctly
- [ ] CodeInput accepts correct codes
- [ ] ColorMatch sequence matching works
- [ ] Rewards added to inventory
- [ ] Events published on solve

### Integration
- [ ] Full gameplay loop completable
- [ ] No errors trong Console
- [ ] Performance acceptable (60 FPS)

### Save/Load
- [ ] PlayerPrefs save/load working
- [ ] State restores correctly on Play
- [ ] Clear save works (debug)

---

## Congratulations! 🎉

Nếu tất cả tests **PASSED**, game "Coder Go Happy" đã setup hoàn chỉnh và ready for content creation!

**Next Steps:**
1. Create game content (levels, items, puzzles)
2. Import art assets (sprites, backgrounds, UI)
3. Add audio (SFX, music)
4. Polish (particle effects, transitions)
5. Build và test on target platforms

**Happy Coding! 🚀**
