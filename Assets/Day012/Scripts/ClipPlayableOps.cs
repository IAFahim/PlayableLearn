using UnityEngine;
using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day12
{
    /// <summary>
    /// Burst-compiled operations for AnimationClipPlayable.
    /// Layer B: Operations - Pure functions using Burst for performance.
    /// This is the "Engine" layer - low-level operations on AnimationClipPlayable.
    /// </summary>
    [BurstCompile]
    public static class ClipPlayableOps
    {
        /// <summary>
        /// Creates an AnimationClipPlayable from the given graph and clip.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Create(in PlayableGraph graph, in AnimationClip clip, out Playable playable)
        {
            if (!graph.IsValid())
            {
                playable = Playable.Null;
                return;
            }

            if (clip == null)
            {
                playable = Playable.Null;
                return;
            }

            playable = AnimationClipPlayable.Create(graph, clip);
        }

        /// <summary>
        /// Destroys the given playable.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(in PlayableGraph graph, in Playable playable)
        {
            if (playable.IsValid() && graph.IsValid())
            {
                graph.DestroyPlayable(playable);
            }
        }

        /// <summary>
        /// Checks if the playable is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(in Playable playable)
        {
            return playable.IsValid();
        }

        /// <summary>
        /// Gets the current time of the clip.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetTime(in Playable playable, out double time)
        {
            time = playable.IsValid() ? playable.GetTime() : 0.0;
        }

        /// <summary>
        /// Sets the current time of the clip.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTime(in Playable playable, double time)
        {
            if (playable.IsValid())
            {
                playable.SetTime(time);
            }
        }

        /// <summary>
        /// Gets the duration of the clip.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetDuration(in Playable playable, out double duration)
        {
            duration = playable.IsValid() ? playable.GetDuration() : 0.0;
        }

        /// <summary>
        /// Gets the clip info including time, duration, and validity.
        /// Combined operation for efficiency.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetClipInfo(in Playable playable, out double time, out double duration, out bool isValid)
        {
            isValid = playable.IsValid();
            if (isValid)
            {
                time = playable.GetTime();
                duration = playable.GetDuration();
            }
            else
            {
                time = 0.0;
                duration = 0.0;
            }
        }

        /// <summary>
        /// Sets the play state (playing/paused).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPlayState(in Playable playable, bool play)
        {
            if (playable.IsValid())
            {
                playable.SetPlayState(play ? PlayState.Playing : PlayState.Paused);
            }
        }

        /// <summary>
        /// Gets the current play state.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetPlayState(in Playable playable, out PlayState state)
        {
            state = playable.IsValid() ? playable.GetPlayState() : PlayState.Paused;
        }

        /// <summary>
        /// Checks if the clip is currently playing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPlaying(in Playable playable)
        {
            return playable.IsValid() && playable.GetPlayState() == PlayState.Playing;
        }
    }
}
