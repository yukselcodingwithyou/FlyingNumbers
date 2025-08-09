# Unity 2D Game Development Prompt for GitHub Copilot
## Game Concept: Flappy Number

I am creating a 2D Unity game similar to *Flappy Bird*, but with enhanced mechanics, UI, and monetization features.  
The main character is a **winged number `1`**, and the gameplay includes obstacles, power-ups, shields, and coins.

---

## 🧩 Development Guidelines

- Use the **Modular Component Pattern (MCP)** to structure the project:
  - Each major game feature should be handled by its own script/component.
  - Scripts should not overlap responsibilities.
  - Use clean, inspector-driven serialized fields.
  - Use events or messaging for inter-component communication.

---

## 🎨 Canva Integration for GameObjects and UI

- **Design UI panels, buttons, icons, characters, and obstacles in Canva** with cartoon style.
- Export all visuals as **PNG files with transparent backgrounds**.
- Import into Unity as `Sprite (2D and UI)` and organize under:

Assets/
├── Sprites/
│ ├── Characters/
│ ├── UI/
│ ├── PowerUps/
│ └── Obstacles/
├── Scripts/
│ ├── Player/
│ ├── UI/
│ ├── Obstacles/
│ ├── PowerUps/
│ ├── Managers/
│ └── Ads/
├── Prefabs/
├── Animations/
├── Audio/
└── Scenes/

yaml
Copy
Edit

- Assign Canva assets to GameObjects and prefabs.
- Animate wings, bubbles, and collisions using AnimatorController.
- UI should be managed via `UIManager.cs`.

---

## ✅ STEP-BY-STEP IMPLEMENTATION (Follow Order)

### 🥇 STEP 1: Core Player Mechanics
1. Create `PlayerController.cs`.
2. Add flap mechanic (on touch or spacebar).
3. Use Rigidbody2D + rotation tilting.
4. Animate wing flap on every jump.
5. Add collision detection.

---

### 🧠 STEP 2: Operator Bubbles
6. Create `OperatorBubble` prefab that:
   - Moves leftward and up/down (sine wave).
   - Has animated wings and a label (e.g., `+1`, `x0`).
7. On collision:
   - Apply operation to number.
   - End game if `x0` unless shield is active.
8. Destroy off-screen.

---

### 💣 STEP 3: Obstacles
9. Create `Mine` (moves up/down + leftward) and `Pipe` (static).
10. Animate mines.
11. Destroy off-screen.
12. Colliding ends game unless shield is active.

---

### 🛡️ STEP 4: Shield System
13. Create `Shield` prefab (random floaty motion).
14. Collectible by player.
15. Protects from 1 hit (mine, pipe, `x0`).
16. Visual shield bubble around player.
17. Track shield count globally using PlayerPrefs or JSON.

---

### 💰 STEP 5: Coins & Economy
18. Create `Coin` prefab with random floaty movement.
19. Collecting 3 coins grants 1 shield.
20. Display total coins in UI and track globally.

---

### 🎮 STEP 6: Game Over & UI
21. Create animated Game Over panel:
   - Score
   - High score (persistent)
   - Coins collected
   - Shields
22. Add restart button and animation.
23. Add pause/resume functionality.
24. Use `UIManager.cs` to manage UI panels.

---

### 🚀 STEP 7: Leveling System
25. Add `LevelManager.cs`.
26. Increase level based on score/time.
27. Increase game speed, bubble spawn rate.
28. Decrease spawn frequency of shields/coins.
29. Increase chance of `x0` bubbles.

---

### 🧲 STEP 8: Power-Ups
30. Add:
   - **Magnet**: Pulls in nearby coins/shields.
   - **Slow Motion**: Slows time temporarily.
   - **Shrink**: Reduces size.
   - **Invincibility**: Immune to obstacles temporarily.

---

### 🧠 STEP 9: Missions & Challenges
31. Create system for:
   - Daily challenges
   - Missions like “Collect 10 coins”
32. Display and reward with coins or shields.

---

### 🎨 STEP 10: Visuals & Feedback
33. Use cartoon-style sprites from Canva.
34. Animate:
   - Wing flaps
   - Collisions
   - Shield pickups
35. Add:
   - Score multiplier bubbles
   - Combo bonuses
   - Juicy feedback: screen shake, pop FX, sound FX
36. Add parallax scrolling backgrounds.
37. Add dynamic soundtrack (faster as level increases).

---

### 🌍 STEP 11: Social & Global Features
38. Add:
   - Unlockable numbers (2, 3, 4...) with different wings
   - Name input for leaderboard
   - Share score button
   - Global leaderboards (if online support added)

---

### 💸 STEP 12: Monetization & Ads
39. In-app purchases:
   - 5 shields for $0.05
   - Coin doubler
   - Remove ads
40. Ad system:
   - Interstitial ads every 3–4 restarts
   - Banner ads (top or bottom)
   - Rewarded ads for shield or revive

---

### ⚙️ STEP 13: Utility Features
41. Pause menu with resume/restart/exit.
42. Settings menu (music, SFX, language).
43. Onboarding/tutorial for first-time players.
44. Save coins, shields, highscores via PlayerPrefs or persistent file.
45. Offline support for core gameplay and purchases when possible.

---

## ✅ Developer Guidelines for GitHub Copilot

- Use clean modular scripts: `PlayerController`, `GameManager`, `UIManager`, `AdManager`, etc.
- Organize all assets by type (Sprites, Scripts, Prefabs, Audio).
- Use `public` fields with `[SerializeField]` for flexibility.
- Favor modularity and single responsibility per script.
- Use `Events`, `UnityEvent`, or C# delegates to keep decoupling between modules.

---

> Follow this full structure and order to develop the game, using Copilot suggestions, Unity's 2D system, and Canva-designed visuals. Prioritize code clarity and feature completion one step at a time.


# GITHUB COPILOT MASTER PROMPT — Flappy Number (Full Build, Animations + Scene Hierarchy)

## Scope
- Unity 2D (C#). Build a game inspired by *Flappy Bird* starring a **winged number `1`**, plus unlockable **Cat**, **Dog**, **Fox**.
- Use **PNG sprites from `Assets/Sprites`** for all visuals.
- Implement: **operator bubbles**, **mines**, **pipes**, **shields**, **coins**, **power-ups**, **leveling**, **starter screen**, **shop**, **game over panel**, **data persistence**.
- Create **wing + feet animations**: wings flap for characters, power-ups, and winged obstacles; feet move on characters.
- **Copilot must create the animations, controllers, prefabs, scenes, and hierarchy** exactly as specified below.

---

## Global Asset & Import Rules
- All PNGs live in `Assets/Sprites/`.
- For every sprite:
  - Texture Type: **Sprite (2D and UI)**
  - Filter Mode: **Point** (or Bilinear)
  - Compression: **Automatic**
  - Pixels Per Unit: **100**
  - Mesh Type: **Full Rect**
  - Pivot: **Center**

---

## Required Project Structure (create if missing)
```
Assets/
├── Sprites/                            # All provided PNGs
├── Animations/
│   ├── Wings/
│   └── Feet/
├── Controllers/
│   ├── WingAnimator.controller
│   └── FeetAnimator.controller
├── ScriptableObjects/
│   └── DefaultFlapProfile.asset
├── Prefabs/
│   ├── Characters/
│   │   ├── Number1.prefab
│   │   ├── Cat.prefab
│   │   ├── Dog.prefab
│   │   └── Fox.prefab
│   ├── Powerups/
│   │   ├── Magnet.prefab
│   │   ├── Slowdown.prefab
│   │   ├── Coin.prefab
│   │   └── Shield.prefab
│   └── Obstacles/
│       ├── Mine.prefab
│       └── Pipe.prefab
├── Scripts/
│   ├── Animation/          # (Controllers & runtime will use these)
│   ├── Gameplay/
│   ├── Managers/
│   ├── UI/
│   ├── Scene/
│   └── Editor/
└── Scenes/
    ├── Start.unity
    ├── Game.unity
    └── Demo.unity
```

---

## Animation System (Copilot must create these assets)

### Scriptable Profile (for tuning)
- Create `DefaultFlapProfile.asset` at `Assets/ScriptableObjects/`.
- Properties and defaults:
  - `restAngle = 0`
  - `minAngle = -12` (loop lower bound)
  - `maxAngle = 12` (loop upper bound)
  - `flapFrequency = 6` (Hz for looping items)
  - `flapImpulseTime = 0.18` (total in+out for player flaps)
  - `feetKickMultiplier = 1`

### Animation Clips (exact names/paths)
Create clips with appropriate keyframes and easing:
- `Assets/Animations/Wings/Wing_Flap_In.anim`
  - Rotate Z: `rest → -35°` in **0.08s**, ease-out.
- `Assets/Animations/Wings/Wing_Flap_Out.anim`
  - Rotate Z: `-35° → rest` in **0.10s**, ease-in.
- `Assets/Animations/Wings/Wing_Looping.anim`
  - Rotate Z oscillation between `minAngle..maxAngle` continuously. Speed driven by a parameter (see below).
- `Assets/Animations/Feet/Feet_Kick.anim`
  - Small Z rotation ±6° **or** small Y bob (±0.02) for **0.18s** total. Ease in/out.

### Animator Controllers (exact names/paths)
- `Assets/Controllers/WingAnimator.controller`
  - Parameters:
    - `Flap` (Trigger)
    - `FlapSpeed` (Float)
    - `Looping` (Bool)
  - States:
    - If `Looping == true`: stay in **Wing_Looping** (loop).
    - Else: `Idle` → (on `Flap`) **Wing_Flap_In** → **Wing_Flap_Out** → `Idle`.
- `Assets/Controllers/FeetAnimator.controller`
  - Parameters:
    - `Kick` (Trigger)
  - States:
    - `Idle` → (on `Kick`) **Feet_Kick** → `Idle`.

### Animation Rules
- **Characters (Number1, Cat, Dog, Fox)**:
  - **Wings** flap **when player flaps** (impulse); optional low-rate idle flutter when not flapping.
  - **Feet** do a short kick **synchronized** with wing flap.
- **Power-ups (Magnet, Slowdown, Coin, Shield)** and **Mine**:
  - **Wings** flap in **loop** mode at constant frequency (use `Looping = true` and `FlapSpeed = flapFrequency`).
- Slight left/right desync: offset wings by ~0.01–0.03s for organic feel (randomized is fine).

---

## Prefab Specifications (Copilot must build)

### Common Pattern for Winged Prefabs
Each winged prefab must have these **children** (empty GameObjects with SpriteRenderers):
```
RootPrefab
├── Body (SpriteRenderer)   # main sprite from Assets/Sprites
├── LeftWing (SpriteRenderer + Animator: WingAnimator)
└── RightWing (SpriteRenderer + Animator: WingAnimator)
```
- Place **LeftWing** and **RightWing** pivots near wing hinges (shoulder area).
- Sorting: wing SpriteRenderers slightly above Body (e.g., sortingOrder +1).

### Characters (Prefabs in `Prefabs/Characters/`)
- **Number1.prefab**, **Cat.prefab**, **Dog.prefab**, **Fox.prefab**
- Children: **LeftWing**, **RightWing**, plus **LeftFoot**, **RightFoot** (feet use **FeetAnimator**).
- Assign `WingAnimator.controller` to wings; `FeetAnimator.controller` to feet.
- Add a component (your naming) to:
  - **Trigger `Flap`** on both wing animators when the player flaps.
  - **Trigger `Kick`** on both feet simultaneously.
  - Optionally set low-rate idle wing flutter.

### Power-ups (Prefabs in `Prefabs/Powerups/`)
- **Magnet**, **Slowdown**, **Coin**, **Shield**
- Children: **LeftWing**, **RightWing** (WingAnimator).
- Set `Looping = true` and set `FlapSpeed` from profile (around 6 Hz).
- Motion: gentle float/random drift + leftward travel if spawned in-game.

### Obstacles (Prefabs in `Prefabs/Obstacles/`)
- **Mine**: winged, up/down bobbing + leftward movement. **Looping** wing flap.
- **Pipe**: static sprite (no wings), moves leftward; get destroyed off-screen.

---

## Scenes & Hierarchy (Copilot must create scenes and place objects)

### Start Scene — `Assets/Scenes/Start.unity`
Hierarchy:
```
Main Camera (Orthographic, size ~5, sky-blue background)
Canvas (Screen Space - Overlay)
├── StartScreen (Panel)
│   ├── Title
│   ├── HighestScoreText
│   ├── CharacterCarousel
│   │   ├── PrevButton
│   │   ├── CharacterPreview (Image)
│   │   └── NextButton
│   ├── PlayButton
│   ├── ShopButton
│   └── CoinsCounter
└── ShopPanel (Panel, initially inactive)
    ├── ShopTitle
    ├── BuyShieldButton (cost label)
    ├── BuyMagnetButton (optional)
    ├── BuySlowdownButton (optional)
    └── CloseButton
EventSystem
```
Behavior:
- HighestScoreText reads from persistence.
- CharacterCarousel cycles between Number1/Cat/Dog/Fox and persists selection.
- PlayButton loads **Game.unity** (or hides Start UI and starts the run).
- ShopButton toggles **ShopPanel**.
- CoinsCounter shows persistent coin total.

### Game Scene — `Assets/Scenes/Game.unity`
Hierarchy:
```
Main Camera (Orthographic)
ParallaxBackground (optional layers)
GameRoot
├── Spawners
│   ├── PipeSpawner
│   ├── BubbleSpawner
│   ├── PowerupSpawner
│   └── MineSpawner
├── PlayerSpawn (Transform at ~(-2, 0))
└── Managers
    ├── GameManager
    ├── LevelManager
    ├── UIManager
    └── SaveSystem (or singleton/utility)
Canvas (Screen Space - Overlay)
├── HUDPanel
│   ├── ScoreText
│   ├── CoinsText
│   └── ShieldIndicator
└── GameOverPanel (inactive)
    ├── ScoreText
    ├── HighScoreText
    ├── TotalPowerupsText
    ├── RestartButton
    └── HomeButton
EventSystem
```
Behavior:
- On scene start, instantiate **selected character** at `PlayerSpawn`.
- Hook player flap input → trigger wings/feet animation impulses.
- Spawners create pipes, bubbles, mines, and power-ups with speeds/difficulty from **LevelManager**.
- Off-screen cleanup for all movers.
- HUD shows live Score & Coins.
- On death: show GameOverPanel with **score, high score, total power-ups earned**. Buttons restart or go home.

### Demo Scene — `Assets/Scenes/Demo.unity`
- Simple playground showing one character plus a few winged power-ups and a mine with looping wings, for quick visual checks.

---

## Gameplay Rules & Systems

### Core Player
- Tap/click/space → add upward velocity (Flappy-style) and **impulse wing+feet animation**.
- Tilting/rotation reacts to velocity.

### Operator Bubbles
- Move left; oscillate vertically (sine).
- Show label (`+1`, `-1`, `x0`, `/2`, etc.) over bubble.
- On collision:
  - Apply operation to current player number/value.
  - If `x0` and **no active shield** → **Game Over**.
- Despawn off-screen.

### Obstacles
- Pipe: fixed; gaps vary; despawn off-screen.
- Mine: leftward + vertical bobbing; **looping wing flap**; collision ends game unless shielded.

### Shields & Coins
- Shields: floaty pickup. On collect, gain **1-hit protection** against pipe/mine/`x0`.
- Coins: floaty pickup; **3 coins = 1 shield** (configurable).
- Persist **coin total** and **shield count** globally.

### Power-Ups
- **Magnet**: pulls nearby coins/shields towards player.
- **Slow Motion**: slows time or obstacle speed briefly.
- **Shrink** (optional): smaller collider.
- **Invincibility** (optional): brief immunity.
- **All winged power-ups loop wings** constantly.

### Leveling / Difficulty
- Difficulty increases with score/time:
  - Increase game speed and spawn rates.
  - Increase frequency of `x0` bubbles.
  - Decrease frequency of shields/coins.

---

## UI Flow
- **Start Screen** (default): Highest score, character select, Play, Shop, Coins counter.
- **Shop**: buy power-ups with coins (e.g., Shield cost 3 coins). Update totals and persist.
- **HUD**: Score, Coins, Shield status while playing.
- **Game Over Panel**: Score, Highest Score (update/persist), Total Power-Ups Earned (session + lifetime), Restart, Home.

---

## Persistence (PlayerPrefs/JSON keys)
- `Coins` (int)
- `Shields` (int)
- `HighScore` (int)
- `TotalPowerups` (int)
- `SelectedCharacterId` (int) — 0:Number1, 1:Cat, 2:Dog, 3:Fox

---

## Ads & IAP (stubs)
- IAP products: **5 shields ($0.05)**, **Coin Doubler**, **Remove Ads**.
- Ads:
  - **Interstitial** every 3–4 restarts.
  - **Banner** at top or bottom.
  - **Rewarded**: revive or add 1 shield.
- Keep these **stubbed** and modular (no vendor lock-in now).

---

## Animation Acceptance Criteria
- Wings flap:
  - **Impulse** on player flap for characters.
  - **Looping** at constant frequency for power-ups and mine.
- Feet kick exactly during player’s wing impulse.
- Slight L/R desync for natural feel.
- Animator parameter names **must match**:
  - Wings: `Flap` (Trigger), `Looping` (Bool), `FlapSpeed` (Float)
  - Feet: `Kick` (Trigger)
- All Animator Controllers & Clips created at the **exact paths** above.

---

## Scene & Hierarchy Acceptance Criteria
- **Start.unity** contains: StartScreen UI (with HighestScoreText, CharacterCarousel, Play, Shop), ShopPanel UI (inactive default), and CoinsCounter. Buttons wired.
- **Game.unity** contains: full hierarchy (Managers, Spawners, PlayerSpawn, HUD, GameOverPanel). Selected character instantiates at runtime.
- **Demo.unity** contains: a quick test layout with one character and looping winged items.
- All prefabs in `Assets/Prefabs/...` created and assigned sprites from `Assets/Sprites`.
- Off-screen cleanup works for movers (bubbles, pipes, mines, power-ups).
- Data persists (coins, shields, high score, selected character).

---

## Developer Guidelines
- Strict **Modular Component Pattern (MCP)**:
  - Gameplay logic, animation control, UI management separated.
  - Use serialized fields for Inspector tuning.
  - Use events/delegates for inter-system messaging (no hard coupling).
- No magic numbers in code; read tunables from the profile or Inspector.
- Keep the Inspector wiring clear and consistent across prefabs.

---

## Build Order (Copilot must follow)
1. Create folders and import sprites.
2. Create animation clips and animator controllers (names/paths above).
3. Create the **DefaultFlapProfile.asset** with default values.
4. Build prefabs: Characters, Power-ups, Obstacles (children, animators, references).
5. Build **Start.unity** (full UI + wiring).
6. Build **Game.unity** (full hierarchy, spawners, managers, HUD, GameOver).
7. Build **Demo.unity** (quick inspection scene).
8. Verify animation behavior, UI flow, and persistence using the acceptance criteria.

---

**Implement exactly as written.**
