# 9-Day Development Roadmap - Coder Go Happy

**Timeline**: February 23 → March 4, 2026  
**Team**: 2 Developers + 3 Artists (1 Designer)  
**Goal**: 3 playable levels with core systems, Level 1 at 100% art, Level 2-3 at 70-80%

---

## Daily Schedule Overview

| Day | Date | Dev 1 (Core Architect) | Dev 2 (Content) | Artist 1 (Character) | Artist 2 (Backgrounds) | Artist 3 (UI/Items) |
|-----|------|----------------------|-----------------|---------------------|----------------------|-------------------|
| **1** | Feb 23 | Architecture Design | Learn + Pair Program | Character Concepts | Style Guide + L1 Concepts | UI Design System |
| **2** | Feb 24 | Core Systems Dev | Core Systems Learning | Character Sprites (Sad/Happy) | Level 1 Scene 1 BG | UI Mockups + Inventory Design |
| **3** | Feb 25 | Scene Manager + Inventory | Level 1 Scene 1 Puzzles | Mini-Bug Design (10x) | Level 1 Scene 2 BG | Level 1 Item Sprites |
| **4** | Feb 26 | Hotspot + Puzzle System | Level 1 Scene 2-3 Puzzles | Character Animations | Level 1 Scene 3 BG | Tool Sprites + Puzzle UI |
| **5** | Feb 27 | Level 1 Integration | Level 1 Polish + Testing | Mini-Bugs L1 Creation | Level 2 Scene 1-2 BG | Level 2 Item Sprites |
| **6** | Feb 28 | Level 2 Implementation | Level 3 Scene 1 | Mini-Bug Design L2 (15x) | Level 2 Scene 3-4 BG | Level 2 Puzzle UI |
| **7** | Mar 1 | Level 2 Puzzles | Level 3 Scene 2-3 | Mini-Bugs L2 Creation | Level 3 Scene 1-2 BG | Level 3 Item Sprites |
| **8** | Mar 2 | Level 2 Complete | Bug Collection System | Mini-Bug Design L3 (20x) | Level 3 Scene 3 BG | Level 3 Puzzle UI + Icons |
| **9** | Mar 4 | Integration + Bug Fixes | UI Integration + Testing | Mini-Bugs L3 Creation | Final Art Polish | Menu Screens + Final UI |

---

## Day-by-Day Detailed Breakdown

### 📅 DAY 1 - Feb 23 (TODAY) - Foundation & Planning

**MORNING (AI-DLC Workflow)**
- ✅ Workspace Detection (DONE)
- ✅ Requirements Analysis (DONE)
- ✅ Workflow Planning (IN PROGRESS)
- ⬜ Application Design START

**AFTERNOON (Team Kickoff)**

**Dev 1 - Core Architect**
- [ ] Review requirements and architecture plan from AI-DLC
- [ ] Design core system architecture
  - Scene management pattern
  - Inventory system design
  - Hotspot detection approach
  - Puzzle framework structure
- [ ] Create project folder structure in Unity
- [ ] Setup core namespaces and base classes
- **Deliverable**: Architecture document, base class stubs

**Dev 2 - Content Developer**
- [ ] Review requirements document
- [ ] Study docs/overview.md (game design)
- [ ] Pair with Dev 1 to understand architecture
- [ ] Setup Unity editor for level design
- [ ] Study DOTween for animations
- **Deliverable**: Understanding of core systems

**Artist 1 - Character Artist**
- [ ] Review game design (Coder character theme)
- [ ] Create character concept sketches
  - Sad state (stressed, crying, scratching head)
  - Happy state (dancing, cool sunglasses)
- [ ] Define character art style (2D Cartoon/Office)
- [ ] Sketch Mini-Bug concept designs
- **Deliverable**: Character concept art (2-3 variations)

**Artist 2 - Background Artist**
- [ ] Create style guide document
  - Color palette (office theme, coding colors)
  - Line style and shading approach
  - Perspective and depth guidelines
- [ ] Concept sketch for Level 1 Scene 1 (Server Room Hallway)
- [ ] Reference gathering for server rooms
- **Deliverable**: Style guide PDF, L1S1 concept

**Artist 3 - UI/Items Designer**
- [ ] Design UI system concept
  - Inventory panel layout
  - Button styles and icons
  - Bug counter design
  - Menu screen layouts
- [ ] Create UI color scheme
- [ ] Define icon style
- **Deliverable**: UI design mockups (Figma/sketch)

**END OF DAY 1 SYNC (30 min)**
- Review all concept work
- Confirm art direction
- Align on technical approach

---

### 📅 DAY 2 - Feb 24 - Core Systems Development

**Dev 1 - Core Architect**
- [ ] Implement SceneManager system
  - Scene loading and transitions
  - Scene state persistence
- [ ] Create InventorySystem base
  - Item data structure (ScriptableObject)
  - Add/remove item methods
  - Inventory UI controller
- [ ] Create GameManager singleton
- **Deliverable**: Working scene transition, inventory add/remove

**Dev 2 - Content Developer**
- [ ] Continue learning core systems (pair with Dev 1 AM)
- [ ] Create Level 1 scene structure in Unity
- [ ] Place camera and basic layout
- [ ] Test scene loading system
- **Deliverable**: Level 1 Unity scenes created (3 empty scenes)

**Artist 1 - Character Artist**
- [ ] Finalize character design based on Day 1 feedback
- [ ] Create Character Sad sprite (high-res)
- [ ] Create Character Happy sprite (high-res)
- [ ] Export sprites for Unity (PNG, transparent)
- **Deliverable**: 2 character sprites ready for Unity

**Artist 2 - Background Artist**
- [ ] Create Level 1 Scene 1 background (Server Room Hallway)
  - 100% final quality
  - Full detail, proper lighting
  - Resolution: 1920x1080 or higher
- [ ] Export layered PSD + Unity PNG
- **Deliverable**: L1 Scene 1 background FINAL

**Artist 3 - UI/Items Designer**
- [ ] Create detailed UI mockups
  - Inventory panel with item slots
  - Bug counter UI
  - Navigation arrows
  - Pause menu
- [ ] Design inventory slot template
- [ ] Create button sprites (normal/hover/pressed states)
- **Deliverable**: Complete UI sprite sheet

---

### 📅 DAY 3 - Feb 25 - Hotspots & Level 1 Start

**Dev 1 - Core Architect**
- [ ] Implement HotspotSystem
  - Clickable hotspot detection
  - Hover effects and cursor changes
  - Item pickup logic
  - Drag-and-drop validation
- [ ] Create InteractionManager
- [ ] Test with dummy objects
- **Deliverable**: Working hotspot clicks and item pickups

**Dev 2 - Content Developer**
- [ ] Import Level 1 Scene 1 background
- [ ] Place hotspots for Scene 1
  - Item locations (flashlight, screwdriver)
  - Clickable objects
  - Navigation points
- [ ] Implement Scene 1 puzzles (basic logic)
- **Deliverable**: Level 1 Scene 1 playable (rough)

**Artist 1 - Character Artist**
- [ ] Design Mini-Bug variations (10 unique designs for Level 1)
  - Green bugs on cables theme
  - Different poses/orientations
- [ ] Create sprite sheet for bugs
- **Deliverable**: 10 Mini-Bug sprites for Level 1

**Artist 2 - Background Artist**
- [ ] Create Level 1 Scene 2 background (Tech Corner)
  - 100% final quality
  - Old PC, cables, tech equipment
- [ ] Export for Unity
- **Deliverable**: L1 Scene 2 background FINAL

**Artist 3 - UI/Items Designer**
- [ ] Create Level 1 item sprites
  - Flashlight
  - Screwdriver
  - USB keycard
  - Code snippet papers
- [ ] Create tool icon versions (for inventory)
- **Deliverable**: All Level 1 item sprites

---

### 📅 DAY 4 - Feb 26 - Puzzle System & Characters

**Dev 1 - Core Architect**
- [ ] Implement PuzzleInputSystem
  - Generic puzzle interface
  - Button/dial/keypad UI
  - Validation logic with configurable solutions
- [ ] Create PuzzleConfig ScriptableObject
- [ ] Test with sample puzzles
- **Deliverable**: Working puzzle input system (reusable)

**Dev 2 - Content Developer**
- [ ] Complete Level 1 Scene 2 puzzles
- [ ] Complete Level 1 Scene 3 puzzles
- [ ] Implement scene-to-scene navigation
- [ ] Integrate character sprites (sad state)
- **Deliverable**: Level 1 all scenes with puzzles

**Artist 1 - Character Artist**
- [ ] Create character animation transitions
  - Sad idle animation (subtle)
  - Happy celebration animation
- [ ] Create sprite animation frames if needed
- [ ] Test in Unity timeline
- **Deliverable**: Character animations ready

**Artist 2 - Background Artist**
- [ ] Create Level 1 Scene 3 background (Central Electrical Cabinet)
  - 100% final quality
  - Junction boxes, switches, cables
- [ ] Export for Unity
- **Deliverable**: L1 Scene 3 background FINAL (Level 1 complete!)

**Artist 3 - UI/Items Designer**
- [ ] Create tool sprites with special effects
  - Flashlight with light beam
  - Scissors cutting effect
  - Hammer impact
  - Magnet attraction visual
- [ ] Create puzzle UI elements (buttons, dials, input panels)
- **Deliverable**: Tools + puzzle UI assets

---

### 📅 DAY 5 - Feb 27 - Level 1 Complete & Level 2 Start

**Dev 1 - Core Architect**
- [ ] Integrate all Level 1 scenes
- [ ] Implement Level 1 completion detection
- [ ] Add character happy state trigger
- [ ] Polish Level 1 interactions
- [ ] START Level 2 Scene 1-2 structure
- **Deliverable**: Level 1 COMPLETE and playable end-to-end

**Dev 2 - Content Developer**
- [ ] Test Level 1 thoroughly
- [ ] Add 10 Mini-Bugs to Level 1 scenes
- [ ] Bug counter UI implementation
- [ ] Fix Level 1 bugs
- [ ] START Level 3 Scene 1 structure
- **Deliverable**: Level 1 tested, bugs placed

**Artist 1 - Character Artist**
- [ ] Create all 10 Level 1 Mini-Bug sprites (finalized)
- [ ] Integrate bugs into Unity
- [ ] Test bug animations/placement
- **Deliverable**: Level 1 bugs complete

**Artist 2 - Background Artist**
- [ ] Create Level 2 Scene 1 background (Reception Desk)
  - 70-80% quality (good but not ultra-polished)
- [ ] Create Level 2 Scene 2 background (Dev Room)
  - 70-80% quality
- **Deliverable**: L2 Scene 1-2 backgrounds

**Artist 3 - UI/Items Designer**
- [ ] Create Level 2 item sprites
  - Scissors (network cable cutters)
  - Magnet
  - USB keys
  - Bracket symbols (), [], {}
- [ ] Create Level 2 puzzle UI variations
- **Deliverable**: Level 2 item sprites (70-80% quality)

---

### 📅 DAY 6 - Feb 28 - Level 2 Implementation

**Dev 1 - Core Architect**
- [ ] Implement Level 2 Scene 1-2 puzzles
- [ ] Adapt puzzle system for bracket puzzles
- [ ] Scene navigation for Level 2
- [ ] Test Level 2 mechanics
- **Deliverable**: Level 2 Scene 1-2 functional

**Dev 2 - Content Developer**
- [ ] Implement Level 3 Scene 1 puzzles
- [ ] Place hotspots for security gate theme
- [ ] Test hammer and lever mechanics
- **Deliverable**: Level 3 Scene 1 rough playable

**Artist 1 - Character Artist**
- [ ] Design 15 Mini-Bug variations for Level 2
  - Hidden in coffee cups and keyboards theme
- [ ] Create concept sketches
- **Deliverable**: 15 Mini-Bug designs for L2

**Artist 2 - Background Artist**
- [ ] Create Level 2 Scene 3 background (Meeting Room)
  - 70-80% quality
- [ ] Create Level 2 Scene 4 background (Storage)
  - 70-80% quality
- **Deliverable**: L2 Scene 3-4 backgrounds (Level 2 backgrounds complete!)

**Artist 3 - UI/Items Designer**
- [ ] Create Level 2 puzzle UI elements
  - Door lock interfaces
  - Bracket input panels
  - Cable cutting minigame UI
- [ ] Polish Level 2 icons
- **Deliverable**: Level 2 puzzle UI complete

---

### 📅 DAY 7 - Mar 1 - Level 2-3 Parallel Dev

**Dev 1 - Core Architect**
- [ ] Complete Level 2 Scene 3-4 puzzles
- [ ] Implement Level 2 completion logic
- [ ] Test Level 2 end-to-end
- [ ] Polish Level 2 interactions
- **Deliverable**: Level 2 COMPLETE

**Dev 2 - Content Developer**
- [ ] Implement Level 3 Scene 2 puzzles (Control Room)
- [ ] Implement Level 3 Scene 3 puzzles (Data Vault)
- [ ] Test Level 3 mechanics
- **Deliverable**: Level 3 all scenes playable

**Artist 1 - Character Artist**
- [ ] Create all 15 Level 2 Mini-Bug sprites (finalized)
- [ ] Export and prepare for Unity integration
- **Deliverable**: Level 2 bugs complete

**Artist 2 - Background Artist**
- [ ] Create Level 3 Scene 1 background (Security Gate)
  - 70-80% quality
- [ ] Create Level 3 Scene 2 background (Control Room)
  - 70-80% quality
- **Deliverable**: L3 Scene 1-2 backgrounds

**Artist 3 - UI/Items Designer**
- [ ] Create Level 3 item sprites
  - Hammer (firewall breaker)
  - Lever (backup generator)
  - Programming language logo puzzles (Python, JS, C#)
- [ ] Create Level 3 puzzle UI
- **Deliverable**: Level 3 item sprites (70-80% quality)

---

### 📅 DAY 8 - Mar 2 - Final Content & Integration

**Dev 1 - Core Architect**
- [ ] Final Level 2 polish
- [ ] Integration testing (all 3 levels)
- [ ] Performance optimization
- [ ] Menu system (main menu, level select)
- **Deliverable**: All levels integrated

**Dev 2 - Content Developer**
- [ ] Implement bug collection counter for all levels
- [ ] Add 15 Mini-Bugs to Level 2
- [ ] UI integration (inventory, HUD, menus)
- [ ] Level progression system (L1→L2→L3)
- **Deliverable**: Bug system complete, UI working

**Artist 1 - Character Artist**
- [ ] Design 20 Mini-Bug variations for Level 3
  - Behind walls and computer screens theme
- [ ] Create all 20 sprites
- [ ] Export for Unity
- **Deliverable**: 20 Mini-Bug sprites for L3

**Artist 2 - Background Artist**
- [ ] Create Level 3 Scene 3 background (Data Vault)
  - 70-80% quality
- [ ] Final polish pass on all backgrounds
- [ ] Fix any art issues found in testing
- **Deliverable**: L3 Scene 3 background (All backgrounds complete!)

**Artist 3 - UI/Items Designer**
- [ ] Create Level 3 puzzle UI elements
- [ ] Create icon sprites for all UI buttons
- [ ] Polish inventory visual design
- **Deliverable**: Level 3 UI complete, icons ready

---

### 📅 DAY 9 - Mar 4 - CORE DEADLINE - Final Assembly

**ALL TEAM MEMBERS - Integration Day**

**MORNING (Final Implementation)**

**Dev 1**
- [ ] Final bug fixes (critical only)
- [ ] Performance check (60 FPS target)
- [ ] Build optimization
- [ ] Menu polish and flow

**Dev 2**
- [ ] Add all 20 Mini-Bugs to Level 3
- [ ] Final UI integration
- [ ] Level completion screens
- [ ] Save system (level unlocks)

**Artist 1**
- [ ] Integrate all Mini-Bugs into Unity scenes
- [ ] Final character animation tweaks
- [ ] Visual polish

**Artist 2**
- [ ] Final art integration
- [ ] Fix any visual glitches
- [ ] Scene lighting adjustments

**Artist 3**
- [ ] Create Main Menu screen visual
- [ ] Create Level Select screen
- [ ] Create Level Complete screen
- [ ] Final UI polish

**AFTERNOON (Testing & Build)**

**ALL TEAM**
- [ ] Full playthrough testing (all 3 levels)
- [ ] Bug list creation
- [ ] Critical bug fixes
- [ ] Build Windows executable
- [ ] Test build on clean machine

**END OF DAY 9 - CORE MILESTONE COMPLETE** ✅
- 3 playable levels
- All core systems working
- Level 1 at 100% art quality
- Level 2-3 at 70-80% art quality
- All puzzles implemented
- All 45 Mini-Bugs (10+15+20) placed
- UI complete

---

## Post-Core Days (Mar 5-20) - Polish Phase

**Days 10-14 (Mar 5-9) - Art Polish**
- Polish Level 2-3 art to 100% quality
- Add visual effects (particles, transitions)
- Sound integration (if time)
- Character animation polish

**Days 15-20 (Mar 10-15) - Bug Fixes & Optimization**
- Address all bug list items
- Performance optimization
- Playtesting with external testers
- Difficulty balancing

**Days 21-25 (Mar 16-20) - Final Polish**
- Final playtesting
- Any remaining bug fixes
- Build final competition version
- Prepare submission materials

**MARCH 20 - FINAL DEADLINE** 🎯
- Competition-ready build
- 100% art quality all levels
- Zero known critical bugs
- Optimized performance

---

## Daily Standup Structure (5-10 minutes)

**Every morning, team sync:**
1. Yesterday achievements
2. Today's plan
3. Blockers/help needed
4. Quick decisions/alignment

**Use Discord/Slack for async updates between standups**

---

## Critical Path Items (Must Not Slip)

🔴 **Day 1-2**: Core systems architecture (blocks everything)  
🔴 **Day 3-4**: Hotspot and puzzle systems (blocks level implementation)  
🔴 **Day 5**: Level 1 complete (first playable milestone)  
🔴 **Day 7**: Level 2 complete (shows reusability working)  
🔴 **Day 9**: All integration complete (core deadline)

---

## Risk Indicators (Check Daily)

⚠️ **If by Day 3**: Core systems not working → Escalate, pair program  
⚠️ **If by Day 5**: Level 1 not complete → Activate backup plan (reduce L3 scope)  
⚠️ **If by Day 7**: Level 2 not working → Cut Level 3 features  
⚠️ **If by Day 8**: Integration issues → Extend Day 9 to weekend if needed  

---

## Communication Channels

- **Discord/Slack**: Daily async updates
- **Morning Standup**: 5-10 min sync
- **Shared Drive**: Art assets, builds, documents
- **Git Repository**: Code (devs commit frequently)
- **Trello/Notion**: Task tracking (optional but recommended)

---

**Roadmap Status**: FINAL  
**Ready for**: Team execution starting Day 1 (TODAY!)
