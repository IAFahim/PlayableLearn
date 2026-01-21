using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day15
{
    /// <summary>
    /// A lightweight data structure for managing weighted blend operations.
    /// Wraps a mixer with explicit weight control for multiple inputs.
    /// Pure Data. No Logic.
    ///
    /// Day 15 Concept: Weighted Blending
    /// - Demonstrates setting explicit weights (e.g., 0.5/0.5) on mixer inputs
    /// - Each input can have an independent weight value
    /// - Weights can be normalized or left as-is
    /// - Provides smooth transitions between animation states
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day15BlendData
    {
        // The graph that owns this playable.
        public PlayableGraph Graph;

        // The mixer playable for blending.
        public Playable Mixer;

        // Number of inputs for blending.
        public int InputCount;

        // Weight for input 0.
        public float Weight0;

        // Weight for input 1.
        public float Weight1;

        // Target weight for input 0 (for smooth transitions).
        public float TargetWeight0;

        // Target weight for input 1 (for smooth transitions).
        public float TargetWeight1;

        // Blend speed for transitions.
        public float BlendSpeed;

        // Whether weights should be normalized.
        public bool NormalizeWeights;

        // Our local flag to track if the blend data is active.
        public bool IsActive;

        // Debugging identifier.
        public int BlendId;

        // Current blend value (0.0 = input 0 only, 1.0 = input 1 only).
        public float CurrentBlend;
    }
}
