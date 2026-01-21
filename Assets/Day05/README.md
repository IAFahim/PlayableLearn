# Day 05: Time Dilation

## Overview
Day 05 demonstrates **Time Dilation** by manipulating `SetSpeed()` on a Playable. This allows you to speed up, slow down, pause, or even reverse the playback of any Playable node.

## Key Concepts

### What is Time Dilation?
Time dilation is the ability to control the flow of time in your PlayableGraph. By using `SetSpeed()`, you can:
- **Speed up** playback (speed > 1.0) for fast-forward effects
- **Slow down** playback (0 < speed < 1.0) for slow-motion effects
- **Pause** playback (speed ≈ 0) to freeze time
- **Reverse** playback (speed < 0) for rewind effects

### How SetSpeed Works
```csharp
// SetSpeed takes a double value where:
// 1.0 = normal speed (default)
// 2.0 = double speed (2x fast forward)
// 0.5 = half speed (slow motion)
// 0.0 = paused
// -1.0 = normal speed in reverse
playable.SetSpeed(speedFactor);
```

## Architecture

### Layer A: Data (Day05SpeedData.cs)
Pure data structure containing:
- `SpeedMultiplier`: Current speed multiplier
- `TargetSpeed`: Target speed for smooth transitions
- `EnableTimeDilation`: Whether time dilation is active
- `InterpolationSpeed`: How fast to transition between speeds

### Layer B: Operations (SpeedOps.cs)
Burst-compiled operations for speed manipulation:
- `CalculateInterpolatedSpeed()`: Smooth speed transitions
- `ClampSpeed()`: Keep speed within valid bounds
- `IsValidSpeed()`: Validate speed values
- `CalculateEffectiveDeltaTime()`: Apply time scaling to delta time
- State query methods: `IsPaused()`, `IsReversed()`, `IsFastForward()`, `IsSlowMotion()`

### Layer C: Extensions (Day05SpeedExtensions.cs)
High-level API methods:
- `Initialize()`: Create a speed control node
- `SetTargetSpeed()`: Set target speed for smooth transition
- `SetSpeedImmediate()`: Set speed without interpolation
- `SetTimeDilationEnabled()`: Enable/disable time dilation
- State query methods: `IsPaused()`, `IsReversed()`, etc.

### PlayableBehaviour (Day05SpeedBehaviour.cs)
Implements the time dilation logic in `PrepareFrame()`:
1. Checks if time dilation is enabled
2. Interpolates towards target speed
3. Validates and normalizes speed
4. Applies speed using `SetSpeed()`

## Usage Example

```csharp
// Create a speed control node
Day05SpeedData speedData;
speedData.Initialize(in graph, "SpeedControl", 2.0f, true, 2.0f);

// Connect it to your existing nodes
// Output -> SpeedControl -> YourPlayable

// Change speed smoothly at runtime
speedData.SetTargetSpeed(0.5f); // Slow motion

// Or change immediately
speedData.SetSpeedImmediate(0.0f); // Pause

// Disable time dilation to restore normal speed
speedData.SetTimeDilationEnabled(false);
```

## Node Connection Pattern

```
PlayableOutput
    ↓
Day05SpeedControl (manipulates SetSpeed)
    ↓
Day04Rotator (rotates transform)
```

The speed control node acts as a wrapper around any other playable, allowing you to control its playback speed without modifying the original playable.

## Inspector Controls

When using `Day05Entry.cs`, you can control time dilation in the Inspector:

- **Initial Speed Multiplier**: Starting speed (default: 1.0)
- **Enable Time Dilation**: Toggle time dilation on/off
- **Interpolation Speed**: How fast speed changes occur (default: 2.0)
- **Target Speed**: Runtime speed control (0-5 range in inspector)

## Visual Feedback

- The cube pulses based on current speed
- Editor gizmos show current speed multiplier
- Speed states are color-coded in editor:
  - Green: Normal speed (≈1.0x)
  - Red: Fast forward (>1.0x)
  - Blue: Slow motion (0-1.0x)
  - Gray: Paused (≈0x)

## Testing

Run the tests in `Day05Tests.cs` to verify:
- Speed control initialization
- Speed interpolation and clamping
- State detection (paused, reversed, fast-forward, slow-motion)
- Node connection and cleanup
- Edge cases (invalid speeds, disabled time dilation)

## Key Learnings

1. **SetSpeed affects the entire playable graph below it**: All children of a speed-controlled playable inherit the speed modification.

2. **Smooth transitions make for better UX**: Use interpolation rather than instant speed changes for more natural-feeling time effects.

3. **Validation is important**: Always validate speed values to prevent NaN, infinity, or extreme values from breaking your simulation.

4. **Time dilation is composable**: You can chain multiple speed controls for complex effects (e.g., global slow motion with local fast-forward).

## Next Steps

- Experiment with different speed values
- Try chaining multiple speed controls
- Combine with other Playable features (mixing, blending, etc.)
- Implement user-controlled time dilation (e.g., bullet time)

## Files

- `Day05.asmdef`: Assembly definition
- `Day05SpeedData.cs`: Data structure
- `SpeedOps.cs`: Burst-compiled operations
- `Day05SpeedBehaviour.cs`: PlayableBehaviour implementation
- `Day05SpeedExtensions.cs`: Extension methods
- `Day05Entry.cs`: MonoBehaviour entry point
- `Day05Tests.cs`: NUnit tests
- `README.md`: This file
