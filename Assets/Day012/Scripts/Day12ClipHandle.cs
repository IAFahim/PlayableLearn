using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day12
{
    /// <summary>
    /// A lightweight handle representing an AnimationClipPlayable instance.
    /// Wraps an AnimationClip for playback in a PlayableGraph.
    /// Pure Data. No Logic.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day12ClipHandle
    {
        // The graph that owns this playable.
        public PlayableGraph Graph;

        // The AnimationClipPlayable wrapper.
        public Playable Playable;

        // The AnimationClip asset being played.
        public AnimationClip Clip;

        // Our local flag to track if the playable is active.
        public bool IsActive;

        // Debugging identifier.
        public int ClipId;
    }
}
