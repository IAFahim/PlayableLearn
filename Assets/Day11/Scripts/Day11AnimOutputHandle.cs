using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day11
{
    /// <summary>
    /// A lightweight handle representing an AnimationPlayableOutput instance.
    /// Connects PlayableGraph output to an Animator component.
    /// Pure Data. No Logic.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day11AnimOutputHandle
    {
        // The graph that owns this output (needed for destruction).
        public PlayableGraph Graph;

        // The raw Unity Engine handle.
        public PlayableOutput Output;

        // The Animator component this output is connected to.
        public Animator Animator;

        // Our local flag to track if the output is active.
        public bool IsActive;

        // Debugging identifier.
        public int OutputId;
    }
}
