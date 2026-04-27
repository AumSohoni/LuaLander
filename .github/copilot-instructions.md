<!-- UNITY CODE ASSIST INSTRUCTIONS START -->
- Project name: 2D-SpaceRocket
- Unity version: Unity 6000.2.10f1
- Active game object:
  - Name: Lander
  - Tag: Untagged
  - Layer: Default
<!-- UNITY CODE ASSIST INSTRUCTIONS END -->

## Build, test, and lint

### Build / compile scripts
```powershell
dotnet build .\2D-SpaceRocket.sln
```

### Run Unity tests (EditMode)
```powershell
"<UnityEditorPath>\Unity.exe" -batchmode -quit -projectPath "C:\Users\aum11\2D-SpaceRocket" -runTests -testPlatform EditMode -testResults "Logs\EditModeTests.xml" -logFile "Logs\EditModeTests.log"
```

### Run a single Unity test
```powershell
"<UnityEditorPath>\Unity.exe" -batchmode -quit -projectPath "C:\Users\aum11\2D-SpaceRocket" -runTests -testPlatform EditMode -testFilter "Namespace.ClassName.TestMethodName" -testResults "Logs\SingleTest.xml" -logFile "Logs\SingleTest.log"
```

### Lint
There is no dedicated linter configuration in this repository.

## High-level architecture

This is a single-scene Unity 2D lander game (`Assets/Scenes/SampleScene.unity`) centered on rigidbody physics, fuel usage, and landing evaluation.

- **Core gameplay (`Assets/Scripts/Lander.cs`)**: owns thrust/rotation input, fuel consumption, pickup handling, collision-based landing evaluation, and landing score calculation.
- **Game state (`Assets/Scripts/GameManager.cs`)**: global singleton that tracks elapsed time and total score, and listens to `Lander` events.
- **Presentation layer**:
  - `LanderVisual.cs` toggles thruster particle systems based on lander force events.
  - `StatsUI.cs` renders live score/time/velocity and fuel bar from `GameManager.Instance` + `Lander.Instance`.
  - `LandedUI.cs` shows end-of-landing results when `onLanded` fires.
  - `LandingPadVisual.cs` displays pad multiplier text from parent `LandingPad`.
- **World interactables**:
  - `LandingPad.cs` is a data component for score multiplier.
  - `CoinPickup.cs` and `FuelPickup.cs` are trigger pickup components destroyed by `Lander`.

## Key conventions in this codebase

- **Singleton-first access**: runtime systems are accessed via `Lander.Instance` and `GameManager.Instance` (no DI/service container pattern in use).
- **Event-driven cross-script communication**: `Lander` is the event hub (`onBeforeForce`, thrust events, `onCoinPickup`, `onLanded`), and visual/UI/manager scripts subscribe to these.
- **Score mutation through event args**: `GameManager` updates `LandedEventArgs.Score` inside the `onLanded` handler; downstream consumers read the mutated score.
- **Input pattern**: movement is polled directly with `Keyboard.current` in `FixedUpdate` instead of using generated `InputAction` callbacks.
- **Landing validation model**: outcomes are based on `collision.relativeVelocity.magnitude` and `Vector2.Dot(Vector2.up, transform.up)` against constants in `Lander`.
- **Fuel model**: `fuelAmount` is consumed per frame when thrust/rotation keys are pressed; fuel pickups can increase both current fuel and the normalization cap (`maxFuel`).