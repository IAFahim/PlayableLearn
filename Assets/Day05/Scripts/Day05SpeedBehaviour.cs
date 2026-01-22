using UnityEngine;
using UnityEngine.Playables;
using Unity.Mathematics;

namespace PlayableLearn.Day05
{
    /// <summary>
    /// A PlayableBehaviour that demonstrates time dilation by manipulating SetSpeed.
    ///
    /// Day 05: Time Dilation
    /// Demonstrates using SetSpeed to control the playback speed of a Playable.
    ///
    /// Key concept: SetSpeed() allows you to speed up, slow down, or reverse
    /// the playback of any Playable, creating time dilation effects.
    /// </summary>
    public class Day05SpeedBehaviour : PlayableBehaviour
    {
        // Speed control properties
        public float SpeedMultiplier { get; set; } = 1.0f;
        public bool EnableTimeDilation { get; set; } = true;
        public float TargetSpeed { get; set; } = 1.0f;
        public float InterpolationSpeed { get; set; } = 2.0f;

        // Track current speed for interpolation
        private float currentSpeed;

        // Track the last applied speed to avoid redundant SetSpeed calls
        private double lastAppliedSpeed;

        // Flag to track if speed needs to be applied
        private bool speedNeedsUpdate = true;

        /// <summary>
        /// Called when the playable is started.
        /// Initialize our speed state.
        /// </summary>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            currentSpeed = SpeedMultiplier;
            lastAppliedSpeed = double.NaN; // Force initial SetSpeed call
            speedNeedsUpdate = true;
        }

        /// <summary>
        /// Called when the playable is paused or stopped.
        /// </summary>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // Day 05: No special pause handling needed
        }

        /// <summary>
        /// Called every frame while the playable is running.
        /// THIS IS THE TIME DILATION LOGIC - manipulating SetSpeed.
        ///
        /// This is where we apply speed changes to control time flow.
        /// </summary>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // Guard clause: Don't apply if time dilation is disabled
            if (!EnableTimeDilation)
            {
                return;
            }

            // Guard clause: Don't apply if weight is 0 (not being used)
            if (info.weight <= 0f)
            {
                return;
            }

            // Get delta time for this frame
            float deltaTime = info.deltaTime;

            // Smoothly interpolate towards target speed
            SpeedOps.CalculateDampedSpeed(currentSpeed, TargetSpeed, InterpolationSpeed, deltaTime, out float newSpeed);
            currentSpeed = newSpeed;

            // Validate speed
            if (!SpeedOps.IsValidSpeed(currentSpeed))
            {
                Debug.LogWarning("[SpeedBehaviour] Invalid speed detected, resetting to 1.0");
                currentSpeed = 1.0f;
            }

            // Normalize speed to reasonable range
            SpeedOps.NormalizeSpeed(currentSpeed, out float normalizedSpeed);
            currentSpeed = normalizedSpeed;

            // Apply the speed to the playable using SetSpeed
            // This is the core time dilation operation!
            // Only call SetSpeed when the speed actually changes to avoid redundant calls
            SpeedOps.CalculateSpeedFactor(currentSpeed, out double speedFactor);

            if (speedNeedsUpdate || !double.IsNaN(lastAppliedSpeed) && math.abs(lastAppliedSpeed - speedFactor) > 1e-6)
            {
                playable.SetSpeed(speedFactor);
                lastAppliedSpeed = speedFactor;
                speedNeedsUpdate = false;
            }

            // Log state changes for debugging (only when speed changes significantly)
            if (math.abs(currentSpeed - SpeedMultiplier) > 0.01f)
            {
                SpeedMultiplier = currentSpeed;
            }
        }

        /// <summary>
        /// Called when the playable is being destroyed.
        /// </summary>
        public override void OnPlayableDestroy(Playable playable)
        {
            // Day 05: No cleanup needed
        }

        /// <summary>
        /// Sets the speed parameters.
        /// </summary>
        public void SetSpeedParameters(float speedMultiplier, bool enableTimeDilation, float interpolationSpeed = 2.0f)
        {
            SpeedMultiplier = speedMultiplier;
            TargetSpeed = speedMultiplier;
            EnableTimeDilation = enableTimeDilation;
            InterpolationSpeed = interpolationSpeed;
            currentSpeed = speedMultiplier;
        }

        /// <summary>
        /// Sets a new target speed for smooth transition.
        /// </summary>
        public void SetTargetSpeed(float targetSpeed)
        {
            TargetSpeed = targetSpeed;
        }

        /// <summary>
        /// Immediately sets the speed without interpolation.
        /// </summary>
        public void SetSpeedImmediate(float speed)
        {
            SpeedMultiplier = speed;
            TargetSpeed = speed;
            currentSpeed = speed;
            speedNeedsUpdate = true; // Flag that speed needs to be applied
        }

        /// <summary>
        /// Gets the current speed (including interpolation).
        /// </summary>
        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }

        /// <summary>
        /// Checks if the playback is paused (speed ≈ 0).
        /// </summary>
        public bool IsPaused()
        {
            return SpeedOps.IsPaused(currentSpeed);
        }

        /// <summary>
        /// Checks if the playback is reversed (speed < 0).
        /// </summary>
        public bool IsReversed()
        {
            return SpeedOps.IsReversed(currentSpeed);
        }

        /// <summary>
        /// Checks if the playback is in fast forward (speed > 1).
        /// </summary>
        public bool IsFastForward()
        {
            return SpeedOps.IsFastForward(currentSpeed);
        }

        /// <summary>
        /// Checks if the playback is in slow motion (0 < speed < 1).
        /// </summary>
        public bool IsSlowMotion()
        {
            return SpeedOps.IsSlowMotion(currentSpeed);
        }
    }
}
