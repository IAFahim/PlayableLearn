using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day16
{
    /// <summary>
    /// Extension methods for Day16CrossfadeData.
    /// Layer C: Extensions - Provides initialization, crossfade management, and lifecycle.
    /// This is the "Adapter" layer that makes crossfading easy to use.
    ///
    /// Day 16 Concept: Crossfade Logic
    /// - Time-based weight transitions using lerping
    /// - Configurable crossfade duration
    /// - Multiple easing curves for smooth transitions
    /// - Progress tracking for crossfade completion
    /// </summary>
    public static class Day16CrossfadeExtensions
    {
        /// <summary>
        /// Initializes a new crossfade data structure.
        /// </summary>
        public static void Initialize(ref this Day16CrossfadeData crossfadeData, in PlayableGraph graph, in Playable mixer, int inputCount = 2)
        {
            if (crossfadeData.IsActive)
            {
                Debug.LogWarning($"[CrossfadeData] Already initialized. Call Dispose first.");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[CrossfadeData] Cannot initialize: Graph is invalid.");
                return;
            }

            if (!mixer.IsValid())
            {
                Debug.LogError($"[CrossfadeData] Cannot initialize: Mixer is invalid.");
                return;
            }

            if (mixer.GetInputCount() < 2)
            {
                Debug.LogError($"[CrossfadeData] Cannot initialize: Mixer must have at least 2 inputs.");
                return;
            }

            crossfadeData.Graph = graph;
            crossfadeData.Mixer = mixer;
            crossfadeData.InputCount = inputCount;
            crossfadeData.CurrentWeight0 = 1.0f;
            crossfadeData.CurrentWeight1 = 0.0f;
            crossfadeData.StartWeight0 = 1.0f;
            crossfadeData.StartWeight1 = 0.0f;
            crossfadeData.TargetWeight0 = 1.0f;
            crossfadeData.TargetWeight1 = 0.0f;
            crossfadeData.CrossfadeDuration = 1.0f;
            crossfadeData.ElapsedTime = 0.0f;
            crossfadeData.IsCrossfading = false;
            crossfadeData.CurveType = (int)CrossfadeOps.CrossfadeCurve.Linear;
            crossfadeData.IsActive = true;
            crossfadeData.CrossfadeId = inputCount.GetHashCode();
            crossfadeData.Progress = 0.0f;
            crossfadeData.NormalizeWeights = false;

            Debug.Log($"[CrossfadeData] Initialized crossfade with {inputCount} inputs.");
        }

        /// <summary>
        /// Disposes the crossfade data.
        /// </summary>
        public static void Dispose(ref this Day16CrossfadeData crossfadeData)
        {
            if (!crossfadeData.IsActive) return;

            crossfadeData.IsActive = false;
            crossfadeData.InputCount = 0;
            crossfadeData.CurrentWeight0 = 0.0f;
            crossfadeData.CurrentWeight1 = 0.0f;
            crossfadeData.StartWeight0 = 0.0f;
            crossfadeData.StartWeight1 = 0.0f;
            crossfadeData.TargetWeight0 = 0.0f;
            crossfadeData.TargetWeight1 = 0.0f;
            crossfadeData.CrossfadeDuration = 0.0f;
            crossfadeData.ElapsedTime = 0.0f;
            crossfadeData.IsCrossfading = false;
            crossfadeData.Progress = 0.0f;
            crossfadeData.CrossfadeId = 0;

            Debug.Log("[CrossfadeData] Disposed.");
        }

        /// <summary>
        /// Starts a crossfade to the target weights with the specified duration.
        /// </summary>
        public static void StartCrossfade(
            ref this Day16CrossfadeData crossfadeData,
            float targetWeight0, float targetWeight1,
            float duration,
            CrossfadeOps.CrossfadeCurve curve = CrossfadeOps.CrossfadeCurve.Linear)
        {
            if (!crossfadeData.IsActive)
            {
                Debug.LogWarning("[CrossfadeData] Cannot start crossfade: Crossfade data is not active.");
                return;
            }

            // Get current weights as starting weights
            CrossfadeOps.GetWeight(in crossfadeData.Mixer, 0, out float currentWeight0);
            CrossfadeOps.GetWeight(in crossfadeData.Mixer, 1, out float currentWeight1);

            crossfadeData.StartWeight0 = currentWeight0;
            crossfadeData.StartWeight1 = currentWeight1;
            crossfadeData.TargetWeight0 = Mathf.Clamp01(targetWeight0);
            crossfadeData.TargetWeight1 = Mathf.Clamp01(targetWeight1);
            crossfadeData.CrossfadeDuration = Mathf.Max(0.001f, duration);
            crossfadeData.ElapsedTime = 0.0f;
            crossfadeData.IsCrossfading = true;
            crossfadeData.CurveType = (int)curve;
            crossfadeData.Progress = 0.0f;

            Debug.Log($"[CrossfadeData] Started crossfade: ({currentWeight0:F2}, {currentWeight1:F2}) -> ({targetWeight0:F2}, {targetWeight1:F2}) over {duration:F2}s with {curve} curve.");
        }

        /// <summary>
        /// Starts a crossfade using blend value (0-1).
        /// </summary>
        public static void StartCrossfadeByBlend(
            ref this Day16CrossfadeData crossfadeData,
            float targetBlend,
            float duration,
            CrossfadeOps.CrossfadeCurve curve = CrossfadeOps.CrossfadeCurve.Linear)
        {
            float clampedBlend = Mathf.Clamp01(targetBlend);
            float targetWeight0 = 1.0f - clampedBlend;
            float targetWeight1 = clampedBlend;

            StartCrossfade(ref crossfadeData, targetWeight0, targetWeight1, duration, curve);
        }

        /// <summary>
        /// Updates the crossfade by deltaTime. Call this in Update.
        /// </summary>
        public static void UpdateCrossfade(ref this Day16CrossfadeData crossfadeData, float deltaTime)
        {
            if (!crossfadeData.IsActive)
            {
                return;
            }

            if (!crossfadeData.IsCrossfading)
            {
                return;
            }

            // Update elapsed time
            crossfadeData.ElapsedTime += deltaTime;

            // Update progress
            crossfadeData.Progress = CrossfadeOps.UpdateProgress(
                crossfadeData.ElapsedTime,
                crossfadeData.CrossfadeDuration
            );

            // Calculate current weights based on progress
            CrossfadeOps.CalculateCrossfadeWeights(
                crossfadeData.StartWeight0,
                crossfadeData.StartWeight1,
                crossfadeData.TargetWeight0,
                crossfadeData.TargetWeight1,
                crossfadeData.Progress,
                crossfadeData.NormalizeWeights,
                out float weight0,
                out float weight1
            );

            // Update current weights
            crossfadeData.CurrentWeight0 = weight0;
            crossfadeData.CurrentWeight1 = weight1;

            // Apply weights to mixer
            CrossfadeOps.ApplyWeights(in crossfadeData.Mixer, weight0, weight1);

            // Check if crossfade is complete
            if (CrossfadeOps.IsCrossfadeComplete(crossfadeData.Progress))
            {
                crossfadeData.IsCrossfading = false;
                crossfadeData.ElapsedTime = crossfadeData.CrossfadeDuration;
                crossfadeData.Progress = 1.0f;

                // Ensure final weights are exactly the target weights
                CrossfadeOps.ApplyWeights(in crossfadeData.Mixer, crossfadeData.TargetWeight0, crossfadeData.TargetWeight1);

                Debug.Log($"[CrossfadeData] Crossfade complete. Final weights: ({crossfadeData.TargetWeight0:F2}, {crossfadeData.TargetWeight1:F2})");
            }
        }

        /// <summary>
        /// Stops the current crossfade and keeps current weights.
        /// </summary>
        public static void StopCrossfade(ref this Day16CrossfadeData crossfadeData)
        {
            if (!crossfadeData.IsActive)
            {
                Debug.LogWarning("[CrossfadeData] Cannot stop crossfade: Crossfade data is not active.");
                return;
            }

            if (!crossfadeData.IsCrossfading)
            {
                Debug.LogWarning("[CrossfadeData] No crossfade in progress to stop.");
                return;
            }

            crossfadeData.IsCrossfading = false;
            Debug.Log($"[CrossfadeData] Crossfade stopped at progress {crossfadeData.Progress:F2}.");
        }

        /// <summary>
        /// Completes the crossfade immediately by jumping to target weights.
        /// </summary>
        public static void CompleteCrossfade(ref this Day16CrossfadeData crossfadeData)
        {
            if (!crossfadeData.IsActive)
            {
                Debug.LogWarning("[CrossfadeData] Cannot complete crossfade: Crossfade data is not active.");
                return;
            }

            if (!crossfadeData.IsCrossfading)
            {
                Debug.LogWarning("[CrossfadeData] No crossfade in progress to complete.");
                return;
            }

            // Jump to target weights
            crossfadeData.CurrentWeight0 = crossfadeData.TargetWeight0;
            crossfadeData.CurrentWeight1 = crossfadeData.TargetWeight1;
            crossfadeData.Progress = 1.0f;
            crossfadeData.ElapsedTime = crossfadeData.CrossfadeDuration;
            crossfadeData.IsCrossfading = false;

            // Apply target weights
            CrossfadeOps.ApplyWeights(in crossfadeData.Mixer, crossfadeData.TargetWeight0, crossfadeData.TargetWeight1);

            Debug.Log($"[CrossfadeData] Crossforce completed immediately. Final weights: ({crossfadeData.TargetWeight0:F2}, {crossfadeData.TargetWeight1:F2})");
        }

        /// <summary>
        /// Sets weights immediately without crossfade.
        /// </summary>
        public static void SetWeightsImmediate(ref this Day16CrossfadeData crossfadeData, float weight0, float weight1)
        {
            if (!crossfadeData.IsActive)
            {
                Debug.LogWarning("[CrossfadeData] Cannot set weights: Crossfade data is not active.");
                return;
            }

            // Stop any ongoing crossfade
            if (crossfadeData.IsCrossfading)
            {
                crossfadeData.IsCrossfading = false;
            }

            float clampedWeight0 = Mathf.Clamp01(weight0);
            float clampedWeight1 = Mathf.Clamp01(weight1);

            crossfadeData.CurrentWeight0 = clampedWeight0;
            crossfadeData.CurrentWeight1 = clampedWeight1;
            crossfadeData.StartWeight0 = clampedWeight0;
            crossfadeData.StartWeight1 = clampedWeight1;
            crossfadeData.TargetWeight0 = clampedWeight0;
            crossfadeData.TargetWeight1 = clampedWeight1;
            crossfadeData.Progress = 1.0f;

            // Apply weights
            CrossfadeOps.SetWeights(in crossfadeData.Mixer, clampedWeight0, clampedWeight1);

            Debug.Log($"[CrossfadeData] Set weights immediately: ({clampedWeight0:F2}, {clampedWeight1:F2})");
        }

        /// <summary>
        /// Sets the crossfade curve type.
        /// </summary>
        public static void SetCurveType(ref this Day16CrossfadeData crossfadeData, CrossfadeOps.CrossfadeCurve curve)
        {
            if (!crossfadeData.IsActive)
            {
                Debug.LogWarning("[CrossfadeData] Cannot set curve type: Crossfade data is not active.");
                return;
            }

            crossfadeData.CurveType = (int)curve;
            Debug.Log($"[CrossfadeData] Curve type set to {curve}.");
        }

        /// <summary>
        /// Sets whether weights should be normalized during crossfade.
        /// </summary>
        public static void SetNormalizeWeights(ref this Day16CrossfadeData crossfadeData, bool normalize)
        {
            if (!crossfadeData.IsActive)
            {
                Debug.LogWarning("[CrossfadeData] Cannot set normalize: Crossfade data is not active.");
                return;
            }

            crossfadeData.NormalizeWeights = normalize;
            Debug.Log($"[CrossfadeData] Normalize weights set to {normalize}.");
        }

        /// <summary>
        /// Checks if a crossfade is currently in progress.
        /// </summary>
        public static bool IsCrossfading(this in Day16CrossfadeData crossfadeData)
        {
            return crossfadeData.IsActive && crossfadeData.IsCrossfading;
        }

        /// <summary>
        /// Checks if the crossfade is complete.
        /// </summary>
        public static bool IsCrossfadeComplete(this in Day16CrossfadeData crossfadeData, float tolerance = 0.001f)
        {
            if (!crossfadeData.IsActive)
            {
                return false;
            }

            return CrossfadeOps.IsCrossfadeComplete(crossfadeData.Progress, tolerance);
        }

        /// <summary>
        /// Gets the current progress of the crossfade (0-1).
        /// </summary>
        public static bool TryGetProgress(this in Day16CrossfadeData crossfadeData, out float progress)
        {
            progress = crossfadeData.IsActive ? crossfadeData.Progress : 0.0f;
            return crossfadeData.IsActive;
        }

        /// <summary>
        /// Gets the time remaining for the crossfade.
        /// </summary>
        public static bool TryGetTimeRemaining(this in Day16CrossfadeData crossfadeData, out float timeRemaining)
        {
            if (!crossfadeData.IsActive || !crossfadeData.IsCrossfading)
            {
                timeRemaining = 0.0f;
                return false;
            }

            timeRemaining = CrossfadeOps.CalculateTimeRemaining(crossfadeData.ElapsedTime, crossfadeData.CrossfadeDuration);
            return true;
        }

        /// <summary>
        /// Gets the current weight of a specific input.
        /// </summary>
        public static bool TryGetWeight(this in Day16CrossfadeData crossfadeData, int inputIndex, out float weight)
        {
            weight = 0.0f;
            if (!crossfadeData.IsActive)
            {
                return false;
            }

            if (inputIndex == 0)
            {
                weight = crossfadeData.CurrentWeight0;
                return true;
            }
            else if (inputIndex == 1)
            {
                weight = crossfadeData.CurrentWeight1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the target weight of a specific input.
        /// </summary>
        public static bool TryGetTargetWeight(this in Day16CrossfadeData crossfadeData, int inputIndex, out float weight)
        {
            weight = 0.0f;
            if (!crossfadeData.IsActive)
            {
                return false;
            }

            if (inputIndex == 0)
            {
                weight = crossfadeData.TargetWeight0;
                return true;
            }
            else if (inputIndex == 1)
            {
                weight = crossfadeData.TargetWeight1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the crossfade data is valid and active.
        /// </summary>
        public static bool IsValidCrossfade(this in Day16CrossfadeData crossfadeData)
        {
            return crossfadeData.IsActive && crossfadeData.Mixer.IsValid();
        }

        /// <summary>
        /// Logs crossfade information to the console.
        /// </summary>
        public static void LogToConsole(this in Day16CrossfadeData crossfadeData, string crossfadeName)
        {
            if (!crossfadeData.IsActive)
            {
                Debug.LogWarning($"[CrossfadeData] Cannot log: Crossfade data is not active.");
                return;
            }

            Debug.Log($"[CrossfadeData] Name: {crossfadeName}");
            Debug.Log($"[CrossfadeData]   Current Weights: Input0={crossfadeData.CurrentWeight0:F2}, Input1={crossfadeData.CurrentWeight1:F2}");
            Debug.Log($"[CrossfadeData]   Target Weights: Input0={crossfadeData.TargetWeight0:F2}, Input1={crossfadeData.TargetWeight1:F2}");
            Debug.Log($"[CrossfadeData]   Progress: {crossfadeData.Progress:P2}");
            Debug.Log($"[CrossfadeData]   Duration: {crossfadeData.CrossfadeDuration:F2}s");
            Debug.Log($"[CrossfadeData]   Elapsed: {crossfadeData.ElapsedTime:F2}s");
            Debug.Log($"[CrossfadeData]   Curve Type: {(CrossfadeOps.CrossfadeCurve)crossfadeData.CurveType}");
            Debug.Log($"[CrossfadeData]   Is Crossfading: {crossfadeData.IsCrossfading}");
            Debug.Log($"[CrossfadeData]   Normalize: {crossfadeData.NormalizeWeights}");

            if (crossfadeData.IsCrossfading)
            {
                crossfadeData.TryGetTimeRemaining(out float timeRemaining);
                Debug.Log($"[CrossfadeData]   Time Remaining: {timeRemaining:F2}s");
            }
        }
    }
}
