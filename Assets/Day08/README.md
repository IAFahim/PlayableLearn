# Scene Binding and Material Property Blocks

## Lesson Overview

This lesson teaches high-performance Timeline-to-Scene integration. Students learn how to safely manipulate Renderer properties without creating Material instances - a critical performance optimization.

---

## Context from Previous Days

**Day06 - Manual Property Blending**: Students implemented weighted average calculations for property interpolation. They learned the math behind blending but only logged values.

**Day07 - Seek & Time Control**: Students learned to manipulate Timeline playback time. They saw how to scrub and control playback but didn't affect scene objects.

---

## The Gap

**Student Assumption**: "To animate a Renderer's color, I just do `renderer.material.color = Color.red`. It works fine."

**Reality**: Every time you access `renderer.material`, Unity creates a NEW Material instance. The original Material asset is untouched, but you now have a duplicate in memory. Do this every frame and you leak memory rapidly.

**Why This Matters**: In Timeline-driven cutscenes, you might animate colors on hundreds of objects. Using `material.color` creates hundreds of Material instances per frame. This causes:
* Memory spikes
* GC pauses
* Increased build sizes
* Unwanted state persistence

---

## Key Technical Skills

### 1. TrackBindingType Attribute

```csharp
[TrackBindingType(typeof(Renderer))]
public class Day08ThemeTrack : TrackAsset { }
```

Tells Timeline Director: "This track needs a Renderer component bound to it." The bound Renderer is passed as `playerData` to ProcessFrame.

### 2. MaterialPropertyBlock Pattern

```csharp
MaterialPropertyBlock block = new MaterialPropertyBlock();
renderer.GetPropertyBlock(block);
block.SetColor(colorId, value);
renderer.SetPropertyBlock(block);
```

**Why This Works**: MaterialPropertyBlock is a lightweight data container that overrides material properties PER-RENDERER without creating instances. The Material asset remains shared.

**Performance**: Zero GC allocation, zero memory leaks.

### 3. Default State Preservation

```csharp
if (!_firstFrameHappened)
{
    _defaultColor = renderer.sharedMaterial.color;
    _firstFrameHappened = true;
}
```

Capture the original state before Timeline modifies it. Use `sharedMaterial` (read-only) instead of `material` (creates instance).

### 4. Empty Space Blending

```csharp
float remainingWeight = 1f - totalWeight;
if (remainingWeight > 0)
    finalColor += _defaultColor * remainingWeight;
```

When clips don't cover the entire timeline, blend the remaining weight with the default color. This prevents "snap back" artifacts.

---

## File-by-File Explanation

### Day08ThemePlayableAsset.cs

**Purpose**: Timeline asset holding the color value for each clip.

**Key Elements**:
```csharp
[Serializable]
public class Day08ThemePlayableAsset : PlayableAsset, ITimelineClipAsset
{
    public Color color = Color.white;
    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<Day08ThemePlayableBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.color = color;
        return playable;
    }
}
```

**Teaching Points**:
* Asset holds inspector-editable color
* `ClipCaps.Blending` enables clip overlap and blend curves
* `CreatePlayable` injects asset's color into the behaviour
* This is a data transfer object - no logic here

---

### Day08ThemePlayableBehaviour.cs

**Purpose**: Runtime data container for the mixer to read.

**Key Elements**:
```csharp
public class Day08ThemePlayableBehaviour : PlayableBehaviour
{
    public Color color;
}
```

**Teaching Points**:
* Pure data class - zero logic
* The mixer reads this from each input
* Color is set by the asset during CreatePlayable

---

### Day08ThemeMixerBehaviour.cs

**Purpose**: Core blending logic with MaterialPropertyBlock application.

**Key Elements**:
```csharp
public class Day08ThemeMixerBehaviour : PlayableBehaviour
{
    private MaterialPropertyBlock _propBlock;
    private static readonly int _colorId = Shader.PropertyToID("_Color");
    private Color _defaultColor;
    private bool _firstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var renderer = playerData as Renderer;
        if (renderer == null) return;

        if (_propBlock == null)
            _propBlock = new MaterialPropertyBlock();

        if (!_firstFrameHappened)
        {
            _defaultColor = renderer.sharedMaterial.color;
            _firstFrameHappened = true;
        }

        Color finalColor = Color.black;
        float totalWeight = 0f;

        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight <= 0.001f) continue;

            var inputPlayable = (ScriptPlayable<Day08ThemePlayableBehaviour>)playable.GetInput(i);
            var behaviour = inputPlayable.GetBehaviour();

            finalColor += behaviour.color * inputWeight;
            totalWeight += inputWeight;
        }

        float remainingWeight = 1f - totalWeight;
        if (remainingWeight > 0)
            finalColor += _defaultColor * remainingWeight;

        renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(_colorId, finalColor);
        renderer.SetPropertyBlock(_propBlock);
    }
}
```

**Teaching Points**:
* `playerData as Renderer` gets the bound object from TrackBindingType
* `Shader.PropertyToID("_Color")` caches the property ID for performance
* `sharedMaterial.color` reads the original color WITHOUT creating instance
* Lazy initialization of MaterialPropertyBlock
* Empty space blending prevents snap-back artifacts
* `SetPropertyBlock` applies overrides without touching the Material asset

**Critical Details**:
* Always call `GetPropertyBlock` before `SetPropertyBlock` - this preserves existing overrides
* Property IDs are cached in `static readonly` for performance
* The `_firstFrameHappened` flag ensures we capture defaults exactly once

---

### Day08ThemeTrack.cs

**Purpose**: Defines the track type and required binding.

**Key Elements**:
```csharp
[TrackColor(0.5f, 0f, 0.5f)]
[TrackClipType(typeof(Day08ThemePlayableAsset))]
[TrackBindingType(typeof(Renderer))]
public class Day08ThemeTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<Day08ThemeMixerBehaviour>.Create(graph, inputCount);
    }
}
```

**Teaching Points**:
* `[TrackBindingType(typeof(Renderer))]` is the critical attribute
* Director enforces this binding in the Timeline editor
* The bound Renderer is passed as `playerData` to ProcessFrame
* Track color is purple for visual distinction

---

## Common Student Questions

**Q: Why use PropertyToID instead of the string "_Color"?**
A: `Shader.PropertyToID` converts the string to an integer ID once. Using the string directly does the conversion every frame. The ID is 100x faster.

**Q: Can I animate other material properties like _Metallic or _Emission?**
A: Absolutely. Use `SetFloat`, `SetVector`, `SetTexture` on MaterialPropertyBlock. Just cache the property ID for each.

**Q: What if no Renderer is bound to the track?**
A: `playerData as Renderer` returns null. The mixer handles this with an early return - it just does nothing that frame.

**Q: Why not just store the renderer reference in Awake?**
A: The binding can change at runtime in the Timeline editor. Using `playerData` ensures you always have the current binding.

**Q: Does SetPropertyBlock work with SkinnedMeshRenderer?**
A: Yes. SkinnedMeshRenderer inherits from Renderer and supports MaterialPropertyBlock fully.

**Q: Can I see PropertyBlock values in the Scene View?**
A: No. PropertyBlock overrides are only visible at runtime (Play mode or built game). This can make debugging trickier.

**Q: What happens if I call renderer.material.color after SetPropertyBlock?**
A: You create a Material instance immediately, defeating the purpose. The PropertyBlock still applies, but you've leaked memory.

**Q: How do I reset the PropertyBlock when Timeline stops?**
A: You need to cache the renderer reference and call `renderer.SetPropertyBlock(null)` in OnGraphStop. This lesson omits it for simplicity.

---

## Homework / Thought Exercises

### Exercise 1: Memory Leak Diagnosis
**Scenario**: A student's profiler shows 500 Material allocations during a 10-second cutscene.

**Question**: Their mixer looks like this:
```csharp
renderer.material.color = finalColor;
```
What's causing the leak? How many allocations per frame?

**Answer**: Every frame, `renderer.material` creates a new Material instance. At 60 FPS for 10 seconds: 60 * 10 = 600 allocations. Replace with MaterialPropertyBlock.

### Exercise 2: Property ID Caching
**Scenario**: You're animating three properties: _Color, _Metallic, _Emission.

**Question**: Is this efficient?
```csharp
renderer.GetPropertyBlock(block);
block.SetColor(Shader.PropertyToID("_Color"), color);
block.SetFloat(Shader.PropertyToID("_Metallic"), metallic);
block.SetColor(Shader.PropertyToID("_Emission"), emission);
renderer.SetPropertyBlock(block);
```

**Answer**: No. `PropertyToID` runs three times per frame. Cache the IDs in static readonly fields:
```csharp
private static readonly int _colorId = Shader.PropertyToID("_Color");
private static readonly int _metallicId = Shader.PropertyToID("_Metallic");
private static readonly int _emissionId = Shader.PropertyToID("_Emission");
```

### Exercise 3: Empty Space Blending
**Scenario**: A timeline has a color clip from 0-2 seconds, and nothing from 2-5 seconds. The object is red by default.

**Question**: Without empty space blending, what happens at 2.1 seconds?

**Answer**: The mixer returns black (finalColor = Color.black with zero total weight). The object snaps to black abruptly. With empty space blending, it fades back to red (default color).

### Exercise 4: Multiple Renderers
**Scenario**: You want to control the color of 5 objects simultaneously with one track.

**Question**: Can TrackBindingType support this? How would you approach it?

**Answer**: TrackBindingType only supports a single binding. Options:
1. Create 5 separate tracks (simple but repetitive)
2. Use a parent GameObject with a custom component that holds renderer references
3. Create a "ColorController" component that propagates colors to child renderers

### Exercise 5: PropertyBlock Persistence
**Scenario**: After Timeline finishes, the object retains the PropertyBlock color.

**Question**: How do you reset the object to its original state when the graph stops?

**Answer**: Cache the renderer in ProcessFrame, then in OnGraphStop:
```csharp
private Renderer _cachedRenderer;

public override void ProcessFrame(Playable playable, FrameData info, object playerData)
{
    _cachedRenderer = playerData as Renderer;
    // ... rest of logic
}

public override void OnGraphStop(Playable playable)
{
    if (_cachedRenderer != null)
        _cachedRenderer.SetPropertyBlock(null);
}
```

---

## Extension Topics

* **Multiple Property Animation**: Blending color, metallic, and smoothness simultaneously
* **Gradient Blending**: Using Gradient objects instead of single colors
* **Particle System Control**: Using MaterialPropertyBlock on ParticleSystemRenderer
* **Shader Graph Integration**: Controlling exposed Shader Graph properties
* **Global Keyword Manipulation**: Setting shader keywords via MaterialPropertyBlock
