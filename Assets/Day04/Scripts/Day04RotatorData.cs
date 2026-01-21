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

        // The target transform to rotate.
        public Transform TargetTransform;

        // Rotation speed in degrees per second.
        public float RotationSpeed;

        // Rotation axis (normalized).
        public Vector3 RotationAxis;
    }
}
