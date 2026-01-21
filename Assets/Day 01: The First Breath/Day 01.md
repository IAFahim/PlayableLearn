# Day 01: The First Breath

## Welcome to the Playables API Curriculum

This is Day 1 of a 100-day journey into Unity's Playables API. Today you will create your first PlayableGraph from code and play an animation on a cube without using Mecanim state machines.

---

## Overview

**Objective:** Create a PlayableGraph programmatically, attach an AnimationClip, and play it on a cube.

**Time:** 15 minutes

**Difficulty:** Beginner

---

## Learning Objectives

After completing this lesson, you will understand:

- How to create a `PlayableGraph` using code
- The Data-Logic-Extension architecture pattern
- How to connect graph outputs to an Animator component
- Proper memory management with `Graph.Destroy()`
- The role of `PlayableBehaviour.PrepareFrame()`

---

## Files Overview

| File | Architecture Layer | Description |
|------|-------------------|-------------|
| `AnimationClipData.cs` | Layer A: Data | Serializable struct for clip configuration |
| `SimpleClipPlayerBehaviour.cs` | Layer B: Logic | Custom PlayableBehaviour for playback tracking |
| `PlayableGraphBuilder.cs` | Layer C: Extension | Graph construction via extension methods |
| `CubeAnimationPlayer.cs` | MonoBehaviour | Runtime component that orchestrates the graph |

---

## Quick Start

### Prerequisites

- Unity 2022.3 or later
- A 3D project template (URP or Built-in)

### Step-by-Step Setup

#### Creating the Test Object

1. **Create a Cube**
   - Navigate to `GameObject > 3D Object > Cube`
   - Name it "AnimatedCube"

2. **Add an Animator Component**
   - Select the cube
   - In the Inspector, click `Add Component`
   - Type "Animator" and select it
   - Ensure "Apply Root Motion" is unchecked

3. **Attach the Script**
   - With the cube selected, click `Add Component`
   - Type "CubeAnimationPlayer" and select it

4. **Assign an Animation Clip**
   - In the Inspector, locate the "Day 01: The First Breath" header
   - Drag any AnimationClip into the "Clip To Play" field
   - Adjust "Playback Speed" if desired (1.0 = normal speed)

5. **Enter Play Mode**
   - Press the Play button in the Unity Editor
   - Observe the cube animating without any Mecanim controller

---

## Creating a Test Animation Clip

If you do not have an animation clip available:

1. **Create New Animation**
   - In the Project window, right-click
   - Select `Create > Animation`
   - Name it "CubeSpin"

2. **Open Animation Window**
   - Select your cube in the Hierarchy
   - Navigate to `Window > Animation > Animation`
   - Click the Create button if prompted

3. **Record Keyframes**
   - Enable recording (red circle icon)
   - At frame 0: Set Rotation to (0, 0, 0)
   - At frame 60: Set Rotation to (0, 360, 0)
   - Save the clip

4. **Assign to Script**
   - Drag "CubeSpin" into the "Clip To Play" field

---

## Architecture Explanation

### The Graph Structure

```
AnimationClipPlayable
    ↓ (weight: 1.0)
ScriptPlayable<SimpleClipPlayerBehaviour>
    ↓
AnimationPlayableOutput
    ↓
Animator Component
    ↓
Transform (Cube)
```

### Layer A: Data

```csharp
[Serializable]
public struct AnimationClipData
{
    public AnimationClip Clip;
    public float Speed;
}
```

This struct is serialized by Unity and exposed in the Inspector.

### Layer B: Logic

```csharp
public override void PrepareFrame(Playable playable, FrameData info)
{
    if (_isPlaying)
        _elapsedTime += info.GetDeltaTime();
}
```

The `PrepareFrame` method executes every frame during graph evaluation, **before** animation is applied.

### Layer C: Extension

```csharp
public static PlayableGraph CreateClipPlayerGraph(
    this PlayableGraph graph,
    AnimationClipData clipData,
    Animator animator,
    out ScriptPlayable<SimpleClipPlayerBehaviour> behaviourNode)
```

Extension methods provide clean, reusable graph construction patterns.

---

## Experiments

Try these modifications to deepen your understanding:

| Experiment | Expected Result |
|------------|-----------------|
| Set Playback Speed to `2.0` | Animation plays twice as fast |
| Set Playback Speed to `-1.0` | Animation plays in reverse |
| Create 5 cubes with different clips | All animate independently |
| Remove `Graph.Destroy()` call | Memory leak warning in Console |

---

## Important Notes

### Memory Management

Always destroy your PlayableGraph when the GameObject is destroyed:

```csharp
void OnDestroy()
{
    if (_graph.IsValid())
        _graph.Destroy();
}
```

### Graph Evaluation Order

1. `PrepareFrame()` is called on all behaviours
2. Animation is calculated
3. Pose is applied to the Animator
4. `OnBehaviourPlay()` callbacks (if any)

### Debugging

Graph names appear in the Unity Profiler. Use descriptive names:

```csharp
var graph = PlayableGraph.Create("Day01_Cube_" + gameObject.name);
```

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Cube does not animate | Verify Animator component exists and clip is assigned |
| "No animation clip assigned" error | Assign an AnimationClip to the "Clip To Play" field |
| Console shows errors | Check that all four script files are present |

---

## What's Next

**Day 02: The Puppeteer**

Add a UI Slider to manually control animation time, enabling scrubbing functionality.

---

## Concepts Index

- **PlayableGraph**: The container for all animation nodes
- **AnimationClipPlayable**: A node that plays an AnimationClip
- **ScriptPlayable\<T\>**: A wrapper for custom PlayableBehaviour logic
- **AnimationPlayableOutput**: Connects the graph to an Animator
- **PrepareFrame()**: Called every frame during graph evaluation
