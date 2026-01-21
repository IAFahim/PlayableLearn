using UnityEngine;
using UnityEngine.Playables;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace PlayableLearn.Day16
{
    /// <summary>
    /// PlayableBehaviour for handling crossfade logic in the PlayableGraph.
    /// This behaviour is attached to the mixer and handles automatic crossfade updates.
    ///
    /// Day 16 Concept: Crossfade Logic
    /// - Automatic time-based weight transitions
    /// - Per-frame crossfade updates
    /// - Supports different easing curves
    /// </summary>
    [BurstCompile]
    public class Day16CrossfadeBehaviour : PlayableBehaviour
    {
        // Starting weights for crossfade
        public float StartWeight0;
        public float StartWeight1;

        // Target weights for crossfade
        public float TargetWeight0;
        public float TargetWeight1;

        // Crossfade duration in seconds
        public float CrossfadeDuration;

        // Elapsed time since crossfade started
        public float ElapsedTime;

        // Whether crossfade is active
        public bool IsCrossfading;

        // Curve type for crossfade
        public CrossfadeOps.CrossfadeCurve CurveType;

        // Whether to normalize weights
        public bool NormalizeWeights;

        /// <summary>
        /// Called when the playable is prepared.
        /// </summary>
        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);

            // Initialize default values
            StartWeight0 = 1.0f;
            StartWeight1 = 0.0f;
            TargetWeight0 = 1.0f;
            TargetWeight1 = 0.0f;
            CrossfadeDuration = 1.0f;
            ElapsedTime = 0.0f;
            IsCrossfading = false;
            CurveType = CrossfadeOps.CrossfadeCurve.Linear;
            NormalizeWeights = false;
        }

        /// <summary>
        /// Called every frame while the playable is active.
        /// This is where we update the crossfade weights.
        /// </summary>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);

            if (!IsCrossfading)
            {
                return;
            }

            // Update elapsed time
            float deltaTime = info.DeltaTime;
            ElapsedTime += deltaTime;

            // Calculate progress
            float progress = CrossfadeOps.UpdateProgress(ElapsedTime, CrossfadeDuration);

            // Apply curve to progress
            float curvedProgress = CrossfadeOps.ApplyCurve(progress, CurveType);

            // Calculate current weights
            float weight0 = math.lerp(StartWeight0, TargetWeight0, curvedProgress);
            float weight1 = math.lerp(StartWeight1, TargetWeight1, curvedProgress);

            // Normalize if requested
            if (NormalizeWeights)
            {
                float total = weight0 + weight1;
                if (total > 0.0001f)
                {
                    weight0 /= total;
                    weight1 /= total;
                }
            }

            // Apply weights to mixer inputs
            if (playable.GetInputCount() >= 2)
            {
                playable.SetInputWeight(0, weight0);
                playable.SetInputWeight(1, weight1);
            }

            // Check if crossfade is complete
            if (CrossfadeOps.IsCrossfadeComplete(progress))
            {
                IsCrossfading = false;
                ElapsedTime = CrossfadeDuration;

                // Ensure final weights are exact
                if (playable.GetInputCount() >= 2)
                {
                    playable.SetInputWeight(0, TargetWeight0);
                    playable.SetInputWeight(1, TargetWeight1);
                }
            }
        }

        /// <summary>
        /// Called when the playable is destroyed.
        /// </summary>
        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            // Clean up
            IsCrossfading = false;
            ElapsedTime = 0.0f;
        }

        /// <summary>
        /// Starts a crossfade to the specified target weights.
        /// </summary>
        public void StartCrossfade(float targetWeight0, float targetWeight1, float duration, CrossfadeOps.CrossfadeCurve curve)
        {
            // Get current weights as starting weights
            // Note: In a real scenario, you'd pass the mixer as a parameter
            // For simplicity, we assume this is called from a context where we can get the weights
            TargetWeight0 = math.clamp(targetWeight0, 0.0f, 1.0f);
            TargetWeight1 = math.clamp(targetWeight1, 0.0f, 1.0f);
            CrossfadeDuration = math.max(0.001f, duration);
            CurveType = curve;
            ElapsedTime = 0.0f;
            IsCrossfading = true;
        }

        /// <summary>
        /// Stops the current crossfade.
        /// </summary>
        public void StopCrossfade()
        {
            IsCrossfading = false;
        }

        /// <summary>
        /// Gets the current crossfade progress (0-1).
        /// </summary>
        public float GetProgress()
        {
            return CrossfadeOps.UpdateProgress(ElapsedTime, CrossfadeDuration);
        }

        /// <summary>
        /// Checks if the crossfade is complete.
        /// </summary>
        public bool IsComplete()
        {
            float progress = GetProgress();
            return CrossfadeOps.IsCrossfadeComplete(progress);
        }

        /// <summary>
        /// Gets the time remaining for the crossfade.
        /// </summary>
        public float GetTimeRemaining()
        {
            return CrossfadeOps.CalculateTimeRemaining(ElapsedTime, CrossfadeDuration);
        }
    }
}
