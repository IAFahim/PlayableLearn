using UnityEngine;
using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace PlayableLearn.Day15
{
    /// <summary>
    /// Burst-compiled operations for weighted blend management.
    /// Layer B: Operations - Pure functions using Burst for performance.
    /// This is the "Engine" layer - low-level operations for blend weights.
    ///
    /// Day 15 Concept: Weighted Blending
    /// - Set explicit weights on mixer inputs (e.g., 0.5/0.5 for equal blend)
    /// - Smooth transitions between weights
    /// - Weight normalization support
    /// </summary>
    [BurstCompile]
    public static class WeightBlendOps
    {
        /// <summary>
        /// Sets explicit weights on both mixer inputs.
        /// Example: SetWeights(mixer, 0.5f, 0.5f) for equal blending.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetWeights(in Playable mixer, float weight0, float weight1)
        {
            if (!mixer.IsValid())
            {
                return;
            }

            if (mixer.GetInputCount() < 2)
            {
                return;
            }

            mixer.SetInputWeight(0, weight0);
            mixer.SetInputWeight(1, weight1);
        }

        /// <summary>
        /// Sets the weight of a specific input port.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetWeight(in Playable mixer, int inputIndex, float weight)
        {
            if (!mixer.IsValid())
            {
                return;
            }

            if (inputIndex < 0 || inputIndex >= mixer.GetInputCount())
            {
                return;
            }

            mixer.SetInputWeight(inputIndex, weight);
        }

        /// <summary>
        /// Gets the weight of a specific input port.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetWeight(in Playable mixer, int inputIndex, out float weight)
        {
            weight = 0.0f;
            if (!mixer.IsValid())
            {
                return;
            }

            if (inputIndex < 0 || inputIndex >= mixer.GetInputCount())
            {
                return;
            }

            weight = mixer.GetInputWeight(inputIndex);
        }

        /// <summary>
        /// Sets weights from a blend value (0-1).
        /// Blend 0.0 = Weight0=1.0, Weight1=0.0 (input 0 only)
        /// Blend 0.5 = Weight0=0.5, Weight1=0.5 (equal blend)
        /// Blend 1.0 = Weight0=0.0, Weight1=1.0 (input 1 only)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBlend(in Playable mixer, float blend)
        {
            if (!mixer.IsValid())
            {
                return;
            }

            if (mixer.GetInputCount() < 2)
            {
                return;
            }

            // Clamp blend to 0-1 range
            float clampedBlend = math.clamp(blend, 0.0f, 1.0f);

            // Set weights based on blend value
            mixer.SetInputWeight(0, 1.0f - clampedBlend);
            mixer.SetInputWeight(1, clampedBlend);
        }

        /// <summary>
        /// Gets the current blend value from input weights.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBlend(in Playable mixer, out float blend)
        {
            blend = 0.0f;
            if (!mixer.IsValid() || mixer.GetInputCount() < 2)
            {
                return;
            }

            // Blend is represented by the weight of input 1
            blend = mixer.GetInputWeight(1);
        }

        /// <summary>
        /// Updates weights towards target weights with the specified speed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateWeights(ref float currentWeight0, ref float currentWeight1, float targetWeight0, float targetWeight1, float blendSpeed, float deltaTime)
        {
            // Calculate interpolation factor based on blend speed
            float t = math.saturate(blendSpeed * deltaTime);

            // Interpolate towards target weights
            currentWeight0 = math.lerp(currentWeight0, targetWeight0, t);
            currentWeight1 = math.lerp(currentWeight1, targetWeight1, t);
        }

        /// <summary>
        /// Normalizes two weights so they sum to 1.0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeWeights(ref float weight0, ref float weight1)
        {
            float total = weight0 + weight1;

            // Avoid division by zero
            if (total < 0.0001f)
            {
                // If total is near zero, set to equal weights
                weight0 = 0.5f;
                weight1 = 0.5f;
            }
            else
            {
                weight0 /= total;
                weight1 /= total;
            }
        }

        /// <summary>
        /// Checks if weights are approximately equal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreWeightsEqual(float weight0, float weight1, float tolerance = 0.001f)
        {
            return math.abs(weight0 - weight1) < tolerance;
        }

        /// <summary>
        /// Checks if weights are at the 0.5/0.5 split.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqualBlend(float weight0, float weight1, float tolerance = 0.001f)
        {
            return math.abs(weight0 - 0.5f) < tolerance && math.abs(weight1 - 0.5f) < tolerance;
        }

        /// <summary>
        /// Sets both weights to 0.5 (equal blend).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetEqualBlend(in Playable mixer)
        {
            SetWeights(in mixer, 0.5f, 0.5f);
        }

        /// <summary>
        /// Applies the weights to the mixer after updating them.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ApplyWeights(in Playable mixer, float weight0, float weight1, bool normalize)
        {
            if (!mixer.IsValid())
            {
                return;
            }

            if (mixer.GetInputCount() < 2)
            {
                return;
            }

            float finalWeight0 = weight0;
            float finalWeight1 = weight1;

            // Normalize if requested
            if (normalize)
            {
                NormalizeWeights(ref finalWeight0, ref finalWeight1);
            }

            // Apply weights
            mixer.SetInputWeight(0, finalWeight0);
            mixer.SetInputWeight(1, finalWeight1);
        }

        /// <summary>
        /// Gets the total weight sum.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetTotalWeight(float weight0, float weight1)
        {
            return weight0 + weight1;
        }

        /// <summary>
        /// Checks if the blend is complete (weights match targets within tolerance).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBlendComplete(float currentWeight0, float currentWeight1, float targetWeight0, float targetWeight1, float tolerance = 0.001f)
        {
            float diff0 = math.abs(currentWeight0 - targetWeight0);
            float diff1 = math.abs(currentWeight1 - targetWeight1);
            return diff0 < tolerance && diff1 < tolerance;
        }
    }
}
