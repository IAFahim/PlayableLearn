# PlayableLearn - Day 06: The Pause Button

## Overview
Day 06 introduces PlayState control for programmatically playing and stopping the PlayableGraph. This demonstrates how to control the graph's execution state at runtime.

## What You'll Learn
- How to check and control PlayableGraph play state
- Understanding PlayState (Playing vs Paused)
- Playing and stopping graphs programmatically
- Detecting state transitions
- Auto-play functionality
- Visualizing play state

## Files Structure
```
Assets/Day06/Scripts/
├── Day06.asmdef                        # Assembly definition
├── Day06Entry.cs                       # MonoBehaviour entry point
├── Day06PlayStateData.cs               # Data layer: PlayState control handle
├── Day06PlayStateExtensions.cs         # Adapter layer: PlayState operations
└── PlayStateOps.cs                     # Operations layer: Burst-compiled play state ops
```

## The Three-Layer Architecture

### Layer A: Data (Day06PlayStateData)
Pure data structure with no logic:
- `PlayableGraph Graph` - The graph being controlled
- `bool IsActive` - Local state tracking
- `int ControllerId` - Debug identifier
- `PlayState CurrentState` - Current play state
- `PlayState PreviousState` - Previous play state (for change detection)
- `bool AutoPlayOnStart` - Whether to auto-play on initialization
- `bool IsGraphValid` - Whether the graph is valid

### Layer B: Operations (PlayStateOps)
Burst-compiled static methods for play state operations:
- `IsPlaying()` - Checks if state is Playing
- `IsPaused()` - Checks if state is Paused
- `DidStateChange()` - Detects state transitions
- `IsTransitionToPause()` - Checks if transitioning to paused
- `IsTransitionToPlay()` - Checks if transitioning to playing
- `GetToggledState()` - Toggles between play and pause
- `IsValidTransition()` - Validates state transitions
- `CalculateProgress()` - Returns progress value (1.0 for playing, 0.0 for paused)

### Layer C: Extensions (Day06PlayStateExtensions)
Public API that combines data and operations:
- `Initialize()` - Creates PlayState control
- `Dispose()` - Cleans up control
- `Play()` - Starts the graph
- `Pause()` - Stops the graph
- `TogglePlayPause()` - Toggles between play and pause
- `UpdateState()` - Updates current state from graph
- `IsPlaying()` - Checks if graph is playing
- `IsPaused()` - Checks if graph is paused
- `JustPaused()` - Detects pause transition
- `JustStartedPlaying()` - Detects play transition
- `GetStateString()` - Gets state as string
- `GetProgress()` - Gets progress for UI
- `LogStateInfo()` - Debug logging

## Key Concepts

### PlayState
The `PlayState` enum represents the execution state of a PlayableGraph:
- `PlayState.Playing` - Graph is actively evaluating
- `PlayState.Paused` - Graph is stopped/paused

### Graph Control Methods
Unity provides methods to control graph execution:
- `graph.Play()` - Starts or resumes graph evaluation
- `graph.Stop()` - Pauses graph evaluation
- `graph.GetGraphPlayState()` - Gets current play state

### State Transitions
Tracking state changes is important for synchronization:
- Playing → Paused: Graph was stopped
- Paused → Playing: Graph was started
- These transitions can be detected by comparing PreviousState and CurrentState

### Auto-Play
Graphs can be configured to automatically start playing:
- Useful for animations that should start immediately
- Can be controlled via `AutoPlayOnStart` parameter
- Default behavior is to auto-play

## Usage Example

```csharp
// Initialize graph
Day01GraphHandle graphHandle;
graphHandle.Initialize("MyGraph");

// Initialize PlayState control with auto-play
Day06PlayStateData playStateData;
playStateData.Initialize(in graphHandle.Graph, "MyPlayStateControl", autoPlayOnStart: true);

// Check if playing
if (playStateData.IsPlaying())
{
    Debug.Log("Graph is playing");
}

// Pause the graph
playStateData.Pause();

// Resume the graph
playStateData.Play();

// Toggle play/pause
playStateData.TogglePlayPause();

// Update state tracking (call in Update)
playStateData.UpdateState();

// Check for transitions
if (playStateData.JustPaused())
{
    Debug.Log("Graph just paused!");
}

if (playStateData.JustStartedPlaying())
{
    Debug.Log("Graph just started playing!");
}
```

## Visual Feedback
Day 06 adds visual feedback to represent play state:
- **Playing**: Green color
- **Paused**: Red color
- **GUI Controls**: On-screen Play/Pause button
- **Status Display**: Shows current state and speed

## Previous Days
- **Day 01**: Created and destroyed PlayableGraph
- **Day 02**: Added ScriptPlayableOutput for console communication
- **Day 03**: Created and linked the first ScriptPlayable node
- **Day 04**: Added the update cycle using ProcessFrame
- **Day 05**: Implemented SetSpeed manipulation for playback control

## Next Steps
- **Day 07**: Will add reverse time functionality with negative speed

## Testing
Run the Unity Test Runner to verify:
- PlayState control initializes correctly
- Auto-play works as expected
- Play/Pause/Toggle operations work correctly
- State transitions are detected properly
- Complete integration with previous days

## Notes
- Day 06 builds upon all previous days (01-05)
- PlayState control is independent of speed control (Day 05)
- The graph can be playing at any speed (including 0) while in Playing state
- Paused state (Stop()) is different from speed 0
- Use UpdateState() in your Update loop to track state changes
- State transitions are useful for triggering events when play state changes
