using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day01.Runtime
{
    public class Day01LoggerPlayableBehaviour : PlayableBehaviour
    {
        public override void OnGraphStart(Playable playable)
        {
            Debug.Log($"<b>[OnGraphStart]</b> {GetPlayableInfo(playable)}");
        }

        public override void OnGraphStop(Playable playable)
        {
            Debug.Log($"<b>[OnGraphStop]</b> {GetPlayableInfo(playable)}");
        }

        public override void OnPlayableCreate(Playable playable)
        {
            Debug.Log($"<b>[OnPlayableCreate]</b> {GetPlayableInfo(playable)}");
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            Debug.Log($"<b>[OnPlayableDestroy]</b> {GetPlayableInfo(playable)}");
        }

        public override void OnBehaviourDelay(Playable playable, FrameData info)
        {
            Debug.Log($"<b>[OnBehaviourDelay]</b>\n{GetPlayableInfo(playable)}\n{GetFrameInfo(info)}");
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log(
                $"<color=green><b>[OnBehaviourPlay]</b></color>\n{GetPlayableInfo(playable)}\n{GetFrameInfo(info)}");
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Debug.Log(
                $"<color=red><b>[OnBehaviourPause]</b></color>\n{GetPlayableInfo(playable)}\n{GetFrameInfo(info)}");
        }

        public override void PrepareData(Playable playable, FrameData info)
        {
            Debug.Log($"<b>[PrepareData]</b>\n{GetPlayableInfo(playable)}\n{GetFrameInfo(info)}");
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            Debug.Log($"<b>[PrepareFrame]</b>\n{GetPlayableInfo(playable)}\n{GetFrameInfo(info)}");
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Debug.Log(
                $"<b>[ProcessFrame]</b>\n{GetPlayableInfo(playable)}\n{GetFrameInfo(info)}\nPlayerData: {(playerData != null ? playerData.ToString() : "null")}");
        }

        private string GetPlayableInfo(Playable p)
        {
            if (!p.IsValid()) return "Playable: <color=red>[INVALID]</color>";

            double duration = p.GetDuration();
            string durationStr = (duration > 1e10 || Math.Abs(duration - double.MaxValue) < 1e10)
                ? "Inf"
                : $"{duration:F3}s";

            return "Playable: [" +
                   $"Time: {p.GetTime():F3}s / {durationStr} | " +
                   $"State: {p.GetPlayState()} | " +
                   $"Speed: {p.GetSpeed():F2} | " +
                   $"Inputs: {p.GetInputCount()} | " +
                   $"Done: {p.IsDone()}]";
        }

        private string GetFrameInfo(FrameData f)
        {
            return $"FrameData: [" +
                   $"ID: {f.frameId} | " +
                   $"dt: {f.deltaTime:F4}s | " +
                   $"Weight: {f.weight:F2} | " +
                   $"EffSpeed: {f.effectiveSpeed:F2} | " +
                   $"EffState: {f.effectivePlayState} | " +
                   $"Eval: {f.evaluationType} | " +
                   $"Seek: {f.seekOccurred} | " +
                   $"Loop: {f.timeLooped}]";
        }
    }
}