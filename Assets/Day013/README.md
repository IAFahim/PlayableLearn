# Day 13: The Mixer

## Overview
Day 13 introduces **AnimationMixerPlayable** - a powerful blending node that combines multiple animation inputs into a single output. This is the foundation for animation blending, transitions, and complex animation systems in Unity's Playables API.

## Key Concepts

### AnimationMixerPlayable
- **AnimationMixerPlayable**: Blends multiple animation inputs based on weights
- Each input port has a weight (0.0 to 1.0) controlling its influence
- Weights can be adjusted in real-time for smooth transitions
- Outputs a blended result combining all active inputs
- Supports any number of inputs (commonly 2 for simple blends)

### The Mixer Pattern
The Mixer is how you blend animations in the Playables system:
1. Create an AnimationMixerPlayable with N inputs
2. Connect multiple AnimationClipPlayables (or other playables) to the mixer
3. Set weights on each input to control blending
4. Connect the mixer to an output (AnimationPlayableOutput)
5. The output drives an Animator component with blended animation

### Weight Blending
- Weight 0.0 = Input has no influence
- Weight 1.0 = Input has full influence
- Multiple inputs can have non-zero weights simultaneously
- Weights can be normalized to sum to 1.0 (optional)
- Common pattern: 2-input blend where weight0 + weight1 = 1.0

## Implementation

### Files Structure
```
Day013/
├── Scripts/
│   ├── Day13.asmdef                    # Assembly definition
│   ├── MixerOps.cs                      # Layer B: Burst-compiled operations
│   ├── Day13MixerHandle.cs              # Layer A: Pure data struct
│   ├── Day13MixerExtensions.cs          # Layer C: Extension methods
│   └── Day13Entry.cs                    # MonoBehaviour demo
└── Tests/
    ├── Day13Tests.asmdef                # Test assembly definition
    └── Day13Tests.cs                    # Unit tests
```

### Layer A: Data
`Day13MixerHandle` - Pure data struct containing:
- `PlayableGraph Graph` - Owner graph
- `Playable Playable` - AnimationMixerPlayable wrapper
- `int InputCount` - Number of inputs the mixer can blend
- `bool IsActive` - Active state flag
- `int MixerId` - Debug identifier

### Layer B: Operations
`MixerOps` - Burst-compiled static methods:
- `Create()` - Creates AnimationMixerPlayable with specified input count
- `Destroy()` - Destroys the mixer playable
- `IsValid()` - Checks mixer validity
- `GetInputCount()` - Gets the number of inputs
- `SetInputWeight()` / `GetInputWeight()` - Weight management
- `ConnectInput()` / `DisconnectInput()` - Input connection management
- `GetInput()` - Gets the playable connected to an input port
- `IsInputConnected()` - Checks if an input port is connected
- `ClearAllWeights()` - Sets all weights to zero
- `NormalizeWeights()` - Normalizes weights so they sum to 1.0

### Layer C: Extensions
`Day13MixerExtensions` - Public API methods:
- `Initialize()` - Sets up AnimationMixerPlayable with input count
- `Dispose()` - Cleanup and resource release
- `LogToConsole()` - Debug output
- `TryGetInputCount()` - Gets number of inputs
- `SetInputWeight()` / `TryGetInputWeight()` - Weight management
- `ConnectInput()` / `DisconnectInput()` - Input connection management
- `IsInputConnected()` - Checks input connection status
- `TryGetPlayable()` - Gets Playable for connections
- `ConnectToOutput()` - Connects mixer to an output
- `ClearAllWeights()` - Clears all weights
- `NormalizeWeights()` - Normalizes all weights
- `SetBlend()` / `TryGetBlend()` - Convenience methods for 2-input blending

## Usage Example

```csharp
// Create the graph
var disposableGraph = new Day10DisposableGraph();
disposableGraph.Initialize("MyBlendGraph");

// Get two AnimationClips (assigned in Inspector or loaded from Resources)
AnimationClip idleClip = myIdleAnimationClip;
AnimationClip walkClip = myWalkAnimationClip;

// Create AnimationClipPlayables
var idleHandle = new Day12ClipHandle();
idleHandle.Initialize(in disposableGraph.Graph, idleClip);

var walkHandle = new Day12ClipHandle();
walkHandle.Initialize(in disposableGraph.Graph, walkClip);

// Create AnimationMixerPlayable with 2 inputs
var mixerHandle = new Day13MixerHandle();
mixerHandle.Initialize(in disposableGraph.Graph, 2);

// Connect clips to mixer inputs
mixerHandle.ConnectInput(ref idleHandle.GetPlayable(), 0, 1.0f);  // Input 0: full weight
mixerHandle.ConnectInput(ref walkHandle.GetPlayable(), 1, 0.0f);  // Input 1: no weight

// Create AnimationPlayableOutput (requires Animator)
Animator animator = GetComponent<Animator>();
var animOutput = new Day11AnimOutputHandle();
animOutput.Initialize(in disposableGraph.Graph, "MyOutput", animator);

// Connect the mixer to the output
if (animOutput.TryGetPlayableOutput(out PlayableOutput output))
{
    mixerHandle.ConnectToOutput(in output);
}

// Control blending
mixerHandle.SetBlend(0.0f);  // 100% idle, 0% walk
mixerHandle.SetBlend(0.5f);  // 50% idle, 50% walk
mixerHandle.SetBlend(1.0f);  // 0% idle, 100% walk
```

## Key Differences from Previous Days

### Day 12 (Single Clip)
- Plays one animation clip at a time
- No blending between animations
- Abrupt transitions between clips
- Simple playback control

### Day 13 (Mixer)
- Blends multiple animations simultaneously
- Smooth transitions between animations
- Weight-based blending control
- Complex animation systems foundation
- Requires multiple inputs (clips or other playables)

## Testing

Day 13 includes comprehensive tests covering:
- Mixer creation and initialization
- Input count management
- Weight management (get/set weights)
- Input connection and disconnection
- Blend operations (2-input convenience methods)
- Weight normalization
- Resource disposal
- Integration with clips and outputs

## Important Notes

1. **Input Count**: Mixer must be created with a fixed number of inputs (cannot be resized)
2. **Weight Range**: Weights are floats from 0.0 (no influence) to 1.0 (full influence)
3. **Input Connection**: Inputs must be connected before they can contribute to the blend
4. **Normalization**: Weights don't need to sum to 1.0, but it's common practice
5. **Performance**: Mixers are efficient but consider the number of inputs for performance
6. **Output Connection**: Mixer must be connected to an output to be visible
7. **Input Types**: Can blend any playable type, not just AnimationClipPlayables

## Building on Previous Days

Day 13 integrates all previous concepts:
- **Day 1**: Graph creation and management
- **Day 2**: Output concepts (ScriptPlayableOutput)
- **Day 11**: AnimationPlayableOutput (required for mixer playback)
- **Day 12**: AnimationClipPlayable (source animations for blending)
- **Day 6**: PlayState control (play/pause blended output)
- **Day 10**: IDisposable pattern (resource cleanup)
- **Day 3-5, 7-9**: Speed control, reverse time, visualization

The Mixer is the bridge between single animation playback and complex animation blending systems.

## Advanced Usage

### Multi-Input Blending
```csharp
// Create mixer with 4 inputs
var mixerHandle = new Day13MixerHandle();
mixerHandle.Initialize(in graph, 4);

// Connect 4 different animations
mixerHandle.ConnectInput(ref idlePlayable, 0, 0.7f);
mixerHandle.ConnectInput(ref walkPlayable, 1, 0.2f);
mixerHandle.ConnectInput(ref runPlayable, 2, 0.1f);
mixerHandle.ConnectInput(ref jumpPlayable, 3, 0.0f);
```

### Normalized Blending
```csharp
// Set weights without worrying about the sum
mixerHandle.SetInputWeight(0, 2.0f);
mixerHandle.SetInputWeight(1, 1.0f);

// Normalize so weights sum to 1.0
mixerHandle.NormalizeWeights();  // Results in 0.67 and 0.33
```

### Smooth Transitions
```csharp
// Gradually transition from idle to walk
void Update()
{
    float currentBlend;
    mixerHandle.TryGetBlend(out currentBlend);
    float newBlend = Mathf.MoveTowards(currentBlend, 1.0f, Time.deltaTime * blendSpeed);
    mixerHandle.SetBlend(newBlend);
}
```

## Next Steps

With Day 13, you now have a complete understanding of:
1. How to create PlayableGraphs (Day 1)
2. How to add outputs (Day 2, Day 11)
3. How to create custom behavior playables (Day 3-10)
4. How to play actual animation clips (Day 12)
5. **How to blend multiple animations together (Day 13)**
6. How to manage resources properly (Day 10)

This completes the foundation for advanced concepts like:
- Animation layering (additive mixing)
- State machines with blend trees
- Complex animation blending systems
- Procedural animation blending
- Animation compression and optimization
