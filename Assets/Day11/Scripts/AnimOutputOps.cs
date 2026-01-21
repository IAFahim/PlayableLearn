using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day11
{
    /// <summary>
    /// Burst-compiled operations for AnimationPlayableOutput.
    /// Layer B: Operations - Pure functions using Burst for performance.
    /// This is the "Engine" layer - low-level operations on AnimationPlayableOutput.
    /// </summary>
    [BurstCompile]
    public static class AnimOutputOps
    {
        /// <summary>
        /// Creates an AnimationPlayableOutput from the given graph.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Create(in PlayableGraph graph, in string outputName, out PlayableOutput output)
        {
            if (!graph.IsValid())
            {
                output = PlayableOutput.Null;
                return;
            }

            output = AnimationPlayableOutput.Create(graph, outputName);
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
        /// Since this is an AnimationPlayableOutput, we know the type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetOutputType(in PlayableOutput output, out AnimOutputType result)
        {
            if (!output.IsOutputValid())
            {
                result = AnimOutputType.Null;
                return;
            }

            result = AnimOutputType.AnimationOutput;
        }

        /// <summary>
        /// Checks if the output is valid and returns both validity and type.
        /// Combined operation for efficiency.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetOutputInfo(in PlayableOutput output, out AnimOutputType type, out bool isValid)
        {
            isValid = output.IsOutputValid();
            GetOutputType(in output, out type);
        }
    }

    /// <summary>
    /// Animation output type enumeration for Burst-safe type checking.
    /// </summary>
    public enum AnimOutputType
    {
        Null,
        AnimationOutput
    }
}
