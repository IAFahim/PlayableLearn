using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day12
{
    /// <summary>
    /// Extension methods for Day12ClipHandle.
    /// Layer C: Extensions - Provides initialization, connection, and lifecycle management.
    /// This is the "Adapter" layer that makes the data structure easy to use.
    /// </summary>
    public static class Day12ClipExtensions
    {
        /// <summary>
        /// Initializes a new AnimationClipPlayable from the given graph and clip.
        /// </summary>
        public static void Initialize(ref this Day12ClipHandle handle, in PlayableGraph graph, AnimationClip clip)
        {
            if (handle.IsActive)
            {
                Debug.LogWarning($"[ClipHandle] Already initialized: {clip?.Name}");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[ClipHandle] Cannot initialize clip: Graph is invalid.");
                return;
            }

            if (clip == null)
            {
                Debug.LogError($"[ClipHandle] Cannot initialize clip: AnimationClip is null.");
                return;
            }

            ClipPlayableOps.Create(in graph, in clip, out handle.Playable);

            if (ClipPlayableOps.IsValid(in handle.Playable))
            {
                handle.Graph = graph;
                handle.Clip = clip;
                handle.IsActive = true;
                handle.ClipId = clip.name.GetHashCode();
                Debug.Log($"[ClipHandle] Created AnimationClipPlayable: {clip.name}");
            }
            else
            {
                Debug.LogError($"[ClipHandle] Failed to create playable for clip: {clip.name}");
            }
        }

        /// <summary>
        /// Disposes the playable, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day12ClipHandle handle)
        {
            if (!handle.IsActive) return;

            ClipPlayableOps.Destroy(in handle.Graph, in handle.Playable);
            handle.IsActive = false;
            handle.ClipId = 0;
            handle.Clip = null;
            Debug.Log("[ClipHandle] AnimationClipPlayable disposed.");
        }

        /// <summary>
        /// Logs the clip information to the console.
        /// </summary>
        public static void LogToConsole(this in Day12ClipHandle handle, string clipName)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning($"[ClipHandle] Cannot log: Clip is not active.");
                return;
            }

            ClipPlayableOps.GetClipInfo(in handle.Playable, out double time, out double duration, out bool isValid);

            if (!isValid)
            {
                Debug.LogWarning($"[ClipHandle] Clip '{clipName}' is not valid.");
                return;
            }

            string clipNameStr = handle.Clip != null ? handle.Clip.name : "None";
            Debug.Log($"[Clip] Name: {clipNameStr}, Time: {time:F2}, Duration: {duration:F2}, IsPlaying: {ClipPlayableOps.IsPlaying(in handle.Playable)}");
        }

        /// <summary>
        /// Gets the current time of the clip.
        /// </summary>
        public static bool TryGetTime(this in Day12ClipHandle handle, out double time)
        {
            time = 0.0;
            if (!handle.IsActive || !ClipPlayableOps.IsValid(in handle.Playable))
            {
                return false;
            }

            ClipPlayableOps.GetTime(in handle.Playable, out time);
            return true;
        }

        /// <summary>
        /// Sets the current time of the clip.
        /// </summary>
        public static void SetTime(ref this Day12ClipHandle handle, double time)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[ClipHandle] Cannot set time: Clip is not active.");
                return;
            }

            ClipPlayableOps.SetTime(in handle.Playable, time);
        }

        /// <summary>
        /// Gets the duration of the clip.
        /// </summary>
        public static bool TryGetDuration(this in Day12ClipHandle handle, out double duration)
        {
            duration = 0.0;
            if (!handle.IsActive || !ClipPlayableOps.IsValid(in handle.Playable))
            {
                return false;
            }

            ClipPlayableOps.GetDuration(in handle.Playable, out duration);
            return true;
        }

        /// <summary>
        /// Sets the play state (playing/paused).
        /// </summary>
        public static void SetPlayState(ref this Day12ClipHandle handle, bool play)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[ClipHandle] Cannot set play state: Clip is not active.");
                return;
            }

            ClipPlayableOps.SetPlayState(in handle.Playable, play);
        }

        /// <summary>
        /// Checks if the clip is currently playing.
        /// </summary>
        public static bool IsPlaying(this in Day12ClipHandle handle)
        {
            if (!handle.IsActive || !ClipPlayableOps.IsValid(in handle.Playable))
            {
                return false;
            }

            return ClipPlayableOps.IsPlaying(in handle.Playable);
        }

        /// <summary>
        /// Checks if the clip is valid and active.
        /// </summary>
        public static bool IsValidClip(this in Day12ClipHandle handle)
        {
            return handle.IsActive && ClipPlayableOps.IsValid(in handle.Playable);
        }

        /// <summary>
        /// Gets the AnimationClip being played.
        /// </summary>
        public static bool TryGetClip(this in Day12ClipHandle handle, out AnimationClip clip)
        {
            clip = null;
            if (!handle.IsActive || handle.Clip == null)
            {
                return false;
            }
            clip = handle.Clip;
            return true;
        }

        /// <summary>
        /// Gets the Playable for connection to other nodes or outputs.
        /// </summary>
        public static bool TryGetPlayable(this in Day12ClipHandle handle, out Playable playable)
        {
            playable = default;
            if (!handle.IsActive || !ClipPlayableOps.IsValid(in handle.Playable))
            {
                return false;
            }
            playable = handle.Playable;
            return true;
        }

        /// <summary>
        /// Connects this clip playable to an output.
        /// </summary>
        public static void ConnectToOutput(this in Day12ClipHandle handle, in PlayableOutput output)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[ClipHandle] Cannot connect: Clip is not active.");
                return;
            }

            if (!output.IsOutputValid())
            {
                Debug.LogWarning("[ClipHandle] Cannot connect: Output is not valid.");
                return;
            }

            output.SetSourcePlayable(handle.Playable, 0);
            Debug.Log($"[ClipHandle] Connected clip '{handle.Clip?.Name}' to output.");
        }

        /// <summary>
        /// Adds an input to this clip playable (for blending/mixing).
        /// </summary>
        public static void AddInput(ref this Day12ClipHandle handle, in Playable input, int inputPort, float weight)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[ClipHandle] Cannot add input: Clip is not active.");
                return;
            }

            if (!input.IsValid())
            {
                Debug.LogWarning("[ClipHandle] Cannot add input: Input playable is not valid.");
                return;
            }

            handle.Playable.AddInput(input, inputPort, weight);
            Debug.Log($"[ClipHandle] Added input to clip '{handle.Clip?.Name}' with weight {weight}.");
        }
    }
}
