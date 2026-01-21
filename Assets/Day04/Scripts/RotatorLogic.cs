using Unity.Burst;
using Unity.Mathematics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day04
{
    /// <summary>
    /// Burst-compiled pure logic operations for rotator nodes.
    /// Layer B: Operations - No Debug calls, pure computation.
    /// </summary>
    [BurstCompile]
    public static class RotatorLogic
    {
        /// <summary>
        /// Calculates the new rotation for a transform based on delta time.
        /// This is the core "Update Cycle" logic for Day 04.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateRotation(in float3 currentEuler, in float3 axis, float speed, float deltaTime, out float3 newEuler)
        {
            // Calculate the rotation amount for this frame
            float rotationAmount = speed * deltaTime;

            // Add the rotation around the specified axis
            float3 rotationDelta = math.normalize(axis) * rotationAmount;
            newEuler = currentEuler + rotationDelta;
        }

        /// <summary>
        /// Normalizes an axis vector to ensure it's a unit vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeAxis(in float3 axis, out float3 normalizedAxis)
        {
            float length = math.length(axis);
            if (length > 1e-6f)
            {
                normalizedAxis = axis / length;
            }
            else
            {
                // Default to Y axis if zero or near-zero
                normalizedAxis = new float3(0, 1, 0);
            }
        }

        /// <summary>
        /// Creates a quaternion from euler angles.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EulerToQuaternion(in float3 euler, out quaternion rotation)
        {
            rotation = quaternion.Euler(euler);
        }

        /// <summary>
        /// Converts a Vector3 to float3 for Burst compatibility.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Vector3ToFloat3(in Vector3 vector, out float3 float3Value)
        {
            float3Value = new float3(vector.x, vector.y, vector.z);
        }

        /// <summary>
        /// Converts a float3 to Vector3.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Float3ToVector3(in float3 float3Value, out Vector3 vector)
        {
            vector = new Vector3(float3Value.x, float3Value.y, float3Value.z);
        }

        /// <summary>
        /// Checks if a rotation speed is valid (not NaN or infinity).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidRotationSpeed(float speed)
        {
            return !math.isnan(speed) && !math.isinf(speed);
        }

        /// <summary>
        /// Clamps rotation speed to a reasonable range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClampRotationSpeed(float speed, float minSpeed, float maxSpeed, out float clampedSpeed)
        {
            clampedSpeed = math.clamp(speed, minSpeed, maxSpeed);
        }
    }
}
