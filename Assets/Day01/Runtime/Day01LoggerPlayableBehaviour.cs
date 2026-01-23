using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day01.Runtime
{
    public class Day01LoggerPlayableBehaviour : PlayableBehaviour
    {
        // Lifecycle
        public override void OnGraphStart(Playable playable) => LogFull("OnGraphStart", playable);
        public override void OnGraphStop(Playable playable) => LogFull("OnGraphStop", playable);
        public override void OnPlayableCreate(Playable playable) => LogFull("OnPlayableCreate", playable);
        public override void OnPlayableDestroy(Playable playable) => LogFull("OnPlayableDestroy", playable);

        // Flow
        public override void OnBehaviourPlay(Playable playable, FrameData info) =>
            LogFull("OnBehaviourPlay", playable, info, "green");

        public override void OnBehaviourPause(Playable playable, FrameData info) =>
            LogFull("OnBehaviourPause", playable, info, "red");

        public override void OnBehaviourDelay(Playable playable, FrameData info) =>
            LogFull("OnBehaviourDelay", playable, info, "yellow");

        // Loop
        public override void PrepareData(Playable playable, FrameData info) => LogFull("PrepareData", playable, info);
        public override void PrepareFrame(Playable playable, FrameData info) => LogFull("PrepareFrame", playable, info);

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) =>
            LogFull("ProcessFrame", playable, info, "white", playerData);

        // --- Deep Dive Logger ---

        private void LogFull(string method, Playable p, FrameData? f = null, string color = "white",
            object playerData = null)
        {
            if (!p.IsValid())
            {
                Debug.Log($"<color={color}><b>[{method}]</b></color> <color=red>INVALID PLAYABLE HANDLE</color>");
                return;
            }

            var graph = p.GetGraph();
            double duration = p.GetDuration();
            string durStr = (duration > 1e10 || duration == double.MaxValue) ? "Inf" : $"{duration:F3}";

            // SECTION 1: THE ENGINE (Time & Motion) - Most Important!
            string log = $"<color={color}><b>[{method}]</b></color>\n" +
                         $"   <color=cyan><b>TIME & STATE</b></color>\n" +
                         $"   • <b>Global Time:</b> {p.GetTime():F3}s / {durStr}s\n" +
                         $"   • <b>State:</b> {p.GetPlayState()} (Graph: {(graph.IsPlaying() ? "Running" : "Stopped")})\n" +
                         $"   • <b>Speed:</b> {p.GetSpeed():F2}x";

            // SECTION 2: THE UPDATE (Frame Data) - Crucial for Logic
            if (f.HasValue)
            {
                FrameData frame = f.Value;
                log += $"\n   <color=cyan><b>FRAME DATA (Delta)</b></color>\n" +
                       $"   • <b>DeltaTime:</b> {frame.deltaTime:F4}s\n" +
                       $"   • <b>Frame ID:</b> {frame.frameId}\n" +
                       $"   • <b>Effective Speed:</b> {frame.effectiveSpeed:F2}x\n" +
                       $"   • <b>Effective State:</b> {frame.effectivePlayState}";
            }

            // SECTION 3: BLENDING (Mixer Info)
            if (f.HasValue)
            {
                FrameData frame = f.Value;
                log += $"\n   <color=orange><b>BLENDING</b></color>\n" +
                       $"   • <b>Weight:</b> {frame.weight:F2}\n" +
                       $"   • <b>Evaluation:</b> {frame.evaluationType}";
            }

            // SECTION 4: FLAGS & EVENTS
            if (f.HasValue)
            {
                FrameData frame = f.Value;
                string flags = "";
                if (frame.seekOccurred) flags += "[SEEK] ";
                if (frame.timeLooped) flags += "[LOOP] ";
                if (p.IsDone()) flags += "[DONE] ";

                if (string.IsNullOrEmpty(flags)) flags = "None";

                log += $"\n   <color=yellow><b>EVENTS</b></color>\n" +
                       $"   • <b>Flags:</b> {flags}";
            }

            // SECTION 5: IDENTITY & STRUCTURE (Static Info)
            log += $"\n   <color=grey><b>IDENTITY & GRAPH</b></color>\n" +
                   $"   • <b>Type:</b> {p.GetPlayableType().Name}\n" +
                   $"   • <b>Graph:</b> {graph.GetEditorName()} (Mode: {graph.GetTimeUpdateMode()})\n" +
                   $"   • <b>Structure:</b> {p.GetInputCount()} Inputs, {p.GetOutputCount()} Outputs";

            if (playerData != null)
            {
                log += $"\n   <color=magenta><b>USER DATA</b></color>\n" +
                       $"   • {playerData}";
            }
            var contextObj = graph.GetResolver() as Object;
            Debug.Log(log, contextObj);
        }
    }
}