using Common;
using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day06.Runtime
{
    public class Day06ShakeMixerBehaviour : PlayableBehaviour
    {
        public override void OnGraphStart(Playable playable)
        {
            playable.LogFull(null, "white", null, "MIXER", depth: 0);
        }

        public override void OnGraphStop(Playable playable)
        {
            playable.LogFull(null, "white", null, "MIXER", depth: 0);
        }

        public override void OnPlayableCreate(Playable playable)
        {
            playable.LogFull(null, "white", null, "MIXER", depth: 0);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            playable.LogFull(null, "white", null, "MIXER", depth: 0);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            playable.LogFull(info, "green", null, "MIXER", depth: 0);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            playable.LogFull(info, "red", null, "MIXER", depth: 0);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            playable.LogFull(info, "orange", null, "MIXER", depth: 0);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            playable.LogFull(info, "white", playerData, "MIXER", depth: 0);
            playable.LogMixingState(info, depth: 0);

            float finalIntensity = 0f; // Local variable for teaching lesson
            int inputCount = playable.GetInputCount(); // Get total input count
            string logDetail = ""; // Build detail string for logging

            for (int i = 0; i < inputCount; i++)
            {
                Playable input = playable.GetInput(i); // Get input at index

                if (!input.IsValid()) // Skip invalid inputs
                {
                    continue;
                }

                float inputWeight = playable.GetInputWeight(i); // Get blend weight for this input

                if (inputWeight <= 0.001f) // Skip if weight is negligible
                {
                    continue;
                }

                if (input.GetPlayableType() == typeof(Day06ShakePlayableBehaviour)) // Check if our shake type
                {
                    ScriptPlayable<Day06ShakePlayableBehaviour> inputPlayable = (ScriptPlayable<Day06ShakePlayableBehaviour>)input; // Explicit cast - no .Cast<T>() in Unity
                    Day06ShakePlayableBehaviour behaviour = inputPlayable.GetBehaviour(); // Get behaviour from playable
                    float weightedValue = behaviour.intensity * inputWeight; // Calculate weighted contribution
                    finalIntensity += weightedValue; // Accumulate total intensity

                    logDetail += $"Input#{i}: {behaviour.intensity:F2} x {inputWeight:F2} = {weightedValue:F2}, "; // Build log string showing contribution
                }
            }

            // Log final result with cyan header and grey details
            UnityEngine.Debug.Log($"<color=cyan>[SHAKE MIXER]</color> total: {finalIntensity:F2}\n<color=grey>{logDetail.TrimEnd(',', ' ')}</color>");
        }
    }
}
