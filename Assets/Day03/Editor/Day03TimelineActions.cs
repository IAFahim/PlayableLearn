using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace AV.Day03.Editor
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SnapState
    {
        public bool IsEnabled;

        public override string ToString() => $"[Snap] {(IsEnabled ? "ON" : "OFF")}"; // Debug view
    }

    public static class SnapLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvertBool(bool current, out bool inverted)
        {
            inverted = !current; // Atomic inversion
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatSnapState(bool isEnabled, out string stateLabel)
        {
            stateLabel = isEnabled ? "ENABLED" : "DISABLED"; // Atomic format
        }
    }

    public static class SnapExtensions
    {
        public static bool TryToggleSnap(ref this SnapState state, out bool previousState)
        {
            previousState = state.IsEnabled;

            SnapLogic.InvertBool(state.IsEnabled, out state.IsEnabled);

            return true; // Success
        }
    }

    public abstract class Day03ToggleSnapAction : TimelineAction
    {
        private SnapState _state;

        [TimelineShortcut("Day03/ToggleSnap", KeyCode.S, ShortcutModifiers.Alt)]
        public static void HandleShortcut(ShortcutArguments args)
        {
            Invoker.InvokeWithSelected<Day03ToggleSnapAction>();
        }

        public override ActionValidity Validate(ActionContext context)
        {
            return ActionValidity.Valid;
        }

        public override bool Execute(ActionContext context)
        {
            _state.IsEnabled = TimelinePreferences.instance.snapToFrame;

            var success = _state.TryToggleSnap(out var previousState);
            if (success)
            {
                TimelinePreferences.instance.snapToFrame = _state.IsEnabled;
                SnapLogic.FormatSnapState(_state.IsEnabled, out var stateLabel);
                Debug.Log($"<color=magenta>[Day03]</color> Snap to Frame is now: {stateLabel}");
            }

            return success; // Return operation result
        }
    }
}
