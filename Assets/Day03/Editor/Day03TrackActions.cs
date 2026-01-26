using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day03.Editor
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TrackActionState
    {
        public int TrackCount;
        public int ClipCount;

        public override string ToString()
        {
            return $"[TrackAction] Tracks: {TrackCount} | Clips: {ClipCount}";
            // Debug view
        }
    }

    public static class TrackActionLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetClipCount(TrackAsset track, out int count)
        {
            count = track.GetClips().Count(); // Atomic fetch
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddCounts(int currentTrack, int addedClips, ref int trackCount, ref int clipCount)
        {
            trackCount = currentTrack + 1;
            clipCount += addedClips; // Atomic operations
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateTrackAsset(TrackAsset track, out bool isValid)
        {
            isValid = track != null; // Atomic validation
        }
    }

    public static class TrackActionExtensions
    {
        public static bool TryCollectClips(TrackAsset track, out TimelineClip[] clips)
        {
            clips = Array.Empty<TimelineClip>();

            TrackActionLogic.ValidateTrackAsset(track, out var isValid);
            if (!isValid) return false; // Guard: Invalid track

            var clipList = new List<TimelineClip>(track.GetClips());
            clips = clipList.ToArray();

            return clips.Length > 0; // Success only if clips found
        }

        public static bool TryUpdateState(ref this TrackActionState state, TrackAsset track,
            out TrackActionState previous)
        {
            previous = state;

            TrackActionLogic.ValidateTrackAsset(track, out var isValid);
            if (!isValid) return false; // Guard: Invalid track

            TrackActionLogic.GetClipCount(track, out var clipCount);
            TrackActionLogic.AddCounts(state.TrackCount, clipCount, ref state.TrackCount, ref state.ClipCount);

            return true; // Success
        }
    }

    [MenuEntry("Day 03/Select All Clips on Track", 200)]
    public class Day03SelectClipsOnTrack : TrackAction
    {
        private TrackActionState _state;

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            _state = new TrackActionState();

            var clipsToSelect = new List<TimelineClip>();

            foreach (var track in tracks)
            {
                var success = TrackActionExtensions.TryCollectClips(track, out var clips);
                if (success)
                {
                    clipsToSelect.AddRange(clips);
                    _state.TryUpdateState(track, out var previous);
                }
            }

            TimelineEditor.selectedClips = clipsToSelect.ToArray();

            Debug.Log($"<color=orange>[Day03]</color> Selected {clipsToSelect.Count} clips.");
            return true; // Success
        }
    }
}