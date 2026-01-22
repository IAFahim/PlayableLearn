using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day17
{
    /// <summary>
    /// A lightweight handle representing an AnimationLayerMixerPlayable instance.
    /// Wraps a layer mixer for hierarchical animation blending with layers.
    /// Pure Data. No Logic.
    ///
    /// Day 17 Concept: Layering
    /// - Hierarchical animation layers (base, additive, override)
    /// - Layer weights for controlling layer influence
    /// - AvatarMask support for layer masking (Day 18)
    /// - Additive animation mixing (Day 20)
    /// - Root motion from layers (Day 19)
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Day17LayerHandle
    {
        // The graph that owns this playable.
        public PlayableGraph Graph;

        // The AnimationLayerMixerPlayable wrapper.
        public Playable Playable;

        // Number of layers this mixer can blend.
        public int LayerCount;

        // Our local flag to track if the playable is active.
        public bool IsActive;

        // Debugging identifier.
        public int LayerMixerId;
    }
}
