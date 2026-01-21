using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    public static class Day01GraphLifecycleExtensions
    {
        public static void InitializeEngine(ref this Day01GraphLifecycleState state, string ownerName)
        {
            if (state.IsCreated) return;

            // Pronounceable name for the profiler
            string uniqueGraphName = $"{ownerName}_CoreGraph";

            Day01GraphLifecycleLogic.CreateGraph(
                in uniqueGraphName,
                out state.MainGraphHandle
            );

            state.IsCreated = true;
            state.IsPlaying = false;
        }

        public static void Play(ref this Day01GraphLifecycleState state)
        {
            state.IsPlaying = true;
            Day01GraphLifecycleLogic.SetPlayState(in state.MainGraphHandle, true);
        }

        public static void Stop(ref this Day01GraphLifecycleState state)
        {
            state.IsPlaying = false;
            Day01GraphLifecycleLogic.SetPlayState(in state.MainGraphHandle, false);
        }

        public static void DisposeEngine(ref this Day01GraphLifecycleState state)
        {
            Day01GraphLifecycleLogic.DestroyGraph(in state.MainGraphHandle);
            state.IsCreated = false;
            state.IsPlaying = false;
        }
    }
}
