using UnityEngine;
using UnityEngine.Playables;
using Unity.Mathematics;

namespace PlayableLearn.Day04
{
    /// <summary>
    /// A PlayableBehaviour that rotates a target transform.
    ///
    /// Day 04: The Update Cycle
    /// Demonstrates using PrepareFrame (ProcessFrame equivalent) to update rotation every frame.
    ///
    /// This is the key innovation of Day 04 - implementing the Update Cycle pattern
    /// where the PlayableBehaviour performs per-frame updates through PrepareFrame.
    /// </summary>
    public class Day04RotatorBehaviour : PlayableBehaviour
    {
        // Reference to the transform we want to rotate
        public Transform TargetTransform { get; set; }

        // Rotation settings
        public float RotationSpeed { get; set; } = 90.0f; // degrees per second
        public Vector3 RotationAxis { get; set; } = Vector3.up;

        // Track current rotation state
        private Vector3 currentEulerAngles;

        /// <summary>
        /// Called when the playable is started.
        /// Initialize our rotation state.
        /// </summary>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (TargetTransform != null)
            {
                currentEulerAngles = TargetTransform.eulerAngles;
            }
        }

        /// <summary>
        /// Called when the playable is paused or stopped.
        /// </summary>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // Day 04: No special pause handling needed
        }

        /// <summary>
        /// Called every frame while the playable is running.
        /// THIS IS THE UPDATE CYCLE - equivalent to ProcessFrame.
        ///
        /// This is where we apply the rotation logic using the Burst-compiled operations.
        /// </summary>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // Guard clause: No target, no rotation
            if (TargetTransform == null)
            {
                return;
            }

            // Guard clause: Don't rotate if weight is 0 (not being used)
            if (info.weight <= 0f)
            {
                return;
            }

            // Get delta time for this frame
            float deltaTime = info.deltaTime;

            // Use the Burst-compiled rotation logic
            RotatorLogic.Vector3ToFloat3(currentEulerAngles, out float3 currentEulerFloat3);
            RotatorLogic.Vector3ToFloat3(RotationAxis, out float3 axisFloat3);

            // Calculate new rotation
            RotatorLogic.CalculateRotation(in currentEulerFloat3, in axisFloat3, RotationSpeed, deltaTime, out float3 newEulerFloat3);

            // Convert back to Vector3
            RotatorLogic.Float3ToVector3(in newEulerFloat3, out Vector3 newEulerAngles);
            currentEulerAngles = newEulerAngles;

            // Apply the rotation to the target transform
            TargetTransform.eulerAngles = currentEulerAngles;
        }

        /// <summary>
        /// Called when the playable is being destroyed.
        /// </summary>
        public override void OnPlayableDestroy(Playable playable)
        {
            // Day 04: No cleanup needed - we don't own the transform
        }

        /// <summary>
        /// Sets the rotation parameters.
        /// </summary>
        public void SetRotationParameters(Transform target, float speed, Vector3 axis)
        {
            TargetTransform = target;
            RotationSpeed = speed;
            RotationAxis = axis;

            if (target != null)
            {
                currentEulerAngles = target.eulerAngles;
            }
        }
    }
}
