using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day02;

namespace PlayableLearn.Day03
{
    /// <summary>
    /// MonoBehaviour entry point for Day 03: The First Node.
    /// Demonstrates creating a ScriptPlayable and linking it to an output.
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day03NodeHandle, Day02OutputHandle, Day01GraphHandle)
    /// - Layer B: Operations (ScriptPlayableOps)
    /// - Layer C: Extensions (Day03NodeHandleExtensions, Day02OutputHandleExtensions, Day01GraphHandleExtensions)
    /// </summary>
    public class Day03Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day01GraphHandle graphHandle;

        [SerializeField]
        private Day02OutputHandle outputHandle;

        [SerializeField]
        private Day03NodeHandle nodeHandle;

        // LAYER C: Adapter Calls
        private void OnEnable()
        {
            InitializeGraph();
            InitializeOutput();
            InitializeNode();
            LinkNodeToOutput();
            LogNodeInfo();
        }

        private void OnDisable()
        {
            CleanupNode();
            CleanupOutput();
            CleanupGraph();
        }

        private void Update()
        {
            // PURE VIEW LOGIC
            // Visual feedback: Pulse size based on time
            float scale = 1.0f + Mathf.Sin(Time.time * 2.0f) * 0.2f;
            transform.localScale = Vector3.one * scale;
        }

        private void InitializeGraph()
        {
            string graphName = $"{gameObject.name}_Graph_{Time.frameCount}";
            var graph = graphHandle;
            graph.Initialize(graphName);
            graphHandle = graph;
        }

        private void InitializeOutput()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 03] Cannot initialize output: Graph is not initialized.");
                return;
            }

            string outputName = $"{gameObject.name}_ConsoleOutput";

            // Get the raw graph from the handle for output creation
            PlayableGraph graph = graphHandle.Graph;
            var output = outputHandle;
            output.Initialize(in graph, outputName);
            outputHandle = output;
        }

        private void InitializeNode()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 03] Cannot initialize node: Graph is not initialized.");
                return;
            }

            string nodeName = $"{gameObject.name}_EmptyNode";
            PlayableGraph graph = graphHandle.Graph;
            var node = nodeHandle;
            node.Initialize(in graph, nodeName);
            nodeHandle = node;
        }

        private void LinkNodeToOutput()
        {
            if (!nodeHandle.IsValidNode())
            {
                Debug.LogError("[Day 03] Cannot link: Node is not valid.");
                return;
            }

            if (!outputHandle.IsValidOutput())
            {
                Debug.LogError("[Day 03] Cannot link: Output is not valid.");
                return;
            }

            // Get the raw output for linking
            PlayableOutput output = outputHandle.Output;
            nodeHandle.ConnectToOutput(in output);
            Debug.Log("[Day 03] Successfully linked node to output!");
        }

        private void LogNodeInfo()
        {
            if (!nodeHandle.IsValidNode())
            {
                Debug.LogWarning("[Day 03] Node is not valid, cannot log info.");
                return;
            }

            string nodeName = $"{gameObject.name}_EmptyNode";
            nodeHandle.LogToConsole(nodeName);

            // Also log the node type
            if (nodeHandle.TryGetNodeType(out PlayableType nodeType))
            {
                Debug.Log($"[Day 03] Node Type: {nodeType}");
            }
        }

        private void CleanupNode()
        {
            var node = nodeHandle;
            node.Dispose();
            nodeHandle = node;
        }

        private void CleanupOutput()
        {
            var output = outputHandle;
            output.Dispose();
            outputHandle = output;
        }

        private void CleanupGraph()
        {
            var graph = graphHandle;
            graph.Dispose();
            graphHandle = graph;
        }
    }
}
