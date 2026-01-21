# PlayableLearn - Day 16: The Crossfade Logic

## Overview
Day 16 introduces **Crossfade Logic** - time-based weight transitions using lerping over time. This builds on Day 15's Weighted Blending by adding smooth, duration-controlled transitions between animation states.

## What You'll Learn
- Time-based crossfade transitions between animations
- Math for lerping weights over time
- Multiple easing curves (Linear, Ease-In, Ease-Out, Ease-In-Out)
- Progress tracking for crossfade completion
- Crossfade duration and elapsed time management
- Weight normalization during crossfade

## Files Structure
```
Assets/Day16/Scripts/
├── Day16.asmdef                          # Assembly definition
├── Day16CrossfadeData.cs                 # Data layer: Crossfade data structure
├── CrossfadeOps.cs                       # Operations layer: Burst-compiled crossfade math
├── Day16CrossfadeExtensions.cs           # Extensions layer: Crossfade management
├── Day16CrossfadeBehaviour.cs            # PlayableBehaviour for automatic updates
└── Day16Entry.cs                         # MonoBehaviour entry point

Assets/Day16/Tests/
├── Day16Tests.asmdef                     # Test assembly definition
└── Day16Tests.cs                         # Comprehensive unit tests
```

## The Three-Layer Architecture

### Layer A: Data (Day16CrossfadeData)
Pure data structure representing crossfade state:
- `PlayableGraph Graph` - The graph that owns this playable
- `Playable Mixer` - The mixer playable for crossfading
- `int InputCount` - Number of inputs for crossfading
- `float CurrentWeight0/1` - Current weights for inputs
- `float StartWeight0/1` - Starting weights for crossfade calculation
- `float TargetWeight0/1` - Target weights to transition to
- `float CrossfadeDuration` - Duration of crossfade in seconds
- `float ElapsedTime` - Time elapsed since crossfade started
- `bool IsCrossfading` - Whether a crossfade is in progress
- `int CurveType` - Easing curve type (0=linear, 1=ease-in, 2=ease-out, 3=ease-in-out)
- `float Progress` - Current crossfade progress (0.0-1.0)
- `bool NormalizeWeights` - Whether to normalize weights during crossfade

### Layer B: Operations (CrossfadeOps)
Burst-compiled static methods for crossfade math:
- `ApplyCurve()` - Applies easing curve to progress value
- `Lerp()` - Linear interpolation between two values
- `CalculateCrossfadeWeights()` - Calculates weights at given progress
- `UpdateProgress()` - Updates progress based on elapsed time
- `IsCrossfadeComplete()` - Checks if crossfade is complete
- `ApplyWeights()` - Applies weights to mixer
- `GetWeight()` - Gets current weight from mixer
- `ValidateCrossfadeParams()` - Validates crossfade parameters
- `CalculateTimeRemaining()` - Calculates time remaining for crossfade
- `NormalizeWeights()` - Normalizes two weights to sum to 1.0
- `AreWeightsEqual()` - Checks if weights are approximately equal
- `AreWeightsAtTarget()` - Checks if current weights match targets

### Layer C: Extensions (Day16CrossfadeExtensions)
Public API for crossfade management:
- `Initialize()` - Creates crossfade data structure
- `Dispose()` - Cleans up crossfade resources
- `StartCrossfade()` - Starts crossfade to target weights
- `StartCrossfadeByBlend()` - Starts crossfade using blend value
- `UpdateCrossfade()` - Updates crossfade by deltaTime (call in Update)
- `StopCrossfade()` - Stops crossfade at current progress
- `CompleteCrossfade()` - Immediately completes crossfade
- `SetWeightsImmediate()` - Sets weights without crossfade
- `SetCurveType()` - Sets easing curve type
- `SetNormalizeWeights()` - Enables/disables weight normalization
- `IsCrossfading()` - Checks if crossfade is in progress
- `IsCrossfadeComplete()` - Checks if crossfade is complete
- `TryGetProgress()` - Gets current crossfade progress
- `TryGetTimeRemaining()` - Gets time remaining for crossfade
- `TryGetWeight()` - Gets current weight of specific input
- `TryGetTargetWeight()` - Gets target weight of specific input
- `IsValidCrossfade()` - Checks if crossfade data is valid
- `LogToConsole()` - Debug output logging

## Key Concepts

### Time-Based Crossfading
Crossfade is a time-based transition between animation states:
- Specify target weights and duration
- Weights automatically interpolate over time
- Progress tracked from 0.0 (start) to 1.0 (complete)
- Call `UpdateCrossfade()` in your Update loop

### Easing Curves
Different easing curves control the feel of transitions:
- **Linear**: Constant rate of change (t)
- **Ease-In**: Starts slow, accelerates (t²)
- **Ease-Out**: Starts fast, decelerates (t * (2-t))
- **Ease-In-Out**: Smooth start and end (3t² - 2t³)

### Lerping Math
Linear interpolation is the core of crossfading:
```
weight = start + (target - start) * curvedProgress
```
Where `curvedProgress` applies the easing curve to the raw time progress.

### Progress Tracking
Monitor crossfade completion:
- `Progress` field (0.0 to 1.0)
- `IsCrossfading()` returns true during transition
- `IsCrossfadeComplete()` returns true when done
- `TryGetTimeRemaining()` gets seconds left

### Duration Control
Specify exactly how long crossfades take:
- `CrossfadeDuration` in seconds
- Minimum 0.001s to prevent division by zero
- Elapsed time tracked automatically
- Crossfade auto-completes when duration reached

## Usage Example

```csharp
// Create the graph
Day10DisposableGraph disposableGraph;
disposableGraph.Initialize("MyAnimationGraph");

// Create a mixer with 2 inputs
Day13MixerHandle mixerHandle;
mixerHandle.Initialize(in disposableGraph.Graph, 2);

// Initialize crossfade data
Day16CrossfadeData crossfadeData;
crossfadeData.Initialize(in disposableGraph.Graph, in mixerPlayable, 2);

// Start a crossfade to input 1 only over 1 second with ease-in-out curve
crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.EaseInOut);

// In your Update loop
void Update()
{
    crossfadeData.UpdateCrossfade(Time.deltaTime);

    // Check progress
    if (crossfadeData.TryGetProgress(out float progress))
    {
        Debug.Log($"Crossfade progress: {progress:P2}");
    }

    // Check if complete
    if (crossfadeData.IsCrossfadeComplete())
    {
        Debug.Log("Crossfade complete!");
    }
}

// Clean up
crossfadeData.Dispose();
mixerHandle.Dispose();
disposableGraph.Dispose();
```

## Easing Curve Examples

```csharp
// Linear - Constant rate
crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);

// Ease-In - Starts slow, speeds up (good for fades in)
crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.EaseIn);

// Ease-Out - Starts fast, slows down (good for fades out)
crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.EaseOut);

// Ease-In-Out - Smooth both ends (most natural)
crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.EaseInOut);
```

## CrossfadeOps Math Details

The core math for crossfading:

```csharp
// 1. Calculate raw progress from time
float rawProgress = elapsedTime / duration;

// 2. Apply easing curve
float curvedProgress = ApplyCurve(rawProgress, curve);

// 3. Lerp each weight
weight0 = lerp(startWeight0, targetWeight0, curvedProgress);
weight1 = lerp(startWeight1, targetWeight1, curvedProgress);

// 4. Optionally normalize
if (normalize)
{
    float total = weight0 + weight1;
    weight0 /= total;
    weight1 /= total;
}

// 5. Apply to mixer
mixer.SetInputWeight(0, weight0);
mixer.SetInputWeight(1, weight1);
```

## Day 16 vs Day 15 Comparison

| Feature | Day 15 (Weighted Blending) | Day 16 (Crossfade Logic) |
|---------|---------------------------|-------------------------|
| Weight Setting | Immediate or speed-based | Time-based with duration |
| Transitions | Simple lerp per frame | Configurable duration |
| Easing | Linear only | 4 curve types |
| Progress Tracking | Blend value only | Full progress & time remaining |
| Completion Detection | Check weights match | IsCrossfadeComplete() |
| Use Case | Continuous blending | One-shot transitions |

## Advanced Features

### Weight Normalization
Enable to ensure weights always sum to 1.0:
```csharp
crossfadeData.SetNormalizeWeights(true);
crossfadeData.StartCrossfade(2.0f, 2.0f, 1.0f); // Will normalize to 0.5/0.5
```

### Immediate Weight Changes
Bypass crossfade for instant changes:
```csharp
crossfadeData.SetWeightsImmediate(0.5f, 0.5f);
```

### Stop and Complete
Control crossfade playback:
```csharp
crossfadeData.StopCrossfade();    // Stop at current progress
crossfadeData.CompleteCrossfade(); // Jump to target immediately
```

### Blend-Value Crossfade
Use blend value (0-1) instead of weights:
```csharp
// 0.0 = input 0 only, 0.5 = equal, 1.0 = input 1 only
crossfadeData.StartCrossfadeByBlend(0.5f, 1.0f, CrossfadeOps.CrossfadeCurve.EaseInOut);
```

## Testing

Day 16 includes comprehensive tests:
- CrossfadeOps math operations
- Curve application (Linear, Ease-In, Ease-Out, Ease-In-Out)
- Progress calculation and completion detection
- Weight interpolation and normalization
- Time remaining calculation
- Parameter validation
- Integration with previous days

Run tests in Unity Test Runner:
```
Window > General > Test Runner > Run All
```

## Build on Previous Days

Day 16 integrates all previous concepts:
- **Day 1**: Graph creation and management
- **Day 2**: Output handling
- **Day 4**: ScriptPlayable behaviours
- **Day 5**: Speed control
- **Day 6**: Play state management
- **Day 7**: Reverse time
- **Day 8**: Graph naming
- **Day 9**: Visualization
- **Day 10**: IDisposable pattern
- **Day 11**: AnimationPlayableOutput
- **Day 12**: AnimationClip playables
- **Day 13**: AnimationMixerPlayable
- **Day 14**: Hard swapping
- **Day 15**: Weighted blending

## Best Practices

1. **Always call UpdateCrossfade() in Update()** - Crossfades need per-frame updates
2. **Use appropriate durations** - Too fast looks jerky, too slow looks laggy
3. **Choose curves wisely** - Ease-In-Out is usually most natural
4. **Monitor completion** - Use IsCrossfadeComplete() before starting new crossfades
5. **Clean up properly** - Always Dispose() when done

## Common Pitfalls

- **Not calling UpdateCrossfade()** - Crossfade won't progress
- **Negative durations** - Will be rejected by validation
- **Forgetting to normalize** - Weights may not sum to 1.0
- **Starting crossfade during crossfade** - Will reset and start new one
- **Not checking completion** - May trigger actions too early

## What's Next?

Day 16 completes the core animation blending system. Future days may explore:
- Multiple input blending (more than 2 inputs)
- Blend trees and state machines
- Additive animation mixing
- Animation layers
- Avatar masks
- IK integration
