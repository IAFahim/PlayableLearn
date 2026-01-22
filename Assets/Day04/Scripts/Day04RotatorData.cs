using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day04
{
    /// <summary>
    /// A lightweight handle representing a Rotator node instance.
    /// Pure Data. No Logic.
    ///
    /// Day 04: The Update Cycle
    /// Demonstrates using PrepareFrame (ProcessFrame) to rotate a generic cube.
    ///
    /// Note: Rotation speed and axis are stored in the Behaviour, not duplicated here.
    /// This maintains a single source of truth.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day04RotatorData
    {
        // The graph that owns this node (needed for destruction).
        public PlayableGraph Graph;

        // The raw Unity Engine handle.
        public Playable Node;

        // Our local flag to track if the node is active.
        public bool IsActive;

        // Debugging identifier.
        public int NodeId;

        // The target transform to rotate (kept here for convenience, could also be in Behaviour)
        public Transform TargetTransform;
    }
}
