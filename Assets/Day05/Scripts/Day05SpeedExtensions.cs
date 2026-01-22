using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day03;

namespace PlayableLearn.Day05
{
    /// <summary>
    /// Extension methods for Day05SpeedData.
    /// Layer C: Extensions - High-level adapter methods that combine operations.
    /// </summary>
    public static class Day05SpeedExtensions
    {
        /// <summary>
        /// Initializes a new speed control node with the specified parameters.
        /// </summary>
        public static void Initialize(ref this Day05SpeedData data, in PlayableGraph graph, string nodeName, float speedMultiplier = 1.0f, bool enableTimeDilation = true, float interpolationSpeed = 2.0f)
        {
            if (data.IsActive)
            {
                Debug.LogWarning($"[SpeedData] Already initialized: {nodeName}");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[SpeedData] Cannot initialize speed control: Graph is invalid.");
                return;
            }

            // Create the ScriptPlayable with Day05SpeedBehaviour
            ScriptPlayableOps.Create<Day05SpeedBehaviour>(in graph, 0, out data.Node);

            if (!ScriptPlayableOps.IsValid(in data.Node))
            {
                Debug.LogError($"[SpeedData] Failed to create speed control node: {nodeName}");
                return;
            }

            // Get the behaviour and set its parameters
            if (data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                behaviour.SetSpeedParameters(speedMultiplier, enableTimeDilation, interpolationSpeed);
                data.Graph = graph;
                data.IsActive = true;
                data.NodeId = nodeName.GetHashCode();
                data.SpeedMultiplier = speedMultiplier;
                data.EnableTimeDilation = enableTimeDilation;
                data.TargetSpeed = speedMultiplier;
                data.InterpolationSpeed = interpolationSpeed;

                Debug.Log($"[SpeedData] Created speed control: {nodeName}, Speed: {speedMultiplier}x, Enabled: {enableTimeDilation}");
            }
            else
            {
                Debug.LogError($"[SpeedData] Failed to get behaviour: {nodeName}");
            }
        }

        /// <summary>
        /// Disposes the speed control node, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day05SpeedData data)
        {
            if (!data.IsActive) return;

            ScriptPlayableOps.Destroy(in data.Graph, in data.Node);
            data.IsActive = false;
            data.NodeId = 0;
            data.SpeedMultiplier = 1.0f;
            data.EnableTimeDilation = false;
            Debug.Log("[SpeedData] Speed control node disposed.");
        }

        /// <summary>
        /// Connects this speed control node to a playable output.
        /// </summary>
        public static void ConnectToOutput(this in Day05SpeedData data, in PlayableOutput output)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning("[SpeedData] Cannot connect: Speed control is not active.");
                return;
            }

            if (!output.IsOutputValid())
            {
                Debug.LogWarning("[SpeedData] Cannot connect: Output is not valid.");
                return;
            }

            ScriptPlayableOps.SetSource(in output, in data.Node);
            Debug.Log("[SpeedData] Connected speed control to output.");
        }

        /// <summary>
        /// Checks if the speed control node is valid and active.
        /// </summary>
        public static bool IsValidSpeedControl(this in Day05SpeedData data)
        {
            return data.IsActive && ScriptPlayableOps.IsValid(in data.Node);
        }

        /// <summary>
        /// Sets a new target speed for smooth transition.
        /// </summary>
        public static void SetTargetSpeed(ref this Day05SpeedData data, float targetSpeed)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                Debug.LogWarning("[SpeedData] Cannot set target speed: Speed control is not valid.");
                return;
            }

            behaviour.SetTargetSpeed(targetSpeed);
            data.TargetSpeed = targetSpeed;
            Debug.Log($"[SpeedData] Target speed set to: {targetSpeed}x");
        }

        /// <summary>
        /// Immediately sets the speed without interpolation.
        /// </summary>
        public static void SetSpeedImmediate(ref this Day05SpeedData data, float speed)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                Debug.LogWarning("[SpeedData] Cannot set immediate speed: Speed control is not valid.");
                return;
            }

            behaviour.SetSpeedImmediate(speed);
            data.SpeedMultiplier = speed;
            data.TargetSpeed = speed;
            Debug.Log($"[SpeedData] Speed immediately set to: {speed}x");
        }

        /// <summary>
        /// Enables or disables time dilation.
        /// </summary>
        public static void SetTimeDilationEnabled(ref this Day05SpeedData data, bool enabled)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                Debug.LogWarning("[SpeedData] Cannot set time dilation: Speed control is not valid.");
                return;
            }

            behaviour.EnableTimeDilation = enabled;
            data.EnableTimeDilation = enabled;
            Debug.Log($"[SpeedData] Time dilation {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Sets the interpolation speed for smooth transitions.
        /// </summary>
        public static void SetInterpolationSpeed(ref this Day05SpeedData data, float interpolationSpeed)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                Debug.LogWarning("[SpeedData] Cannot set interpolation speed: Speed control is not valid.");
                return;
            }

            behaviour.InterpolationSpeed = interpolationSpeed;
            data.InterpolationSpeed = interpolationSpeed;
            Debug.Log($"[SpeedData] Interpolation speed set to: {interpolationSpeed}");
        }

        /// <summary>
        /// Gets the current speed (including interpolation).
        /// </summary>
        public static float GetCurrentSpeed(this in Day05SpeedData data)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                Debug.LogWarning("[SpeedData] Cannot get current speed: Speed control is not valid.");
                return 1.0f;
            }

            return behaviour.GetCurrentSpeed();
        }

        /// <summary>
        /// Checks if the playback is paused.
        /// </summary>
        public static bool IsPaused(this in Day05SpeedData data)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                return false;
            }

            return behaviour.IsPaused();
        }

        /// <summary>
        /// Checks if the playback is reversed.
        /// </summary>
        public static bool IsReversed(this in Day05SpeedData data)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                return false;
            }

            return behaviour.IsReversed();
        }

        /// <summary>
        /// Checks if the playback is in fast forward.
        /// </summary>
        public static bool IsFastForward(this in Day05SpeedData data)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                return false;
            }

            return behaviour.IsFastForward();
        }

        /// <summary>
        /// Checks if the playback is in slow motion.
        /// </summary>
        public static bool IsSlowMotion(this in Day05SpeedData data)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                return false;
            }

            return behaviour.IsSlowMotion();
        }

        /// <summary>
        /// Gets information about the speed control's configuration.
        /// </summary>
        public static void LogSpeedInfo(this in Day05SpeedData data, string controlName)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning($"[SpeedData] Cannot log: Speed control is not active.");
                return;
            }

            float currentSpeed = data.GetCurrentSpeed();
            string speedState = data.IsPaused() ? "Paused" :
                               data.IsReversed() ? "Reversed" :
                               data.IsFastForward() ? "Fast Forward" :
                               data.IsSlowMotion() ? "Slow Motion" : "Normal";

            Debug.Log($"[SpeedControl] Name: {controlName}, Speed: {currentSpeed:F2}x, State: {speedState}, Enabled: {data.EnableTimeDilation}");
        }
    }
}
