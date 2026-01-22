using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day10
{
    /// <summary>
    /// A disposable wrapper for PlayableGraph following proper disposal patterns.
    /// Provides deterministic cleanup and resource management for PlayableGraph instances.
    /// Pure Data. No Logic.
    ///
    /// NOTE: This is a struct, not a class. The Dispose method should ONLY be called via
    /// the ref extension method Day10DisposableGraphExtensions.Dispose(ref this).
    /// Direct calls to Dispose() will not modify the original struct due to C# value semantics.
    /// This struct does NOT implement IDisposable to prevent accidental misuse (e.g., in using statements).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day10DisposableGraph
    {
        // The raw Unity Engine handle.
        public PlayableGraph Graph;

        // Our local flag to avoid asking the C++ engine "Do you exist?" every frame.
        public bool IsActive;

        // Track if this instance has been disposed.
        public bool IsDisposed;

        // Debugging identifier.
        public int GraphId;

        // Internal dispose method - should NOT be called directly!
        // Use Day10DisposableGraphExtensions.Dispose(ref this) instead.
        internal void DisposeInternal()
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
