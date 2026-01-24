using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day02.Runtime
{
    public interface ISimpleSystem
    {
        void Initialize(SimpleState initialState);
        bool TryActivate();
        bool TryDeactivate();
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SimpleState
    {
        public bool IsActive;
        public float ActivationTime;

        public override string ToString() => $"[SimpleState] Active: {IsActive} @ {ActivationTime:F2}s"; // Debug view
    }

    public static class SimpleLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetCurrentTime(out float time)
        {
            time = UnityEngine.Time.time; // Atomic time fetch
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateActiveState(bool current, out bool next)
        {
            next = !current; // Atomic toggle
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateActivation(bool isActive, out bool canProceed)
        {
            canProceed = true; // Always valid for simple state
        }
    }

    public static class SimpleExtensions
    {
        public static bool TryActivate(ref this SimpleState state, out float activationTime)
        {
            activationTime = state.ActivationTime;

            if (state.IsActive) return false; // Guard: Already active

            SimpleLogic.ValidateActivation(state.IsActive, out var canProceed);
            if (!canProceed) return false;

            SimpleLogic.GetCurrentTime(out var time);
            SimpleLogic.CalculateActiveState(state.IsActive, out state.IsActive);
            state.ActivationTime = time;

            return true; // Success
        }

        public static bool TryDeactivate(ref this SimpleState state, out float deactivationTime)
        {
            deactivationTime = state.ActivationTime;

            if (!state.IsActive) return false; // Guard: Already inactive

            SimpleLogic.CalculateActiveState(state.IsActive, out state.IsActive);

            return true; // Success
        }
    }

    public class Day02SimplePlayableBehaviour : PlayableBehaviour, ISimpleSystem
    {
        private SimpleState _state;

        void ISimpleSystem.Initialize(SimpleState initialState)
        {
            _state = initialState;
        }

        bool ISimpleSystem.TryActivate()
        {
            return _state.TryActivate(out var time);
        }

        bool ISimpleSystem.TryDeactivate()
        {
            return _state.TryDeactivate(out var time);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            ((ISimpleSystem)this).TryActivate();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            ((ISimpleSystem)this).TryDeactivate();
        }
    }
}
