using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;

namespace PlayableLearn.Day02
{
    /// <summary>
    /// MonoBehaviour entry point for Day 02: The Output.
    /// Demonstrates creating a ScriptPlayableOutput and communicating with the console.
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day02OutputHandle, Day01GraphHandle)
    /// - Layer B: Operations (ScriptOutputOps)
    /// - Layer C: Extensions (Day02OutputHandleExtensions, Day01GraphHandleExtensions)
    /// </summary>
    public class Day02Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day01GraphHandle graphHandle;

        [SerializeField]
        private Day02OutputHandle outputHandle;

        // LAYER C: Adapter Calls
        private void OnEnable()
        {
            InitializeGraph();
            InitializeOutput();
            LogOutputInfo();
        }

        private void OnDisable()
        {
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
                Debug.LogError("[Day 02] Cannot initialize output: Graph is not initialized.");
                return;
            }

            string outputName = $"{gameObject.name}_ConsoleOutput";

            // Get the raw graph from the handle for output creation
            // This is acceptable since Output requires a Graph reference
            PlayableGraph graph = graphHandle.Graph;
            var output = outputHandle;
            output.Initialize(in graph, outputName);
            outputHandle = output;
        }

        private void LogOutputInfo()
        {
            if (!outputHandle.IsValidOutput())
            {
                Debug.LogWarning("[Day 02] Output is not valid, cannot log info.");
                return;
            }

            // Demonstrate console communication
            outputHandle.LogToConsole($"{gameObject.name}_ConsoleOutput");

            // Also log the output type
            if (outputHandle.TryGetOutputType(out OutputType outputType))
            {
                Debug.Log($"[Day 02] Output Type: {outputType}");
            }
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
