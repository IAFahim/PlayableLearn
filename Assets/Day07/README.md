# Seek & Time Control

## Lesson Overview

This lesson bridges the gap between passive Timeline playback and active time manipulation. Students learn how to scrub, seek, and control playback speed programmatically - essential skills for debug tools, preview systems, and timeline editors.

---

## Context from Previous Days

**Day02 - Timeline Playback Basics**: Students learned how to use `PlayableDirector` to play Timeline assets. They understood the simple play/stop/pause workflow.

**Day05 - Runtime Graph Manipulation**: Students discovered that Timeline's PlayableGraph is built once and stays static. They learned to call `RebuildGraph()` after making structural changes.

**Day06 - Manual Blending Implementation**: Students implemented property blending in mixer behaviours. They saw how Timeline calculates weights during playback.

---

## The Gap

**Student Assumption**: "Timeline is like a video - you press play and it runs from start to finish. The director controls everything."

**Reality**: Timeline is a state machine that YOU control. You can scrub to any time, evaluate single frames, and manipulate playback speed. The director just manages the current time state.

**Why This Matters**: Building tools like timeline editors, scrubbers, or preview systems requires manual time control. Debugging complex sequences often needs frame-by-frame stepping. Speed control is essential for slow-motion analysis or fast-forward testing.

---

## Key Technical Skills

### 1. Time Properties

**`director.time`**: Current playback position (0 to duration). Setting this seeks the timeline.

**`director.initialTime`**: Starting time when Play() is called. Useful for delayed-start scenarios.

**`director.playableGraph.GetRootPlayable(0)`**: Access to the root playable for speed control.

### 2. Seeking Pattern

The correct sequence for manual seeking:

```csharp
director.Pause();                    // Stop automatic time advancement
director.time = targetTime;           // Set desired position
director.Evaluate();                  // Process the frame at new time
```

**Why This Order?**
* Pause prevents the time from changing during your seek operation
* Set time updates the internal state
* Evaluate ensures all systems update to the new time

### 3. Manual Evaluation

**`director.Evaluate()`**: Processes one frame of the timeline at the current time.

Use `Evaluate()` when you want to inspect state without advancing time.

### 4. Speed Control

Timeline doesn't have a built-in speed property. Instead, control speed through the PlayableGraph:

```csharp
director.playableGraph.GetRootPlayable(0).SetSpeed(speedMultiplier);
```

Speed multipliers:
* 0.0 = Paused (frozen)
* 0.5 = Half speed
* 1.0 = Normal speed (default)
* 2.0 = Double speed
* -1.0 = Reverse playback

---

## File-by-File Explanation

### Day07SeekButtons.cs

**Purpose**: MonoBehaviour methods for scrubbing to specific timeline positions.

**Key Elements**:
```csharp
public class Day07SeekButtons : MonoBehaviour
{
    private PlayableDirector director;

    void Awake() => director = GetComponent<PlayableDirector>();

    public void SeekToStart()
    {
        director.Pause();
        director.time = 0.0;
        director.Evaluate();
    }

    public void SeekToMiddle()
    {
        director.Pause();
        director.time = director.duration * 0.5;
        director.Evaluate();
    }

    public void SeekToEnd()
    {
        director.Pause();
        director.time = director.duration;
        director.Evaluate();
    }

    public void SeekToNormalized(float normalizedTime)
    {
        director.Pause();
        director.time = director.duration * normalizedTime;
        director.Evaluate();
    }
}
```

**Teaching Points**:
* Always pause before seeking to prevent race conditions
* `director.duration` gives total timeline length
* Normalized time (0-1) is useful for sliders and progress bars
* `Evaluate()` updates the scene to reflect the new time

**UI Integration**:
```csharp
// Unity UI Button setup
seekButton.onClick.AddListener(() => SeekToNormalized(0.75f));

// Slider for scrubbing
slider.onValueChanged.AddListener(OnSliderChanged);
```

---

### Day07SpeedControl.cs

**Purpose**: Runtime control of Timeline playback speed.

**Key Elements**:
```csharp
public class Day07SpeedControl : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 1.0f;
    private PlayableDirector director;
    private Playable rootPlayable;

    void Awake() => director = GetComponent<PlayableDirector>();

    void OnEnable()
    {
        director.Play();
        rootPlayable = director.playableGraph.GetRootPlayable(0);
        UpdateSpeed();
    }

    void UpdateSpeed()
    {
        if (rootPlayable.IsValid())
            rootPlayable.SetSpeed(speedMultiplier);
    }

    public void SetSpeed(double speed) => rootPlayable.SetSpeed(speed);
    public void SlowMotion() => rootPlayable.SetSpeed(0.5);
    public void FastForward() => rootPlayable.SetSpeed(2.0);
    public void Reverse() => rootPlayable.SetSpeed(-1.0);
}
```

**Teaching Points**:
* Speed must be set after the graph is built (OnEnable or after Play())
* Store `rootPlayable` reference to avoid repeated `GetRootPlayable(0)` calls
* `IsValid()` check prevents errors when graph is destroyed
* Negative speed enables reverse playback
* Speed changes are immediate - no need to rebuild graph

**Speed vs Time Scale**:
* `SetSpeed()` affects only this Timeline
* `Time.timeScale` affects entire game (including physics)
* Use `SetSpeed()` for timeline-specific slow-motion
* Use `Time.timeScale` for global bullet-time effects

---

### Day07TimelineInspector.cs

**Purpose**: Custom inspector exposing internal PlayableDirector state for debugging.

**Key Elements**:
```csharp
[CustomEditor(typeof(PlayableDirector))]
public class Day07TimelineInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayableDirector director = (PlayableDirector)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Timeline Info", EditorStyles.boldLabel);

        EditorGUILayout.DoubleField("Current Time", director.time);
        EditorGUILayout.DoubleField("Duration", director.duration);
        EditorGUILayout.EnumPopup("Play State", director.state);

        if (director.playableGraph.IsValid())
        {
            Playable rootPlayable = director.playableGraph.GetRootPlayable(0);
            if (rootPlayable.IsValid())
                EditorGUILayout.DoubleField("Root Speed", rootPlayable.GetSpeed());
        }
    }
}
```

**Teaching Points**:
* Custom inspectors let you expose hidden PlayableDirector state
* `director.state` shows Playing/Paused/Invalid states
* `playableGraph.IsValid()` checks if the graph is currently built
* Accessing root playable requires knowing the index (usually 0)

---

## Common Student Questions

**Q: I set director.time but nothing happens. Why?**
A: You need to call `director.Evaluate()` after setting time. The time property only stores the position; Evaluate() actually processes the frame.

**Q: Should I Pause() before every seek operation?**
A: Yes, unless you're in manual control mode. If Timeline is playing, setting time is unpredictable because the director will immediately advance it again.

**Q: Can I seek while the Timeline is playing?**
A: Technically yes, but it's fighting against automatic time progression. The result is usually a jump or glitch. Pause first, seek, then optionally resume.

**Q: What's the difference between Evaluate() and Update()?**
A: `Evaluate()` processes the current time. `Update(deltaTime)` advances time by that amount AND evaluates. Use Evaluate() for scrubbing, Update() for custom time progression.

**Q: Why doesn't PlayableDirector have a speed property?**
A: Speed is a PlayableGraph concept, not a director concept. Timeline's director is a thin wrapper; the actual playback engine is the PlayableGraph.

**Q: Is reverse playback limited to Animation Tracks?**
A: No, reverse works for all track types. However, some playables may not support reverse correctly (depends on implementation).

---

## Homework / Thought Exercises

### Exercise 1: Frame-By-Frame Debugging
**Scenario**: You need to inspect each frame of a complex animation sequence.

**Question**: Write a function that advances exactly one frame at a time, accounting for variable framerates.

**Answer**:
```csharp
public void StepOneFrame()
{
    director.Pause();
    float deltaTime = 1f / (float)Application.targetFrameRate;
    director.time += deltaTime;
    director.Evaluate();
}
```

**Follow-up**: What if `Application.targetFrameRate` is 0 (unlimited)?

### Exercise 2: The "Jump" Problem
**Scenario**: A student's seek button causes the timeline to jump to random positions.

**Question**: What's wrong with this code?
```csharp
public void SeekToTime(float t)
{
    director.time = t;
    director.Evaluate();
}
```

**Answer**: Missing `Pause()` call. If Timeline is playing, the automatic time advancement conflicts with the manual seek, causing unpredictable jumps.

### Exercise 3: Speed Preset System
**Scenario**: Implement a speed preset system with options: Stop, Slow (0.25x), Normal (1x), Fast (2x), Very Fast (5x).

**Question**: How would you structure this to avoid repeated `GetRootPlayable()` calls?

**Answer**: Cache the rootPlayable in OnEnable to avoid repeated lookups. Store presets in a Dictionary mapping speed values to names.

### Exercise 4: Scrubbing Performance
**Scenario**: A timeline scrubber updates 60 times per second while dragging.

**Question**: Is it efficient to call Pause() -> SetTime() -> Evaluate() on every drag event?

**Answer**: Yes, but optimize by:
* Pausing once at drag start, not every frame
* Only calling Evaluate() every N frames during rapid scrubbing
* Using a dirty flag to batch updates
* Calling Evaluate() in LateUpdate() instead of every event

### Exercise 5: Reverse Playback Edge Cases
**Scenario**: You're implementing a reverse playback feature for a tutorial system.

**Question**: What happens when reverse playback reaches time = 0? Does it stop or continue into negative time?

**Answer**: Timeline continues into negative time but no clips evaluate. You need to manually check:
```csharp
if (director.time <= 0f && rootPlayable.GetSpeed() < 0f)
{
    director.Pause();
    director.time = 0f;
}
```

**Follow-up**: How would you implement "loop reverse" (0 -> end -> 0 -> end)?

---

## Extension Topics

* **Time Curves**: Non-linear time progression (ease-in/ease-out scrubbing)
* **Marker-Based Seeking**: Jumping to specific markers instead of absolute time
* **Multi-Timeline Sync**: Coordinating seek operations across multiple directors
* **Smooth Speed Transitions**: Fading between speed values instead of instant changes
* **State Preservation**: Remembering playback position across scene loads
