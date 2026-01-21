using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day02
{
    public static class Day02OutputHandleExtensions
    {
        /// <summary>
        /// Initializes a new ScriptPlayableOutput from the given graph.
        /// </summary>
        public static void Initialize(ref this Day02OutputHandle handle, in PlayableGraph graph, string outputName)
        {
            if (handle.IsActive)
            {
                Debug.LogWarning($"[OutputHandle] Already initialized: {outputName}");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[OutputHandle] Cannot initialize output: Graph is invalid.");
                return;
            }

            ScriptOutputOps.Create(in graph, in outputName, out handle.Output);

            if (ScriptOutputOps.IsValid(in handle.Output))
            {
                handle.IsActive = true;
                handle.OutputId = outputName.GetHashCode();
                Debug.Log($"[OutputHandle] Created output: {outputName}");
            }
            else
            {
                Debug.LogError($"[OutputHandle] Failed to create output: {outputName}");
            }
        }

        /// <summary>
        /// Disposes the output, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day02OutputHandle handle)
        {
            if (!handle.IsActive) return;

            ScriptOutputOps.Destroy(in handle.Output);
            handle.IsActive = false;
            handle.OutputId = 0;
            Debug.Log("[OutputHandle] Output disposed.");
        }

        /// <summary>
        /// Logs the output information to the console.
        /// This is the main feature for Day 02 - demonstrating output communication.
        /// </summary>
        public static void LogToConsole(this in Day02OutputHandle handle, string outputName)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning($"[OutputHandle] Cannot log: Output is not active.");
                return;
            }

            ScriptOutputOps.LogToConsole(in handle.Output, in outputName);
        }

        /// <summary>
        /// Gets the output type as a string.
        /// </summary>
        public static bool TryGetOutputType(this in Day02OutputHandle handle, out string outputType)
        {
            if (!handle.IsActive || !ScriptOutputOps.IsValid(in handle.Output))
            {
                outputType = "Invalid";
                return false;
            }

            outputType = ScriptOutputOps.GetOutputType(in handle.Output);
            return true;
        }

        /// <summary>
        /// Checks if the output is valid and active.
        /// </summary>
        public static bool IsValidOutput(this in Day02OutputHandle handle)
        {
            return handle.IsActive && ScriptOutputOps.IsValid(in handle.Output);
        }
    }
}
