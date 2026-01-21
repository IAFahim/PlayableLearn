# Day 12: The Clip Player

## Overview
Day 12 introduces **AnimationClipPlayable** - a wrapper that plays actual animation clips (AnimationClip assets) in a PlayableGraph. This is a fundamental building block for animation playback in Unity's Playables system.

## Key Concepts

### AnimationClipPlayable
- **AnimationClipPlayable**: Wraps an AnimationClip for playback in a PlayableGraph
- Plays actual animation data from AnimationClip assets
- Provides time control (play, pause, seek)
- Supports duration queries and time wrapping
- Can be connected to outputs (AnimationPlayableOutput, ScriptPlayableOutput)

### The Clip Player Pattern
The Clip Player is how you play animations in the Playables system:
1. Create an AnimationClipPlayable with an AnimationClip
2. Connect it to an output (AnimationPlayableOutput)
3. The output drives an Animator component
4. The animation plays on the animated object

## Implementation

### Files Structure
```
Day012/
├── Scripts/
│   ├── Day12.asmdef                    # Assembly definition
│   ├── ClipPlayableOps.cs              # Layer B: Burst-compiled operations
│   ├── Day12ClipHandle.cs              # Layer A: Pure data struct
│   ├── Day12ClipExtensions.cs          # Layer C: Extension methods
│   └── Day12Entry.cs                   # MonoBehaviour demo
└── Tests/
    ├── Day12Tests.asmdef               # Test assembly definition
    └── Day12Tests.cs                   # Unit tests
```

### Layer A: Data
`Day12ClipHandle` - Pure data struct containing:
- `PlayableGraph Graph` - Owner graph
- `Playable Playable` - AnimationClipPlayable wrapper
- `AnimationClip Clip` - The animation clip being played
- `bool IsActive` - Active state flag
- `int ClipId` - Debug identifier

### Layer B: Operations
`ClipPlayableOps` - Burst-compiled static methods:
- `Create()` - Creates AnimationClipPlayable from clip
- `Destroy()` - Destroys the playable
- `IsValid()` - Checks playable validity
- `GetTime()` / `SetTime()` - Time management
- `GetDuration()` - Gets clip duration
- `GetClipInfo()` - Combined time, duration, and validity check
- `SetPlayState()` / `GetPlayState()` - Play state control
- `IsPlaying()` - Checks if currently playing

### Layer C: Extensions
`Day12ClipExtensions` - Public API methods:
- `Initialize()` - Sets up AnimationClipPlayable with clip
- `Dispose()` - Cleanup and resource release
- `LogToConsole()` - Debug output
- `TryGetTime()` / `SetTime()` - Time management
- `TryGetDuration()` - Duration query
- `SetPlayState()` / `IsPlaying()` - Playback control
- `TryGetClip()` - Retrieves the AnimationClip
- `TryGetPlayable()` - Gets Playable for connections
- `ConnectToOutput()` - Connects to an output
- `AddInput()` - Adds input for blending/mixing

## Usage Example

```csharp
// Create the graph
var disposableGraph = new Day10DisposableGraph();
disposableGraph.Initialize("MyAnimationGraph");

// Get an AnimationClip (assigned in Inspector or loaded from Resources)
AnimationClip clip = myAnimationClip;

// Create AnimationClipPlayable
var clipHandle = new Day12ClipHandle();
clipHandle.Initialize(in disposableGraph.Graph, clip);

// Create AnimationPlayableOutput (requires Animator)
Animator animator = GetComponent<Animator>();
var animOutput = new Day11AnimOutputHandle();
animOutput.Initialize(in disposableGraph.Graph, "MyOutput", animator);

// Connect the clip to the output
if (animOutput.TryGetPlayableOutput(out PlayableOutput output))
{
    clipHandle.ConnectToOutput(in output);
}

// Control playback
clipHandle.SetPlayState(true); // Play
clipHandle.SetTime(0.5); // Seek to 0.5 seconds
clipHandle.SetPlayState(false); // Pause
```

## Key Differences from Previous Days

### Day 3-11 (Custom Playables)
- Custom behavior playables (rotators, speed control, etc.)
- Procedural animation generation
- Custom data and behavior

### Day 12 (AnimationClipPlayable)
- Plays actual AnimationClip assets
- Traditional animation playback
- Time-based animation data
- Duration and time management
- The "standard" way to play animations in Playables

## Testing

Day 12 includes comprehensive tests covering:
- Clip playable creation and initialization
- Time management (get/set time)
- Duration queries
- Play state control
- Resource disposal
- Integration with outputs and previous days' features

## Important Notes

1. **AnimationClip Requirement**: AnimationClipPlayable MUST have an AnimationClip to work
2. **Time Management**: Clips have finite duration and need time wrapping/looping
3. **Output Connection**: Clips must be connected to outputs to be visible
4. **Play State**: Clips can be played/paused independently of graph evaluation
5. **Seeking**: Clips support random access (set time to any position)

## Building on Previous Days

Day 12 integrates all previous concepts:
- **Day 1**: Graph creation and management
- **Day 2**: Output concepts (ScriptPlayableOutput)
- **Day 11**: AnimationPlayableOutput (required for clip playback)
- **Day 6**: PlayState control (play/pause)
- **Day 10**: IDisposable pattern (resource cleanup)
- **Day 3-5, 7-9**: Speed control, reverse time, visualization

The Clip Player is the bridge between the Playables system and traditional animation assets.

## Next Steps

With Day 12, you now have a complete understanding of:
1. How to create PlayableGraphs (Day 1)
2. How to add outputs (Day 2, Day 11)
3. How to create custom behavior playables (Day 3-10)
4. How to play actual animation clips (Day 12)
5. How to manage resources properly (Day 10)

This completes the foundation for more advanced concepts like animation blending, state machines, and mixer playables!
