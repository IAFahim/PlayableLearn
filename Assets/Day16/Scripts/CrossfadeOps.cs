using UnityEngine;
using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace PlayableLearn.Day16
{
    /// <summary>
    /// Burst-compiled operations for crossfade management.
    /// Layer B: Operations - Pure functions using Burst for performance.
    /// This is the "Engine" layer - low-level operations for crossfade transitions.
    ///
    /// Day 16 Concept: Crossfade Logic
    /// - Time-based weight transitions (lerping over time)
    /// - Configurable crossfade duration
    /// - Multiple easing curves (linear, ease-in, ease-out, ease-in-out)
    /// - Progress tracking
    /// </summary>
    [BurstCompile]
    public static class CrossfadeOps
    {
        /// <summary>
        /// Crossfade curve types.
        /// </summary>
        public enum CrossfadeCurve
        {
            Linear = 0,
            EaseIn = 1,
            EaseOut = 2,
            EaseInOut = 3
        }

        /// <summary>
        /// Calculates the interpolated value based on the curve type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApplyCurve(float t, CrossfadeCurve curve)
        {
            switch (curve)
            {
                case CrossfadeCurve.Linear:
                    return t;

                case CrossfadeCurve.EaseIn:
                    // Quadratic ease-in: t^2
                    return t * t;

                case CrossfadeCurve.EaseOut:
                    // Quadratic ease-out: t * (2 - t)
                    return t * (2.0f - t);

                case CrossfadeCurve.EaseInOut:
                    // Smooth step: 3t^2 - 2t^3
                    return t * t * (3.0f - 2.0f * t);

                default:
                    return t;
            }
        }

        /// <summary>
        /// Lerps between two float values using linear interpolation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t)
        {
            return math.lerp(a, b, t);
        }

        /// <summary>
        /// Calculates crossfade weights at a given progress (0-1).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateCrossfadeWeights(
            float startWeight0, float startWeight1,
            float targetWeight0, float targetWeight1,
            float progress, bool normalize,
            out float weight0, out float weight1)
        {
            // Apply curve to progress
            float curvedProgress = math.clamp(progress, 0.0f, 1.0f);

            // Interpolate weights
            weight0 = math.lerp(startWeight0, targetWeight0, curvedProgress);
            weight1 = math.lerp(startWeight1, targetWeight1, curvedProgress);

            // Normalize if requested
            if (normalize)
            {
                float total = weight0 + weight1;
                if (total > 0.0001f)
                {
                    weight0 /= total;
                    weight1 /= total;
                }
                else
                {
                    // If total is near zero, set to equal weights
                    weight0 = 0.5f;
                    weight1 = 0.5f;
                }
            }
        }

        /// <summary>
        /// Updates crossfade progress based on elapsed time and duration.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float UpdateProgress(float elapsedTime, float duration)
        {
            if (duration <= 0.0f)
            {
                return 1.0f;
            }

            return math.clamp(elapsedTime / duration, 0.0f, 1.0f);
        }

        /// <summary>
        /// Checks if a crossfade is complete based on progress.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCrossfadeComplete(float progress, float tolerance = 0.001f)
        {
            return progress >= (1.0f - tolerance);
        }

        /// <summary>
        /// Applies weights to a mixer playable.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ApplyWeights(in Playable mixer, float weight0, float weight1)
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
        /// Gets the current weight from a mixer input.
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
        /// Validates crossfade parameters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ValidateCrossfadeParams(float duration, float startWeight0, float startWeight1, float targetWeight0, float targetWeight1)
        {
            // Duration must be positive
            if (duration < 0.0f)
            {
                return false;
            }

            // Weights should be in reasonable range
            if (startWeight0 < 0.0f || startWeight0 > 1.0f ||
                startWeight1 < 0.0f || startWeight1 > 1.0f ||
                targetWeight0 < 0.0f || targetWeight0 > 1.0f ||
                targetWeight1 < 0.0f || targetWeight1 > 1.0f)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the estimated time remaining for a crossfade.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalculateTimeRemaining(float elapsedTime, float duration)
        {
            return math.max(0.0f, duration - elapsedTime);
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
        /// Checks if the current weights match the target weights within tolerance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreWeightsAtTarget(
            float currentWeight0, float currentWeight1,
            float targetWeight0, float targetWeight1,
            float tolerance = 0.001f)
        {
            float diff0 = math.abs(currentWeight0 - targetWeight0);
            float diff1 = math.abs(currentWeight1 - targetWeight1);
            return diff0 < tolerance && diff1 < tolerance;
        }

        /// <summary>
        /// Clamps a value between 0 and 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            return math.clamp(value, 0.0f, 1.0f);
        }

        /// <summary>
        /// Sets explicit weights on both mixer inputs.
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
    }
}
