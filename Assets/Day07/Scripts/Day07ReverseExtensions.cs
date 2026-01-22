using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day05;

namespace PlayableLearn.Day07
{
    /// <summary>
    /// Extension methods for Day07ReverseData.
    /// Layer C: Extensions - High-level adapter methods that combine operations.
    /// </summary>
    public static class Day07ReverseExtensions
    {
        private const double DefaultWrapLimit = 10.0; // Default 10 seconds wrap limit

        /// <summary>
        /// Initializes Reverse Time control for the given graph.
        /// </summary>
        public static void Initialize(ref this Day07ReverseData data, in PlayableGraph graph, string controllerName, bool enableReverseTime, bool enableTimeWrapping)
        {
            if (data.IsActive)
            {
                Debug.LogWarning($"[ReverseTimeControl] Already initialized: {controllerName}");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[ReverseTimeControl] Cannot initialize: Graph is invalid.");
                return;
            }

            data.Graph = graph;
            data.IsActive = true;
            data.ControllerId = controllerName.GetHashCode();
            data.IsValidControl = true;
            data.EnableReverseTime = enableReverseTime;
            data.EnableTimeWrapping = enableTimeWrapping;
            data.AccumulatedTime = 0.0;
            data.WrapLimit = DefaultWrapLimit;
            data.LastKnownSpeed = 1.0; // Start with forward speed

            Debug.Log($"[ReverseTimeControl] Initialized: {controllerName}, Reverse: {enableReverseTime}, Wrapping: {enableTimeWrapping}");
        }

        /// <summary>
        /// Disposes the Reverse Time control.
        /// </summary>
        public static void Dispose(ref this Day07ReverseData data)
        {
            if (!data.IsActive) return;

            data.IsActive = false;
            data.ControllerId = 0;
            data.IsValidControl = false;
            Debug.Log("[ReverseTimeControl] Disposed.");
        }

        /// <summary>
        /// Updates time tracking based on delta time.
        /// Should be called in Update().
        /// </summary>
        public static void UpdateTimeTracking(ref this Day07ReverseData data, float deltaTime)
        {
            if (!data.IsActive) return;

            if (!data.Graph.IsValid())
            {
                data.IsValidControl = false;
                return;
            }

            // Get current speed from speed control if available
            // Note: This requires access to Day05SpeedData which should be managed by the entry point
            // For now, we'll track time based on delta time and last known speed
            double newAccumulatedTime = ReverseTimeOps.UpdateAccumulatedTime(
                data.AccumulatedTime,
                deltaTime,
                data.LastKnownSpeed
            );

            // Apply time wrapping if enabled
            if (ReverseTimeOps.IsTimeWrappingEnabled(data.EnableTimeWrapping))
            {
                if (ReverseTimeOps.ShouldWrapTime(newAccumulatedTime, data.WrapLimit))
                {
                    double overflow = ReverseTimeOps.CalculateWrapOverflow(newAccumulatedTime, data.WrapLimit);
                    newAccumulatedTime = ReverseTimeOps.WrapTime(newAccumulatedTime, data.WrapLimit);
                    Debug.Log($"[ReverseTimeControl] Time wrapped: {overflow:F2}s overflow");
                }
            }

            data.AccumulatedTime = newAccumulatedTime;
        }

        /// <summary>
        /// Sets the current speed for time tracking.
        /// This should be called from the speed control system.
        /// </summary>
        public static void SetCurrentSpeed(ref this Day07ReverseData data, float speed)
        {
            if (!data.IsActive) return;

            // Check for direction change
            if (ReverseTimeOps.DidDirectionChange(data.LastKnownSpeed, speed))
            {
                if (ReverseTimeOps.IsTransitionToReverse(data.LastKnownSpeed, speed))
                {
                    Debug.Log("[ReverseTimeControl] Direction changed: Forward -> Reverse");
                }
                else if (ReverseTimeOps.IsTransitionToForward(data.LastKnownSpeed, speed))
                {
                    Debug.Log("[ReverseTimeControl] Direction changed: Reverse -> Forward");
                }
            }

            data.LastKnownSpeed = speed;
        }

        /// <summary>
        /// Checks if time is currently flowing in reverse (negative speed).
        /// </summary>
        public static bool IsReversing(this in Day07ReverseData data)
        {
            if (!data.IsActive || !data.EnableReverseTime) return false;

            return ReverseTimeOps.IsReversing(data.LastKnownSpeed);
        }

        /// <summary>
        /// Checks if time is currently flowing forward (positive speed).
        /// </summary>
        public static bool IsForward(this in Day07ReverseData data)
        {
            if (!data.IsActive) return false;

            return ReverseTimeOps.IsForward(data.LastKnownSpeed);
        }

        /// <summary>
        /// Checks if time is stopped (zero or very close to zero speed).
        /// </summary>
        public static bool IsStopped(this in Day07ReverseData data)
        {
            if (!data.IsActive) return false;

            return ReverseTimeOps.IsStopped(data.LastKnownSpeed);
        }

        /// <summary>
        /// Gets the current accumulated time.
        /// </summary>
        public static double GetAccumulatedTime(this in Day07ReverseData data)
        {
            if (!data.IsActive) return 0.0;

            return data.AccumulatedTime;
        }

        /// <summary>
        /// Sets the wrap limit for time wrapping.
        /// </summary>
        public static void SetWrapLimit(ref this Day07ReverseData data, double wrapLimit)
        {
            if (!data.IsActive) return;

            if (!ReverseTimeOps.IsValidWrapLimit(wrapLimit))
            {
                Debug.LogWarning("[ReverseTimeControl] Invalid wrap limit. Must be positive.");
                return;
            }

            data.WrapLimit = wrapLimit;
            Debug.Log($"[ReverseTimeControl] Wrap limit set to: {wrapLimit:F2}s");
        }

        /// <summary>
        /// Clamps a target speed within the specified bounds.
        /// </summary>
        public static float ClampSpeed(this in Day07ReverseData data, float minSpeed, float maxSpeed, float targetSpeed)
        {
            if (!data.IsActive || !data.EnableReverseTime) return targetSpeed;

            return ReverseTimeOps.ClampSpeed(minSpeed, maxSpeed, targetSpeed);
        }

        /// <summary>
        /// Gets the speed magnitude (absolute value).
        /// </summary>
        public static float GetSpeedMagnitude(this in Day07ReverseData data)
        {
            if (!data.IsActive) return 0.0f;

            return ReverseTimeOps.GetSpeedMagnitude((float)data.LastKnownSpeed);
        }

        /// <summary>
        /// Gets the speed direction as an integer (-1, 0, or 1).
        /// </summary>
        public static int GetSpeedDirection(this in Day07ReverseData data)
        {
            if (!data.IsActive) return 0;

            return ReverseTimeOps.GetSpeedDirection(data.LastKnownSpeed);
        }

        /// <summary>
        /// Gets the time direction symbol (">>" for forward, "<<" for reverse, "||" for stopped).
        /// </summary>
        public static string GetDirectionSymbol(this in Day07ReverseData data)
        {
            if (!data.IsActive) return "||";

            int direction = data.GetSpeedDirection();
            return direction > 0 ? ">>" : (direction < 0 ? "<<" : "||");
        }

        /// <summary>
        /// Gets normalized time progress (0.0 to 1.0) within the wrap limit.
        /// </summary>
        public static float GetNormalizedProgress(this in Day07ReverseData data)
        {
            if (!data.IsActive) return 0.0f;

            return ReverseTimeOps.CalculateNormalizedProgress(data.AccumulatedTime, data.WrapLimit);
        }

        /// <summary>
        /// Checks if the Reverse Time control is valid and active.
        /// </summary>
        public static bool IsValidReverseControl(this in Day07ReverseData data)
        {
            return data.IsActive && data.IsValidControl && data.Graph.IsValid();
        }

        /// <summary>
        /// Logs the current reverse time information.
        /// </summary>
        public static void LogReverseInfo(this in Day07ReverseData data, string controllerName)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning($"[ReverseTimeControl] Cannot log: Control is not active.");
                return;
            }

            string directionStr = data.GetDirectionSymbol();
            double accumulatedTime = data.GetAccumulatedTime();
            float speedMagnitude = data.GetSpeedMagnitude();
            bool isReversing = data.IsReversing();
            bool isForward = data.IsForward();
            bool isStopped = data.IsStopped();

            Debug.Log($"[ReverseTimeControl] Name: {controllerName}, Direction: {directionStr}, Time: {accumulatedTime:F2}s, Speed: {speedMagnitude:F2}x, Reversing: {isReversing}, Forward: {isForward}, Stopped: {isStopped}");
        }

        /// <summary>
        /// Enables or disables reverse time functionality.
        /// </summary>
        public static void SetReverseEnabled(ref this Day07ReverseData data, bool enabled)
        {
            if (!data.IsActive) return;

            data.EnableReverseTime = enabled;
            Debug.Log($"[ReverseTimeControl] Reverse time {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Enables or disables time wrapping functionality.
        /// </summary>
        public static void SetTimeWrappingEnabled(ref this Day07ReverseData data, bool enabled)
        {
            if (!data.IsActive) return;

            data.EnableTimeWrapping = enabled;
            Debug.Log($"[ReverseTimeControl] Time wrapping {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Resets accumulated time to zero.
        /// </summary>
        public static void ResetTime(ref this Day07ReverseData data)
        {
            if (!data.IsActive) return;

            data.AccumulatedTime = 0.0;
            Debug.Log("[ReverseTimeControl] Time reset to 0.0s");
        }

        /// <summary>
        /// Toggles reverse time on/off (sets speed to negative or positive of same magnitude).
        /// </summary>
        public static void ToggleReverse(this in Day07ReverseData data)
        {
            if (!data.IsActive || !data.EnableReverseTime) return;

            // Note: This method requires access to the speed control to actually change the speed
            // For now, we just log the intent
            bool currentState = data.IsReversing();
            Debug.Log($"[ReverseTimeControl] Toggle reverse requested. Current state: {(currentState ? "Reversing" : "Forward")}");
        }

        /// <summary>
        /// Gets the wrap limit.
        /// </summary>
        public static double GetWrapLimit(this in Day07ReverseData data)
        {
            if (!data.IsActive) return DefaultWrapLimit;

            return data.WrapLimit;
        }

        /// <summary>
        /// Checks if time wrapping is currently active.
        /// </summary>
        public static bool IsTimeWrappingActive(this in Day07ReverseData data)
        {
            if (!data.IsActive) return false;

            return data.EnableTimeWrapping && ReverseTimeOps.IsValidWrapLimit(data.WrapLimit);
        }

        /// <summary>
        /// Checks if the accumulated time is near the wrap limit.
        /// </summary>
        public static bool IsNearWrapLimit(this in Day07ReverseData data, double threshold)
        {
            if (!data.IsActive || !data.IsTimeWrappingActive()) return false;

            double distanceToLimit = data.WrapLimit - System.Math.Abs(data.AccumulatedTime);
            return distanceToLimit <= threshold;
        }
    }
}
