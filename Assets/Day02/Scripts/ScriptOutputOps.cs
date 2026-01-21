using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day02
{
    [BurstCompile]
    public static class ScriptOutputOps
    {
        /// <summary>
        /// Creates a ScriptPlayableOutput from the given graph.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Create(in PlayableGraph graph, in string outputName, out PlayableOutput output)
        {
            if (!graph.IsValid())
            {
                output = PlayableOutput.Null;
                return;
            }

            output = ScriptPlayableOutput.Create(graph, outputName);
        }

        /// <summary>
        /// Destroys the given output.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(in PlayableOutput output)
        {
            if (output.IsValid())
            {
                output.Destroy();
            }
        }

        /// <summary>
        /// Checks if the output is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(in PlayableOutput output)
        {
            return output.IsValid();
        }

        /// <summary>
        /// Gets the output type as a string for logging.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetOutputType(in PlayableOutput output)
        {
            if (!output.IsValid())
            {
                return "Null";
            }

            return output.GetOutputType().ToString();
        }

        /// <summary>
        /// Logs the output information to console.
        /// This is the main functionality for Day 02 - talking to the console.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogToConsole(in PlayableOutput output, in string outputName)
        {
            if (!output.IsValid())
            {
                DebugLogger.Log($"[ScriptOutput] Output '{outputName}' is not valid.");
                return;
            }

            string outputType = GetOutputType(in output);
            DebugLogger.Log($"[ScriptOutput] Name: {outputName}, Type: {outputType}, IsPlayableOutput: {output.IsOutputValid()}");
        }

        /// <summary>
        /// Simple debug logger that writes to console.
        /// Using a separate logger to maintain Burst compatibility where possible.
        /// </summary>
        private static class DebugLogger
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Log(in string message)
            {
                UnityEngine.Debug.Log(message);
            }
        }
    }
}
