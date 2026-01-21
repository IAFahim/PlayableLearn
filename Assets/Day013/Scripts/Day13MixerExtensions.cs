using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day13
{
    /// <summary>
    /// Extension methods for Day13MixerHandle.
    /// Layer C: Extensions - Provides initialization, connection, and lifecycle management.
    /// This is the "Adapter" layer that makes the data structure easy to use.
    /// </summary>
    public static class Day13MixerExtensions
    {
        /// <summary>
        /// Initializes a new AnimationMixerPlayable with the specified number of inputs.
        /// </summary>
        public static void Initialize(ref this Day13MixerHandle handle, in PlayableGraph graph, int inputCount)
        {
            if (handle.IsActive)
            {
                Debug.LogWarning($"[MixerHandle] Already initialized with {handle.InputCount} inputs.");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[MixerHandle] Cannot initialize mixer: Graph is invalid.");
                return;
            }

            if (inputCount <= 0)
            {
                Debug.LogError($"[MixerHandle] Cannot initialize mixer: Input count must be positive (got {inputCount}).");
                return;
            }

            MixerOps.Create(in graph, inputCount, out handle.Playable);

            if (MixerOps.IsValid(in handle.Playable))
            {
                handle.Graph = graph;
                handle.InputCount = inputCount;
                handle.IsActive = true;
                handle.MixerId = inputCount.GetHashCode();
                Debug.Log($"[MixerHandle] Created AnimationMixerPlayable with {inputCount} inputs.");
            }
            else
            {
                Debug.LogError($"[MixerHandle] Failed to create mixer with {inputCount} inputs.");
            }
        }

        /// <summary>
        /// Disposes the mixer, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day13MixerHandle handle)
        {
            if (!handle.IsActive) return;

            MixerOps.Destroy(in handle.Graph, in handle.Playable);
            handle.IsActive = false;
            handle.InputCount = 0;
            handle.MixerId = 0;
            Debug.Log("[MixerHandle] AnimationMixerPlayable disposed.");
        }

        /// <summary>
        /// Logs the mixer information to the console.
        /// </summary>
        public static void LogToConsole(this in Day13MixerHandle handle, string mixerName)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning($"[MixerHandle] Cannot log: Mixer is not active.");
                return;
            }

            if (!MixerOps.IsValid(in handle.Playable))
            {
                Debug.LogWarning($"[MixerHandle] Mixer '{mixerName}' is not valid.");
                return;
            }

            MixerOps.GetInputCount(in handle.Playable, out int inputCount);
            Debug.Log($"[Mixer] Name: {mixerName}, Inputs: {inputCount}, Active: {handle.IsActive}");

            // Log each input's weight
            for (int i = 0; i < inputCount; i++)
            {
                if (MixerOps.IsInputConnected(in handle.Playable, i))
                {
                    MixerOps.GetInputWeight(in handle.Playable, i, out float weight);
                    Debug.Log($"[Mixer]   Input {i}: Weight = {weight:F2}, Connected = Yes");
                }
                else
                {
                    Debug.Log($"[Mixer]   Input {i}: Connected = No");
                }
            }
        }

        /// <summary>
        /// Gets the number of inputs the mixer can blend.
        /// </summary>
        public static bool TryGetInputCount(this in Day13MixerHandle handle, out int inputCount)
        {
            inputCount = 0;
            if (!handle.IsActive || !MixerOps.IsValid(in handle.Playable))
            {
                return false;
            }

            MixerOps.GetInputCount(in handle.Playable, out inputCount);
            return true;
        }

        /// <summary>
        /// Sets the weight of an input port (blending factor).
        /// </summary>
        public static void SetInputWeight(ref this Day13MixerHandle handle, int inputIndex, float weight)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[MixerHandle] Cannot set weight: Mixer is not active.");
                return;
            }

            if (inputIndex < 0 || inputIndex >= handle.InputCount)
            {
                Debug.LogWarning($"[MixerHandle] Input index {inputIndex} is out of range (0-{handle.InputCount - 1}).");
                return;
            }

            MixerOps.SetInputWeight(in handle.Playable, inputIndex, weight);
        }

        /// <summary>
        /// Gets the weight of an input port.
        /// </summary>
        public static bool TryGetInputWeight(this in Day13MixerHandle handle, int inputIndex, out float weight)
        {
            weight = 0.0f;
            if (!handle.IsActive || !MixerOps.IsValid(in handle.Playable))
            {
                return false;
            }

            if (inputIndex < 0 || inputIndex >= handle.InputCount)
            {
                return false;
            }

            MixerOps.GetInputWeight(in handle.Playable, inputIndex, out weight);
            return true;
        }

        /// <summary>
        /// Connects a playable to a specific input port on the mixer.
        /// </summary>
        public static bool ConnectInput(ref this Day13MixerHandle handle, in Playable input, int inputIndex, float initialWeight = 0.0f)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[MixerHandle] Cannot connect input: Mixer is not active.");
                return false;
            }

            if (!input.IsValid())
            {
                Debug.LogWarning("[MixerHandle] Cannot connect input: Input playable is not valid.");
                return false;
            }

            if (inputIndex < 0 || inputIndex >= handle.InputCount)
            {
                Debug.LogWarning($"[MixerHandle] Input index {inputIndex} is out of range (0-{handle.InputCount - 1}).");
                return false;
            }

            if (MixerOps.ConnectInput(in handle.Playable, in input, inputIndex))
            {
                MixerOps.SetInputWeight(in handle.Playable, inputIndex, initialWeight);
                Debug.Log($"[MixerHandle] Connected input to port {inputIndex} with weight {initialWeight}.");
                return true;
            }

            Debug.LogError($"[MixerHandle] Failed to connect input to port {inputIndex}.");
            return false;
        }

        /// <summary>
        /// Disconnects an input from the mixer.
        /// </summary>
        public static void DisconnectInput(ref this Day13MixerHandle handle, int inputIndex)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[MixerHandle] Cannot disconnect: Mixer is not active.");
                return;
            }

            if (inputIndex < 0 || inputIndex >= handle.InputCount)
            {
                Debug.LogWarning($"[MixerHandle] Input index {inputIndex} is out of range (0-{handle.InputCount - 1}).");
                return;
            }

            MixerOps.DisconnectInput(in handle.Playable, inputIndex);
            Debug.Log($"[MixerHandle] Disconnected input from port {inputIndex}.");
        }

        /// <summary>
        /// Checks if the mixer is valid and active.
        /// </summary>
        public static bool IsValidMixer(this in Day13MixerHandle handle)
        {
            return handle.IsActive && MixerOps.IsValid(in handle.Playable);
        }

        /// <summary>
        /// Gets the Playable for connection to other nodes or outputs.
        /// </summary>
        public static bool TryGetPlayable(this in Day13MixerHandle handle, out Playable playable)
        {
            playable = default;
            if (!handle.IsActive || !MixerOps.IsValid(in handle.Playable))
            {
                return false;
            }
            playable = handle.Playable;
            return true;
        }

        /// <summary>
        /// Connects this mixer playable to an output.
        /// </summary>
        public static void ConnectToOutput(this in Day13MixerHandle handle, in PlayableOutput output)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[MixerHandle] Cannot connect: Mixer is not active.");
                return;
            }

            if (!output.IsOutputValid())
            {
                Debug.LogWarning("[MixerHandle] Cannot connect: Output is not valid.");
                return;
            }

            output.SetSourcePlayable(handle.Playable, 0);
            Debug.Log($"[MixerHandle] Connected mixer to output.");
        }

        /// <summary>
        /// Checks if an input port is connected.
        /// </summary>
        public static bool IsInputConnected(this in Day13MixerHandle handle, int inputIndex)
        {
            if (!handle.IsActive || inputIndex < 0 || inputIndex >= handle.InputCount)
            {
                return false;
            }

            return MixerOps.IsInputConnected(in handle.Playable, inputIndex);
        }

        /// <summary>
        /// Clears all input weights to zero (disable blending).
        /// </summary>
        public static void ClearAllWeights(ref this Day13MixerHandle handle)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[MixerHandle] Cannot clear weights: Mixer is not active.");
                return;
            }

            MixerOps.ClearAllWeights(in handle.Playable);
            Debug.Log("[MixerHandle] Cleared all input weights.");
        }

        /// <summary>
        /// Normalizes all input weights so they sum to 1.0.
        /// </summary>
        public static void NormalizeWeights(ref this Day13MixerHandle handle)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[MixerHandle] Cannot normalize weights: Mixer is not active.");
                return;
            }

            MixerOps.NormalizeWeights(in handle.Playable);
            Debug.Log("[MixerHandle] Normalized all input weights.");
        }

        /// <summary>
        /// Sets a simple blend between two inputs using a blend parameter (0-1).
        /// </summary>
        public static void SetBlend(ref this Day13MixerHandle handle, float blend)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[MixerHandle] Cannot set blend: Mixer is not active.");
                return;
            }

            if (handle.InputCount < 2)
            {
                Debug.LogWarning("[MixerHandle] Cannot set blend: Mixer must have at least 2 inputs.");
                return;
            }

            // Clamp blend to 0-1 range
            float clampedBlend = Mathf.Clamp01(blend);

            // Set weights for 2-input blend
            // Input 0: (1 - blend), Input 1: blend
            SetInputWeight(ref handle, 0, 1.0f - clampedBlend);
            SetInputWeight(ref handle, 1, clampedBlend);
        }

        /// <summary>
        /// Gets the current blend value between two inputs.
        /// </summary>
        public static bool TryGetBlend(this in Day13MixerHandle handle, out float blend)
        {
            blend = 0.0f;
            if (!handle.IsActive || handle.InputCount < 2)
            {
                return false;
            }

            // Get weight of input 1 (this represents the blend value)
            return TryGetInputWeight(handle, 1, out blend);
        }
    }
}
