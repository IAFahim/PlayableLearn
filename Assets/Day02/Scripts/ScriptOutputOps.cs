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
        public static void Destroy(in PlayableGraph graph, in PlayableOutput output)
        {
            if (output.IsOutputValid() && graph.IsValid())
            {
                graph.DestroyOutput(output);
            }
        }

        /// <summary>
        /// Checks if the output is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(in PlayableOutput output)
        {
            return output.IsOutputValid();
        }

        /// <summary>
        /// Gets the output type as a string for logging.
        /// Since this is a ScriptPlayableOutput, we know the type is "ScriptOutput".
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetOutputType(in PlayableOutput output)
        {
            if (!output.IsOutputValid())
            {
                return "Null";
            }

            // PlayableOutput doesn't expose a type property directly
            // Since we're creating ScriptPlayableOutput, we know the type
            return "ScriptOutput";
        }

        /// <summary>
        /// Logs the output information to console.
        /// This is the main functionality for Day 02 - talking to the console.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogToConsole(in PlayableOutput output, in string outputName)
        {
            if (!output.IsOutputValid())
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
