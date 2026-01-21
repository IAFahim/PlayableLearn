# PlayableLearn - Day 15: Weighted Blending

## Overview
Day 15 introduces **Weighted Blending** - the ability to set explicit weights on mixer inputs. This demonstrates the core concept of setting specific weight values like 0.5/0.5 for equal blending between two animations.

## What You'll Learn
- How to set explicit weights on mixer inputs (e.g., 0.5/0.5 for equal blend)
- The difference between weighted blending and hard swapping
- Understanding weight normalization (ensuring weights sum to 1.0)
- Smooth transitions between weight values
- Practical use cases for weighted blending
- Managing blend state and interpolation

## Files Structure
```
Assets/Day15/Scripts/
├── Day15.asmdef                          # Assembly definition
├── Day15BlendData.cs                     # Data layer: Weighted blend data structure
├── WeightBlendOps.cs                     # Operations layer: Burst-compiled weight operations
├── Day15BlendExtensions.cs               # Extensions layer: Weight management API
└── Day15Entry.cs                         # MonoBehaviour entry point

Assets/Day15/Tests/
├── Day15Tests.asmdef                     # Test assembly definition (to be created)
└── Day15Tests.cs                         # Unit tests (to be created)
```

## The Three-Layer Architecture

### Layer A: Data (Day15BlendData)
Pure data structure for managing weighted blend operations:
- `PlayableGraph Graph` - The graph that owns this blend
- `Playable Mixer` - The mixer playable for blending
- `int InputCount` - Number of inputs for blending
- `float Weight0` - Current weight for input 0
- `float Weight1` - Current weight for input 1
- `float TargetWeight0` - Target weight for input 0 (for smooth transitions)
- `float TargetWeight1` - Target weight for input 1 (for smooth transitions)
- `float BlendSpeed` - Speed for transitioning weights
- `bool NormalizeWeights` - Whether weights should be normalized
- `bool IsActive` - Local flag to track if blend data is active
- `int BlendId` - Debugging identifier
- `float CurrentBlend` - Current blend value (0-1)

### Layer B: Operations (WeightBlendOps)
Burst-compiled static methods for weight operations:
- `SetWeights()` - Sets explicit weights on both inputs
- `SetWeight()` - Sets weight of a specific input
- `GetWeight()` - Gets weight of a specific input
- `SetBlend()` - Sets weights from blend value (0-1)
- `GetBlend()` - Gets current blend value
- `UpdateWeights()` - Updates weights towards targets (for smooth transitions)
- `NormalizeWeights()` - Normalizes weights to sum to 1.0
- `SetEqualBlend()` - Sets both weights to 0.5
- `ApplyWeights()` - Applies weights with optional normalization

### Layer C: Extensions (Day15BlendExtensions)
Public API for weighted blend management:
- `Initialize()` - Initializes blend data with mixer
- `Dispose()` - Cleans up blend data
- `SetWeights()` - Sets explicit weights (e.g., 0.5f, 0.5f)
- `SetWeight()` - Sets weight of a specific input
- `TryGetWeight()` - Gets current weight of an input
- `SetBlend()` - Sets blend value (0-1)
- `TryGetBlend()` - Gets current blend value
- `SetEqualBlend()` - Sets equal blend (0.5/0.5)
- `SetBlendSpeed()` - Sets transition speed
- `SetNormalizeWeights()` - Enables/disables normalization
- `UpdateWeights()` - Updates weights towards targets (call in Update)
- `IsBlendComplete()` - Checks if weights match targets
- `IsEqualBlend()` - Checks if at equal blend (0.5/0.5)
- `LogToConsole()` - Debug logging

## Key Concepts

### Weighted Blending vs Hard Swapping
- **Weighted Blending**: Both inputs contribute to output based on weights
  - 0.5/0.5 = Equal blend of both animations
  - 0.7/0.3 = 70% input 0, 30% input 1
  - Allows smooth transitions
- **Hard Swapping** (Day 14): One input fully active, other disconnected
  - Input 0 OR Input 1 (not both)
  - Immediate switch
  - No blending

### Weight Normalization
- Weights don't have to sum to 1.0, but it's often desired
- Normalization ensures: weight0 + weight1 = 1.0
- Example: 0.5/0.5 is already normalized
- Example: 2.0/2.0 normalizes to 0.5/0.5
- Can be enabled/disabled via `SetNormalizeWeights()`

### Smooth Transitions
- Set target weights, then update over time
- `UpdateWeights()` interpolates towards targets
- Blend speed controls transition rate
- Call `UpdateWeights()` in MonoBehaviour Update()
- Check completion with `IsBlendComplete()`

### Equal Blend (0.5/0.5)
- Special case: both inputs contribute equally
- Common for transitioning between states
- Sets both weights to 0.5
- Can be checked with `IsEqualBlend()`

## Usage Example

```csharp
// Create the graph and mixer
Day10DisposableGraph disposableGraph;
disposableGraph.Initialize("WeightedBlendGraph");

Day13MixerHandle mixerHandle;
mixerHandle.Initialize(in disposableGraph.Graph, 2);

// Initialize weighted blend data
Day15BlendData blendData;
blendData.Initialize(in disposableGraph.Graph, in mixerHandle.Playable, 2);

// Set equal blend (0.5/0.5)
blendData.SetEqualBlend(applyImmediately: true);

// Or set custom weights
blendData.SetWeights(0.7f, 0.3f, applyImmediately: true);

// Or set smooth transition
blendData.SetBlendSpeed(2.0f);
blendData.SetWeights(0.0f, 1.0f, applyImmediately: false);

// Update in MonoBehaviour Update()
void Update()
{
    blendData.UpdateWeights(Time.deltaTime);
}

// Check if equal blend
if (blendData.IsEqualBlend())
{
    Debug.Log("Currently at equal blend!");
}
```

## Common Weight Configurations

| Weight0 | Weight1 | Blend Value | Description |
|---------|---------|-------------|-------------|
| 1.0 | 0.0 | 0.0 | Input 0 only |
| 0.5 | 0.5 | 0.5 | Equal blend |
| 0.0 | 1.0 | 1.0 | Input 1 only |
| 0.7 | 0.3 | 0.3 | Mostly input 0 |
| 0.3 | 0.7 | 0.7 | Mostly input 1 |

## Integration with Previous Days

Day 15 builds upon:
- **Day 10**: IDisposable pattern for cleanup
- **Day 11**: AnimationPlayableOutput for driving animation
- **Day 12**: AnimationClipPlayable for playing clips
- **Day 13**: AnimationMixerPlayable for blending
- **Day 14**: Hard swapping (contrast with weighted blending)

## Testing
Run the Unity Test Runner to verify:
- Weight initialization works correctly
- Explicit weight setting (0.5/0.5) functions properly
- Weight normalization produces correct results
- Smooth transitions interpolate correctly
- Equal blend detection works
- Blend value calculations are accurate
- Complete integration with mixer and clips

## Notes
- Day 15 demonstrates the fundamental concept of weighted blending
- Explicit weight control is essential for animation blending
- Weight normalization is optional but recommended for consistency
- Smooth transitions improve animation quality
- Equal blend (0.5/0.5) is a common special case
- Weights can be set immediately or transitioned smoothly
- Call `UpdateWeights()` every frame for smooth transitions
- Always check `IsValidBlend()` before operations
- Remember to dispose blend data when done

## Next Steps
- **Day 16**: Crossfade Logic - Math for lerping weights over time
- Learn to create automated crossfades between states
- Understand crossfade curves and timing
