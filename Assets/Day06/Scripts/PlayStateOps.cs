using Unity.Burst;
using Unity.Mathematics;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;

namespace PlayableLearn.Day06
{
    /// <summary>
    /// Burst-compiled pure logic operations for PlayState control.
    /// Layer B: Operations - No Debug calls, pure computation.
    /// </summary>
    [BurstCompile]
    public static class PlayStateOps
    {
        /// <summary>
        /// Checks if the graph is in a playing state.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPlaying(PlayState state)
        {
            return state == PlayState.Playing;
        }

        /// <summary>
        /// Checks if the graph is paused.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPaused(PlayState state)
        {
            return state == PlayState.Paused;
        }

        /// <summary>
        /// Checks if the play state changed between two values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DidStateChange(PlayState previousState, PlayState currentState)
        {
            return previousState != currentState;
        }

        /// <summary>
        /// Checks if the transition from previous to current state is a pause operation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTransitionToPause(PlayState previousState, PlayState currentState)
        {
            return IsPlaying(previousState) && IsPaused(currentState);
        }

        /// <summary>
        /// Checks if the transition from previous to current state is a play operation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTransitionToPlay(PlayState previousState, PlayState currentState)
        {
            return IsPaused(previousState) && IsPlaying(currentState);
        }

        /// <summary>
        /// Determines the next state based on a toggle request.
        /// If currently playing, will pause. If currently paused, will play.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetToggledState(PlayState currentState, out PlayState nextState)
        {
            nextState = IsPlaying(currentState) ? PlayState.Paused : PlayState.Playing;
        }

        /// <summary>
        /// Checks if a PlayState value is valid (not null/invalid).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidState(PlayState state)
        {
            // PlayState is an enum, so we check if it's within valid range
            return state == PlayState.Playing || state == PlayState.Paused;
        }

        /// <summary>
        /// Converts PlayState to a human-readable string representation (as int).
        /// 0 = Playing, 1 = Paused
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StateToInt(PlayState state, out int stateValue)
        {
            stateValue = (int)state;
        }

        /// <summary>
        /// Checks if the graph should be auto-played based on initial state.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldAutoPlay(bool autoPlayOnStart)
        {
            return autoPlayOnStart;
        }

        /// <summary>
        /// Validates that a graph can be controlled (is valid).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanControlGraph(bool isGraphValid)
        {
            return isGraphValid;
        }

        /// <summary>
        /// Checks if a state transition is valid.
        /// Currently allows: Playing -> Paused, Paused -> Playing
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidTransition(PlayState fromState, PlayState toState)
        {
            // Allow transitions between Playing and Paused
            return (fromState == PlayState.Playing && toState == PlayState.Paused) ||
                   (fromState == PlayState.Paused && toState == PlayState.Playing);
        }

        /// <summary>
        /// Calculates the state progress (for debugging/visualization).
        /// Returns 1.0 for playing, 0.0 for paused.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateProgress(PlayState state, out float progress)
        {
            progress = IsPlaying(state) ? 1.0f : 0.0f;
        }
    }
}
