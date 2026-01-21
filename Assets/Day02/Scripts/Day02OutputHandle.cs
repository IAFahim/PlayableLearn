using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day02
{
    /// <summary>
    /// A lightweight handle representing a ScriptPlayableOutput instance.
    /// Pure Data. No Logic.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day02OutputHandle
    {
        // The raw Unity Engine handle.
        public PlayableOutput Output;

        // Our local flag to track if the output is active.
        public bool IsActive;

        // Debugging identifier.
        public int OutputId;
    }
}
