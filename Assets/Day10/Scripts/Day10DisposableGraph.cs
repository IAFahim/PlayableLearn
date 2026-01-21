using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day10
{
    /// <summary>
    /// A disposable wrapper for PlayableGraph following proper IDisposable patterns.
    /// Provides deterministic cleanup and resource management for PlayableGraph instances.
    /// Pure Data. No Logic.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day10DisposableGraph : IDisposable
    {
        // The raw Unity Engine handle.
        public PlayableGraph Graph;

        // Our local flag to avoid asking the C++ engine "Do you exist?" every frame.
        public bool IsActive;

        // Track if this instance has been disposed.
        public bool IsDisposed;

        // Debugging identifier.
        public int GraphId;

        // IDisposable implementation
        public void Dispose()
        {
            if (!IsDisposed && IsActive)
            {
                DisposalOps.DisposeGraph(in Graph);
                IsActive = false;
                IsDisposed = true;
                GraphId = 0;
            }
        }
    }
}
