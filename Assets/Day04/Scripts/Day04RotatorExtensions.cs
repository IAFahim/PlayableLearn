using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day03;

namespace PlayableLearn.Day04
{
    /// <summary>
    /// Extension methods for Day04RotatorData.
    /// Layer C: Extensions - High-level adapter methods that combine operations.
    /// </summary>
    public static class Day04RotatorExtensions
    {
        /// <summary>
        /// Initializes a new rotator node with the specified parameters.
        /// </summary>
        public static void Initialize(ref this Day04RotatorData data, in PlayableGraph graph, string nodeName, Transform target, float rotationSpeed, Vector3 rotationAxis)
        {
            if (data.IsActive)
            {
                Debug.LogWarning($"[RotatorData] Already initialized: {nodeName}");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[RotatorData] Cannot initialize rotator: Graph is invalid.");
                return;
            }

            // Create the ScriptPlayable with Day04RotatorBehaviour
            ScriptPlayableOps.Create<Day04RotatorBehaviour>(in graph, 0, out data.Node);

            if (!ScriptPlayableOps.IsValid(in data.Node))
            {
                Debug.LogError($"[RotatorData] Failed to create rotator node: {nodeName}");
                return;
            }

            // Get the behaviour and set its parameters
            if (data.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour))
            {
                behaviour.SetRotationParameters(target, rotationSpeed, rotationAxis);
                data.Graph = graph;
                data.IsActive = true;
                data.NodeId = nodeName.GetHashCode();
                data.TargetTransform = target;
                data.RotationSpeed = rotationSpeed;
                data.RotationAxis = rotationAxis;

                Debug.Log($"[RotatorData] Created rotator: {nodeName}, Speed: {rotationSpeed}°/s, Axis: {rotationAxis}");
            }
            else
            {
                Debug.LogError($"[RotatorData] Failed to get behaviour: {nodeName}");
            }
        }

        /// <summary>
        /// Disposes the rotator node, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day04RotatorData data)
        {
            if (!data.IsActive) return;

            ScriptPlayableOps.Destroy(in data.Graph, in data.Node);
            data.IsActive = false;
            data.NodeId = 0;
            data.TargetTransform = null;
            Debug.Log("[RotatorData] Rotator node disposed.");
        }

        /// <summary>
        /// Connects this rotator node to a playable output.
        /// </summary>
        public static void ConnectToOutput(this in Day04RotatorData data, in PlayableOutput output)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning("[RotatorData] Cannot connect: Rotator is not active.");
                return;
            }

            if (!output.IsOutputValid())
            {
                Debug.LogWarning("[RotatorData] Cannot connect: Output is not valid.");
                return;
            }

            ScriptPlayableOps.SetSource(in output, in data.Node);
            Debug.Log("[RotatorData] Connected rotator to output.");
        }

        /// <summary>
        /// Checks if the rotator node is valid and active.
        /// </summary>
        public static bool IsValidRotator(this in Day04RotatorData data)
        {
            return data.IsActive && ScriptPlayableOps.IsValid(in data.Node);
        }

        /// <summary>
        /// Updates the rotation speed at runtime.
        /// </summary>
        public static void SetRotationSpeed(this in Day04RotatorData data, float newSpeed)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour))
            {
                Debug.LogWarning("[RotatorData] Cannot set rotation speed: Rotator is not valid.");
                return;
            }

            behaviour.RotationSpeed = newSpeed;
            Debug.Log($"[RotatorData] Rotation speed updated to: {newSpeed}°/s");
        }

        /// <summary>
        /// Updates the rotation axis at runtime.
        /// </summary>
        public static void SetRotationAxis(this in Day04RotatorData data, Vector3 newAxis)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour))
            {
                Debug.LogWarning("[RotatorData] Cannot set rotation axis: Rotator is not valid.");
                return;
            }

            behaviour.RotationAxis = newAxis;
            Debug.Log($"[RotatorData] Rotation axis updated to: {newAxis}");
        }

        /// <summary>
        /// Updates the target transform at runtime.
        /// </summary>
        public static void SetTargetTransform(this in Day04RotatorData data, Transform newTarget)
        {
            if (!data.IsActive || !data.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour))
            {
                Debug.LogWarning("[RotatorData] Cannot set target transform: Rotator is not valid.");
                return;
            }

            behaviour.TargetTransform = newTarget;
            Debug.Log($"[RotatorData] Target transform updated to: {(newTarget != null ? newTarget.name : "null")}");
        }

        /// <summary>
        /// Gets information about the rotator's configuration.
        /// </summary>
        public static void LogRotatorInfo(this in Day04RotatorData data, string rotatorName)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning($"[RotatorData] Cannot log: Rotator is not active.");
                return;
            }

            string targetName = data.TargetTransform != null ? data.TargetTransform.name : "None";
            Debug.Log($"[Rotator] Name: {rotatorName}, Target: {targetName}, Speed: {data.RotationSpeed}°/s, Axis: {data.RotationAxis}");
        }
    }
}
