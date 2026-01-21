using UnityEngine;
using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day01
{
    // Burst optimization not strictly necessary for API calls,
    // but we maintain the habit for future calculation methods.
    [BurstCompile]
    public static class Day01GraphLifecycleLogic
    {
        /// <summary>
        /// Allocates the graph in the C++ engine.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateGraph(in string debugName, out PlayableGraph newGraph)
        {
            newGraph = PlayableGraph.Create(debugName);

            // Critical for seeing the graph in the "PlayableGraph Visualizer" tool
            newGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        }

        /// <summary>
        /// Destroys the graph to prevent memory leaks.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGraph(in PlayableGraph graphToDestroy)
        {
            if (graphToDestroy.IsValid())
            {
                graphToDestroy.Destroy();
            }
        }

        /// <summary>
        /// Toggles the play state.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPlayState(in PlayableGraph graph, in bool shouldPlay)
        {
            if (!graph.IsValid()) return;

            if (shouldPlay)
            {
                graph.Play();
            }
            else
            {
                graph.Stop();
            }
        }
    }
}
