# AV.Traject Timeline Integration

High-performance trajectory system for Unity Timeline with Data-Oriented Design (DOD) architecture.

## Features

- **Timeline Integration**: Use trajectory assets directly in Unity Timeline
- **DOD Architecture**: 4-layer architecture (Data, Logic, Extensions, Bridge)
- **Multiple Trajectory Types**: Linear, Arc, and Helix trajectories included
- **Blending Support**: Full Timeline clip blending support
- **Performance**: Built on Unity.Mathematics for optimal performance

## Installation

This package is installed locally in your project at:
```
Packages/com.iafahim.traject/
```

## Dependencies

- Unity 2022.3+
- Unity Timeline 1.7.0+
- Unity Mathematics 1.2.0+

## Quick Start

### 1. Create a Trajectory Asset

Right-click in Project window:
```
Create > AV > Traject > Linear
```

### 2. Create a Timeline

1. Create a Timeline asset (if you don't have one)
2. Select your GameObject with a Transform
3. In Timeline, add a **Traject Timeline Track**
4. Drag your Trajectory Asset onto the track to create a clip

### 3. Configure the Clip

Select the clip in Timeline and configure:
- **Asset**: The trajectory asset to use
- **Range**: Distance of the trajectory
- **Use Cached Origin**: Whether to cache the start position

## Architecture

### Layer A: Pure Data
Location: `Runtime/Core/`
- `TrajectState` - Runtime state struct
- `TrajectBasis` - Coordinate space definition
- `TrajectTimer` - Simple timer implementation

### Layer B: Core Logic
Location: `Runtime/Shapes/*/`
- `LinearLogic` - Linear trajectory math
- `ArcLogic` - Arc trajectory math
- `HelixLogic` - Helix trajectory math

### Layer C: Extensions
Location: `Runtime/Extensions/`
- `TrajectExtensions` - Extension methods for state manipulation
- `TrajectStateExtensions` - State-specific operations

### Layer D: Timeline Bridge
Location: `Runtime/Timeline/`
- `TrajectTimelineTrack` - Timeline track asset
- `TrajectTimelineClip` - Timeline clip asset
- `TrajectTimelineBehaviour` - Playable behaviour
- `TrajectTimelineMixerBehaviour` - Mixer for blending

## Available Trajectories

### LinearTraject
Straight-line movement along forward direction.

**Create**: `Create > AV > Traject > Linear`

### ArcTraject
Curved trajectory with configurable height.

**Create**: `Create > AV > Traject > Arc`

### HelixTraject
Spiral trajectory around forward axis.

**Create**: `Create > AV > Traject > Helix`

## Scripting API

### Using Trajectory Assets

```csharp
using AV.Traject.Runtime.Core;
using AV.Traject.Runtime.Integration;

// Evaluate a trajectory at specific time
TrajectBasis basis = new TrajectBasis(
    transform.position,
    transform.forward,
    transform.right,
    transform.up
);

myTrajectoryAsset.Evaluate(in basis, range, t, out float3 position);
transform.position = position;
```

### Timeline Integration

```csharp
using AV.Traject.Runtime.Timeline;

// Creating tracks programmatically
var track = timelineAsset.CreateTrack<TrajectTimelineTrack>(binding);
var clip = track.CreateClip(cliAsset);
```

## Inspector Fields

### TrajectTimelineClip
- **Asset**: The trajectory asset to evaluate
- **Range**: Distance in world units
- **Use Cached Origin**: Cache start position (recommended for Timeline)

## Performance Considerations

1. **Use Cached Origin**: Enabled by default for Timeline clips
2. **Avoid Hot Reloading**: Trajectories use burst-compiled math
3. **Pooling**: The Authoring component supports object pooling via `ResetOrigin()`

## License

MIT License

## Author

IAFahim
