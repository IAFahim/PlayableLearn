using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day16
{
    /// <summary>
    /// A lightweight data structure for managing crossfade operations with time-based weight transitions.
    /// Wraps a mixer with explicit crossfade control using lerping over time.
    /// Pure Data. No Logic.
    ///
    /// Day 16 Concept: Crossfade Logic
    /// - Demonstrates smooth weight transitions over time (crossfading)
    /// - Time-based lerping between blend weights
    /// - Configurable crossfade duration
    /// - Progress tracking for crossfade completion
    /// - Supports different crossfade curves (linear, ease-in, ease-out)
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day16CrossfadeData
    {
        // The graph that owns this playable.
        public PlayableGraph Graph;

        // The mixer playable for crossfading.
        public Playable Mixer;

        // Number of inputs for crossfading.
        public int InputCount;

        // Current weight for input 0.
        public float CurrentWeight0;

        // Current weight for input 1.
        public float CurrentWeight1;

        // Starting weight for input 0 (for crossfade calculation).
        public float StartWeight0;

        // Starting weight for input 1 (for crossfade calculation).
        public float StartWeight1;

        // Target weight for input 0.
        public float TargetWeight0;

        // Target weight for input 1.
        public float TargetWeight1;

        // Crossfade duration in seconds.
        public float CrossfadeDuration;

        // Elapsed time since crossfade started.
        public float ElapsedTime;

        // Whether a crossfade is currently in progress.
        public bool IsCrossfading;

        // Crossfade curve type (0=linear, 1=ease-in, 2=ease-out, 3=ease-in-out).
        public int CurveType;

        // Our local flag to track if the crossfade data is active.
        public bool IsActive;

        // Debugging identifier.
        public int CrossfadeId;

        // Current crossfade progress (0.0 = started, 1.0 = complete).
        public float Progress;

        // Whether weights should be normalized.
        public bool NormalizeWeights;
    }
}
