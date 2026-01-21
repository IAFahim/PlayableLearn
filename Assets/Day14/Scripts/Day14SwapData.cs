using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day14
{
    /// <summary>
    /// A lightweight handle representing hard swap connection operations.
    /// Manages disconnecting input 0 and connecting input 1 for hard animation swaps.
    /// Pure Data. No Logic.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day14SwapData
    {
        // The graph that owns the playables.
        public PlayableGraph Graph;

        // The mixer playable that inputs will be connected to.
        public Playable MixerPlayable;

        // Input 0 playable (to be disconnected during swap).
        public Playable Input0Playable;

        // Input 1 playable (to be connected during swap).
        public Playable Input1Playable;

        // Track if currently using input 0 or input 1.
        public bool IsUsingInput1;

        // Our local flag to track if the swap data is active.
        public bool IsActive;

        // Debugging identifier.
        public int SwapId;

        // Track if input 0 is connected.
        public bool IsInput0Connected;

        // Track if input 1 is connected.
        public bool IsInput1Connected;
    }
}
