using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Common
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct LogState
    {
        public bool IsValid;
        public double Duration;
        public double Time;
        public float Speed;
        public PlayState State;

        public override string ToString() => $"[LogState] Valid:{IsValid} Time:{Time:F2}s Speed:{Speed:F1}x"; // Debug view
    }

    public static class LogLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidatePlayable(Playable p, out bool isValid)
        {
            isValid = p.IsValid(); // Atomic validation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetPlayableDuration(Playable p, out double duration, out string durationStr)
        {
            duration = p.GetDuration();
            durationStr = (duration > 1e10 || duration == double.MaxValue) ? "Inf" : $"{duration:F3}"; // Atomic fetch and format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetTimeAndSpeed(Playable p, out double time, out float speed)
        {
            time = p.GetTime();
            speed = (float)p.GetSpeed(); // Atomic fetch
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetPlayState(Playable p, out PlayState state, out string stateStr)
        {
            state = p.GetPlayState();
            stateStr = state.ToString(); // Atomic fetch and format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatGraphInfo(PlayableGraph graph, out string graphName, out string isPlayingStr)
        {
            graphName = graph.GetEditorName();
            isPlayingStr = graph.IsPlaying() ? "Running" : "Stopped"; // Atomic fetch and format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatFrameData(FrameData frame, out string frameInfo)
        {
            frameInfo = $"DeltaTime: {frame.deltaTime:F4}s | FrameID: {frame.frameId} | EffectiveSpeed: {frame.effectiveSpeed:F2}x"; // Atomic format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatBlendingInfo(FrameData frame, out string blendInfo)
        {
            blendInfo = $"Weight: {frame.weight:F2} | Evaluation: {frame.evaluationType}"; // Atomic format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatEventFlags(Playable p, FrameData frame, out string flags)
        {
            flags = "";
            if (frame.seekOccurred) flags += "[SEEK] ";
            if (frame.timeLooped) flags += "[LOOP] ";
            if (p.IsDone()) flags += "[DONE] ";
            if (string.IsNullOrEmpty(flags)) flags = "None"; // Atomic format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetMixingState(Playable p, FrameData frame, out bool hasActiveMixing, out string mixInfo)
        {
            hasActiveMixing = false;
            mixInfo = string.Empty;

            var inputCount = p.GetInputCount();
            if (inputCount == 0) return;

            var activeInputCount = 0; // Count active inputs instead of just checking existence

            for (int i = 0; i < inputCount; i++)
            {
                var input = p.GetInput(i);
                if (input.IsValid() && p.GetInputWeight(i) > 0f)
                {
                    activeInputCount++;
                }
            }

            hasActiveMixing = activeInputCount > 1; // Only true when 2+ inputs are active

            if (hasActiveMixing)
            {
                mixInfo = $"[MIXING @ {frame.frameId}] Active Inputs: ";
                for (int i = 0; i < inputCount; i++)
                {
                    var input = p.GetInput(i);
                    if (input.IsValid())
                    {
                        var weight = p.GetInputWeight(i);
                        if (weight > 0f)
                        {
                            mixInfo += $"#{i}({weight:F2}) ";
                        }
                    }
                }
            }
        }
    }

    public static class LogExtensions
    {
        public static bool TryLogFull(ref this LogState state, Playable p, FrameData? f, string color, object playerData, string method, string role, string indent, out string message)
        {
            message = string.Empty;

            LogLogic.ValidatePlayable(p, out var isValid);
            if (!isValid)
            {
                message = $"<color={color}><b>[{role}] {method}</b></color> <color=red>INVALID PLAYABLE HANDLE</color>";
                return false; // Guard: Invalid playable
            }

            LogLogic.GetPlayableDuration(p, out var duration, out var durStr);
            LogLogic.GetTimeAndSpeed(p, out var time, out var speed);
            LogLogic.GetPlayState(p, out var playState, out var stateStr);
            LogLogic.FormatGraphInfo(p.GetGraph(), out var graphName, out var isPlayingStr);

            state.IsValid = isValid;
            state.Duration = duration;
            state.Time = time;
            state.Speed = speed;
            state.State = playState;

            var prefix = string.IsNullOrEmpty(indent) ? "" : $"{indent} ";
            message = $"<color={color}><b>{prefix}[{role}] {method}</b></color>\n" +
                     $"{prefix}  <color=cyan><b>TIME & STATE</b></color>\n" +
                     $"{prefix}  • <b>Global Time:</b> {time:F3}s / {durStr}s\n" +
                     $"{prefix}  • <b>State:</b> {stateStr} (Graph: {isPlayingStr})\n" +
                     $"{prefix}  • <b>Speed:</b> {speed:F2}x";

            if (f.HasValue)
            {
                FrameData frame = f.Value;
                LogLogic.FormatFrameData(frame, out var frameInfo);
                LogLogic.FormatBlendingInfo(frame, out var blendInfo);
                LogLogic.FormatEventFlags(p, frame, out var flags);

                message += $"\n{prefix}  <color=cyan><b>FRAME DATA</b></color>\n" +
                          $"{prefix}  • {frameInfo}\n" +
                          $"{prefix}  <color=orange><b>BLENDING</b></color>\n" +
                          $"{prefix}  • {blendInfo}\n" +
                          $"{prefix}  <color=yellow><b>EVENTS</b></color>\n" +
                          $"{prefix}  • <b>Flags:</b> {flags}";
            }

            message += $"\n{prefix}  <color=grey><b>IDENTITY & GRAPH</b></color>\n" +
                      $"{prefix}  • <b>Type:</b> {p.GetPlayableType().Name}\n" +
                      $"{prefix}  • <b>Graph:</b> {graphName} (Mode: {p.GetGraph().GetTimeUpdateMode()})\n" +
                      $"{prefix}  • <b>Structure:</b> {p.GetInputCount()} Inputs, {p.GetOutputCount()} Outputs";

            if (playerData != null)
            {
                message += $"\n{prefix}  <color=magenta><b>USER DATA</b></color>\n" +
                          $"{prefix}  • {playerData}";
            }

            return true; // Success
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TrackLogState
    {
        public bool IsValid;
        public double Start;
        public double End;
        public double Duration;
        public int ClipCount;
        public int MarkerCount;
        public bool IsMuted;
        public bool IsLocked;
        public bool HasCurves;
        public int ChildTrackCount;

        public override string ToString() => $"[TrackLogState] Clips:{ClipCount} Duration:{Duration:F2}s Markers:{MarkerCount}";
    }

    public static class TrackAssetLoggerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetTrackInfo(this TrackAsset track, out int clipCount, out int markerCount, out double start, out double end, out double duration)
        {
            var clips = track.GetClips();
            clipCount = 0;
            foreach (var _ in clips) clipCount++; // Force enumeration without alloc
            markerCount = track.GetMarkerCount();
            start = track.start;
            end = track.end;
            duration = track.duration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatTrackInfo(this TrackAsset track, out string trackInfo)
        {
            track.GetTrackInfo(out var clipCount, out var markerCount, out var start, out var end, out var duration);
            var mutedStr = track.muted ? "[MUTED]" : "";
            var lockedStr = track.locked ? "[LOCKED]" : "";
            var curvesStr = track.hasCurves ? "[CURVES]" : "";
            trackInfo = $"Clips:{clipCount} Markers:{markerCount} Duration:{duration:F3}s ({start:F3}s - {end:F3}s) {mutedStr}{lockedStr}{curvesStr}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetChildTrackInfo(this TrackAsset track, out int childCount, out string childInfo)
        {
            var childTracks = track.GetChildTracks();
            childCount = 0;
            foreach (var _ in childTracks) childCount++;
            childInfo = childCount > 0 ? $"ChildTracks:{childCount}" : "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetTimelineInfo(this TrackAsset track, out string timelineName)
        {
            var timeline = track.timelineAsset;
            timelineName = timeline != null ? timeline.name : "<No Timeline>";
        }

        public static void LogBeforeSerialize(this TrackAsset track, string color = "cyan")
        {
            track.FormatTrackInfo(out var trackInfo);
            track.GetChildTrackInfo(out var childCount, out var childInfo);
            track.GetTimelineInfo(out var timelineName);
            var childStr = string.IsNullOrEmpty(childInfo) ? "" : $" | {childInfo}";

            Debug.Log($"<color={color}><b>[TRACK] {track.name} OnBeforeSerialize</b></color>\n" +
                      $"  • Timeline: {timelineName}\n" +
                      $"  • {trackInfo}{childStr}");
        }

        public static void LogAfterDeserialize(this TrackAsset track, string color = "lime")
        {
            track.FormatTrackInfo(out var trackInfo);
            track.GetChildTrackInfo(out var childCount, out var childInfo);
            track.GetTimelineInfo(out var timelineName);
            var childStr = string.IsNullOrEmpty(childInfo) ? "" : $" | {childInfo}";

            Debug.Log($"<color={color}><b>[TRACK] {track.name} OnAfterDeserialize</b></color>\n" +
                      $"  • Timeline: {timelineName}\n" +
                      $"  • {trackInfo}{childStr}");
        }

        public static void LogVersionUpgrade(this TrackAsset track, int oldVersion, int newVersion, string color = "yellow")
        {
            Debug.Log($"<color={color}><b>[TRACK] {track.name} Version Upgrade</b></color>\n" +
                      $"  • Version: {oldVersion} → {newVersion}");
        }

        public static void LogTrackMixerCreated(this TrackAsset track, Playable mixerPlayable, string color = "magenta")
        {
            track.FormatTrackInfo(out var trackInfo);
            var inputCount = mixerPlayable.GetInputCount();
            var typeName = mixerPlayable.GetPlayableType().Name;

            Debug.Log($"<color={color}><b>[TRACK] {track.name} CreateTrackMixer</b></color>\n" +
                      $"  • Mixer: {typeName} with {inputCount} inputs\n" +
                      $"  • {trackInfo}");
        }

        public static void LogOnCreateClip(this TrackAsset track, TimelineClip clip, string color = "blue")
        {
            Debug.Log($"<color={color}><b>[TRACK] {track.name} OnCreateClip</b></color>\n" +
                      $"  • Clip: {clip.displayName}\n" +
                      $"  • Duration: {clip.duration:F3}s | Start: {clip.start:F3}s | End: {clip.end:F3}s\n" +
                      $"  • Asset: {clip.asset?.GetType().Name ?? "null"}");
        }
    }

    public static class CommonLogger
    {
        private static readonly string[] IndentMap = new string[]
        {
            "",           // 0 depth
            "│",        // 1 depth - single clip
            "│  │",     // 2 depth - nested
            "│  │  │"   // 3+ depth
        };

        // Simple, clean log without verbose details
        public static void LogSimple(
            this Playable p,
            FrameData? f = null,
            string color = "white",
            object playerData = null,
            string role = "PLAYABLE",
            int depth = 0,
            [CallerMemberName] string method = ""
        )
        {
            LogLogic.GetTimeAndSpeed(p, out var time, out var speed);
            var indent = depth < IndentMap.Length ? IndentMap[depth] : new string(' ', depth * 2);
            var playerDataStr = playerData != null ? $" | Data: {playerData}" : "";
            Debug.Log($"<color={color}>{indent}[{role}] {method} | Time:{time:F3}s Speed:{speed:F1}x{playerDataStr}</color>");
        }

        public static void LogFull(
            this Playable p,
            FrameData? f = null,
            string color = "white",
            object playerData = null,
            string role = "PLAYABLE",
            int depth = 0,
            [CallerMemberName] string method = ""
        )
        {
            var state = new LogState();
            var indent = depth < IndentMap.Length ? IndentMap[depth] : new string(' ', depth * 2);
            var success = state.TryLogFull(p, f, color, playerData, method, role, indent, out var message);

            // No contextObj = no stack trace spam!
            Debug.Log(message);
        }

        public static void LogMixingState(this Playable p, FrameData frame, int depth = 0)
        {
            LogLogic.GetMixingState(p, frame, out var hasActiveMixing, out var mixInfo);

            if (hasActiveMixing)
            {
                var indent = depth < IndentMap.Length ? IndentMap[depth] : new string(' ', depth * 2);
                // No contextObj = no stack trace spam!
                Debug.Log($"<color=yellow>{indent}{mixInfo}</color>");
            }
        }
    }
}
