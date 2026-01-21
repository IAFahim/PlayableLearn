# PlayableLearn - Day 07: The Reverse Time

## Overview
Day 07 introduces Reverse Time functionality with negative speed support and time wrapping. This demonstrates how to implement reverse playback and cyclic time behavior in the PlayableGraph system.

## What You'll Learn
- How to use negative speed values for reverse playback
- Understanding time direction (forward vs reverse)
- Implementing time wrapping for cyclic behavior
- Tracking accumulated time across direction changes
- Detecting direction transitions
- Visualizing reverse time state

## Files Structure
```
Assets/Day07/Scripts/
├── Day07.asmdef                        # Assembly definition
├── Day07Entry.cs                       # MonoBehaviour entry point
├── Day07ReverseData.cs                 # Data layer: Reverse time control handle
├── Day07ReverseExtensions.cs           # Adapter layer: Reverse time operations
└── ReverseTimeOps.cs                   # Operations layer: Burst-compiled reverse time ops
```

## The Three-Layer Architecture

### Layer A: Data (Day07ReverseData)
Pure data structure with no logic:
- `PlayableGraph Graph` - The graph being controlled
- `bool IsActive` - Local state tracking
- `int ControllerId` - Debug identifier
- `bool EnableReverseTime` - Whether reverse time is enabled
- `bool EnableTimeWrapping` - Whether time wrapping is enabled
- `double AccumulatedTime` - Accumulated time tracking (can be negative)
- `double WrapLimit` - Time wrapping limit in seconds
- `double LastKnownSpeed` - Speed reference for direction tracking
- `bool IsValidControl` - Whether the control is valid

### Layer B: Operations (ReverseTimeOps)
Burst-compiled static methods for reverse time operations:
- `IsReversing()` - Checks if speed is negative
- `IsForward()` - Checks if speed is positive
- `IsStopped()` - Checks if speed is near zero
- `ClampSpeed()` - Clamps speed within bounds
- `WrapTime()` - Wraps accumulated time within limit
- `UpdateAccumulatedTime()` - Updates time based on delta time and speed
- `DidDirectionChange()` - Detects direction changes
- `IsTransitionToReverse()` - Checks if transitioning to reverse
- `IsTransitionToForward()` - Checks if transitioning to forward
- `GetSpeedMagnitude()` - Gets absolute speed value
- `GetSpeedDirection()` - Gets direction as -1, 0, or 1
- `CalculateNormalizedProgress()` - Gets normalized time progress
- `CalculateWrapOverflow()` - Calculates time wrap overflow

### Layer C: Extensions (Day07ReverseExtensions)
Public API that combines data and operations:
- `Initialize()` - Creates reverse time control
- `Dispose()` - Cleans up control
- `UpdateTimeTracking()` - Updates accumulated time
- `SetCurrentSpeed()` - Sets current speed for tracking
- `IsReversing()` - Checks if time is flowing backward
- `IsForward()` - Checks if time is flowing forward
- `IsStopped()` - Checks if time is stopped
- `GetAccumulatedTime()` - Gets accumulated time
- `SetWrapLimit()` - Sets time wrapping limit
- `ClampSpeed()` - Clamps speed within bounds
- `GetSpeedMagnitude()` - Gets speed magnitude
- `GetSpeedDirection()` - Gets direction as integer
- `GetDirectionSymbol()` - Gets direction symbol (>>, <<, ||)
- `GetNormalizedProgress()` - Gets normalized time progress
- `SetReverseEnabled()` - Enables/disables reverse time
- `SetTimeWrappingEnabled()` - Enables/disables time wrapping
- `ResetTime()` - Resets accumulated time to zero
- `IsNearWrapLimit()` - Checks if near wrap limit
- `LogReverseInfo()` - Debug logging

## Key Concepts

### Negative Speed
- Speed values < 0 indicate reverse time flow
- Speed values > 0 indicate forward time flow
- Speed values ≈ 0 indicate stopped time
- The magnitude of negative speed determines reverse playback rate

### Time Wrapping
- Time can wrap within a specified limit (e.g., 10 seconds)
- Wrapping occurs from +limit to -limit and vice versa
- Useful for creating cyclic or looping behaviors
- Can be enabled/disabled independently

### Accumulated Time
- Tracks total time elapsed, including negative values
- Can go negative when time is reversing
- Wraps around when time wrapping is enabled
- Useful for tracking total playback time regardless of direction

### Direction Transitions
- Forward → Reverse: Speed changed from positive to negative
- Reverse → Forward: Speed changed from negative to positive
- These transitions can be detected for triggering events

## Usage Example

```csharp
// Initialize graph and previous days' components
Day01GraphHandle graphHandle;
graphHandle.Initialize("MyGraph");

// Initialize speed control
Day05SpeedData speedData;
speedData.Initialize(in graphHandle.Graph, "SpeedControl", 1.0f, true, 2.0f);

// Initialize reverse time control
Day07ReverseData reverseData;
reverseData.Initialize(in graphHandle.Graph, "ReverseTimeControl", true, true);

// Set wrap limit to 10 seconds
reverseData.SetWrapLimit(10.0);

// Update time tracking (call in Update)
reverseData.UpdateTimeTracking(Time.deltaTime);

// Update speed tracking
reverseData.SetCurrentSpeed(speedData.GetCurrentSpeed());

// Check if reversing
if (reverseData.IsReversing())
{
    Debug.Log("Time is flowing backward!");
}

// Check if forward
if (reverseData.IsForward())
{
    Debug.Log("Time is flowing forward!");
}

// Get accumulated time
double totalTime = reverseData.GetAccumulatedTime();
Debug.Log($"Accumulated time: {totalTime:F2}s");

// Clamp speed to allow reverse playback
float clampedSpeed = reverseData.ClampSpeed(-2.0f, 2.0f, targetSpeed);
speedData.SetTargetSpeed(clampedSpeed);

// Get normalized progress (0.0 to 1.0)
float progress = reverseData.GetNormalizedProgress();

// Get direction symbol
string direction = reverseData.GetDirectionSymbol(); // ">>", "<<", or "||"
```

## Visual Feedback
Day 07 adds visual feedback to represent reverse time:
- **Forward time**: Green color
- **Reverse time**: Magenta color
- **Paused**: Red color
- **GUI Controls**: On-screen toggle button for reverse mode
- **Status Display**: Shows direction, accumulated time, and speed

## Previous Days
- **Day 01**: Created and destroyed PlayableGraph
- **Day 02**: Added ScriptPlayableOutput for console communication
- **Day 03**: Created and linked the first ScriptPlayable node
- **Day 04**: Added the update cycle using ProcessFrame
- **Day 05**: Implemented SetSpeed manipulation for playback control
- **Day 06**: Added PlayState control for playing and stopping the graph

## Testing
Run the Unity Test Runner to verify:
- Reverse time control initializes correctly
- Negative speed values work correctly
- Time wrapping functions as expected
- Direction transitions are detected properly
- Accumulated time tracks correctly
- Complete integration with previous days

## Notes
- Day 07 builds upon all previous days (01-06)
- Reverse time requires speed control (Day 05) to be initialized
- Time wrapping is independent of reverse time enablement
- Use UpdateTimeTracking() in your Update loop to track accumulated time
- Use SetCurrentSpeed() to update direction tracking
- Direction changes are useful for triggering events
- Negative speeds can be clamped using ClampSpeed()
- Time wrapping creates cyclic behavior within the specified limit
