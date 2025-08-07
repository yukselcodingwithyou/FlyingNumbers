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