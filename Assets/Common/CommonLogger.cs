using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;
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
        public static bool TryLogFull(ref this LogState state, Playable p, FrameData? f, string color, object playerData, string method, string role, out string message)
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

            message = $"<color={color}><b>[{role}] {method}</b></color>\n" +
                     $"   <color=cyan><b>TIME & STATE</b></color>\n" +
                     $"   • <b>Global Time:</b> {time:F3}s / {durStr}s\n" +
                     $"   • <b>State:</b> {stateStr} (Graph: {isPlayingStr})\n" +
                     $"   • <b>Speed:</b> {speed:F2}x";

            if (f.HasValue)
            {
                FrameData frame = f.Value;
                LogLogic.FormatFrameData(frame, out var frameInfo);
                LogLogic.FormatBlendingInfo(frame, out var blendInfo);
                LogLogic.FormatEventFlags(p, frame, out var flags);

                message += $"\n   <color=cyan><b>FRAME DATA</b></color>\n" +
                          $"   • {frameInfo}\n" +
                          $"   <color=orange><b>BLENDING</b></color>\n" +
                          $"   • {blendInfo}\n" +
                          $"   <color=yellow><b>EVENTS</b></color>\n" +
                          $"   • <b>Flags:</b> {flags}";
            }

            message += $"\n   <color=grey><b>IDENTITY & GRAPH</b></color>\n" +
                      $"   • <b>Type:</b> {p.GetPlayableType().Name}\n" +
                      $"   • <b>Graph:</b> {graphName} (Mode: {p.GetGraph().GetTimeUpdateMode()})\n" +
                      $"   • <b>Structure:</b> {p.GetInputCount()} Inputs, {p.GetOutputCount()} Outputs";

            if (playerData != null)
            {
                message += $"\n   <color=magenta><b>USER DATA</b></color>\n" +
                          $"   • {playerData}";
            }

            return true; // Success
        }
    }

    public static class CommonLogger
    {
        public static void LogFull(
            this Playable p,
            FrameData? f = null,
            string color = "white",
            object playerData = null,
            string role = "PLAYABLE",
            [CallerMemberName] string method = ""
        )
        {
            var state = new LogState();
            var success = state.TryLogFull(p, f, color, playerData, method, role, out var message);

            var contextObj = p.GetGraph().GetResolver() as Object;
            Debug.Log(message, contextObj);
        }

        public static void LogMixingState(this Playable p, FrameData frame)
        {
            LogLogic.GetMixingState(p, frame, out var hasActiveMixing, out var mixInfo);

            if (hasActiveMixing)
            {
                var contextObj = p.GetGraph().GetResolver() as Object;
                Debug.Log($"<color=yellow>{mixInfo}</color>", contextObj);
            }
        }
    }
}
