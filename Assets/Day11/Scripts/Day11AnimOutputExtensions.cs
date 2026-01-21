using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day11
{
    /// <summary>
    /// Extension methods for Day11AnimOutputHandle.
    /// Layer C: Extensions - Provides initialization, connection, and lifecycle management.
    /// This is the "Adapter" layer that makes the data structure easy to use.
    /// </summary>
    public static class Day11AnimOutputExtensions
    {
        /// <summary>
        /// Initializes a new AnimationPlayableOutput from the given graph and connects it to an Animator.
        /// </summary>
        public static void Initialize(ref this Day11AnimOutputHandle handle, in PlayableGraph graph, string outputName, Animator animator)
        {
            if (handle.IsActive)
            {
                Debug.LogWarning($"[AnimOutputHandle] Already initialized: {outputName}");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[AnimOutputHandle] Cannot initialize output: Graph is invalid.");
                return;
            }

            if (animator == null)
            {
                Debug.LogError($"[AnimOutputHandle] Cannot initialize output: Animator is null.");
                return;
            }

            AnimOutputOps.Create(in graph, in outputName, out handle.Output);

            if (AnimOutputOps.IsValid(in handle.Output))
            {
                // Connect the output to the Animator
                AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, outputName);
                output.SetTarget(animator);

                handle.Graph = graph;
                handle.Output = output;
                handle.Animator = animator;
                handle.IsActive = true;
                handle.OutputId = outputName.GetHashCode();
                Debug.Log($"[AnimOutputHandle] Created AnimationPlayableOutput: {outputName} connected to Animator: {animator.name}");
            }
            else
            {
                Debug.LogError($"[AnimOutputHandle] Failed to create output: {outputName}");
            }
        }

        /// <summary>
        /// Disposes the output, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day11AnimOutputHandle handle)
        {
            if (!handle.IsActive) return;

            AnimOutputOps.Destroy(in handle.Graph, in handle.Output);
            handle.IsActive = false;
            handle.OutputId = 0;
            handle.Animator = null;
            Debug.Log("[AnimOutputHandle] AnimationPlayableOutput disposed.");
        }

        /// <summary>
        /// Logs the output information to the console.
        /// </summary>
        public static void LogToConsole(this in Day11AnimOutputHandle handle, string outputName)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning($"[AnimOutputHandle] Cannot log: Output is not active.");
                return;
            }

            AnimOutputOps.GetOutputInfo(in handle.Output, out AnimOutputType type, out bool isValid);

            if (!isValid)
            {
                Debug.LogWarning($"[AnimOutputHandle] Output '{outputName}' is not valid.");
                return;
            }

            string animatorName = handle.Animator != null ? handle.Animator.name : "None";
            Debug.Log($"[AnimOutput] Name: {outputName}, Type: {type}, IsPlayableOutput: {isValid}, Animator: {animatorName}");
        }

        /// <summary>
        /// Gets the output type as an enum.
        /// </summary>
        public static bool TryGetOutputType(this in Day11AnimOutputHandle handle, out AnimOutputType outputType)
        {
            if (!handle.IsActive || !AnimOutputOps.IsValid(in handle.Output))
            {
                outputType = AnimOutputType.Null;
                return false;
            }

            AnimOutputOps.GetOutputType(in handle.Output, out outputType);
            return true;
        }

        /// <summary>
        /// Checks if the output is valid and active.
        /// </summary>
        public static bool IsValidOutput(this in Day11AnimOutputHandle handle)
        {
            return handle.IsActive && AnimOutputOps.IsValid(in handle.Output);
        }

        /// <summary>
        /// Gets the Animator connected to this output.
        /// </summary>
        public static bool TryGetAnimator(this in Day11AnimOutputHandle handle, out Animator animator)
        {
            animator = null;
            if (!handle.IsActive || handle.Animator == null)
            {
                return false;
            }
            animator = handle.Animator;
            return true;
        }

        /// <summary>
        /// Sets a new Animator target for this output.
        /// </summary>
        public static void SetAnimatorTarget(ref this Day11AnimOutputHandle handle, Animator newAnimator)
        {
            if (!handle.IsActive)
            {
                Debug.LogWarning("[AnimOutputHandle] Cannot set target: Output is not active.");
                return;
            }

            if (newAnimator == null)
            {
                Debug.LogWarning("[AnimOutputHandle] Cannot set target: New Animator is null.");
                return;
            }

            // Get the AnimationPlayableOutput and set the new target
            AnimationPlayableOutput animOutput = AnimationPlayableOutput.Create(handle.Graph, "TempOutput");
            animOutput.SetTarget(newAnimator);

            handle.Animator = newAnimator;
            Debug.Log($"[AnimOutputHandle] Set new Animator target: {newAnimator.name}");
        }

        /// <summary>
        /// Gets the PlayableOutput for connection to playables.
        /// </summary>
        public static bool TryGetPlayableOutput(this in Day11AnimOutputHandle handle, out PlayableOutput output)
        {
            output = default;
            if (!handle.IsActive || !AnimOutputOps.IsValid(in handle.Output))
            {
                return false;
            }
            output = handle.Output;
            return true;
        }
    }
}
