# 🎓 Mastering Unity Timeline: The Architect's Handbook

This guide distills the high-performance patterns found across massive production codebases. It moves beyond basic "How-To" into "How-To-Architect" for scale and performance.

---

## 🏛️ 1. The High-Velocity Architecture (Generics Pattern)
*Ref: TweenPlayables*

If you are building many tracks (Audio, UI, Transform), don't write them from scratch. Use **Generic Base Classes** to enforce a "Source of Truth" for your logic.

### The Accelerator Base
```csharp
public abstract class TweenAnimationTrack<TBinding, TMixer, TBehaviour> : TrackAsset 
    where TBinding : Component 
    where TBehaviour : TweenBehaviour<TBinding>, new()
    where TMixer : TweenMixer<TBinding, TBehaviour>, new() 
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) =>
        ScriptPlayable<TMixer>.Create(graph, inputCount);
}
```
**Why this wins:** You can now create a new `LightTrack` in 3 lines of code by inheriting from the base. All complex `GatherProperties` and `Mixer` boilerplate stays in the base class.

---

## 🎨 2. The Material Architect (Optimization Pattern)
*Ref: unity-material-timeline*

**The Problem:** Animating `renderer.material` creates a new material instance every frame, causing massive GC spikes.
**The Solution:** Use `MaterialPropertyBlock` in the Mixer.

### The Performance-First Mixer
```csharp
public override void ProcessFrame(Playable playable, FrameData info, object playerData)
{
    var renderer = playerData as Renderer;
    var block = new MaterialPropertyBlock(); // Ideally cached
    
    renderer.GetPropertyBlock(block); // 1. Get current
    
    // 2. Logic: Blend values into 'block'...
    block.SetColor("_Color", blendedColor);
    
    renderer.SetPropertyBlock(block); // 3. Set without allocations
}
```

---

## 🛤️ 3. The Path-Driven Logic (Procedural Pattern)
*Ref: NavMeshAgentProgress*

Timeline is usually used for LERPing values. Advanced systems use it to drive **State Engines**. 

**Pattern:** Use `playable.GetTime() / playable.GetDuration()` as a **Normalized Progress [0-1]** value to drive complex math (like NavMesh pathing) rather than just moving a slider.

```csharp
// Inside ProcessFrame
float progress = (float)(playable.GetTime() / playable.GetDuration());
Vector3 targetPos = MyMath.GetPointOnSpline(progress); 
transform.position = targetPos;
```

---

## 🔔 4. The Marker & Notification System
*Ref: MessageMarker / TMPEffects*

Don't poll for events. Use `INotification`. 

1.  **Marker:** Create a class inheriting from `Marker, INotification`.
2.  **Receiver:** Implement `INotificationReceiver` on a MonoBehaviour.
3.  **Retroactive Flag:** Use `INotificationOptionProvider` to ensure an event fires **even if the user skips past it** (critical for game state integrity).

```csharp
public NotificationFlags flags => NotificationFlags.Retroactive | NotificationFlags.TriggerOnce;
```

---

## 🛠️ 5. Advanced Editor Tooling (UX Pattern)
*Ref: CustomTimelineEditor / CustomClipEditor*

### Custom Clip Backgrounds
You can draw textures or waveforms directly onto the Timeline clips using `ClipEditor.DrawBackground`. 

```csharp
[CustomTimelineEditor(typeof(MyClip))]
public class MyClipEditor : ClipEditor {
    public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region) {
        // Draw custom gradients or icons inside the Timeline window!
        GUI.DrawTexture(region.position, myGradientTexture);
    }
}
```

---

## 🛡️ The "Social Contract" of Timeline Development

1.  **The Default Fallback:** Your Mixer must **always** restore values in `OnPlayableDestroy`. If you don't, your characters will stay in their "damaged" or "faded" state forever after the timeline ends.
2.  **The Scrubbing Rule:** `GatherProperties` is your only defense against Scene Corruption. Use it to tell Unity which properties to "snapshot" before the timeline starts.
3.  **The Allocation Ban:** `ProcessFrame` is a "Hot Path". No `new`, no `GetComponent`, no `LINQ`. Use `playerData` for your references.
4.  **Relative vs Absolute:** High-end tweens (like in `TweenPlayables`) support a **Relative** flag. Store the `InitialValue` in `OnBehaviourPlay` and add the timeline delta to it. This allows Timelines to play correctly regardless of where the character is standing in the world.