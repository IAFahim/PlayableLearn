using UnityEngine;
using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day17
{
    /// <summary>
    /// Burst-compiled operations for AnimationLayerMixerPlayable.
    /// Layer B: Operations - Pure functions using Burst for performance.
    /// This is the "Engine" layer - low-level operations on AnimationLayerMixerPlayable.
    ///
    /// Day 17 Concept: Layering
    /// - Hierarchical animation blending with layers
    /// - Layer weights for controlling layer influence
    /// - Layer 0 is always the base layer (thumbody)
    /// - Layers 1+ are additive or override layers
    /// </summary>
    [BurstCompile]
    public static class LayerMixerOps
    {
        /// <summary>
        /// Creates an AnimationLayerMixerPlayable with the specified number of layers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Create(in PlayableGraph graph, int layerCount, out Playable playable)
        {
            if (!graph.IsValid())
            {
                playable = Playable.Null;
                return;
            }

            if (layerCount <= 0)
            {
                playable = Playable.Null;
                return;
            }

            playable = AnimationLayerMixerPlayable.Create(graph, layerCount);
        }

        /// <summary>
        /// Destroys the given layer mixer playable.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(in PlayableGraph graph, in Playable playable)
        {
            if (playable.IsValid() && graph.IsValid())
            {
                graph.DestroyPlayable(playable);
            }
        }

        /// <summary>
        /// Checks if the layer mixer playable is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(in Playable playable)
        {
            return playable.IsValid();
        }

        /// <summary>
        /// Gets the number of layers the mixer can blend.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetLayerCount(in Playable playable, out int layerCount)
        {
            layerCount = playable.IsValid() ? playable.GetInputCount() : 0;
        }

        /// <summary>
        /// Sets the weight of a layer (blending factor for that layer).
        /// Layer 0 is the base layer and should typically remain at weight 1.0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLayerWeight(in Playable playable, int layerIndex, float weight)
        {
            if (playable.IsValid() && layerIndex >= 0 && layerIndex < playable.GetInputCount())
            {
                playable.SetInputWeight(layerIndex, weight);
            }
        }

        /// <summary>
        /// Gets the weight of a layer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetLayerWeight(in Playable playable, int layerIndex, out float weight)
        {
            weight = 0.0f;
            if (playable.IsValid() && layerIndex >= 0 && layerIndex < playable.GetInputCount())
            {
                weight = playable.GetInputWeight(layerIndex);
            }
        }

        /// <summary>
        /// Connects a playable to a specific layer port on the mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ConnectLayer(in Playable mixer, in Playable input, int layerIndex)
        {
            if (!mixer.IsValid() || !input.IsValid())
            {
                return false;
            }

            if (layerIndex < 0 || layerIndex >= mixer.GetInputCount())
            {
                return false;
            }

            return mixer.AddInput(input, layerIndex, 0.0f) >= 0;
        }

        /// <summary>
        /// Disconnects a layer from the mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisconnectLayer(in Playable mixer, int layerIndex)
        {
            if (mixer.IsValid() && layerIndex >= 0 && layerIndex < mixer.GetInputCount())
            {
                mixer.DisconnectInput(layerIndex);
            }
        }

        /// <summary>
        /// Gets the playable connected to a specific layer port.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetLayer(in Playable mixer, int layerIndex, out Playable layer)
        {
            layer = Playable.Null;
            if (mixer.IsValid() && layerIndex >= 0 && layerIndex < mixer.GetInputCount())
            {
                layer = mixer.GetInput(layerIndex);
            }
        }

        /// <summary>
        /// Checks if a layer port is connected.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLayerConnected(in Playable mixer, int layerIndex)
        {
            return mixer.IsValid() && layerIndex >= 0 && layerIndex < mixer.GetInputCount() && mixer.IsInputConnected(layerIndex);
        }

        /// <summary>
        /// Sets the weight of all layers to zero except layer 0 (base layer).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearAllLayerWeights(in Playable playable)
        {
            if (!playable.IsValid()) return;

            int count = playable.GetInputCount();
            for (int i = 1; i < count; i++)
            {
                playable.SetInputWeight(i, 0.0f);
            }
        }

        /// <summary>
        /// Sets the weight of the base layer (layer 0) to 1.0 and all other layers to 0.0.
        /// This is the default state for a layer mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetToBaseLayer(in Playable playable)
        {
            if (!playable.IsValid()) return;

            int count = playable.GetInputCount();
            playable.SetInputWeight(0, 1.0f);
            for (int i = 1; i < count; i++)
            {
                playable.SetInputWeight(i, 0.0f);
            }
        }

        /// <summary>
        /// Checks if the given layer index is the base layer (layer 0).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBaseLayer(int layerIndex)
        {
            return layerIndex == 0;
        }

        /// <summary>
        /// Checks if the given layer index is a valid layer (not base layer).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidLayer(int layerIndex)
        {
            return layerIndex > 0;
        }

        /// <summary>
        /// Gets a bitmask of active layers (layers with weight > threshold).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetActiveLayerMask(in Playable playable, float threshold, out int activeMask)
        {
            activeMask = 0;
            if (!playable.IsValid()) return;

            int count = playable.GetInputCount();
            for (int i = 0; i < count; i++)
            {
                float weight = playable.GetInputWeight(i);
                if (weight > threshold)
                {
                    activeMask |= (1 << i);
                }
            }
        }

        /// <summary>
        /// Checks if a specific layer is active (weight > threshold).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLayerActive(in Playable playable, int layerIndex, float threshold)
        {
            if (!playable.IsValid() || layerIndex < 0 || layerIndex >= playable.GetInputCount())
            {
                return false;
            }

            float weight = playable.GetInputWeight(layerIndex);
            return weight > threshold;
        }

        /// <summary>
        /// Gets the total weight of all layers (for debugging/validation).
        /// Note: Layer mixers don't necessarily need to sum to 1.0 like regular mixers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetTotalLayerWeight(in Playable playable, out float totalWeight)
        {
            totalWeight = 0.0f;
            if (!playable.IsValid()) return;

            int count = playable.GetInputCount();
            for (int i = 0; i < count; i++)
            {
                totalWeight += playable.GetInputWeight(i);
            }
        }

        /// <summary>
        /// Clamps a layer weight to the valid range [0, 1].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampLayerWeight(float weight)
        {
            return math.clamp(weight, 0.0f, 1.0f);
        }

        /// <summary>
        /// Validates layer index is within valid range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ValidateLayerIndex(in Playable playable, int layerIndex)
        {
            if (!playable.IsValid()) return false;
            if (layerIndex < 0) return false;
            if (layerIndex >= playable.GetInputCount()) return false;
            return true;
        }

        /// <summary>
        /// Copies layer weights from one mixer to another.
        /// Both mixers must have the same number of layers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyLayerWeights(in Playable source, in Playable destination)
        {
            if (!source.IsValid() || !destination.IsValid()) return;

            int sourceCount = source.GetInputCount();
            int destCount = destination.GetInputCount();

            if (sourceCount != destCount) return;

            for (int i = 0; i < sourceCount; i++)
            {
                float weight = source.GetInputWeight(i);
                destination.SetInputWeight(i, weight);
            }
        }
    }
}
