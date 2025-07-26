# FlyingNumbers
A Flappy Bird style math game.

## Goal
Fly as far as possible while building the highest number you can. Passing pipes adds to your score and colliding with math operator objects changes your current number.

## Mechanics and Controls
- **Flap:** press the space bar, left mouse button or tap the screen to move upward.
- **Operators:** touch `+`, `-`, `*` or `/` prefabs to modify your number and gain a point.
- **Pipes:** hitting a pipe ends the run.

## Getting Started
### Requirements
- Unity **2021.3.0f1** or newer.
- The project references required packages (including the **Input System**) in
  `Packages/manifest.json`, so Unity will automatically install them when you
  open the project.

### Opening and Running
1. Launch Unity Hub and add this repository's folder as a project.
2. Open the project using Unity 2021.3.0f1.
3. Load `Assets/Scenes/SampleScene.unity`.
4. Press **Play** to start the game.

## Scripts
- `PlayerController.cs` handles upward flapping, gravity, wing animation, operator collisions, and pipe collision for game over. It uses the Unity **Input System** through an `InputActionReference`.
- `Operator.cs` is added to operator objects (`+1`, `-2`, `*3`, etc.) to define the mathematical operation applied when the player collides with them.
- `Spawner.cs` periodically spawns pipe obstacles with a configurable gap and can
  randomly place operator-number prefabs inside that gap.
- `ScoreManager.cs` keeps track of the player's score and high score, displaying
  them on the UI and exposing a restart method for the game over menu.
- `Pipe.cs` awards a point when the player successfully passes a pipe.

### Attaching scripts
1. Create an Input Actions asset with a `Flap` action bound to the space bar and primary press (mouse or touch). Unity will prompt to enable the **Input System** if it is not already active.
2. Add the `PlayerController` component to your player sprite and assign the `flapAction` field with the `Flap` action from the asset. Ensure the sprite also has `Rigidbody2D` and `Animator` components with a `Flap` trigger.
3. For operator sprites (e.g. `+1`), add the `Operator` component and set the type and value in the Inspector.
4. Pipe colliders should have the `Pipe` tag so that the player triggers game over on contact.
5. Create an empty GameObject positioned off screen to the right and add the
   `Spawner` component. Assign a pipe prefab and optional operator prefabs and
   configure the spawn settings in the Inspector.
