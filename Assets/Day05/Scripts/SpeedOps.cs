using Unity.Burst;
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day05
{
    /// <summary>
    /// Burst-compiled pure logic operations for speed manipulation.
    /// Layer B: Operations - No Debug calls, pure computation.
    /// </summary>
    [BurstCompile]
    public static class SpeedOps
    {
        /// <summary>
        /// Calculates the new speed multiplier with linear interpolation.
        /// Used for smooth speed transitions.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateInterpolatedSpeed(float currentSpeed, float targetSpeed, float interpolationSpeed, float deltaTime, out float newSpeed)
        {
            // Linear interpolation towards target speed
            float t = math.saturate(interpolationSpeed * deltaTime);
            newSpeed = math.lerp(currentSpeed, targetSpeed, t);
        }

        /// <summary>
        /// Clamps speed multiplier to a reasonable range.
        /// Prevents negative speeds and extreme values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClampSpeed(float speed, float minSpeed, float maxSpeed, out float clampedSpeed)
        {
            clampedSpeed = math.clamp(speed, minSpeed, maxSpeed);
        }

        /// <summary>
        /// Checks if a speed value is valid (not NaN or infinity).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidSpeed(float speed)
        {
            return !math.isnan(speed) && !math.isinf(speed);
        }

        /// <summary>
        /// Calculates the effective delta time based on speed multiplier.
        /// This is what actually applies time dilation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateEffectiveDeltaTime(float deltaTime, float speedMultiplier, out float effectiveDeltaTime)
        {
            effectiveDeltaTime = deltaTime * speedMultiplier;
        }

        /// <summary>
        /// Checks if time dilation should be applied.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldApplyTimeDilation(bool enableTimeDilation, float speedMultiplier)
        {
            // Only apply if enabled and speed is not approximately 1.0
            return enableTimeDilation && math.abs(speedMultiplier - 1.0f) >= 1e-6f;
        }

        /// <summary>
        /// Calculates the speed factor for SetSpeed operation.
        /// SetSpeed uses a double, so we need to convert.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateSpeedFactor(float speedMultiplier, out double speedFactor)
        {
            speedFactor = speedMultiplier;
        }

        /// <summary>
        /// Smoothly dampens speed towards target (exponential approach).
        /// Alternative to linear interpolation for more natural transitions.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateDampedSpeed(float currentSpeed, float targetSpeed, float dampingFactor, float deltaTime, out float newSpeed)
        {
            // Exponential interpolation: new = current + (target - current) * factor
            float delta = (targetSpeed - currentSpeed) * math.saturate(dampingFactor * deltaTime);
            newSpeed = currentSpeed + delta;
        }

        /// <summary>
        /// Checks if speed is approximately zero (paused).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPaused(float speed)
        {
            return math.abs(speed) < 1e-6f;
        }

        /// <summary>
        /// Checks if speed is reversed (negative).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReversed(float speed)
        {
            return speed < 0.0f;
        }

        /// <summary>
        /// Checks if speed is faster than normal (speed > 1.0).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFastForward(float speed)
        {
            return speed > 1.0f;
        }

        /// <summary>
        /// Checks if speed is slower than normal (0 < speed < 1.0).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSlowMotion(float speed)
        {
            return speed > 0.0f && speed < 1.0f;
        }

        /// <summary>
        /// Normalizes speed to ensure it's within valid range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeSpeed(float speed, out float normalizedSpeed)
        {
            if (IsValidSpeed(speed))
            {
                ClampSpeed(speed, 0.0f, 10.0f, out normalizedSpeed);
            }
            else
            {
                normalizedSpeed = 1.0f; // Default to normal speed
            }
        }
    }
}
