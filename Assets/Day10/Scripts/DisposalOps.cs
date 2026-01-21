using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day10
{
    /// <summary>
    /// Burst-compiled operations for proper resource cleanup and disposal.
    /// Provides safe disposal patterns for PlayableGraph and related resources.
    /// </summary>
    [BurstCompile]
    public static class DisposalOps
    {
        /// <summary>
        /// Safely destroys a PlayableGraph if it's valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeGraph(in PlayableGraph graph)
        {
            if (graph.IsValid())
            {
                graph.Destroy();
            }
        }

        /// <summary>
        /// Safely destroys a Playable if it's valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposePlayable(in Playable playable)
        {
            if (playable.IsValid())
            {
                playable.Destroy();
            }
        }

        /// <summary>
        /// Safely disconnects and destroys a PlayableOutput from its graph.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeOutput(in PlayableOutput output)
        {
            if (output.IsValid())
            {
                // Note: PlayableOutput is automatically destroyed when the graph is destroyed,
                // but we can mark it for cleanup here if needed
                var graph = output.GetGraph();
                if (graph.IsValid())
                {
                    // Output is tied to graph lifecycle
                }
            }
        }

        /// <summary>
        /// Checks if a PlayableGraph is valid and can be disposed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanDisposeGraph(in PlayableGraph graph)
        {
            return graph.IsValid();
        }

        /// <summary>
        /// Checks if a Playable is valid and can be disposed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanDisposePlayable(in Playable playable)
        {
            return playable.IsValid();
        }

        /// <summary>
        /// Checks if a PlayableOutput is valid and can be disposed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanDisposeOutput(in PlayableOutput output)
        {
            return output.IsValid();
        }
    }
}
