using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day05
{
    /// <summary>
    /// A lightweight handle representing a Speed Control node instance.
    /// Pure Data. No Logic.
    ///
    /// Day 05: Time Dilation
    /// Demonstrates manipulating SetSpeed to control playback speed of a Playable.
    ///
    /// Note: Speed properties are stored in the Behaviour, not duplicated here.
    /// This maintains a single source of truth.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day05SpeedData
    {
        // The graph that owns this node (needed for destruction).
        public PlayableGraph Graph;

        // The raw Unity Engine handle.
        public Playable Node;

        // Our local flag to track if the node is active.
        public bool IsActive;

        // Debugging identifier.
        public int NodeId;
    }
}
