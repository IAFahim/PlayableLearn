using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day13
{
    /// <summary>
    /// A lightweight handle representing an AnimationMixerPlayable instance.
    /// Wraps a mixer for blending multiple animation inputs.
    /// Pure Data. No Logic.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day13MixerHandle
    {
        // The graph that owns this playable.
        public PlayableGraph Graph;

        // The AnimationMixerPlayable wrapper.
        public Playable Playable;

        // Number of inputs this mixer can blend.
        public int InputCount;

        // Our local flag to track if the playable is active.
        public bool IsActive;

        // Debugging identifier.
        public int MixerId;
    }
}
