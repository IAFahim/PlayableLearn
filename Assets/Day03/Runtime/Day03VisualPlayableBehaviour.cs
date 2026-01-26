using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day03.Runtime
{
    public interface IVisualSystem
    {
        void Initialize(VisualState initialState);
        bool TrySetDisplayName(string name);
        bool TryGetDisplayName(out string name);
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct VisualState
    {
        public string DisplayName;
        public bool IsVisible;

        public override string ToString()
        {
            return $"[Visual] {DisplayName} ({(IsVisible ? "VISIBLE" : "HIDDEN")})";
            // Debug view
        }
    }

    public static class VisualLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateName(string name, out bool isValid)
        {
            isValid = !string.IsNullOrEmpty(name); // Atomic validation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatDisplayName(string name, out string formatted)
        {
            formatted = string.IsNullOrEmpty(name) ? "Untitled" : name; // Atomic format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateVisibility(bool current, out bool next)
        {
            next = !current; // Atomic toggle
        }
    }

    public static class VisualExtensions
    {
        public static bool TrySetDisplayName(ref this VisualState state, string name, out string previousName)
        {
            previousName = state.DisplayName;

            VisualLogic.ValidateName(name, out var isValid);
            if (!isValid) return false; // Guard: Invalid name

            VisualLogic.FormatDisplayName(name, out var formatted);
            state.DisplayName = formatted;

            return true; // Success
        }

        public static bool TryGetDisplayName(this VisualState state, out string name)
        {
            name = state.DisplayName;

            if (string.IsNullOrEmpty(name)) return false; // Guard: No name set

            VisualLogic.FormatDisplayName(name, out var formatted);
            name = formatted;

            return true; // Success
        }

        public static bool TryToggleVisibility(ref this VisualState state, out bool newVisibility)
        {
            newVisibility = state.IsVisible;

            VisualLogic.CalculateVisibility(state.IsVisible, out state.IsVisible);
            newVisibility = state.IsVisible;

            return true; // Success
        }
    }

    public class Day03VisualPlayableBehaviour : PlayableBehaviour, IVisualSystem
    {
        private VisualState _state;

        void IVisualSystem.Initialize(VisualState initialState)
        {
            _state = initialState;
        }

        bool IVisualSystem.TrySetDisplayName(string name)
        {
            return _state.TrySetDisplayName(name, out _);
        }

        bool IVisualSystem.TryGetDisplayName(out string name)
        {
            return _state.TryGetDisplayName(out name);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is GameObject go && go != null) ((IVisualSystem)this).TrySetDisplayName(go.name);
        }
    }
}