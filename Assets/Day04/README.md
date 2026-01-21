# Day 04: The Update Cycle

## Overview
Day 04 introduces the **Update Cycle** pattern using PlayableBehaviour's `PrepareFrame` method (the Playable equivalent of `ProcessFrame`). We implement a rotating cube that updates every frame through the Playable system.

## What You'll Learn
- How to use `PrepareFrame` for per-frame updates
- Implementing the Update Cycle pattern in Playables
- Integrating Burst-compiled logic with PlayableBehaviour
- Managing transform updates through the Playable graph

## Key Concepts

### PrepareFrame (ProcessFrame Equivalent)
Unity Playables use `PrepareFrame` instead of `ProcessFrame`:
- Called every frame while the playable is running
- Receives `FrameData` with deltaTime and weight information
- Perfect for per-frame logic like rotation, animation, etc.

### The Update Cycle Pattern
```
OnBehaviourPlay → PrepareFrame (every frame) → OnBehaviourPause → OnPlayableDestroy
```

## Architecture

### Layer A: Data (`Day04RotatorData`)
```csharp
- PlayableGraph Graph
- Playable Node
- bool IsActive
- int NodeId
- Transform TargetTransform
- float RotationSpeed
- Vector3 RotationAxis
```

### Layer B: Operations (`RotatorLogic`)
Burst-compiled pure logic operations:
- `CalculateRotation()` - Core rotation computation
- `NormalizeAxis()` - Ensure unit vector
- `Vector3ToFloat3()` / `Float3ToVector3()` - Type conversions
- `ClampRotationSpeed()` - Safety validation

### Layer C: Extensions (`Day04RotatorExtensions`)
High-level adapter methods:
- `Initialize()` - Create and configure rotator node
- `Dispose()` - Clean up resources
- `ConnectToOutput()` - Link to output
- `SetRotationSpeed()` / `SetRotationAxis()` - Runtime updates
- `LogRotatorInfo()` - Debug information

## Files Structure
```
Day04/
├── Scripts/
│   ├── Day04.asmdef
│   ├── Day04RotatorData.cs          # Layer A: Data structure
│   ├── RotatorLogic.cs               # Layer B: Burst-compiled operations
│   ├── Day04RotatorBehaviour.cs      # PlayableBehaviour with PrepareFrame
│   ├── Day04RotatorExtensions.cs     # Layer C: Extension methods
│   └── Day04Entry.cs                 # MonoBehaviour entry point
├── Tests/
│   └── Day04Tests.cs                 # NUnit test suite
└── README.md
```

## Usage

### Basic Setup
1. Create a GameObject in your scene (e.g., a Cube)
2. Add the `Day04Entry` component
3. Configure rotation settings in Inspector:
   - **Rotation Speed**: Degrees per second (default: 90°/s)
   - **Rotation Axis**: Direction to rotate (default: Y-up)

### Runtime Control
```csharp
// Change rotation speed at runtime
rotatorData.SetRotationSpeed(180.0f);

// Change rotation axis at runtime
rotatorData.SetRotationAxis(Vector3.right);

// Change target transform
rotatorData.SetTargetTransform(otherTransform);
```

## How It Works

1. **OnEnable**: Creates the PlayableGraph, Output, and Rotator node
2. **Link**: Connects the rotator to the output
3. **OnBehaviourPlay**: Initializes the rotation state
4. **PrepareFrame** (every frame):
   - Checks if target exists and weight > 0
   - Calculates new rotation using Burst logic
   - Applies rotation to target transform
5. **OnDisable**: Cleans up all resources

## Key Differences from Previous Days

| Day 03 | Day 04 |
|--------|--------|
| Empty behaviour | Active PrepareFrame implementation |
| Static node | Per-frame updates |
| No side effects | Rotates a transform |
| Demonstrates structure | Demonstrates the Update Cycle |

## Testing
Run the test suite to verify:
- Rotator initialization
- Rotation calculations
- Runtime parameter updates
- Resource cleanup
- Complete flow (graph → output → rotator)

## Dependencies
- Day01: GraphHandle (PlayableGraph management)
- Day02: OutputHandle (PlayableOutput management)
- Day03: NodeHandle and ScriptPlayableOps (ScriptPlayable creation)
- Unity.Burst: For high-performance rotation calculations
- Unity.Mathematics: For float3 and quaternion operations

## Performance Notes
- All rotation calculations are Burst-compiled
- Minimal overhead in PrepareFrame
- No allocations per frame
- Suitable for production use

## Next Steps
- Day 05 will build on this to introduce...
- Try rotating multiple objects with different speeds
- Experiment with different rotation axes
- Modify the rotation logic to use quaternions directly
