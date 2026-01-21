using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day07
{
    /// <summary>
    /// A lightweight handle representing Reverse Time control for a PlayableGraph.
    /// Pure Data. No Logic.
    ///
    /// Day 07: The Reverse Time
    /// Demonstrates reverse time functionality with negative speed and time wrapping.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day07ReverseData
    {
        // The graph that this reverse time control affects
        public PlayableGraph Graph;

        // Local flag to track if reverse time control is active
        public bool IsActive;

        // Debugging identifier
        public int ControllerId;

        // Whether reverse time is enabled
        public bool EnableReverseTime;

        // Whether time wrapping is enabled (wraps accumulated time)
        public bool EnableTimeWrapping;

        // Accumulated time tracking (can go negative when reversing)
        public double AccumulatedTime;

        // Time wrapping limit (in seconds)
        public double WrapLimit;

        // Speed reference for determining direction
        public double LastKnownSpeed;

        // Whether the control is valid
        public bool IsValidControl;
    }
}
