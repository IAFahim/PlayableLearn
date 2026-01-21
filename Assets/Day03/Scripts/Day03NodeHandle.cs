using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day03
{
    /// <summary>
    /// A lightweight handle representing a ScriptPlayable node instance.
    /// Pure Data. No Logic.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day03NodeHandle
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
