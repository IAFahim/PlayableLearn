# Day 14: Hard Swapping

## Overview
Day 14 introduces **Hard Swapping** - an immediate animation transition technique that disconnects one input and connects another without blending. Unlike the weighted blending from Day 13, hard swapping creates an instantaneous cut between animations.

## Key Concepts

### Hard Swapping vs. Blending
- **Hard Swapping (Day 14)**: Immediate, abrupt transition between animations
  - Disconnects input 0, connects input 1 (or vice versa)
  - No crossfade or blending period
  - One animation is fully replaced by another
  - Useful for instant state changes (attacks, reactions, teleport effects)

- **Blending (Day 13)**: Smooth, gradual transition between animations
  - Both animations play simultaneously with weighted influence
  - Smooth crossfade over time
  - Gradual transition from one state to another
  - Useful for locomotion, idle-to-walk transitions

### The Swap Pattern
The hard swap pattern works by:
1. Create a mixer with at least 2 input ports
2. Connect two different animation clips to the inputs
3. Disconnect one input and immediately connect the other
4. Set the weight of the newly connected input
5. The output instantly switches to the new animation

### Connection Management
- **Input 0**: Default/primary animation (e.g., Idle)
- **Input 1**: Secondary animation (e.g., Walk, Run, Attack)
- Swap operations manage which input is currently active
- Only one input should be connected at a time for true hard swaps

## Implementation

### Files Structure
```
Day14/
├── Scripts/
│   ├── Day14.asmdef                    # Assembly definition
│   ├── SwapConnectionOps.cs            # Layer B: Burst-compiled swap operations
│   ├── Day14SwapData.cs                # Layer A: Pure data struct
│   ├── Day14SwapExtensions.cs          # Layer C: Extension methods
│   └── Day14Entry.cs                   # MonoBehaviour demo
└── Tests/
    ├── Day14Tests.asmdef                # Test assembly definition
    └── Day14Tests.cs                    # Unit tests
```

### Layer A: Data
`Day14SwapData` - Pure data struct containing:
- `PlayableGraph Graph` - Owner graph
- `Playable MixerPlayable` - The mixer for swapping inputs
- `Playable Input0Playable` - First input (to disconnect)
- `Playable Input1Playable` - Second input (to connect)
- `bool IsUsingInput1` - Tracks which input is currently active
- `bool IsActive` - Active state flag
- `int SwapId` - Debug identifier
- `bool IsInput0Connected` - Connection state tracking
- `bool IsInput1Connected` - Connection state tracking

### Layer B: Operations
`SwapConnectionOps` - Burst-compiled static methods:
- `DisconnectInput0()` - Disconnects input 0 from mixer
- `DisconnectInput1()` - Disconnects input 1 from mixer
- `ConnectInput0()` - Connects input 0 to mixer with weight
- `ConnectInput1()` - Connects input 1 to mixer with weight
- `HardSwapToInput1()` - Disconnects input 0, connects input 1
- `HardSwapToInput0()` - Disconnects input 1, connects input 0
- `IsInput0Connected()` / `IsInput1Connected()` - Connection status checks
- `GetInput0Weight()` / `GetInput1Weight()` - Weight queries
- `SetInput0Weight()` / `SetInput1Weight()` - Weight control
- `HasValidInputPorts()` - Validates mixer has sufficient inputs

### Layer C: Extensions
`Day14SwapExtensions` - Public API methods:
- `Initialize()` - Sets up swap data with mixer and inputs
- `Dispose()` - Cleanup and resource release
- `SwapToInput1()` - Performs hard swap to input 1
- `SwapToInput0()` - Performs hard swap to input 0
- `ToggleInput()` - Switches between input 0 and input 1
- `TryGetInput0Weight()` / `TryGetInput1Weight()` - Weight queries
- `SetInput0Weight()` / `SetInput1Weight()` - Weight control
- `IsValidSwap()` - Checks swap data validity
- `IsUsingInput1()` - Returns current active input
- `IsInput0Connected()` / `IsInput1Connected()` - Connection status
- `LogToConsole()` - Debug output
- `DisconnectAll()` - Disconnects both inputs

## Usage Example

```csharp
// Create the graph
var disposableGraph = new Day10DisposableGraph();
disposableGraph.Initialize("MySwapGraph");

// Get two AnimationClips
AnimationClip idleClip = myIdleAnimationClip;
AnimationClip attackClip = myAttackAnimationClip;

// Create AnimationClipPlayables
var idleHandle = new Day12ClipHandle();
idleHandle.Initialize(in disposableGraph.Graph, idleClip);

var attackHandle = new Day12ClipHandle();
attackHandle.Initialize(in disposableGraph.Graph, attackClip);

// Create AnimationMixerPlayable with 2 inputs
var mixerHandle = new Day13MixerHandle();
mixerHandle.Initialize(in disposableGraph.Graph, 2);

// Get playables for swap operations
idleHandle.TryGetPlayable(out Playable input0);
attackHandle.TryGetPlayable(out Playable input1);
mixerHandle.TryGetPlayable(out Playable mixer);

// Initialize swap data
var swapData = new Day14SwapData();
swapData.Initialize(in disposableGraph.Graph, in mixer, in input0, in input1);

// Connect idle to input 0 (initial state)
mixerHandle.ConnectInput(ref input0, 0, 1.0f);

// Create output and connect mixer
Animator animator = GetComponent<Animator>();
var animOutput = new Day11AnimOutputHandle();
animOutput.Initialize(in disposableGraph.Graph, "MyOutput", animator);

if (animOutput.TryGetPlayableOutput(out PlayableOutput output))
{
    mixerHandle.ConnectToOutput(in output);
}

// Perform hard swap to attack animation
swapData.SwapToInput1(1.0f);  // Instant switch to attack

// Swap back to idle
swapData.SwapToInput0(1.0f);  // Instant switch to idle

// Or toggle between inputs
swapData.ToggleInput(1.0f);   // Switches to whichever input is not active
```

## Key Differences from Day 13

### Day 13 (Weighted Blending)
- Multiple animations play simultaneously
- Smooth transitions with weight manipulation
- Gradual crossfade between states
- Both inputs contribute to output
- Best for: Locomotion, directional blending, smooth state transitions

### Day 14 (Hard Swapping)
- Only one animation plays at a time
- Instantaneous transitions
- Abrupt cuts between states
- Only one input contributes to output
- Best for: Attacks, reactions, teleport effects, instant state changes

## Testing

Day 14 includes comprehensive tests covering:
- Swap data initialization and validation
- Hard swap operations (swap to input 0, swap to input 1)
- Input connection and disconnection
- Weight management for individual inputs
- Toggle operations between inputs
- Multiple sequential swaps
- Integration with mixers and clips
- Resource disposal

## Important Notes

1. **Immediate Transition**: Hard swaps are instant - no blending period
2. **Input Disconnection**: Swapping automatically disconnects the previous input
3. **Weight Control**: Swap operations include weight parameter (typically 1.0)
4. **Input Requirements**: Mixer must have at least 2 input ports for swapping
5. **State Tracking**: Swap data tracks which input is currently active
6. **Connection Status**: Both internal and actual connection states are tracked
7. **Validation**: All operations validate graph, mixer, and input playables

## Building on Previous Days

Day 14 integrates all previous concepts:
- **Day 1**: Graph creation and management
- **Day 2**: Output concepts (ScriptPlayableOutput)
- **Day 11**: AnimationPlayableOutput (required for animation playback)
- **Day 12**: AnimationClipPlayable (source animations for swapping)
- **Day 13**: AnimationMixerPlayable (provides input ports for swapping)
- **Day 6**: PlayState control (play/pause swapped animation)
- **Day 10**: IDisposable pattern (resource cleanup)
- **Day 3-5, 7-9**: Speed control, reverse time, visualization

Hard swapping extends the mixer system from Day 13 to support instant animation transitions.

## Advanced Usage

### Manual Swap Control
```csharp
// Full manual control over swap operations
void PerformAttack()
{
    // Disconnect idle, connect attack
    swapData.SwapToInput1(1.0f);

    // Attack animation duration
    StartCoroutine(ResetToIdleAfterDelay(attackClip.length));
}

IEnumerator ResetToIdleAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    swapData.SwapToInput0(1.0f);
}
```

### Weighted Hard Swap
```csharp
// Swap with partial weight (unusual but possible)
swapData.SwapToInput1(0.5f);  // Swap to input 1 with 50% weight
```

### Conditional Swapping
```csharp
// Swap only if not already on target input
void EnsureInput1Active()
{
    if (!swapData.IsUsingInput1())
    {
        swapData.SwapToInput1(1.0f);
    }
}
```

### Connection Monitoring
```csharp
// Check connection states
void DebugConnections()
{
    bool input0Connected = swapData.IsInput0Connected();
    bool input1Connected = swapData.IsInput1Connected();

    Debug.Log($"Input 0: {(input0Connected ? "Connected" : "Disconnected")}");
    Debug.Log($"Input 1: {(input1Connected ? "Connected" : "Disconnected")}");
}
```

## Performance Considerations

1. **Swap Efficiency**: Hard swaps are very fast - single disconnect + connect operations
2. **Graph Updates**: Swapping during graph evaluation is safe
3. **Memory**: No additional memory allocation during swap operations
4. **Burst Compilation**: All swap operations are Burst-compiled for performance

## Common Use Cases

1. **Attack Animations**: Instant transition from idle/locomotion to attack
2. **Reaction Animations**: Immediate response to hit or impact
3. **Teleport Effects**: Instant state changes during teleportation
4. **Weapon Switching**: Swap idle states based on equipped weapon
5. **Death Animations**: Instant transition to death state
6. **UI Transitions**: Immediate animation state changes for UI elements

## Next Steps

With Day 14, you now have complete understanding of:
1. How to create PlayableGraphs (Day 1)
2. How to add outputs (Day 2, Day 11)
3. How to create custom behavior playables (Day 3-10)
4. How to play actual animation clips (Day 12)
5. How to blend multiple animations (Day 13)
6. **How to perform instant hard swaps between animations (Day 14)**
7. How to manage resources properly (Day 10)

This completes the foundation for:
- Animation state machines
- Complex animation blending systems
- Smooth and instant animation transitions
- State-based animation control

## Comparison: Hard Swap vs. Blend

| Aspect | Hard Swap (Day 14) | Blend (Day 13) |
|--------|-------------------|----------------|
| Transition | Instant | Gradual |
| Inputs Active | One at a time | Both simultaneously |
| Use Case | Attacks, reactions | Locomotion, transitions |
| Weight | Binary (0 or 1) | Continuous (0.0 to 1.0) |
| Visual Result | Abrupt cut | Smooth crossfade |
| Performance | Slightly faster | Minimal overhead |
| Complexity | Simpler | More complex |
