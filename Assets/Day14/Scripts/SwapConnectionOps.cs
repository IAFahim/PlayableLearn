using UnityEngine;
using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day14
{
    /// <summary>
    /// Burst-compiled operations for hard swapping connections on a mixer.
    /// Layer B: Operations - Pure functions using Burst for performance.
    /// This is the "Engine" layer - low-level operations for disconnecting and connecting inputs.
    /// </summary>
    [BurstCompile]
    public static class SwapConnectionOps
    {
        /// <summary>
        /// Disconnects input 0 from the mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisconnectInput0(in Playable mixer)
        {
            if (!mixer.IsValid())
            {
                return;
            }

            if (mixer.GetInputCount() < 1)
            {
                return;
            }

            if (mixer.IsInputConnected(0))
            {
                mixer.DisconnectInput(0);
            }
        }

        /// <summary>
        /// Disconnects input 1 from the mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisconnectInput1(in Playable mixer)
        {
            if (!mixer.IsValid())
            {
                return;
            }

            if (mixer.GetInputCount() < 2)
            {
                return;
            }

            if (mixer.IsInputConnected(1))
            {
                mixer.DisconnectInput(1);
            }
        }

        /// <summary>
        /// Connects input 0 to the mixer with the specified weight.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ConnectInput0(in Playable mixer, in Playable input, float weight)
        {
            if (!mixer.IsValid() || !input.IsValid())
            {
                return false;
            }

            if (mixer.GetInputCount() < 1)
            {
                return false;
            }

            // Disconnect input 0 if already connected
            if (mixer.IsInputConnected(0))
            {
                mixer.DisconnectInput(0);
            }

            // Connect input 0
            int inputPort = mixer.AddInput(input, 0, weight);

            if (inputPort >= 0)
            {
                mixer.SetInputWeight(0, weight);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Connects input 1 to the mixer with the specified weight.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ConnectInput1(in Playable mixer, in Playable input, float weight)
        {
            if (!mixer.IsValid() || !input.IsValid())
            {
                return false;
            }

            if (mixer.GetInputCount() < 2)
            {
                return false;
            }

            // Disconnect input 1 if already connected
            if (mixer.IsInputConnected(1))
            {
                mixer.DisconnectInput(1);
            }

            // Connect input 1
            int inputPort = mixer.AddInput(input, 1, weight);

            if (inputPort >= 0)
            {
                mixer.SetInputWeight(1, weight);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs a hard swap: disconnects input 0 and connects input 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HardSwapToInput1(in Playable mixer, in Playable input1, float weight)
        {
            if (!mixer.IsValid() || !input1.IsValid())
            {
                return false;
            }

            if (mixer.GetInputCount() < 2)
            {
                return false;
            }

            // Disconnect input 0
            DisconnectInput0(in mixer);

            // Connect input 1
            return ConnectInput1(in mixer, in input1, weight);
        }

        /// <summary>
        /// Performs a hard swap: disconnects input 1 and connects input 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HardSwapToInput0(in Playable mixer, in Playable input0, float weight)
        {
            if (!mixer.IsValid() || !input0.IsValid())
            {
                return false;
            }

            if (mixer.GetInputCount() < 2)
            {
                return false;
            }

            // Disconnect input 1
            DisconnectInput1(in mixer);

            // Connect input 0
            return ConnectInput0(in mixer, in input0, weight);
        }

        /// <summary>
        /// Checks if input 0 is connected to the mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInput0Connected(in Playable mixer)
        {
            if (!mixer.IsValid() || mixer.GetInputCount() < 1)
            {
                return false;
            }

            return mixer.IsInputConnected(0);
        }

        /// <summary>
        /// Checks if input 1 is connected to the mixer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInput1Connected(in Playable mixer)
        {
            if (!mixer.IsValid() || mixer.GetInputCount() < 2)
            {
                return false;
            }

            return mixer.IsInputConnected(1);
        }

        /// <summary>
        /// Gets the weight of input 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetInput0Weight(in Playable mixer, out float weight)
        {
            weight = 0.0f;
            if (!mixer.IsValid() || mixer.GetInputCount() < 1)
            {
                return;
            }

            weight = mixer.GetInputWeight(0);
        }

        /// <summary>
        /// Gets the weight of input 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetInput1Weight(in Playable mixer, out float weight)
        {
            weight = 0.0f;
            if (!mixer.IsValid() || mixer.GetInputCount() < 2)
            {
                return;
            }

            weight = mixer.GetInputWeight(1);
        }

        /// <summary>
        /// Sets the weight of input 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInput0Weight(in Playable mixer, float weight)
        {
            if (!mixer.IsValid() || mixer.GetInputCount() < 1)
            {
                return;
            }

            mixer.SetInputWeight(0, weight);
        }

        /// <summary>
        /// Sets the weight of input 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInput1Weight(in Playable mixer, float weight)
        {
            if (!mixer.IsValid() || mixer.GetInputCount() < 2)
            {
                return;
            }

            mixer.SetInputWeight(1, weight);
        }

        /// <summary>
        /// Validates that the mixer has at least 2 inputs.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasValidInputPorts(in Playable mixer)
        {
            if (!mixer.IsValid())
            {
                return false;
            }

            return mixer.GetInputCount() >= 2;
        }
    }
}
