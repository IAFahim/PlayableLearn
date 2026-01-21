using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day14
{
    /// <summary>
    /// Extension methods for Day14SwapData.
    /// Layer C: Extensions - Provides initialization, connection swapping, and lifecycle management.
    /// This is the "Adapter" layer that makes the swap data structure easy to use.
    /// </summary>
    public static class Day14SwapExtensions
    {
        /// <summary>
        /// Initializes the swap data with the mixer and input playables.
        /// </summary>
        public static void Initialize(ref this Day14SwapData swapData, in PlayableGraph graph, in Playable mixerPlayable, in Playable input0, in Playable input1)
        {
            if (swapData.IsActive)
            {
                Debug.LogWarning($"[SwapData] Already initialized. Swap ID: {swapData.SwapId}.");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[SwapData] Cannot initialize: Graph is invalid.");
                return;
            }

            if (!mixerPlayable.IsValid())
            {
                Debug.LogError($"[SwapData] Cannot initialize: Mixer playable is invalid.");
                return;
            }

            if (!SwapConnectionOps.HasValidInputPorts(in mixerPlayable))
            {
                Debug.LogError($"[SwapData] Cannot initialize: Mixer must have at least 2 input ports.");
                return;
            }

            if (!input0.IsValid() || !input1.IsValid())
            {
                Debug.LogError($"[SwapData] Cannot initialize: Input playables must be valid.");
                return;
            }

            swapData.Graph = graph;
            swapData.MixerPlayable = mixerPlayable;
            swapData.Input0Playable = input0;
            swapData.Input1Playable = input1;
            swapData.IsUsingInput1 = false;
            swapData.IsActive = true;
            swapData.SwapId = (input0.GetHashCode() + input1.GetHashCode()) % int.MaxValue;
            swapData.IsInput0Connected = SwapConnectionOps.IsInput0Connected(in mixerPlayable);
            swapData.IsInput1Connected = SwapConnectionOps.IsInput1Connected(in mixerPlayable);

            Debug.Log($"[SwapData] Initialized with Swap ID: {swapData.SwapId}.");
        }

        /// <summary>
        /// Disposes the swap data, cleaning up resources.
        /// </summary>
        public static void Dispose(ref this Day14SwapData swapData)
        {
            if (!swapData.IsActive) return;

            swapData.IsActive = false;
            swapData.IsUsingInput1 = false;
            swapData.SwapId = 0;
            swapData.IsInput0Connected = false;
            swapData.IsInput1Connected = false;

            Debug.Log("[SwapData] Disposed.");
        }

        /// <summary>
        /// Performs a hard swap to input 1: disconnects input 0 and connects input 1.
        /// </summary>
        public static bool SwapToInput1(ref this Day14SwapData swapData, float weight = 1.0f)
        {
            if (!swapData.IsActive)
            {
                Debug.LogWarning("[SwapData] Cannot swap: Swap data is not active.");
                return false;
            }

            if (!swapData.MixerPlayable.IsValid() || !swapData.Input1Playable.IsValid())
            {
                Debug.LogError("[SwapData] Cannot swap: Mixer or input 1 playable is invalid.");
                return false;
            }

            bool success = SwapConnectionOps.HardSwapToInput1(in swapData.MixerPlayable, in swapData.Input1Playable, weight);

            if (success)
            {
                swapData.IsUsingInput1 = true;
                swapData.IsInput0Connected = false;
                swapData.IsInput1Connected = true;
                Debug.Log($"[SwapData] Swapped to Input 1 with weight {weight}.");
            }
            else
            {
                Debug.LogError("[SwapData] Failed to swap to Input 1.");
            }

            return success;
        }

        /// <summary>
        /// Performs a hard swap to input 0: disconnects input 1 and connects input 0.
        /// </summary>
        public static bool SwapToInput0(ref this Day14SwapData swapData, float weight = 1.0f)
        {
            if (!swapData.IsActive)
            {
                Debug.LogWarning("[SwapData] Cannot swap: Swap data is not active.");
                return false;
            }

            if (!swapData.MixerPlayable.IsValid() || !swapData.Input0Playable.IsValid())
            {
                Debug.LogError("[SwapData] Cannot swap: Mixer or input 0 playable is invalid.");
                return false;
            }

            bool success = SwapConnectionOps.HardSwapToInput0(in swapData.MixerPlayable, in swapData.Input0Playable, weight);

            if (success)
            {
                swapData.IsUsingInput1 = false;
                swapData.IsInput0Connected = true;
                swapData.IsInput1Connected = false;
                Debug.Log($"[SwapData] Swapped to Input 0 with weight {weight}.");
            }
            else
            {
                Debug.LogError("[SwapData] Failed to swap to Input 0.");
            }

            return success;
        }

        /// <summary>
        /// Toggles between input 0 and input 1.
        /// </summary>
        public static bool ToggleInput(ref this Day14SwapData swapData, float weight = 1.0f)
        {
            if (swapData.IsUsingInput1)
            {
                return SwapToInput0(ref swapData, weight);
            }
            else
            {
                return SwapToInput1(ref swapData, weight);
            }
        }

        /// <summary>
        /// Gets the weight of input 0.
        /// </summary>
        public static bool TryGetInput0Weight(this in Day14SwapData swapData, out float weight)
        {
            weight = 0.0f;
            if (!swapData.IsActive || !swapData.MixerPlayable.IsValid())
            {
                return false;
            }

            SwapConnectionOps.GetInput0Weight(in swapData.MixerPlayable, out weight);
            return true;
        }

        /// <summary>
        /// Gets the weight of input 1.
        /// </summary>
        public static bool TryGetInput1Weight(this in Day14SwapData swapData, out float weight)
        {
            weight = 0.0f;
            if (!swapData.IsActive || !swapData.MixerPlayable.IsValid())
            {
                return false;
            }

            SwapConnectionOps.GetInput1Weight(in swapData.MixerPlayable, out weight);
            return true;
        }

        /// <summary>
        /// Sets the weight of input 0.
        /// </summary>
        public static void SetInput0Weight(ref this Day14SwapData swapData, float weight)
        {
            if (!swapData.IsActive)
            {
                Debug.LogWarning("[SwapData] Cannot set weight: Swap data is not active.");
                return;
            }

            SwapConnectionOps.SetInput0Weight(in swapData.MixerPlayable, weight);
        }

        /// <summary>
        /// Sets the weight of input 1.
        /// </summary>
        public static void SetInput1Weight(ref this Day14SwapData swapData, float weight)
        {
            if (!swapData.IsActive)
            {
                Debug.LogWarning("[SwapData] Cannot set weight: Swap data is not active.");
                return;
            }

            SwapConnectionOps.SetInput1Weight(in swapData.MixerPlayable, weight);
        }

        /// <summary>
        /// Checks if the swap data is valid and active.
        /// </summary>
        public static bool IsValidSwap(this in Day14SwapData swapData)
        {
            return swapData.IsActive &&
                   swapData.MixerPlayable.IsValid() &&
                   swapData.Input0Playable.IsValid() &&
                   swapData.Input1Playable.IsValid();
        }

        /// <summary>
        /// Checks if currently using input 1.
        /// </summary>
        public static bool IsUsingInput1(this in Day14SwapData swapData)
        {
            return swapData.IsActive && swapData.IsUsingInput1;
        }

        /// <summary>
        /// Checks if input 0 is connected.
        /// </summary>
        public static bool IsInput0Connected(this in Day14SwapData swapData)
        {
            if (!swapData.IsActive || !swapData.MixerPlayable.IsValid())
            {
                return false;
            }

            return SwapConnectionOps.IsInput0Connected(in swapData.MixerPlayable);
        }

        /// <summary>
        /// Checks if input 1 is connected.
        /// </summary>
        public static bool IsInput1Connected(this in Day14SwapData swapData)
        {
            if (!swapData.IsActive || !swapData.MixerPlayable.IsValid())
            {
                return false;
            }

            return SwapConnectionOps.IsInput1Connected(in swapData.MixerPlayable);
        }

        /// <summary>
        /// Logs the swap information to the console.
        /// </summary>
        public static void LogToConsole(this in Day14SwapData swapData, string swapName)
        {
            if (!swapData.IsActive)
            {
                Debug.LogWarning($"[SwapData] Cannot log: Swap data is not active.");
                return;
            }

            if (!swapData.IsValidSwap())
            {
                Debug.LogWarning($"[SwapData] Swap '{swapName}' is not valid.");
                return;
            }

            Debug.Log($"[SwapData] Name: {swapName}, Swap ID: {swapData.SwapId}, Active: {swapData.IsActive}");
            Debug.Log($"[SwapData]   Using Input 1: {swapData.IsUsingInput1}");
            Debug.Log($"[SwapData]   Input 0 Connected: {swapData.IsInput0Connected(swapData)}");
            Debug.Log($"[SwapData]   Input 1 Connected: {swapData.IsInput1Connected(swapData)}");

            if (swapData.TryGetInput0Weight(out float weight0))
            {
                Debug.Log($"[SwapData]   Input 0 Weight: {weight0:F2}");
            }

            if (swapData.TryGetInput1Weight(out float weight1))
            {
                Debug.Log($"[SwapData]   Input 1 Weight: {weight1:F2}");
            }
        }

        /// <summary>
        /// Disconnects both inputs from the mixer.
        /// </summary>
        public static void DisconnectAll(ref this Day14SwapData swapData)
        {
            if (!swapData.IsActive)
            {
                Debug.LogWarning("[SwapData] Cannot disconnect: Swap data is not active.");
                return;
            }

            SwapConnectionOps.DisconnectInput0(in swapData.MixerPlayable);
            SwapConnectionOps.DisconnectInput1(in swapData.MixerPlayable);

            swapData.IsInput0Connected = false;
            swapData.IsInput1Connected = false;

            Debug.Log("[SwapData] Disconnected all inputs.");
        }
    }
}
