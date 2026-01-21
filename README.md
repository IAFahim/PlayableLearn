# PlayableLearn - Day 03: The First Node

## Overview
Day 03 introduces the creation and linking of the first ScriptPlayable node. This demonstrates how to create nodes in the PlayableGraph and connect them to outputs.

## What You'll Learn
- How to create a ScriptPlayable node
- Understanding PlayableBehaviour
- Linking nodes to outputs
- The relationship between Graph, Output, and Nodes

## Files Structure
```
Assets/Day03/Scripts/
├── Day03.asmdef                    # Assembly definition
├── Day03Entry.cs                   # MonoBehaviour entry point
├── Day03NodeHandle.cs              # Data layer: Node handle
├── Day03NodeHandleExtensions.cs    # Adapter layer: Node operations
├── ScriptPlayableOps.cs            # Operations layer: Burst-compiled node ops
└── Day03EmptyBehaviour.cs          # Empty PlayableBehaviour for demonstration
```

## The Three-Layer Architecture

### Layer A: Data (Day03NodeHandle)
Pure data structure with no logic:
- `PlayableGraph Graph` - The owning graph
- `Playable Node` - The raw Unity Playable
- `bool IsActive` - Local state tracking
- `int NodeId` - Debug identifier

### Layer B: Operations (ScriptPlayableOps)
Burst-compiled static methods for node operations:
- `Create<T>()` - Creates a ScriptPlayable with specified behaviour
- `Destroy()` - Destroys a playable node
- `IsValid()` - Checks if playable is valid
- `Connect()` - Connects playables together
- `SetSource()` - Sets playable as output source
- `GetBehaviour()` - Retrieves the behaviour instance

### Layer C: Extensions (Day03NodeHandleExtensions)
Public API that combines data and operations:
- `Initialize()` - Creates a new node
- `Dispose()` - Cleans up the node
- `ConnectToOutput()` - Links node to output
- `IsValidNode()` - Validation check
- `LogToConsole()` - Debug logging

## Key Concepts

### ScriptPlayable
A `ScriptPlayable` is a type of Playable that uses a `PlayableBehaviour` for its logic. Unlike other playable types (AnimationClipPlayable, AudioMixerPlayable, etc.), ScriptPlayables allow you to define custom behavior.

### PlayableBehaviour
The `PlayableBehaviour` class is where your custom logic goes:
- `OnBehaviourPlay()` - Called when the playable starts
- `OnBehaviourPause()` - Called when the playable pauses
- `PrepareFrame()` - Called every frame for updates
- `OnPlayableDestroy()` - Called when the playable is destroyed

### Node Linking
Nodes must be linked to outputs to have any effect:
1. Create a graph
2. Create an output from the graph
3. Create a node from the graph
4. Set the node as the output's source

## Usage Example

```csharp
// Initialize graph
Day01GraphHandle graphHandle;
graphHandle.Initialize("MyGraph");

// Initialize output
Day02OutputHandle outputHandle;
outputHandle.Initialize(in graphHandle.Graph, "MyOutput");

// Initialize node
Day03NodeHandle nodeHandle;
nodeHandle.Initialize(in graphHandle.Graph, "MyNode");

// Link node to output
nodeHandle.ConnectToOutput(in outputHandle.Output);
```

## Previous Days
- **Day 01**: Created and destroyed PlayableGraph
- **Day 02**: Added ScriptPlayableOutput for console communication

## Next Steps
- **Day 04**: Will add the update cycle using ProcessFrame

## Testing
Run the Unity Test Runner to verify:
- Node creation succeeds
- Node connects to output correctly
- Node disposal cleans up resources
- Port information is accurate

## Notes
- Day 03 uses an empty behaviour to prove nodes can be created and linked
- Future days will add actual logic to the behaviours
- The graph structure is: Graph → Output → Node
