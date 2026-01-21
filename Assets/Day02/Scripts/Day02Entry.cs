using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day02
{
    /// <summary>
    /// MonoBehaviour entry point for Day 02: The Output.
    /// Demonstrates creating a ScriptPlayableOutput and communicating with the console.
    /// Follows the same layered architecture as Day 01:
    /// - Layer A: Data (Day02OutputHandle)
    /// - Layer B: Operations (ScriptOutputOps)
    /// - Layer C: Extensions (Day02OutputHandleExtensions)
    /// </summary>
    public class Day02Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day02OutputHandle day02OutputHandle;

        // Graph reference for creating the output
        private PlayableGraph graph;
        private bool isGraphInitialized;

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
            // Visual feedback: Pulse size based on time
            float scale = 1.0f + Mathf.Sin(Time.time * 2.0f) * 0.2f;
            transform.localScale = Vector3.one * scale;
        }

        private void InitializeGraph()
        {
            if (isGraphInitialized)
            {
                Debug.LogWarning("[Day 02] Graph already initialized.");
                return;
            }

            string graphName = $"{gameObject.name}_Graph_{Time.frameCount}";
            graph = PlayableGraph.Create(graphName);
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            isGraphInitialized = true;

            Debug.Log($"[Day 02] Graph created: {graphName}");
        }

        private void InitializeOutput()
        {
            if (!isGraphInitialized)
            {
                Debug.LogError("[Day 02] Cannot initialize output: Graph is not initialized.");
                return;
            }

            string outputName = $"{gameObject.name}_ConsoleOutput";
            day02OutputHandle.Initialize(in graph, outputName);
        }

        private void LogOutputInfo()
        {
            if (!day02OutputHandle.IsValidOutput())
            {
                Debug.LogWarning("[Day 02] Output is not valid, cannot log info.");
                return;
            }

            // Demonstrate console communication
            day02OutputHandle.LogToConsole($"{gameObject.name}_ConsoleOutput");

            // Also log the output type
            if (day02OutputHandle.TryGetOutputType(out string outputType))
            {
                Debug.Log($"[Day 02] Output Type: {outputType}");
            }
        }

        private void CleanupOutput()
        {
            day02OutputHandle.Dispose();
        }

        private void CleanupGraph()
        {
            if (!isGraphInitialized) return;

            if (graph.IsValid())
            {
                graph.Destroy();
            }

            isGraphInitialized = false;
            Debug.Log("[Day 02] Graph destroyed.");
        }
    }
}
