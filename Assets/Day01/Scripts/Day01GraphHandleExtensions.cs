using UnityEngine;

namespace PlayableLearn.Day01
{
    public static class Day01GraphHandleExtensions
    {
        public static void Initialize(ref this Day01GraphHandle handle, string ownerName)
        {
            if (handle.IsActive)
            {
                Debug.LogWarning($"[GraphHandle] Already initialized: {ownerName}");
                return;
            }

            // Create unique name for Profiler/Visualizer
            string graphName = $"{ownerName}_Graph_{Time.frameCount}";

            GraphOps.Create(in graphName, out handle.Graph);

            handle.IsActive = true;
            handle.GraphId = graphName.GetHashCode(); // Store ID, not string

            // Auto-play for Day 01
            GraphOps.Play(ref handle.Graph);
        }

        public static void Dispose(ref this Day01GraphHandle handle)
        {
            if (!handle.IsActive) return;

            GraphOps.Destroy(ref handle.Graph);
            handle.IsActive = false;
            handle.GraphId = 0;
        }

        /// <summary>
        /// Reads the current time safely.
        /// For Day 01, returns Unity game time since graph uses GameTime mode.
        /// returns true if graph is running, false otherwise.
        /// </summary>
        public static bool TryGetTime(in this Day01GraphHandle handle, out float time)
        {
            var day01GraphHandle = handle;
            if (!day01GraphHandle.IsActive || !GraphOps.IsRunning(ref day01GraphHandle.Graph))
            {
                time = 0;
                return false;
            }

            // Graph is set to GameTime mode, so we use Unity's time
            time = Time.time;
            return true;
        }
    }
}