using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Holds the raw memory reference to the PlayableGraph.
    /// Acts as the handle for our engine.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day01GraphLifecycleState
    {
        // NO single letters. "Graph" is too generic.
        // We call it what it is: The Handle.
        public PlayableGraph MainGraphHandle;

        public bool IsCreated;
        public bool IsPlaying;
    }
}
