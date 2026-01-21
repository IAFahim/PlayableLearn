using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day03;
using PlayableLearn.Day04;

namespace PlayableLearn.Day05
{
    /// <summary>
    /// MonoBehaviour entry point for Day 05: Time Dilation.
    /// Demonstrates manipulating SetSpeed to control playback speed.
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day05SpeedData, Day04RotatorData, Day02OutputHandle, Day01GraphHandle)
    /// - Layer B: Operations (SpeedOps, RotatorLogic, ScriptPlayableOps)
    /// - Layer C: Extensions (Day05SpeedExtensions, Day04RotatorExtensions, Day02OutputHandleExtensions, Day01GraphHandleExtensions)
    /// </summary>
    public class Day05Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day01GraphHandle graphHandle;

        [SerializeField]
        private Day02OutputHandle outputHandle;

        [SerializeField]
        private Day04RotatorData rotatorData;

        [SerializeField]
        private Day05SpeedData speedData;

        // Speed control settings - exposed in Inspector for easy experimentation
        [Header("Time Dilation Settings")]
        [SerializeField]
        private float initialSpeedMultiplier = 1.0f;

        [SerializeField]
        private bool enableTimeDilation = true;

        [SerializeField]
        private float interpolationSpeed = 2.0f;

        // Rotation settings (from Day 04)
        [Header("Rotation Settings")]
        [SerializeField]
        private float rotationSpeed = 90.0f;

        [SerializeField]
        private Vector3 rotationAxis = Vector3.up;

        // Runtime speed control
        [Header("Runtime Speed Control")]
        [SerializeField]
        [Range(0.0f, 5.0f)]
        private float targetSpeed = 1.0f;

        // LAYER C: Adapter Calls
        private void OnEnable()
        {
            InitializeGraph();
            InitializeOutput();
            InitializeRotator();
            InitializeSpeedControl();
            LinkNodesTogether();
            LogSystemInfo();
        }

        private void OnDisable()
        {
            CleanupSpeedControl();
            CleanupRotator();
            CleanupOutput();
            CleanupGraph();
        }

        private void Update()
        {
            // PURE VIEW LOGIC
            // Update target speed from inspector
            if (speedData.IsValidSpeedControl() && math.abs(targetSpeed - speedData.TargetSpeed) > 0.01f)
            {
                speedData.SetTargetSpeed(targetSpeed);
            }

            // Visual feedback: Show time dilation effect with a pulse
            float currentSpeed = speedData.IsValidSpeedControl() ? speedData.GetCurrentSpeed() : 1.0f;
            float pulse = 1.0f + Mathf.Sin(Time.time * currentSpeed * 3.0f) * 0.05f;
            transform.localScale = Vector3.one * pulse;
        }

        private void InitializeGraph()
        {
            string graphName = $"{gameObject.name}_Graph_{Time.frameCount}";
            var graph = graphHandle;
            graph.Initialize(graphName);
            graphHandle = graph;
        }

        private void InitializeOutput()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 05] Cannot initialize output: Graph is not initialized.");
                return;
            }

            string outputName = $"{gameObject.name}_ConsoleOutput";
            PlayableGraph graph = graphHandle.Graph;
            var output = outputHandle;
            output.Initialize(in graph, outputName);
            outputHandle = output;
        }

        private void InitializeRotator()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 05] Cannot initialize rotator: Graph is not initialized.");
                return;
            }

            string rotatorName = $"{gameObject.name}_Rotator";
            PlayableGraph graph = graphHandle.Graph;
            var rotator = rotatorData;
            rotator.Initialize(in graph, rotatorName, transform, rotationSpeed, rotationAxis);
            rotatorData = rotator;
        }

        private void InitializeSpeedControl()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 05] Cannot initialize speed control: Graph is not initialized.");
                return;
            }

            string speedControlName = $"{gameObject.name}_SpeedControl";
            PlayableGraph graph = graphHandle.Graph;
            var speed = speedData;
            speed.Initialize(in graph, speedControlName, initialSpeedMultiplier, enableTimeDilation, interpolationSpeed);
            speedData = speed;

            // Set initial target speed
            targetSpeed = initialSpeedMultiplier;
        }

        private void LinkNodesTogether()
        {
            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 05] Cannot link: Speed control is not valid.");
                return;
            }

            if (!rotatorData.IsValidRotator())
            {
                Debug.LogError("[Day 05] Cannot link: Rotator is not valid.");
                return;
            }

            // Connect rotator to speed control
            // This creates the chain: Output -> SpeedControl -> Rotator
            Playable speedPlayable = speedData.Node;
            rotatorData.Node.AddInput(speedPlayable, 0, 1.0f);

            // Connect speed control to output
            PlayableOutput output = outputHandle.Output;
            speedData.ConnectToOutput(in output);

            Debug.Log("[Day 05] Successfully linked nodes: Output -> SpeedControl -> Rotator!");
        }

        private void LogSystemInfo()
        {
            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogWarning("[Day 05] Speed control is not valid, cannot log info.");
                return;
            }

            string speedControlName = $"{gameObject.name}_SpeedControl";
            speedData.LogSpeedInfo(speedControlName);

            // Log the node type to verify it's a ScriptPlayable
            if (speedData.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour))
            {
                Debug.Log($"[Day 05] Behaviour Type: {behaviour.GetType().Name}");
                Debug.Log($"[Day 05] Initial Speed: {behaviour.SpeedMultiplier}x");
                Debug.Log($"[Day 05] Time Dilation Enabled: {behaviour.EnableTimeDilation}");
                Debug.Log($"[Day 05] Interpolation Speed: {behaviour.InterpolationSpeed}");
            }
        }

        private void CleanupSpeedControl()
        {
            var speed = speedData;
            speed.Dispose();
            speedData = speed;
        }

        private void CleanupRotator()
        {
            var rotator = rotatorData;
            rotator.Dispose();
            rotatorData = rotator;
        }

        private void CleanupOutput()
        {
            var output = outputHandle;
            output.Dispose();
            outputHandle = output;
        }

        private void CleanupGraph()
        {
            var graph = graphHandle;
            graph.Dispose();
            graphHandle = graph;
        }

        /// <summary>
        /// Called by Unity when the component is reset in the Inspector.
        /// Good for setting default values.
        /// </summary>
        private void Reset()
        {
            initialSpeedMultiplier = 1.0f;
            enableTimeDilation = true;
            interpolationSpeed = 2.0f;
            targetSpeed = 1.0f;
            rotationSpeed = 90.0f;
            rotationAxis = Vector3.up;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only: Draw gizmos to visualize the rotation axis and speed.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!rotatorData.IsValidRotator()) return;

            // Draw rotation axis (from Day 04)
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, rotationAxis.normalized * 2.0f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + rotationAxis.normalized * 2.0f, 0.1f);

            // Draw speed indicator
            if (speedData.IsValidSpeedControl())
            {
                float currentSpeed = speedData.GetCurrentSpeed();
                Gizmos.color = currentSpeed > 1.0f ? Color.red :
                              currentSpeed < 1.0f && currentSpeed > 0 ? Color.blue :
                              currentSpeed <= 0 ? Color.gray : Color.green;

                Vector3 speedLabelPos = transform.position + Vector3.up * 2.5f;
                UnityEditor.Handles.Label(speedLabelPos, $"Speed: {currentSpeed:F2}x");
            }
        }
#endif
    }
}
