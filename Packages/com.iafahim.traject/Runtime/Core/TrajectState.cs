using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace AV.Traject.Runtime.Core
{
    [Serializable]
    [Flags]
    public enum TrajectStatusFlags : byte
    {
        None = 0,
        IsPlaying = 1 << 0,
        IsLooping = 1 << 1,
        IsPingPong = 1 << 2,
        HasCompleted = 1 << 3
    }

    /// <summary>
    /// Simple timer struct for tracking elapsed time.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TrajectTimer
    {
        public float Current;
        public float Duration;

        public TrajectTimer(float duration, float current = 0f)
        {
            Duration = duration;
            Current = current;
        }

        public float GetRatio()
        {
            return Duration > 0f ? Current / Duration : 0f;
        }

        public bool IsFull()
        {
            return Current >= Duration;
        }

        public void Reset()
        {
            Current = 0f;
        }

        public bool TickAndCheckComplete(float deltaTime)
        {
            Current = math.min(Current + deltaTime, Duration);
            return Current >= Duration;
        }
    }

    /// <summary>
    /// Pure runtime state for trajectory playback.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TrajectState
    {
        /// <summary>The playback timer tracking elapsed time.</summary>
        public TrajectTimer PlaybackTimer;

        /// <summary>Playback speed multiplier (1.0 = Normal, -1.0 = Reverse).</summary>
        public float PlaybackSpeedMultiplier;

        /// <summary>Current status flags.</summary>
        public TrajectStatusFlags StatusFlags;

        public static TrajectState Default => new TrajectState
        {
            PlaybackTimer = new TrajectTimer(1f, 0f),
            PlaybackSpeedMultiplier = 1f,
            StatusFlags = TrajectStatusFlags.IsPlaying
        };
    }
}
