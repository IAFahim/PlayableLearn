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
        /// Gets the output type as an enum for identification.
        /// Since this is a ScriptPlayableOutput, we know the type.
        /// Uses out parameter per protocol - no return values from Ops.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetOutputType(in PlayableOutput output, out OutputType result)
        {
            if (!output.IsOutputValid())
            {
                result = OutputType.Null;
                return;
            }

            // Since we're creating ScriptPlayableOutput, we know the type
            result = OutputType.ScriptOutput;
        }

        /// <summary>
        /// Checks if the output is valid and returns both validity and type.
        /// Combined operation for efficiency.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetOutputInfo(in PlayableOutput output, out OutputType type, out bool isValid)
        {
            isValid = output.IsOutputValid();
            GetOutputType(in output, out type);
        }
    }

    /// <summary>
    /// Output type enumeration for Burst-safe type checking.
    /// </summary>
    public enum OutputType
    {
        Null,
        ScriptOutput
    }
}
