using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day15
{
    /// <summary>
    /// Extension methods for Day15BlendData.
    /// Layer C: Extensions - Provides initialization, weight management, and lifecycle.
    /// This is the "Adapter" layer that makes weighted blending easy to use.
    ///
    /// Day 15 Concept: Weighted Blending
    /// - Set explicit weights like 0.5/0.5 for equal blending
    /// - Smooth transitions between weights
    /// - Weight normalization support
    /// </summary>
    public static class Day15BlendExtensions
    {
        /// <summary>
        /// Initializes a new weighted blend data structure.
        /// </summary>
        public static void Initialize(ref this Day15BlendData blendData, in PlayableGraph graph, in Playable mixer, int inputCount = 2)
        {
            if (blendData.IsActive)
            {
                Debug.LogWarning($"[BlendData] Already initialized. Call Dispose first.");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[BlendData] Cannot initialize: Graph is invalid.");
                return;
            }

            if (!mixer.IsValid())
            {
                Debug.LogError($"[BlendData] Cannot initialize: Mixer is invalid.");
                return;
            }

            if (mixer.GetInputCount() < 2)
            {
                Debug.LogError($"[BlendData] Cannot initialize: Mixer must have at least 2 inputs.");
                return;
            }

            blendData.Graph = graph;
            blendData.Mixer = mixer;
            blendData.InputCount = inputCount;
            blendData.Weight0 = 1.0f;
            blendData.Weight1 = 0.0f;
            blendData.TargetWeight0 = 1.0f;
            blendData.TargetWeight1 = 0.0f;
            blendData.BlendSpeed = 2.0f;
            blendData.NormalizeWeights = false;
            blendData.IsActive = true;
            blendData.BlendId = inputCount.GetHashCode();
            blendData.CurrentBlend = 0.0f;

            Debug.Log($"[BlendData] Initialized weighted blend with {inputCount} inputs.");
        }

        /// <summary>
        /// Disposes the blend data.
        /// </summary>
        public static void Dispose(ref this Day15BlendData blendData)
        {
            if (!blendData.IsActive) return;

            blendData.IsActive = false;
            blendData.InputCount = 0;
            blendData.Weight0 = 0.0f;
            blendData.Weight1 = 0.0f;
            blendData.TargetWeight0 = 0.0f;
            blendData.TargetWeight1 = 0.0f;
            blendData.BlendSpeed = 0.0f;
            blendData.CurrentBlend = 0.0f;
            blendData.BlendId = 0;

            Debug.Log("[BlendData] Disposed.");
        }

        /// <summary>
        /// Sets explicit weights on both inputs (e.g., 0.5f, 0.5f for equal blend).
        /// </summary>
        public static void SetWeights(ref this Day15BlendData blendData, float weight0, float weight1, bool applyImmediately = true)
        {
            if (!blendData.IsActive)
            {
                Debug.LogWarning("[BlendData] Cannot set weights: Blend data is not active.");
                return;
            }

            blendData.TargetWeight0 = weight0;
            blendData.TargetWeight1 = weight1;

            if (applyImmediately)
            {
                blendData.Weight0 = weight0;
                blendData.Weight1 = weight1;

                WeightBlendOps.SetWeights(in blendData.Mixer, weight0, weight1);
                UpdateBlendValue(ref blendData);

                Debug.Log($"[BlendData] Set weights: Input0={weight0:F2}, Input1={weight1:F2}");
            }
        }

        /// <summary>
        /// Sets the weight of a specific input.
        /// </summary>
        public static void SetWeight(ref this Day15BlendData blendData, int inputIndex, float weight, bool applyImmediately = true)
        {
            if (!blendData.IsActive)
            {
                Debug.LogWarning("[BlendData] Cannot set weight: Blend data is not active.");
                return;
            }

            if (inputIndex == 0)
            {
                blendData.TargetWeight0 = weight;
                if (applyImmediately)
                {
                    blendData.Weight0 = weight;
                    WeightBlendOps.SetWeight(in blendData.Mixer, 0, weight);
                    UpdateBlendValue(ref blendData);
                }
            }
            else if (inputIndex == 1)
            {
                blendData.TargetWeight1 = weight;
                if (applyImmediately)
                {
                    blendData.Weight1 = weight;
                    WeightBlendOps.SetWeight(in blendData.Mixer, 1, weight);
                    UpdateBlendValue(ref blendData);
                }
            }
            else
            {
                Debug.LogWarning($"[BlendData] Input index {inputIndex} is out of range (0-1).");
            }
        }

        /// <summary>
        /// Gets the current weight of a specific input.
        /// </summary>
        public static bool TryGetWeight(this in Day15BlendData blendData, int inputIndex, out float weight)
        {
            weight = 0.0f;
            if (!blendData.IsActive)
            {
                return false;
            }

            if (inputIndex == 0)
            {
                weight = blendData.Weight0;
                return true;
            }
            else if (inputIndex == 1)
            {
                weight = blendData.Weight1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets blend value (0.0 = input 0 only, 0.5 = equal blend, 1.0 = input 1 only).
        /// </summary>
        public static void SetBlend(ref this Day15BlendData blendData, float blend, bool applyImmediately = true)
        {
            if (!blendData.IsActive)
            {
                Debug.LogWarning("[BlendData] Cannot set blend: Blend data is not active.");
                return;
            }

            float clampedBlend = Mathf.Clamp01(blend);
            float weight0 = 1.0f - clampedBlend;
            float weight1 = clampedBlend;

            SetWeights(ref blendData, weight0, weight1, applyImmediately);
        }

        /// <summary>
        /// Gets the current blend value.
        /// </summary>
        public static bool TryGetBlend(this in Day15BlendData blendData, out float blend)
        {
            blend = blendData.CurrentBlend;
            return blendData.IsActive;
        }

        /// <summary>
        /// Sets both weights to 0.5 for equal blending.
        /// </summary>
        public static void SetEqualBlend(ref this Day15BlendData blendData, bool applyImmediately = true)
        {
            SetWeights(ref blendData, 0.5f, 0.5f, applyImmediately);
            Debug.Log("[BlendData] Set equal blend (0.5/0.5).");
        }

        /// <summary>
        /// Sets blend speed for smooth transitions.
        /// </summary>
        public static void SetBlendSpeed(ref this Day15BlendData blendData, float blendSpeed)
        {
            if (!blendData.IsActive)
            {
                Debug.LogWarning("[BlendData] Cannot set blend speed: Blend data is not active.");
                return;
            }

            blendData.BlendSpeed = Mathf.Max(0.0f, blendSpeed);
            Debug.Log($"[BlendData] Set blend speed to {blendData.BlendSpeed:F2}.");
        }

        /// <summary>
        /// Enables or disables weight normalization.
        /// </summary>
        public static void SetNormalizeWeights(ref this Day15BlendData blendData, bool normalize)
        {
            if (!blendData.IsActive)
            {
                Debug.LogWarning("[BlendData] Cannot set normalize: Blend data is not active.");
                return;
            }

            blendData.NormalizeWeights = normalize;
            Debug.Log($"[BlendData] Normalize weights set to {normalize}.");
        }

        /// <summary>
        /// Updates weights towards target values (call this in Update for smooth transitions).
        /// </summary>
        public static void UpdateWeights(ref this Day15BlendData blendData, float deltaTime)
        {
            if (!blendData.IsActive)
            {
                return;
            }

            // Update current weights towards targets
            WeightBlendOps.UpdateWeights(
                ref blendData.Weight0,
                ref blendData.Weight1,
                blendData.TargetWeight0,
                blendData.TargetWeight1,
                blendData.BlendSpeed,
                deltaTime
            );

            // Apply weights to mixer
            WeightBlendOps.ApplyWeights(
                in blendData.Mixer,
                blendData.Weight0,
                blendData.Weight1,
                blendData.NormalizeWeights
            );

            // Update blend value
            UpdateBlendValue(ref blendData);
        }

        /// <summary>
        /// Checks if the blend is complete (weights match targets).
        /// </summary>
        public static bool IsBlendComplete(this in Day15BlendData blendData, float tolerance = 0.001f)
        {
            if (!blendData.IsActive)
            {
                return false;
            }

            return WeightBlendOps.IsBlendComplete(
                blendData.Weight0,
                blendData.Weight1,
                blendData.TargetWeight0,
                blendData.TargetWeight1,
                tolerance
            );
        }

        /// <summary>
        /// Checks if weights are at equal blend (0.5/0.5).
        /// </summary>
        public static bool IsEqualBlend(this in Day15BlendData blendData, float tolerance = 0.001f)
        {
            if (!blendData.IsActive)
            {
                return false;
            }

            return WeightBlendOps.IsEqualBlend(blendData.Weight0, blendData.Weight1, tolerance);
        }

        /// <summary>
        /// Checks if the blend data is valid and active.
        /// </summary>
        public static bool IsValidBlend(this in Day15BlendData blendData)
        {
            return blendData.IsActive && blendData.Mixer.IsValid();
        }

        /// <summary>
        /// Logs blend information to the console.
        /// </summary>
        public static void LogToConsole(this in Day15BlendData blendData, string blendName)
        {
            if (!blendData.IsActive)
            {
                Debug.LogWarning($"[BlendData] Cannot log: Blend data is not active.");
                return;
            }

            Debug.Log($"[BlendData] Name: {blendName}");
            Debug.Log($"[BlendData]   Current Weights: Input0={blendData.Weight0:F2}, Input1={blendData.Weight1:F2}");
            Debug.Log($"[BlendData]   Target Weights: Input0={blendData.TargetWeight0:F2}, Input1={blendData.TargetWeight1:F2}");
            Debug.Log($"[BlendData]   Blend Value: {blendData.CurrentBlend:F2}");
            Debug.Log($"[BlendData]   Blend Speed: {blendData.BlendSpeed:F2}");
            Debug.Log($"[BlendData]   Normalize: {blendData.NormalizeWeights}");
            Debug.Log($"[BlendData]   Equal Blend: {IsEqualBlend(blendData)}");
            Debug.Log($"[BlendData]   Blend Complete: {IsBlendComplete(blendData)}");
        }

        /// <summary>
        /// Updates the internal blend value based on current weights.
        /// </summary>
        private static void UpdateBlendValue(ref Day15BlendData blendData)
        {
            // Blend is represented by weight of input 1
            blendData.CurrentBlend = blendData.Weight1;
        }
    }
}
