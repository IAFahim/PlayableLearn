using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day03;

namespace PlayableLearn.Day04
{
    /// <summary>
    /// MonoBehaviour entry point for Day 04: The Update Cycle.
    /// Demonstrates using PrepareFrame (ProcessFrame) to rotate a generic cube.
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day04RotatorData, Day02OutputHandle, Day01GraphHandle)
    /// - Layer B: Operations (RotatorLogic, ScriptPlayableOps)
    /// - Layer C: Extensions (Day04RotatorExtensions, Day02OutputHandleExtensions, Day01GraphHandleExtensions)
    /// </summary>
    public class Day04Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day01GraphHandle graphHandle;

        [SerializeField]
        private Day02OutputHandle outputHandle;

        [SerializeField]
        private Day04RotatorData rotatorData;

        // Rotation settings - exposed in Inspector for easy experimentation
        [Header("Rotation Settings")]
        [SerializeField]
        private float rotationSpeed = 90.0f;

        [SerializeField]
        private Vector3 rotationAxis = Vector3.up;

        // LAYER C: Adapter Calls
        private void OnEnable()
        {
            InitializeGraph();
            InitializeOutput();
            InitializeRotator();
            LinkRotatorToOutput();
            LogRotatorInfo();
        }

        private void OnDisable()
        {
            CleanupRotator();
            CleanupOutput();
            CleanupGraph();
        }

        private void Update()
        {
            // PURE VIEW LOGIC
            // Visual feedback: Show the cube is active with a subtle pulse
            float pulse = 1.0f + Mathf.Sin(Time.time * 3.0f) * 0.05f;
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
                Debug.LogError("[Day 04] Cannot initialize output: Graph is not initialized.");
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
                Debug.LogError("[Day 04] Cannot initialize rotator: Graph is not initialized.");
                return;
            }

            string rotatorName = $"{gameObject.name}_Rotator";
            PlayableGraph graph = graphHandle.Graph;
            var rotator = rotatorData;
            rotator.Initialize(in graph, rotatorName, transform, rotationSpeed, rotationAxis);
            rotatorData = rotator;
        }

        private void LinkRotatorToOutput()
        {
            if (!rotatorData.IsValidRotator())
            {
                Debug.LogError("[Day 04] Cannot link: Rotator is not valid.");
                return;
            }

            if (!outputHandle.IsValidOutput())
            {
                Debug.LogError("[Day 04] Cannot link: Output is not valid.");
                return;
            }

            PlayableOutput output = outputHandle.Output;
            rotatorData.ConnectToOutput(in output);
            Debug.Log("[Day 04] Successfully linked rotator to output!");
        }

        private void LogRotatorInfo()
        {
            if (!rotatorData.IsValidRotator())
            {
                Debug.LogWarning("[Day 04] Rotator is not valid, cannot log info.");
                return;
            }

            string rotatorName = $"{gameObject.name}_Rotator";
            rotatorData.LogRotatorInfo(rotatorName);

            // Log the node type to verify it's a ScriptPlayable
            if (rotatorData.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour))
            {
                Debug.Log($"[Day 04] Behaviour Type: {behaviour.GetType().Name}");
                string targetName = behaviour.TargetTransform != null ? behaviour.TargetTransform.name : "None";
                Debug.Log($"[Day 04] Target Transform: {targetName}");
                Debug.Log($"[Day 04] Rotation Speed: {behaviour.RotationSpeed}°/s");
                Debug.Log($"[Day 04] Rotation Axis: {behaviour.RotationAxis}");
            }
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
            rotationSpeed = 90.0f;
            rotationAxis = Vector3.up;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only: Draw gizmos to visualize the rotation axis.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!rotatorData.IsValidRotator()) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, rotationAxis.normalized * 2.0f);

            // Draw a small sphere at the end of the axis
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + rotationAxis.normalized * 2.0f, 0.1f);
        }
#endif
    }
}
