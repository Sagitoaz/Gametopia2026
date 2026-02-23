# Quick Reference Guide - Team Assignments

**Project**: Coder Go Happy  
**Core Deadline**: March 4, 2026 (9 days from Feb 23)  
**Final Deadline**: March 20, 2026

---

## 👥 Team Roles & Primary Responsibilities

### 🔧 Dev 1 - Core Systems Architect
**Focus**: Build reusable framework first, then level implementation
- Days 1-2: Core systems architecture
- Days 3-4: Hotspot & puzzle framework
- Days 5-7: Level 1-2 implementation
- Days 8-9: Integration & bug fixes

### 🔧 Dev 2 - Content Implementation
**Focus**: Learn systems, implement level content
- Days 1-2: Learn core systems (pair with Dev 1)
- Days 3-5: Level 1 puzzles & content
- Days 6-7: Level 3 implementation
- Days 8-9: Bug system, UI, testing

### 🎨 Artist 1 - Character & Mini-Bugs
**Focus**: Character sprites and all collectible bugs
- Days 1-2: Character design & sprites (sad/happy)
- Days 3-9: Mini-Bugs design & creation (45 total: 10+15+20)
- Character animations throughout

### 🎨 Artist 2 - Backgrounds
**Focus**: Scene backgrounds, Level 1 priority
- Day 1: Style guide
- Days 2-4: Level 1 backgrounds (100% quality)
- Days 5-7: Level 2 backgrounds (70-80%)
- Days 8-9: Level 3 backgrounds (70-80%)

### 🎨 Artist 3 - UI & Items (Designer)
**Focus**: All UI elements and item sprites
- Days 1-2: UI design system
- Days 3-4: Level 1 items & tools (100%)
- Days 5-9: Level 2-3 items, puzzle UI, menus (70-80%)

---

## 📊 Core Systems (Dev 1 Priority)

**Must Complete Days 1-4:**
1. **SceneManager** - Loading, transitions, state persistence
2. **InventorySystem** - Add/remove items, UI display
3. **HotspotSystem** - Click detection, hover, drag-drop
4. **PuzzleInputSystem** - Generic puzzle framework
5. **GameManager** - Singleton, game state

**Use ScriptableObjects for data-driven design**

---

## 🎮 Level Content Breakdown

### Level 1: The Legacy System (3 scenes)
- **Art Quality**: 100% FINAL by Day 5
- **Scenes**: Server Room Hallway, Tech Corner, Central Electrical Cabinet
- **Tools**: Flashlight, Screwdriver
- **Puzzles**: 3-6 total (1-2 per scene)
- **Bugs**: 10 Mini-Bugs (green, on cables)
- **Symbols**: Programming operators (&&, ||, !, ==)

### Level 2: The Logic Gate Maze (4 scenes)
- **Art Quality**: 70-80% by Day 7
- **Scenes**: Reception, Dev Room, Meeting Room, Storage
- **Tools**: Scissors, Magnet
- **Puzzles**: 4-8 total (1-2 per scene)
- **Bugs**: 15 Mini-Bugs (in coffee cups, under keyboards)
- **Symbols**: Brackets (), [], {}

### Level 3: The Security Breach (3 scenes)
- **Art Quality**: 70-80% by Day 9
- **Scenes**: Security Gate, Control Room, Data Vault
- **Tools**: Hammer, Lever
- **Puzzles**: 3-6 total (1-2 per scene)
- **Bugs**: 20 Mini-Bugs (behind walls, screens)
- **Symbols**: Programming language logos (Python, JS, C#)

---

## 🎯 Daily Goals (Quick Version)

| Day | Dev Goal | Art Goal |
|-----|----------|----------|
| 1-2 | Core systems working | Character + Style guide done |
| 3-4 | Hotspot & puzzles ready | L1 backgrounds + items complete |
| 5 | **Level 1 COMPLETE** | L1 art 100% + bugs placed |
| 6-7 | **Level 2 COMPLETE** | L2 backgrounds + items done |
| 8 | Level 3 done | L3 backgrounds done |
| 9 | **ALL INTEGRATED** | All bugs placed, UI complete |

---

## ⚠️ Critical Success Factors

**MUST HAVE by March 4:**
- ✅ All 3 levels playable end-to-end
- ✅ All core systems functional
- ✅ Level 1 at 100% art quality
- ✅ Level 2-3 at 70-80% art quality
- ✅ All 45 Mini-Bugs placed (10+15+20)
- ✅ UI complete (inventory, menus, HUD)

**ACCEPTABLE for Core Deadline:**
- ⚠️ Some bugs/glitches (will fix in polish phase)
- ⚠️ Basic sound/music (can add later)
- ⚠️ Level 2-3 not ultra-polished (polish Mar 5-20)

---

## 🚨 Backup Plan (If Timeline Slips)

**Priority Order (Cut in this sequence if needed):**
1. **First**: Reduce Level 3 scope → push to polish phase
2. **Second**: Drop Level 2 art to 50% placeholder
3. **Third**: Reduce Level 2 puzzle count
4. **Last Resort**: Focus only Level 1 perfect + core systems

---

## 📁 File Structure Reference

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── SceneManager.cs
│   │   └── InventorySystem.cs
│   ├── Gameplay/
│   │   ├── HotspotSystem.cs
│   │   ├── PuzzleSystem.cs
│   │   └── BugCollectionSystem.cs
│   └── Levels/
│       ├── Level1/
│       ├── Level2/
│       └── Level3/
├── Scenes/
│   ├── MainMenu.unity
│   ├── Level1_Scene1.unity
│   ├── Level1_Scene2.unity
│   └── ...
├── Sprites/
│   ├── Characters/
│   ├── Backgrounds/
│   ├── Items/
│   ├── UI/
│   └── MiniBugs/
├── ScriptableObjects/
│   ├── Items/
│   └── Puzzles/
└── Prefabs/
    ├── UI/
    └── Interaction/
```

---

## 🔗 Key Documents

- **Full Requirements**: `aidlc-docs/inception/requirements/requirements.md`
- **Execution Plan**: `aidlc-docs/inception/plans/execution-plan.md`
- **9-Day Roadmap**: `aidlc-docs/inception/plans/9-day-roadmap.md`
- **Game Design**: `docs/overview.md`
- **This Guide**: `aidlc-docs/inception/plans/quick-reference.md`

---

## 💬 Daily Standup Format (5-10 min)

1. **Yesterday**: What did you complete?
2. **Today**: What will you work on?
3. **Blockers**: Any help needed?
4. **Quick Decisions**: Any alignment needed?

---

## 🎯 Definition of Done

**For Each Day:**
- [ ] Deliverables listed in roadmap completed
- [ ] Code committed (devs)
- [ ] Assets exported to Unity (artists)
- [ ] No blockers for tomorrow's work

**For Core Milestone (Day 9):**
- [ ] Complete playthrough of all 3 levels possible
- [ ] All systems working (may have minor bugs)
- [ ] Level 1 looks polished
- [ ] Builds without errors

---

**Last Updated**: Feb 23, 2026  
**Team**: Let's build an amazing game! 🚀
