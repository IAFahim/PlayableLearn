using Unity.Burst;
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day07
{
    /// <summary>
    /// Burst-compiled pure logic operations for Reverse Time control.
    /// Layer B: Operations - No Debug calls, pure computation.
    /// </summary>
    [BurstCompile]
    public static class ReverseTimeOps
    {
        /// <summary>
        /// Checks if the given speed indicates reverse time (negative speed).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReversing(double speed)
        {
            return speed < 0.0;
        }

        /// <summary>
        /// Checks if the given speed indicates forward time (positive speed).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsForward(double speed)
        {
            return speed > 0.0;
        }

        /// <summary>
        /// Checks if the given speed indicates stopped time (zero or very close to zero).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStopped(double speed)
        {
            return math.abs(speed) < 1e-10;
        }

        /// <summary>
        /// Clamps a speed value between minimum and maximum bounds.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampSpeed(float minSpeed, float maxSpeed, float targetSpeed)
        {
            return math.clamp(targetSpeed, minSpeed, maxSpeed);
        }

        /// <summary>
        /// Wraps accumulated time within the specified limit.
        /// Time will wrap from +limit to -limit and vice versa.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double WrapTime(double accumulatedTime, double wrapLimit)
        {
            if (wrapLimit <= 0.0) return accumulatedTime;

            // Wrap using modulo-like behavior that preserves sign
            double doubleLimit = wrapLimit * 2.0;
            double wrapped = ((accumulatedTime + wrapLimit) % doubleLimit + doubleLimit) % doubleLimit - wrapLimit;
            return wrapped;
        }

        /// <summary>
        /// Updates accumulated time based on delta time and speed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double UpdateAccumulatedTime(double currentAccumulatedTime, double deltaTime, double speed)
        {
            return currentAccumulatedTime + (deltaTime * speed);
        }

        /// <summary>
        /// Determines if time should wrap based on accumulated time and limit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldWrapTime(double accumulatedTime, double wrapLimit)
        {
            return wrapLimit > 0.0 && (math.abs(accumulatedTime) >= wrapLimit);
        }

        /// <summary>
        /// Checks if two speeds have different directions (forward vs reverse).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DidDirectionChange(double previousSpeed, double currentSpeed)
        {
            // If one is negative and the other is positive (and neither is zero)
            return (previousSpeed * currentSpeed) < 0.0;
        }

        /// <summary>
        /// Checks if the speed transitioned from forward to reverse.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTransitionToReverse(double previousSpeed, double currentSpeed)
        {
            return previousSpeed >= 0.0 && currentSpeed < 0.0;
        }

        /// <summary>
        /// Checks if the speed transitioned from reverse to forward.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTransitionToForward(double previousSpeed, double currentSpeed)
        {
            return previousSpeed < 0.0 && currentSpeed >= 0.0;
        }

        /// <summary>
        /// Gets the absolute speed value (magnitude) regardless of direction.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSpeedMagnitude(float speed)
        {
            return math.abs(speed);
        }

        /// <summary>
        /// Gets the speed direction as a normalized value (-1, 0, or 1).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSpeedDirection(double speed)
        {
            if (speed > 1e-10) return 1;
            if (speed < -1e-10) return -1;
            return 0;
        }

        /// <summary>
        /// Checks if a speed value is within the valid reverse range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidReverseSpeed(float speed, float minSpeed, float maxSpeed)
        {
            return speed >= minSpeed && speed <= maxSpeed;
        }

        /// <summary>
        /// Checks if reverse time functionality is enabled.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReverseEnabled(bool enableReverseTime)
        {
            return enableReverseTime;
        }

        /// <summary>
        /// Checks if time wrapping functionality is enabled.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTimeWrappingEnabled(bool enableTimeWrapping)
        {
            return enableTimeWrapping;
        }

        /// <summary>
        /// Calculates normalized time progress (0.0 to 1.0) within the wrap limit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalculateNormalizedProgress(double accumulatedTime, double wrapLimit)
        {
            if (wrapLimit <= 0.0) return 0.0f;

            double doubleLimit = wrapLimit * 2.0;
            double normalizedTime = ((accumulatedTime + wrapLimit) % doubleLimit + doubleLimit) % doubleLimit;
            return (float)(normalizedTime / doubleLimit);
        }

        /// <summary>
        /// Gets the time direction as a string representation.
        /// Returns ">>" for forward, "<<" for reverse, "||" for stopped.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDirectionSymbol(double speed)
        {
            int direction = GetSpeedDirection(speed);
            return direction; // 1 for forward, -1 for reverse, 0 for stopped
        }

        /// <summary>
        /// Validates that a wrap limit is positive.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidWrapLimit(double wrapLimit)
        {
            return wrapLimit > 0.0;
        }

        /// <summary>
        /// Calculates how much the time has wrapped (the overflow amount).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CalculateWrapOverflow(double accumulatedTime, double wrapLimit)
        {
            if (wrapLimit <= 0.0) return 0.0;

            if (accumulatedTime > wrapLimit)
            {
                return accumulatedTime - wrapLimit;
            }
            else if (accumulatedTime < -wrapLimit)
            {
                return accumulatedTime + wrapLimit;
            }

            return 0.0;
        }
    }
}
