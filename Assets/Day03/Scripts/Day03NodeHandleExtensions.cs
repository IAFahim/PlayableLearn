using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day03
{
    public static class Day03NodeHandleExtensions
    {
        /// <summary>
        /// Initializes a new ScriptPlayable node with the Day03EmptyBehaviour.
        /// </summary>
        public static void Initialize(ref this Day03NodeHandle handle, in PlayableGraph graph, string nodeName)
        {
            if (handle.IsActive)
            {
                Debug.LogWarning($"[NodeHandle] Already initialized: {nodeName}");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[NodeHandle] Cannot initialize node: Graph is invalid.");
                return;
            }

            // Create the ScriptPlayable with Day03EmptyBehaviour
            ScriptPlayableOps.Create<Day03EmptyBehaviour>(in graph, 0, out handle.Node);

            if (ScriptPlayableOps.IsValid(in handle.Node))
            {
                handle.Graph = graph;
                handle.IsActive = true;
                handle.NodeId = nodeName.GetHashCode();
                Debug.Log($"[NodeHandle] Created node: {nodeName}");
            }
            else
            {
                Debug.LogError($"[NodeHandle] Failed to create node: {nodeName}");
            }
        }

        /// <summary>
        /// Disposes the node, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day03NodeHandle handle)
        {
            if (!handle.IsActive) return;

            ScriptPlayableOps.Destroy(in handle.Graph, in handle.Node);
            handle.IsActive = false;
            handle.NodeId = 0;
            Debug.Log("[NodeHandle] Node disposed.");
        }

        /// <summary>
        /// Connects this node to a playable output.
        /// This is the key feature for Day 03 - linking nodes to outputs.
        /// </summary>
        public static void ConnectToOutput(this in Day03NodeHandle handle, in PlayableOutput output)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[NodeHandle] Cannot connect: Node is not active.");
                return;
            }

            if (!output.IsOutputValid())
            {
                Debug.LogWarning("[NodeHandle] Cannot connect: Output is not valid.");
                return;
            }

            ScriptPlayableOps.SetSource(in output, in handle.Node);
            Debug.Log("[NodeHandle] Connected node to output.");
        }

        /// <summary>
        /// Checks if the node is valid and active.
        /// </summary>
        public static bool IsValidNode(this in Day03NodeHandle handle)
        {
            return handle.IsActive && ScriptPlayableOps.IsValid(in handle.Node);
        }

        /// <summary>
        /// Gets the node type as an enum.
        /// </summary>
        public static bool TryGetNodeType(this in Day03NodeHandle handle, out PlayableType nodeType)
        {
            if (!handle.IsActive || !ScriptPlayableOps.IsValid(in handle.Node))
            {
                nodeType = PlayableType.Null;
                return false;
            }

            ScriptPlayableOps.GetPlayableType(in handle.Node, out nodeType);
            return true;
        }

        /// <summary>
        /// Gets information about the node's ports.
        /// </summary>
        public static bool TryGetPortInfo(this in Day03NodeHandle handle, out int inputCount, out int outputCount)
        {
            if (!handle.IsActive || !ScriptPlayableOps.IsValid(in handle.Node))
            {
                inputCount = 0;
                outputCount = 0;
                return false;
            }

            ScriptPlayableOps.GetInputCount(in handle.Node, out inputCount);
            ScriptPlayableOps.GetOutputCount(in handle.Node, out outputCount);
            return true;
        }

        /// <summary>
        /// Logs the node information to the console.
        /// This demonstrates that the node exists and is properly configured.
        /// </summary>
        public static void LogToConsole(this in Day03NodeHandle handle, string nodeName)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning($"[NodeHandle] Cannot log: Node is not active.");
                return;
            }

            if (!ScriptPlayableOps.IsValid(in handle.Node))
            {
                Debug.LogWarning($"[NodeHandle] Node '{nodeName}' is not valid.");
                return;
            }

            if (TryGetPortInfo(in handle, out int inputCount, out int outputCount))
            {
                Debug.Log($"[ScriptNode] Name: {nodeName}, Inputs: {inputCount}, Outputs: {outputCount}");
            }
        }
    }
}
