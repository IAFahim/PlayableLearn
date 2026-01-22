using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;

namespace PlayableLearn.Day10
{
    /// <summary>
    /// Extension methods for Day10DisposableGraph providing initialization and lifecycle management.
    /// </summary>
    public static class Day10DisposableGraphExtensions
    {
        private static int graphIdCounter = 0;

        /// <summary>
        /// Initializes a new disposable graph with proper error checking.
        /// </summary>
        public static void Initialize(ref this Day10DisposableGraph disposableGraph, string ownerName)
        {
            if (disposableGraph.IsActive)
            {
                Debug.LogWarning($"[DisposableGraph] Already initialized: {ownerName} (ID: {disposableGraph.GraphId})");
                return;
            }

            if (disposableGraph.IsDisposed)
            {
                Debug.LogWarning($"[DisposableGraph] Attempting to initialize a disposed graph: {ownerName}");
                disposableGraph.IsDisposed = false;
            }

            // Create unique name for Profiler/Visualizer
            string graphName = $"{ownerName}_DisposableGraph_{GetNextId()}";

            GraphOps.Create(in graphName, out disposableGraph.Graph);

            disposableGraph.IsActive = true;
            disposableGraph.IsDisposed = false;
            disposableGraph.GraphId = graphName.GetHashCode();

            Debug.Log($"[DisposableGraph] Created graph '{graphName}' for {ownerName}");
        }

        /// <summary>
        /// Disposes the graph using the standard Dispose pattern.
        /// </summary>
        public static void Dispose(ref this Day10DisposableGraph disposableGraph)
        {
            if (!disposableGraph.IsActive || disposableGraph.IsDisposed)
            {
                return;
            }

            disposableGraph.Dispose();
            Debug.Log($"[DisposableGraph] Disposed graph (ID: {disposableGraph.GraphId})");
        }

        /// <summary>
        /// Safely checks if the graph is valid and ready for use.
        /// </summary>
        public static bool IsValid(in this Day10DisposableGraph disposableGraph)
        {
            return disposableGraph.IsActive && !disposableGraph.IsDisposed && DisposalOps.CanDisposeGraph(in disposableGraph.Graph);
        }

        /// <summary>
        /// Checks if the graph has been properly disposed.
        /// </summary>
        public static bool IsDisposedGraph(in this Day10DisposableGraph disposableGraph)
        {
            return disposableGraph.IsDisposed;
        }

        /// <summary>
        /// Resets the graph for potential reuse (after disposal).
        /// </summary>
        public static void Reset(ref this Day10DisposableGraph disposableGraph)
        {
            if (disposableGraph.IsActive && !disposableGraph.IsDisposed)
            {
                Debug.LogWarning("[DisposableGraph] Resetting an active graph. Use Dispose() first.");
                disposableGraph.Dispose();
            }

            disposableGraph = new Day10DisposableGraph
            {
                IsActive = false,
                IsDisposed = false,
                GraphId = 0
            };
        }

        /// <summary>
        /// Gets the next unique ID for graph naming.
        /// </summary>
        private static int GetNextId()
        {
            return System.Threading.Interlocked.Increment(ref graphIdCounter);
        }

        /// <summary>
        /// Resets the ID counter (useful for testing).
        /// </summary>
        public static void ResetIdCounter()
        {
            graphIdCounter = 0;
        }

        /// <summary>
        /// Logs diagnostic information about the disposable graph state.
        /// </summary>
        public static void LogState(in this Day10DisposableGraph disposableGraph, string context)
        {
            Debug.Log($"[DisposableGraph] {context} - Active: {disposableGraph.IsActive}, Disposed: {disposableGraph.IsDisposed}, Valid: {disposableGraph.IsValid()}, ID: {disposableGraph.GraphId}");
        }
    }
}
