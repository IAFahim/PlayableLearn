# Day 17: Layering - AnimationLayerMixerPlayable

## Overview
Day 17 introduces **AnimationLayerMixerPlayable**, a powerful playable type for hierarchical animation blending with layers. Unlike regular mixers which normalize weights, layer mixers allow independent control of each layer's weight.

## Key Concepts

### Layering
- **Base Layer (Layer 0)**: Always the foundation (thumbody), typically at weight 1.0
- **Additive/Override Layers (Layer 1+)**: Additional animation layers that add to or override the base
- **Independent Weights**: Each layer has its own weight (unlike mixer normalization)

### Layer Mixer vs Regular Mixer
- **AnimationMixerPlayable**: Normalizes weights to sum to 1.0
- **AnimationLayerMixerPlayable**: Each layer has independent weight (no normalization)

### Use Cases
- **Upper Body Animation**: Layer aiming/shooting on top of lower body movement
- **Additive Animations**: Breathing, recoil, or facial expressions on top of base motion
- **Partial Body Override**: Only affect specific body parts (with AvatarMasks - Day 18)

## Files

### Core Implementation
- **Day17.asmdef**: Assembly definition referencing all previous days
- **Day17LayerHandle.cs**: Data structure wrapping AnimationLayerMixerPlayable
- **LayerMixerOps.cs**: Burst-compiled operations for layer mixer
- **Day17LayerExtensions.cs**: Extension methods for easy layer mixer usage
- **Day17Entry.cs**: MonoBehaviour entry point demonstrating layering

### Tests
- **Day17Tests.cs**: Comprehensive test suite for layering functionality
- **Day17Tests.asmdef**: Test assembly definition

## Architecture

### Layer A: Data (Pure State)
```csharp
public struct Day17LayerHandle
{
    public PlayableGraph Graph;
    public Playable Playable;
    public int LayerCount;
    public bool IsActive;
    public int LayerMixerId;
}
```

### Layer B: Operations (Burst-Compiled)
- `LayerMixerOps.Create()`: Create layer mixer
- `LayerMixerOps.SetLayerWeight()`: Set layer weight
- `LayerMixerOps.GetLayerWeight()`: Get layer weight
- `LayerMixerOps.IsLayerActive()`: Check if layer is active
- `LayerMixerOps.ResetToBaseLayer()`: Reset to base layer only

### Layer C: Extensions (Adapter Layer)
- `Initialize()`: Initialize layer mixer
- `SetLayerWeight()`: Set layer weight with validation
- `EnableLayer()`: Enable layer (weight = 1.0)
- `DisableLayer()`: Disable layer (weight = 0.0)
- `SetTwoLayerBlend()`: Simple 2-layer blend control

## Usage Example

```csharp
// Create layer mixer with 2 layers
var layerHandle = new Day17LayerHandle();
layerHandle.Initialize(in graph, 2);

// Connect base animation to layer 0
layerHandle.ConnectLayer(ref basePlayable, 0, 1.0f);

// Connect additive animation to layer 1
layerHandle.ConnectLayer(ref additivePlayable, 1, 0.0f);

// Enable additive layer (50% weight)
layerHandle.SetLayerWeight(1, 0.5f);
```

## GUI Controls
- **Enable Additive Layer**: Set layer 1 weight to 1.0
- **Disable Additive Layer**: Set layer 1 weight to 0.0
- **Additive Layer 50%**: Set layer 1 weight to 0.5
- **Reset to Base Layer**: Reset to base layer only

## Visualization
- **Magenta**: Additive layer is active
- **Gray**: Base layer only
- **Red**: Error state

## Key Differences from Regular Mixer

### Weights
- **Mixer**: Weights normalized (sum to 1.0)
- **Layer Mixer**: Independent weights (no normalization)

### Layer 0 (Base Layer)
- Always present and typically at weight 1.0
- Cannot be disabled (use ResetToBaseLayer() instead)
- Serves as foundation for all other layers

### Layers 1+ (Additive/Override)
- Independent weight control (0.0 to 1.0)
- Can be enabled/disabled independently
- Add to or override base layer motion

## Future Days
- **Day 18**: Masking - AvatarMask support for layers
- **Day 19**: Root Motion - Reading root motion from layers
- **Day 20**: Additive Mixing - True additive animation blending

## Testing
All layer operations are tested in `Day17Tests.cs`:
- Layer mixer creation and disposal
- Layer weight control
- Layer activation/deactivation
- Two-layer blending
- Integration with previous days

## Performance Notes
- Layer mixers are optimized for hierarchical blending
- Use Burst-compiled operations for best performance
- Minimize layer count (2-3 layers typical)

## See Also
- [Day 13: AnimationMixerPlayable](../Day013/README.md) - Regular mixer with weight normalization
- [Day 16: Crossfade Logic](../Day16/README.md) - Time-based weight transitions
- Unity Documentation: [AnimationLayerMixerPlayable](https://docs.unity3d.com/ScriptReference/Animations.AnimationLayerMixerPlayable.html)
