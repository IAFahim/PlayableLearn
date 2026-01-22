using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day17
{
    /// <summary>
    /// Extension methods for Day17LayerHandle.
    /// Layer C: Extensions - Provides initialization, connection, and lifecycle management.
    /// This is the "Adapter" layer that makes the data structure easy to use.
    ///
    /// Day 17 Concept: Layering
    /// - Hierarchical animation blending with layers
    /// - Layer weights for controlling layer influence
    /// - Base layer (layer 0) vs additive/override layers (layer 1+)
    /// </summary>
    public static class Day17LayerExtensions
    {
        /// <summary>
        /// Initializes a new AnimationLayerMixerPlayable with the specified number of layers.
        /// </summary>
        public static void Initialize(ref this Day17LayerHandle handle, in PlayableGraph graph, int layerCount)
        {
            if (handle.IsActive)
            {
                Debug.LogWarning($"[LayerHandle] Already initialized with {handle.LayerCount} layers.");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[LayerHandle] Cannot initialize layer mixer: Graph is invalid.");
                return;
            }

            if (layerCount <= 0)
            {
                Debug.LogError($"[LayerHandle] Cannot initialize layer mixer: Layer count must be positive (got {layerCount}).");
                return;
            }

            LayerMixerOps.Create(in graph, layerCount, out handle.Playable);

            if (LayerMixerOps.IsValid(in handle.Playable))
            {
                handle.Graph = graph;
                handle.LayerCount = layerCount;
                handle.IsActive = true;
                handle.LayerMixerId = layerCount.GetHashCode();

                // Initialize with base layer active
                LayerMixerOps.ResetToBaseLayer(in handle.Playable);

                Debug.Log($"[LayerHandle] Created AnimationLayerMixerPlayable with {layerCount} layers.");
            }
            else
            {
                Debug.LogError($"[LayerHandle] Failed to create layer mixer with {layerCount} layers.");
            }
        }

        /// <summary>
        /// Disposes the layer mixer, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day17LayerHandle handle)
        {
            if (!handle.IsActive) return;

            LayerMixerOps.Destroy(in handle.Graph, in handle.Playable);
            handle.IsActive = false;
            handle.LayerCount = 0;
            handle.LayerMixerId = 0;
            Debug.Log("[LayerHandle] AnimationLayerMixerPlayable disposed.");
        }

        /// <summary>
        /// Logs the layer mixer information to the console.
        /// </summary>
        public static void LogToConsole(this in Day17LayerHandle handle, string mixerName)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning($"[LayerHandle] Cannot log: Layer mixer is not active.");
                return;
            }

            if (!LayerMixerOps.IsValid(in handle.Playable))
            {
                Debug.LogWarning($"[LayerHandle] Layer mixer '{mixerName}' is not valid.");
                return;
            }

            LayerMixerOps.GetLayerCount(in handle.Playable, out int layerCount);
            Debug.Log($"[LayerMixer] Name: {mixerName}, Layers: {layerCount}, Active: {handle.IsActive}");

            // Log each layer's weight
            for (int i = 0; i < layerCount; i++)
            {
                if (LayerMixerOps.IsLayerConnected(in handle.Playable, i))
                {
                    LayerMixerOps.GetLayerWeight(in handle.Playable, i, out float weight);
                    string layerType = LayerMixerOps.IsBaseLayer(i) ? "Base" : "Additive/Override";
                    Debug.Log($"[LayerMixer]   Layer {i} ({layerType}): Weight = {weight:F2}, Connected = Yes");
                }
                else
                {
                    string layerType = LayerMixerOps.IsBaseLayer(i) ? "Base" : "Additive/Override";
                    Debug.Log($"[LayerMixer]   Layer {i} ({layerType}): Connected = No");
                }
            }
        }

        /// <summary>
        /// Gets the number of layers the mixer can blend.
        /// </summary>
        public static bool TryGetLayerCount(this in Day17LayerHandle handle, out int layerCount)
        {
            layerCount = 0;
            if (!handle.IsActive || !LayerMixerOps.IsValid(in handle.Playable))
            {
                return false;
            }

            LayerMixerOps.GetLayerCount(in handle.Playable, out layerCount);
            return true;
        }

        /// <summary>
        /// Sets the weight of a layer (blending factor for that layer).
        /// </summary>
        public static void SetLayerWeight(ref this Day17LayerHandle handle, int layerIndex, float weight)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[LayerHandle] Cannot set layer weight: Layer mixer is not active.");
                return;
            }

            if (layerIndex < 0 || layerIndex >= handle.LayerCount)
            {
                Debug.LogWarning($"[LayerHandle] Layer index {layerIndex} is out of range (0-{handle.LayerCount - 1}).");
                return;
            }

            float clampedWeight = LayerMixerOps.ClampLayerWeight(weight);
            LayerMixerOps.SetLayerWeight(in handle.Playable, layerIndex, clampedWeight);
        }

        /// <summary>
        /// Gets the weight of a layer.
        /// </summary>
        public static bool TryGetLayerWeight(this in Day17LayerHandle handle, int layerIndex, out float weight)
        {
            weight = 0.0f;
            if (!handle.IsActive || !LayerMixerOps.IsValid(in handle.Playable))
            {
                return false;
            }

            if (layerIndex < 0 || layerIndex >= handle.LayerCount)
            {
                return false;
            }

            LayerMixerOps.GetLayerWeight(in handle.Playable, layerIndex, out weight);
            return true;
        }

        /// <summary>
        /// Connects a playable to a specific layer port on the mixer.
        /// </summary>
        public static bool ConnectLayer(ref this Day17LayerHandle handle, in Playable input, int layerIndex, float initialWeight = 0.0f)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[LayerHandle] Cannot connect layer: Layer mixer is not active.");
                return false;
            }

            if (!input.IsValid())
            {
                Debug.LogWarning("[LayerHandle] Cannot connect layer: Input playable is not valid.");
                return false;
            }

            if (layerIndex < 0 || layerIndex >= handle.LayerCount)
            {
                Debug.LogWarning($"[LayerHandle] Layer index {layerIndex} is out of range (0-{handle.LayerCount - 1}).");
                return false;
            }

            if (LayerMixerOps.ConnectLayer(in handle.Playable, in input, layerIndex))
            {
                float clampedWeight = LayerMixerOps.ClampLayerWeight(initialWeight);
                LayerMixerOps.SetLayerWeight(in handle.Playable, layerIndex, clampedWeight);
                string layerType = LayerMixerOps.IsBaseLayer(layerIndex) ? "Base" : "Additive/Override";
                Debug.Log($"[LayerHandle] Connected input to {layerType} layer {layerIndex} with weight {clampedWeight}.");
                return true;
            }

            Debug.LogError($"[LayerHandle] Failed to connect input to layer {layerIndex}.");
            return false;
        }

        /// <summary>
        /// Disconnects a layer from the mixer.
        /// </summary>
        public static void DisconnectLayer(ref this Day17LayerHandle handle, int layerIndex)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[LayerHandle] Cannot disconnect: Layer mixer is not active.");
                return;
            }

            if (layerIndex < 0 || layerIndex >= handle.LayerCount)
            {
                Debug.LogWarning($"[LayerHandle] Layer index {layerIndex} is out of range (0-{handle.LayerCount - 1}).");
                return;
            }

            LayerMixerOps.DisconnectLayer(in handle.Playable, layerIndex);
            string layerType = LayerMixerOps.IsBaseLayer(layerIndex) ? "Base" : "Additive/Override";
            Debug.Log($"[LayerHandle] Disconnected {layerType} layer {layerIndex}.");
        }

        /// <summary>
        /// Checks if the layer mixer is valid and active.
        /// </summary>
        public static bool IsValidLayerMixer(this in Day17LayerHandle handle)
        {
            return handle.IsActive && LayerMixerOps.IsValid(in handle.Playable);
        }

        /// <summary>
        /// Gets the Playable for connection to other nodes or outputs.
        /// </summary>
        public static bool TryGetPlayable(this in Day17LayerHandle handle, out Playable playable)
        {
            playable = default;
            if (!handle.IsActive || !LayerMixerOps.IsValid(in handle.Playable))
            {
                return false;
            }
            playable = handle.Playable;
            return true;
        }

        /// <summary>
        /// Connects this layer mixer playable to an output.
        /// </summary>
        public static void ConnectToOutput(this in Day17LayerHandle handle, in PlayableOutput output)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[LayerHandle] Cannot connect: Layer mixer is not active.");
                return;
            }

            if (!output.IsOutputValid())
            {
                Debug.LogWarning("[LayerHandle] Cannot connect: Output is not valid.");
                return;
            }

            output.SetSourcePlayable(handle.Playable, 0);
            Debug.Log($"[LayerHandle] Connected layer mixer to output.");
        }

        /// <summary>
        /// Checks if a layer port is connected.
        /// </summary>
        public static bool IsLayerConnected(this in Day17LayerHandle handle, int layerIndex)
        {
            if (!handle.IsActive || layerIndex < 0 || layerIndex >= handle.LayerCount)
            {
                return false;
            }

            return LayerMixerOps.IsLayerConnected(in handle.Playable, layerIndex);
        }

        /// <summary>
        /// Clears all layer weights to zero except layer 0 (base layer).
        /// </summary>
        public static void ClearAllLayerWeights(ref this Day17LayerHandle handle)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[LayerHandle] Cannot clear layer weights: Layer mixer is not active.");
                return;
            }

            LayerMixerOps.ClearAllLayerWeights(in handle.Playable);
            Debug.Log("[LayerHandle] Cleared all layer weights (base layer preserved).");
        }

        /// <summary>
        /// Resets the layer mixer to base layer only (layer 0 at weight 1.0, all others at 0.0).
        /// </summary>
        public static void ResetToBaseLayer(ref this Day17LayerHandle handle)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[LayerHandle] Cannot reset: Layer mixer is not active.");
                return;
            }

            LayerMixerOps.ResetToBaseLayer(in handle.Playable);
            Debug.Log("[LayerHandle] Reset to base layer only.");
        }

        /// <summary>
        /// Checks if a specific layer is active (weight > threshold).
        /// </summary>
        public static bool IsLayerActive(this in Day17LayerHandle handle, int layerIndex, float threshold = 0.001f)
        {
            if (!handle.IsActive || layerIndex < 0 || layerIndex >= handle.LayerCount)
            {
                return false;
            }

            return LayerMixerOps.IsLayerActive(in handle.Playable, layerIndex, threshold);
        }

        /// <summary>
        /// Gets a bitmask of active layers (layers with weight > threshold).
        /// </summary>
        public static bool TryGetActiveLayerMask(this in Day17LayerHandle handle, float threshold, out int activeMask)
        {
            activeMask = 0;
            if (!handle.IsActive || !LayerMixerOps.IsValid(in handle.Playable))
            {
                return false;
            }

            LayerMixerOps.GetActiveLayerMask(in handle.Playable, threshold, out activeMask);
            return true;
        }

        /// <summary>
        /// Sets up a simple two-layer blend (base layer + one additive/override layer).
        /// </summary>
        public static void SetTwoLayerBlend(ref this Day17LayerHandle handle, float layerWeight)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[LayerHandle] Cannot set blend: Layer mixer is not active.");
                return;
            }

            if (handle.LayerCount < 2)
            {
                Debug.LogWarning("[LayerHandle] Cannot set blend: Layer mixer must have at least 2 layers.");
                return;
            }

            // Base layer (layer 0) always at 1.0
            SetLayerWeight(ref handle, 0, 1.0f);

            // Additive/override layer (layer 1) controlled by blend parameter
            float clampedWeight = Mathf.Clamp01(layerWeight);
            SetLayerWeight(ref handle, 1, clampedWeight);
        }

        /// <summary>
        /// Gets the current blend value for a two-layer setup.
        /// </summary>
        public static bool TryGetTwoLayerBlend(this in Day17LayerHandle handle, out float blend)
        {
            blend = 0.0f;
            if (!handle.IsActive || handle.LayerCount < 2)
            {
                return false;
            }

            return TryGetLayerWeight(handle, 1, out blend);
        }

        /// <summary>
        /// Enables a specific layer by setting its weight to 1.0.
        /// </summary>
        public static void EnableLayer(ref this Day17LayerHandle handle, int layerIndex)
        {
            SetLayerWeight(ref handle, layerIndex, 1.0f);
        }

        /// <summary>
        /// Disables a specific layer by setting its weight to 0.0.
        /// Note: Cannot disable the base layer (layer 0).
        /// </summary>
        public static void DisableLayer(ref this Day17LayerHandle handle, int layerIndex)
        {
            if (LayerMixerOps.IsBaseLayer(layerIndex))
            {
                Debug.LogWarning("[LayerHandle] Cannot disable base layer (layer 0). Use ResetToBaseLayer() instead.");
                return;
            }

            SetLayerWeight(ref handle, layerIndex, 0.0f);
        }

        /// <summary>
        /// Gets the total weight of all layers (for debugging/validation).
        /// </summary>
        public static bool TryGetTotalLayerWeight(this in Day17LayerHandle handle, out float totalWeight)
        {
            totalWeight = 0.0f;
            if (!handle.IsActive || !LayerMixerOps.IsValid(in handle.Playable))
            {
                return false;
            }

            LayerMixerOps.GetTotalLayerWeight(in handle.Playable, out totalWeight);
            return true;
        }
    }
}
