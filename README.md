# PlayableLearn - Day 08: The Director Name

## Overview
Day 08 introduces Graph Naming functionality for debugging in the Unity Profiler. This demonstrates how to assign meaningful names to PlayableGraphs, making them easier to identify and debug in the Unity Profiler window.

## What You'll Learn
- How to name PlayableGraphs for Profiler visibility
- Understanding graph naming and sanitization
- Implementing dynamic graph renaming based on state
- Tracking graph name changes
- Validating graph names
- Using named graphs in the Unity Profiler

## Files Structure
```
Assets/Day08/Scripts/
├── Day08.asmdef                        # Assembly definition
├── Day08Entry.cs                       # MonoBehaviour entry point
├── Day08NamedGraphData.cs              # Data layer: Named graph handle
├── Day08NamedGraphExtensions.cs        # Adapter layer: Graph naming operations
└── GraphNamingOps.cs                   # Operations layer: Burst-compiled naming ops
```

## The Three-Layer Architecture

### Layer A: Data (Day08NamedGraphData)
Pure data structure with no logic:
- `PlayableGraph Graph` - The graph being named
- `string GraphName` - The current name of the graph
- `string PreviousName` - The previous name of the graph
- `bool IsActive` - Local state tracking
- `int ControllerId` - Debug identifier
- `bool IsValidControl` - Whether the control is valid
- `bool IsNameSet` - Whether the name has been set
- `int NameChangeCount` - Number of name changes

### Layer B: Operations (GraphNamingOps)
Burst-compiled static methods for graph naming operations:
- `IsValidName()` - Checks if a graph name is valid
- `AreNamesEqual()` - Checks if two graph names are equal (case-insensitive)
- `HasNameChanged()` - Checks if the name has changed
- `GenerateUniqueName()` - Generates a unique graph name with counter
- `SanitizeName()` - Sanitizes a graph name by removing invalid characters
- `IsNameTooLong()` - Checks if a graph name exceeds maximum length
- `TruncateName()` - Truncates a graph name to maximum length
- `FormatName()` - Formats a graph name with prefix and suffix
- `IncrementNameCount()` - Increments the name change count
- `IsValidControllerId()` - Validates a controller ID
- `GenerateControllerId()` - Generates a controller ID

### Layer C: Extensions (Day08NamedGraphExtensions)
Public API that combines data and operations:
- `Initialize()` - Creates named graph handle
- `Dispose()` - Cleans up named graph handle
- `SetName()` - Sets a new name for the graph
- `GetName()` - Gets the current name of the graph
- `GetPreviousName()` - Gets the previous name of the graph
- `HasNameChanged()` - Checks if the graph name has changed
- `GetNameChangeCount()` - Gets the number of name changes
- `SetFormattedName()` - Sets a formatted name with prefix and suffix
- `SetUniqueName()` - Sets a unique name with counter
- `RenameWithCounter()` - Renames the graph with a counter
- `IsValidControl()` - Checks if the named graph is valid
- `IsNameSet()` - Checks if the graph name is set
- `GetControllerId()` - Gets the controller ID
- `LogNamedGraphInfo()` - Debug logging
- `ResetControllerIdCounter()` - Resets the controller ID counter

## Key Concepts

### Graph Naming
- PlayableGraphs can be named for easier identification in the Profiler
- Names are sanitized to remove invalid characters
- Names are truncated to a maximum length (128 characters)
- Names are case-insensitive when comparing

### Name Sanitization
- Invalid characters are replaced with underscores
- Leading/trailing whitespace is removed
- Empty names after sanitization become "UnnamedGraph"
- Invalid characters: `< > : " / \ | ? *`

### Dynamic Renaming
- Graph names can be updated dynamically based on state
- Useful for showing current playback state in the Profiler
- Can combine state information (playing/paused, forward/reverse)
- Name changes are tracked with a counter

### Name Change Tracking
- Previous name is stored for reference
- Number of name changes is tracked
- Useful for debugging and auditing
- Can be reset for testing purposes

## Usage Example

```csharp
// Initialize graph
Day01GraphHandle graphHandle;
graphHandle.Initialize("MyGraph");

// Initialize named graph
Day08NamedGraphData namedGraphData;
namedGraphData.Initialize(in graphHandle.Graph, "MyPlayableGraph");

// Get the current name
string currentName = namedGraphData.GetName();
Debug.Log($"Graph name: {currentName}");

// Set a new name
namedGraphData.SetName("UpdatedGraphName");

// Check if name has changed
if (namedGraphData.HasNameChanged())
{
    Debug.Log($"Name changed from: {namedGraphData.GetPreviousName()}");
    Debug.Log($"Name change count: {namedGraphData.GetNameChangeCount()}");
}

// Set a formatted name with prefix and suffix
namedGraphData.SetFormattedName("Prefix", "BaseName", "Suffix");
// Result: "Prefix_BaseName_Suffix"

// Set a unique name with counter
namedGraphData.SetUniqueName("MyGraph", 5);
// Result: "MyGraph_5"

// Dynamic renaming based on state
string stateName = playStateData.IsPlaying() ? "Playing" : "Paused";
string directionName = reverseData.IsReversing() ? "Reverse" : "Forward";
string dynamicName = $"MyGraph_{stateName}_{directionName}";
namedGraphData.SetName(dynamicName);

// Log named graph info
namedGraphData.LogNamedGraphInfo("Current Graph State");
```

## Visual Feedback
Day 08 adds visual feedback to represent graph naming:
- **Named graph**: Cyan color
- **Forward time**: Green color
- **Reverse time**: Magenta color
- **Paused**: Red color
- **GUI Controls**: Displays graph name and controller ID
- **Gizmos**: Shows graph name in Scene view

## Profiler Integration
Named graphs appear in the Unity Profiler with their assigned names:
1. Open Unity Profiler (Window > Analysis > Profiler)
2. Enable the Profiler module
3. Play the game
4. Look for your graph name in the Profiler timeline
5. Named graphs are easier to identify and debug

## Previous Days
- **Day 01**: Created and destroyed PlayableGraph
- **Day 02**: Added ScriptPlayableOutput for console communication
- **Day 03**: Created and linked the first ScriptPlayable node
- **Day 04**: Added the update cycle using ProcessFrame
- **Day 05**: Implemented SetSpeed manipulation for playback control
- **Day 06**: Added PlayState control for playing and stopping the graph
- **Day 07**: Implemented reverse time functionality with negative speed

## Testing
Run the Unity Test Runner to verify:
- Named graph initializes correctly
- Graph names are sanitized properly
- Name changes are tracked correctly
- Dynamic renaming works as expected
- Controller IDs are assigned correctly
- Complete integration with previous days

## Notes
- Day 08 builds upon all previous days (01-07)
- Graph naming is essential for debugging in the Profiler
- Names are automatically sanitized and truncated
- Use meaningful names for better debugging experience
- Dynamic renaming can show runtime state in the Profiler
- Name changes are tracked for auditing purposes
- Controller IDs help identify multiple named graphs
