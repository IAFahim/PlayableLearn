using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// A lightweight handle representing an active PlayableGraph instance.
    /// Pure Data. No Logic.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct GraphHandle
    {
        // The raw Unity Engine handle.
        public PlayableGraph Graph;

        // Our local flag to avoid asking the C++ engine "Do you exist?" every frame.
        public bool IsActive;

        // Debugging identifier.
        // We do not store strings here if we want this to be Blittable/Burst-friendly in the future,
        // but for Day 01 (Lifecycle), we accept the exception for the sake of the Inspector.
        public int GraphId;
    }
}
