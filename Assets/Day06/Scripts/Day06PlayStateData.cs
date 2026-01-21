using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day06
{
    /// <summary>
    /// A lightweight handle representing PlayState control for a PlayableGraph.
    /// Pure Data. No Logic.
    ///
    /// Day 06: The Pause Button
    /// Demonstrates playing/stopping the graph programmatically using PlayState.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day06PlayStateData
    {
        // The graph that this PlayState controls
        public PlayableGraph Graph;

        // Local flag to track if PlayState control is active
        public bool IsActive;

        // Debugging identifier
        public int ControllerId;

        // Current play state of the graph
        public PlayState CurrentState;

        // Previous play state (for state change detection)
        public PlayState PreviousState;

        // Whether to auto-play on initialization
        public bool AutoPlayOnStart;

        // Whether the graph is valid
        public bool IsGraphValid;
    }
}
