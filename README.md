# FlyingNumbers
A Flappy Bird style math game.

## Scripts
- `PlayerController.cs` handles upward flapping, gravity, wing animation, operator collisions, and pipe collision for game over. It uses the Unity **Input System** through an `InputActionReference`.
- `Operator.cs` is added to operator objects (`+1`, `-2`, `*3`, etc.) to define the mathematical operation applied when the player collides with them.
- `Spawner.cs` periodically spawns pipe obstacles with a configurable gap and can
  randomly place operator-number prefabs inside that gap.

### Attaching scripts
1. Import the **Input System** package and create an Input Actions asset with a `Flap` action bound to the space bar and primary press (mouse or touch).
2. Add the `PlayerController` component to your player sprite and assign the `flapAction` field with the `Flap` action from the asset. Ensure the sprite also has `Rigidbody2D` and `Animator` components with a `Flap` trigger.
3. For operator sprites (e.g. `+1`), add the `Operator` component and set the type and value in the Inspector.
4. Pipe colliders should have the `Pipe` tag so that the player triggers game over on contact.
5. Create an empty GameObject positioned off screen to the right and add the
   `Spawner` component. Assign a pipe prefab and optional operator prefabs and
   configure the spawn settings in the Inspector.
