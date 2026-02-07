# Manual Property Blending

## Lesson Overview

This lesson bridges the gap between Unity's automatic animation blending and manual blending for custom Timeline playables. Students learn how to implement proper property interpolation in mixer behaviours.

---

## Context from Previous Days

**Day02 - Blending Logs**: Students explored Playable lifecycle callbacks through extensive logging. They saw how clips overlap and blend in the console, understanding WHEN blending occurs in the execution flow.

**Day03 - Data Flow**: Students traced the execution order through `PrepareData`, `PrepareFrame`, and `ProcessFrame`. They learned that data preparation happens before frame processing.

**Day05 - Graph Rebuilding**: Students discovered that modifying Timeline tracks at runtime requires explicit `RebuildGraph()` calls. They learned the graph is static once built unless explicitly reconstructed.

---

## The Gap

**Student Assumption**: "Unity handles blending automatically for all Timeline clips, including my custom scripts."

**Reality**: Unity's Animation Track handles blending automatically because it's a built-in system. For custom playables with exposed properties, YOU must implement manual blending in the mixer behaviour.

**Why This Matters**: Without manual blending, overlapping clips will snap between values instead of smoothly interpolating. This creates jarring transitions that undermine Timeline's core strength.

---

## Key Technical Skill: Input Casting

### The Pattern

```csharp
for (int i = 0; i < playable.GetInputCount(); i++)
{
    ScriptPlayable<YourBehaviour> input = (ScriptPlayable<YourBehaviour>)playable.GetInput(i);
    YourBehaviour behaviour = input.GetBehaviour();
    // Access behaviour properties
}
```

### Why This Pattern?

1. **`playable.GetInput(i)`** returns a generic `Playable` struct
2. **Cast to `ScriptPlayable<T>`** to access behaviour-specific methods
3. **`GetBehaviour()`** retrieves your custom behaviour instance
4. Access exposed properties for blending calculations

### Common Mistakes

* Using `playable.GetInput(i)` directly without casting
* Forgetting to check `input.IsValid()` before accessing
* Assuming input count is fixed (it changes with clip overlap)
* Not handling the case of zero inputs

---

## File-by-File Explanation

### Day06Behaviour.cs

**Purpose**: Holds the properties that will be blended between clips.

**Key Elements**:
```csharp
public class Day06Behaviour : PlayableBehaviour
{
    public float intensity;        // The value to blend
    public Color color;            // Another blendable value
    public bool isActive;          // Boolean flag
}
```

**Teaching Points**:
* These are per-clip values set in the inspector or via code
* The mixer reads these from all active inputs
* No blending logic here - just data storage

---

### Day06PlayableAsset.cs

**Purpose**: Timeline asset that creates the playable and enables blending.

**Key Elements**:
```csharp
public class Day06PlayableAsset : PlayableAsset, ITimelineClipAsset
{
    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<Day06Behaviour>.Create(graph);
    }
}
```

**Teaching Points**:
* `ClipCaps.Blending` tells Timeline this clip supports overlap
* Without this flag, clips cannot be arranged with blending curves
* Asset creates the playable but doesn't handle blending itself

---

### Day06MixerBehaviour.cs

**Purpose**: Manually blends properties from all active input clips.

**Key Elements**:
```csharp
public class Day06MixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float blendedIntensity = 0f;
        Color blendedColor = Color.black;
        float totalWeight = 0f;

        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight <= 0f) continue;

            ScriptPlayable<Day06Behaviour> input = (ScriptPlayable<Day06Behaviour>)playable.GetInput(i);
            Day06Behaviour behaviour = input.GetBehaviour();

            blendedIntensity += behaviour.intensity * inputWeight;
            blendedColor += behaviour.color * inputWeight;
            totalWeight += inputWeight;
        }

        if (totalWeight > 0f)
        {
            blendedIntensity /= totalWeight;
            blendedColor /= totalWeight;
        }

        // Apply blended values to target
        ApplyToTarget(blendedIntensity, blendedColor);
    }
}
```

**Teaching Points**:
* Loop through ALL inputs (overlapping clips create multiple inputs)
* Get each input's weight (0-1) based on Timeline blend curves
* Multiply property value by weight, accumulate
* Normalize by total weight to prevent value boosting
* Skip inputs with zero weight for optimization

**Weight Distribution Example**:
```
Clip A: intensity=10, weight=0.7  -> contribution=7
Clip B: intensity=5,  weight=0.3  -> contribution=1.5
Total: 18 / 1.0 = 8.5 (blended result)
```

---

### Day06Track.cs

**Purpose**: Defines the track type and creates the mixer.

**Key Elements**:
```csharp
[TrackClipType(typeof(Day06PlayableAsset))]
public class Day06Track : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<Day06MixerBehaviour>.Create(graph, inputCount);
    }
}
```

**Teaching Points**:
* `CreateTrackMixer` is called whenever Timeline builds the graph
* `inputCount` parameter is reserved for future use
* The mixer receives all clips on this track as inputs

---

## Common Student Questions

**Q: Why don't I see blending happening?**
A: Check that `ClipCaps.Blending` is returned in your PlayableAsset. Without this, Timeline disables blend curves in the editor.

**Q: My values are getting huge/maxed out.**
A: You're probably adding weighted values without normalizing. Divide by total weight at the end.

**Q: When should I implement manual blending?**
A: Any time your playable exposes properties that should interpolate smoothly between clips. If your clips just trigger events, blending isn't needed.

**Q: Can I blend different types of data?**
A: Yes, but choose appropriate interpolation:
* Scalars: Linear interpolation (weighted average)
* Colors: RGB components separately
* Quaternions: Spherical linear interpolation
* Booleans: Typically use threshold (weight > 0.5)

---

## Homework / Thought Exercises

### Exercise 1: Diagnose Broken Blending
A student's mixer looks like this:
```csharp
blendedValue += behaviour.value * inputWeight;
// Missing normalization
```
**Question**: What happens when three clips overlap with equal weight (0.33 each), all having value=10?
**Answer**: Result is 30 instead of 10. The value gets tripled because weights aren't normalized.

### Exercise 2: Ease-In/Ease-Out Impact
**Question**: If Clip A has a custom ease-in curve and Clip B has linear blending, how does this affect `GetInputWeight()` during the transition?
**Answer**: `GetInputWeight()` automatically reflects the curve shapes. Your mixer doesn't need to know about curves - just use the weight value as-is.

### Exercise 3: Performance Optimization
**Question**: The mixer runs every frame. Is it safe to skip inputs with zero weight, or might this cause visual glitches?
**Answer**: Safe to skip. `GetInputWeight()` returns exactly 0.0 when the input is fully outside the blend region. This is a valid optimization.

### Exercise 4: Edge Case Handling
**Question**: What should your mixer do when `inputCount` is 0?
**Answer**: Return to a default/safe state. No clips are active, so reset blended values to defaults.

### Exercise 5: Blending Directions
**Question**: When blending from Clip A to Clip B, does your mixer need to know which is "incoming" vs "outgoing"?
**Answer**: No. The weighted average math works identically in both directions. Timeline handles directionality through weights.

---

## Extension Topics

* **Multiple Property Types**: Blending position, rotation, and scale simultaneously
* **Curve-Driven Blending**: Using AnimationCurves for non-linear property interpolation
* **Masked Blending**: Only blending certain properties while others snap
* **Custom Weight Functions**: Overriding Timeline weights with your own logic
