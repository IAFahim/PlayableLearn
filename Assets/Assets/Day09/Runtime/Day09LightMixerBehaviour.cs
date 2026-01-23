using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day09.Runtime
{
    // The "Logic" Node (Mixer)
    public class Day09LightMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Light trackBinding = playerData as Light;
            if (trackBinding == null) return;

            int inputCount = playable.GetInputCount();

            Color finalColor = Color.black;
            float finalIntensity = 0f;
            float totalWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                
                if (inputWeight <= 0.0001f) continue;
                
                ScriptPlayable<Day09LightControlBehaviour> inputPlayable = 
                    (ScriptPlayable<Day09LightControlBehaviour>)playable.GetInput(i);
                
                Day09LightControlBehaviour inputBehaviour = inputPlayable.GetBehaviour();
                
                finalColor += inputBehaviour.color * inputWeight;
                finalIntensity += inputBehaviour.intensity * inputWeight;
                totalWeight += inputWeight;
            }

            trackBinding.color = finalColor;
            trackBinding.intensity = finalIntensity;
        }
    }
}
