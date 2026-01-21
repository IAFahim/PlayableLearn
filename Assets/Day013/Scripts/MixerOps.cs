using UnityEngine;
using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day13
{
    /// <summary>
    /// Burst-compiled operations for AnimationMixerPlayable.
    /// Layer B: Operations - Pure functions using Burst for performance.
    /// This is the "Engine" layer - low-level operations on AnimationMixerPlayable.
    /// </summary>
    [BurstCompile]
    public static class MixerOps
    {
        /// <summary>
        /// Creates an AnimationMixerPlayable with the specified number of inputs.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Create(in PlayableGraph graph, int inputCount, out Playable playable)
        {
            if (!graph.IsValid())
            {
                playable = Playable.Null;
                return;
            }

            if (inputCount <= 0)
            {
                playable = Playable.Null;
                return;
            }

            playable = AnimationMixerPlayable.Create(graph, inputCount);
        }

        /// <summary>
        /// Destroys the given mixer playable.
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
        /// Checks if the mixer playable is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(in Playable playable)
        {
            return playable.IsValid();
        }

        /// <summary>
        /// Gets the number of inputs the mixer can blend.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetInputCount(in Playable playable, out int inputCount)
        {
            inputCount = playable.IsValid() ? playable.GetInputCount() : 0;
        }

        /// <summary>
        /// Sets the weight of an input port (blending factor).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInputWeight(in Playable playable, int inputIndex, float weight)
        {
            if (playable.IsValid() && inputIndex >= 0 && inputIndex < playable.GetInputCount())
            {
                playable.SetInputWeight(inputIndex, weight);
            }
        }

        /// <summary>
        /// Gets the weight of an input port.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetInputWeight(in Playable playable, int inputIndex, out float weight)
        {
            weight = 0.0f;
            if (playable.IsValid() && inputIndex >= 0 && inputIndex < playable.GetInputCount())
            {
                weight = playable.GetInputWeight(inputIndex);
            }
        }

        /// <summary>
        /// Connects a playable to a specific input port on the mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ConnectInput(in Playable mixer, in Playable input, int inputIndex)
        {
            if (!mixer.IsValid() || !input.IsValid())
            {
                return false;
            }

            if (inputIndex < 0 || inputIndex >= mixer.GetInputCount())
            {
                return false;
            }

            return mixer.AddInput(input, inputIndex, 0.0f) >= 0;
        }

        /// <summary>
        /// Disconnects an input from the mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisconnectInput(in Playable mixer, int inputIndex)
        {
            if (mixer.IsValid() && inputIndex >= 0 && inputIndex < mixer.GetInputCount())
            {
                mixer.DisconnectInput(inputIndex);
            }
        }

        /// <summary>
        /// Gets the playable connected to a specific input port.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetInput(in Playable mixer, int inputIndex, out Playable input)
        {
            input = Playable.Null;
            if (mixer.IsValid() && inputIndex >= 0 && inputIndex < mixer.GetInputCount())
            {
                input = mixer.GetInput(inputIndex);
            }
        }

        /// <summary>
        /// Checks if an input port is connected.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInputConnected(in Playable mixer, int inputIndex)
        {
            return mixer.IsValid() && inputIndex >= 0 && inputIndex < mixer.GetInputCount() && mixer.IsInputConnected(inputIndex);
        }

        /// <summary>
        /// Sets the weight of all inputs to zero (disable blending).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearAllWeights(in Playable playable)
        {
            if (!playable.IsValid()) return;

            int count = playable.GetInputCount();
            for (int i = 0; i < count; i++)
            {
                playable.SetInputWeight(i, 0.0f);
            }
        }

        /// <summary>
        /// Normalizes all input weights so they sum to 1.0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeWeights(in Playable playable)
        {
            if (!playable.IsValid()) return;

            int count = playable.GetInputCount();
            float totalWeight = 0.0f;

            // Calculate total weight
            for (int i = 0; i < count; i++)
            {
                totalWeight += playable.GetInputWeight(i);
            }

            // Avoid division by zero
            if (totalWeight <= 0.0001f) return;

            // Normalize weights
            for (int i = 0; i < count; i++)
            {
                float weight = playable.GetInputWeight(i);
                playable.SetInputWeight(i, weight / totalWeight);
            }
        }
    }
}
