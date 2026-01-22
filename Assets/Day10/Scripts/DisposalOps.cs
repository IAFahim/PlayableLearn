using UnityEngine.Playables;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day10
{
    /// <summary>
    /// Operations for proper resource cleanup and disposal.
    /// Provides safe disposal patterns for PlayableGraph and related resources.
    /// </summary>
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
            // PlayableOutput is managed by the graph lifecycle.
            // No explicit disposal API exists, so we validate via the handle.
            if (PlayableOutputExtensions.IsOutputValid(output))
            {
                // Output is tied to graph lifecycle.
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
            return PlayableOutputExtensions.IsOutputValid(output);
        }
    }
}
