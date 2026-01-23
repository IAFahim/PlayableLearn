using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day09.Runtime
{
    // The "Logic" Node (Mixer)
    public class LightMixerBehaviour : PlayableBehaviour
    {
        // This method runs every frame the graph is active.
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
                
                // Skip zero weight inputs
                if (inputWeight <= 0.0001f) continue;
                
                ScriptPlayable<LightControlBehaviour> inputPlayable = 
                    (ScriptPlayable<LightControlBehaviour>)playable.GetInput(i);
                
                LightControlBehaviour inputBehaviour = inputPlayable.GetBehaviour();
                
                finalColor += inputBehaviour.color * inputWeight;
                finalIntensity += inputBehaviour.intensity * inputWeight;
                totalWeight += inputWeight;
            }

            // Apply to the Light
            trackBinding.color = finalColor;
            trackBinding.intensity = finalIntensity;
        }
    }
}
