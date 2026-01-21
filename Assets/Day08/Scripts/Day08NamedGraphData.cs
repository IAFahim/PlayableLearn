using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day08
{
    /// <summary>
    /// A lightweight handle representing a Named PlayableGraph for Profiler visibility.
    /// Pure Data. No Logic.
    ///
    /// Day 08: The Director Name
    /// Demonstrates graph naming for debugging in the Unity Profiler.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day08NamedGraphData
    {
        // The graph that owns this named graph handle
        public PlayableGraph Graph;

        // The name assigned to the graph for Profiler visibility
        public string GraphName;

        // Local flag to track if named graph is active
        public bool IsActive;

        // Debugging identifier
        public int ControllerId;

        // Whether the control is valid
        public bool IsValidControl;

        // Previous name (for tracking name changes)
        public string PreviousName;

        // Whether the name has been set
        public bool IsNameSet;

        // Name change count (for debugging)
        public int NameChangeCount;
    }
}
