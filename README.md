# 🔄 Rotato

A 2D casual puzzle-platformer built in Unity, where the core mechanic is rotating gravity itself to navigate each level.

**🎮 [Play on itch.io](https://triph204.itch.io/rotato)** · **📺 [Watch Demo](https://youtu.be/1x2CiXAKpQc)**

---

## About

Rotato is built around a single idea: gravity isn't fixed. The player can rotate gravity 90° at a time (Q/E) to walk on walls and ceilings, using momentum and timing to navigate obstacle-filled levels.

## Features

- **Gravity-rotation mechanic** — rotate the direction of gravity in real time to change which surface is "down"
- **Physics-based movement** — real `Rigidbody2D` forces and velocity, not scripted animation, so momentum carries correctly across rotations
- **Ground & fall detection** with audio feedback (footsteps, landing, rotation sound)
- **Obstacle & trap variety** — moving traps, rotating hazards, pass-through doors
- **Level select & progression** system
- **Responsive WebGL UI** — playable directly in browser

## Tech Highlights

The most interesting piece of engineering in this project is how gravity rotation is implemented **without branching logic for each of the 4 directions**:

```csharp
private static readonly Vector2[] GravVectors = { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
private static readonly float[] GravAngles = { 0f, -90f, 180f, 90f };

_gravDir = ((_gravDir + dir) % 4 + 4) % 4; // wraps around like a compass
```

Instead of `if/else` per direction, gravity direction is stored as an index into a lookup table, and rotation is just index arithmetic (with modulo wraparound). Force and velocity are then computed once using `Vector2.Dot` projections onto the current gravity/right axis — meaning the same movement code works correctly for all four orientations.

| Concept | Where it's used |
|---|---|
| **Lookup tables over conditionals** | `GravVectors[]` / `GravAngles[]` indexed by `_gravDir` |
| **Vector projection (`Dot` product)** | Computing horizontal velocity relative to the current gravity axis |
| **Custom gravity via `AddForce`** | Built-in Unity gravity is disabled (`gravityScale = 0`); gravity is simulated manually to support 4 directions |
| **Input System package** | `PlayerMovement.OnMove` / `OnJump` callbacks bound via Unity's new Input System |
| **Coroutine-based respawn** | `RespawnRoutine()` delays and resets player state after death |

## Tech Stack

- **Engine:** Unity 6000.5.2f1
- **Language:** C#
- **Physics:** Rigidbody2D (Dynamic)
- **Input:** Unity Input System package
- **Target platforms:** WebGL

## Project Structure

```
Assets/Scripts/
├── PlayerMovement.cs   # Core gravity-rotation & physics movement
├── PlayerColision.cs   # Collision/death handling
├── TrapBall.cs          # Waypoint-based moving hazard
├── CircleTrap.cs        # Rotating hazard
├── DoorOpen.cs / DoorPass.cs
├── Level/                # Level select buttons
└── Gamemanager.cs        # Scene transitions
```

## How to Run

1. Clone the repository
2. Open with **Unity 6000.5.2f1** (or later)
3. Ensure the **Input System** package is installed (Package Manager)
4. Open the main scene and press Play

## Links

- 🌐 WebGL: https://triph204.itch.io/rotato
- 🎥 Demo video: https://youtu.be/1x2CiXAKpQc

## Author

**Hoàng Hữu Hậu** — [github.com/triph204](https://github.com/triph204)
