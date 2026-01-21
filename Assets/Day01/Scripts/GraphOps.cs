using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day01
{
    [BurstCompile]
    public static class GraphOps
    {
        /// <summary>
        /// Allocates the graph.
        /// Note: This specific method cannot be Bursted because PlayableGraph.Create
        /// touches the managed heap (string) and main thread APIs.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Create(in string debugName, out PlayableGraph newGraph)
        {
            newGraph = PlayableGraph.Create(debugName);
            newGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(in PlayableGraph graph)
        {
            if (graph.IsValid())
            {
                graph.Destroy();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(in PlayableGraph graph)
        {
            if (graph.IsValid()) graph.Play();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Stop(in PlayableGraph graph)
        {
            if (graph.IsValid()) graph.Stop();
        }

        /// <summary>
        /// Safely queries if the graph is valid and running.
        /// For Day 01, we don't have nodes yet, so we check validity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRunning(in PlayableGraph graph)
        {
            return graph.IsValid();
        }
    }
}
