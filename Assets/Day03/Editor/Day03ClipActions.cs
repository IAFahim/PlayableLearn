using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AV.Day03.Runtime;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day03.Editor
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ClipActionState
    {
        public int ClipCount;
        public double TotalDuration;

        public override string ToString()
        {
            return $"[ClipAction] Clips: {ClipCount} | Duration: {TotalDuration:F2}s";
            // Debug view
        }
    }

    public static class ClipActionLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateSpeedMultiplier(double current, double multiplier, out double newSpeed)
        {
            newSpeed = current * multiplier; // Atomic calculation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateClipAssetType(TimelineClip clip, Type targetType, out bool isValid)
        {
            isValid = clip.asset != null && clip.asset.GetType() == targetType; // Atomic validation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateClipDuration(TimelineClip clip, out double duration)
        {
            duration = clip.duration; // Atomic fetch
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SumDurations(double current, double addition, out double total)
        {
            total = current + addition; // Atomic sum
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IncrementCount(int current, out int next)
        {
            next = current + 1; // Atomic increment
        }
    }

    public static class ClipActionExtensions
    {
        public static bool TryLogInfo(this TimelineClip clip, out string message)
        {
            message = string.Empty;

            if (clip.asset == null) return false; // Guard: No asset

            ClipActionLogic.CalculateClipDuration(clip, out var duration);
            message =
                $"<color=cyan>[Day03 Action]</color> Clip: {clip.displayName} | Start: {clip.start:F2} | Duration: {duration:F2}";

            return true; // Success
        }

        public static bool TryDoubleSpeed(this TimelineClip clip, out double newSpeed)
        {
            newSpeed = clip.timeScale;

            if (clip.timeScale <= 0) return false; // Guard: Invalid speed

            ClipActionLogic.CalculateSpeedMultiplier(clip.timeScale, 2.0, out newSpeed);
            clip.timeScale = newSpeed;

            return true; // Success
        }

        public static bool TryCollectClipInfo(ref this ClipActionState state, TimelineClip clip, out int updatedCount)
        {
            updatedCount = state.ClipCount;

            if (clip.asset == null) return false; // Guard: No asset

            ClipActionLogic.CalculateClipDuration(clip, out var duration);
            ClipActionLogic.SumDurations(state.TotalDuration, duration, out state.TotalDuration);
            ClipActionLogic.IncrementCount(state.ClipCount, out state.ClipCount);
            updatedCount = state.ClipCount;

            return true; // Success
        }
    }

    [MenuEntry("Day 03/Quick Log Clip Info", 100)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.Active)]
    public class Day03LogClipAction : ClipAction
    {
        private ClipActionState _state;

        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            foreach (var clip in clips)
            {
                ClipActionLogic.ValidateClipAssetType(clip, typeof(Day03DisplayNamePlayableAsset), out var isValid);
                if (isValid) return ActionValidity.Valid;
            }

            return ActionValidity.NotApplicable;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            _state = new ClipActionState();

            foreach (var clip in clips)
            {
                var success = clip.TryLogInfo(out var message);
                if (success) Debug.Log(message);

                _state.TryCollectClipInfo(clip, out var count);
            }

            return true; // Success
        }
    }

    public class Day03DoubleSpeedAction : ClipAction
    {
        [TimelineShortcut("Day03/DoubleClipSpeed", KeyCode.H, ShortcutModifiers.Shift)]
        public static void HandleShortCut(ShortcutArguments args)
        {
            Invoker.InvokeWithSelectedClips<Day03DoubleSpeedAction>();
        }

        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            Undo.RegisterCompleteObjectUndo(TimelineEditor.inspectedAsset, "Double Speed");

            foreach (var clip in clips)
            {
                var success = clip.TryDoubleSpeed(out var newSpeed);
                if (success)
                    Debug.Log(
                        $"<color=green>[Day03]</color> Speed doubled for {clip.displayName} (New: {newSpeed:F2}x)");
            }

            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
            return true; // Success
        }
    }
}