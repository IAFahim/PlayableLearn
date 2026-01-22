using UnityEngine.Playables;
using Unity.Burst;
using System.Runtime.CompilerServices;

namespace PlayableLearn.Day03
{
    [BurstCompile]
    public static class ScriptPlayableOps
    {
        /// <summary>
        /// Creates a ScriptPlayable with the specified behaviour.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Create<T>(in PlayableGraph graph, int portCount, out Playable playable) where T : PlayableBehaviour, new()
        {
            if (!graph.IsValid())
            {
                playable = Playable.Null;
                return;
            }

            playable = ScriptPlayable<T>.Create(graph, portCount);
        }

        /// <summary>
        /// Destroys the given playable.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(in PlayableGraph graph, in Playable playable)
        {
            if (playable.IsValid() && graph.IsValid())
            {
                graph.DestroySubgraph(playable);
            }
        }

        /// <summary>
        /// Checks if the playable is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(in Playable playable)
        {
            return playable.IsValid();
        }

        /// <summary>
        /// Gets the playable type as an enum for identification.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetPlayableType(in Playable playable, out PlayableType result)
        {
            if (!playable.IsValid())
            {
                result = PlayableType.Null;
                return;
            }

            // Since we're creating ScriptPlayables, we know the type
            result = PlayableType.ScriptPlayable;
        }

        /// <summary>
        /// Gets the number of input ports on the playable.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetInputCount(in Playable playable, out int count)
        {
            if (!playable.IsValid())
            {
                count = 0;
                return;
            }

            count = playable.GetInputCount();
        }

        /// <summary>
        /// Gets the number of output ports on the playable.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetOutputCount(in Playable playable, out int count)
        {
            if (!playable.IsValid())
            {
                count = 0;
                return;
            }

            count = playable.GetOutputCount();
        }

        /// <summary>
        /// Connects a playable output to a playable input.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect(in PlayableGraph graph, in Playable source, int sourcePort, in Playable destination, int destinationPort)
        {
            if (!graph.IsValid() || !source.IsValid() || !destination.IsValid())
            {
                return;
            }

            if (sourcePort >= source.GetOutputCount() || destinationPort >= destination.GetInputCount())
            {
                return;
            }

            graph.Connect(source, sourcePort, destination, destinationPort);
        }

        /// <summary>
        /// Sets the playable as the source of an output.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSource(in PlayableOutput output, in Playable playable)
        {
            if (!output.IsOutputValid() || !playable.IsValid())
            {
                return;
            }

            output.SetSourcePlayable(playable, 0);
        }

    }

    /// <summary>
    /// Playable type enumeration for Burst-safe type checking.
    /// </summary>
    public enum PlayableType
    {
        Null,
        ScriptPlayable,
        Unknown
    }
}
