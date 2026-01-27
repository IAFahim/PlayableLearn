using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Mathematics;

namespace AV.Traject.Runtime.Timeline
{
    public class TrajectTimelineMixerBehaviour : PlayableBehaviour
    {
        Transform target;
        readonly List<ScriptPlayable<TrajectTimelineBehaviour>> playables = new();

        public override void OnPlayableDestroy(Playable playable)
        {
            playables.Clear();
            target = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int inputCount = playable.GetInputCount();

            if (target == null)
            {
                target = playerData as Transform;
                if (target == null) return;

                for (int i = 0; i < inputCount; i++)
                {
                    var instance = (ScriptPlayable<TrajectTimelineBehaviour>)playable.GetInput(i);
                    playables.Add(instance);
                }

                foreach (var p in playables)
                {
                    p.GetBehaviour().Initialize(target);
                }
            }

            int activeInputCount = 0;
            float3 blendedPosition = float3.zero;
            float totalWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                var inputPlayable = (ScriptPlayable<TrajectTimelineBehaviour>)playable.GetInput(i);
                var behaviour = inputPlayable.GetBehaviour();
                var inputWeight = playable.GetInputWeight(i);
                var inputProgress = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());

                if (inputWeight == 0f) continue;

                if (target != null && behaviour.Asset != null)
                {
                    behaviour.Evaluate(target, inputProgress, out float3 pos);
                    blendedPosition += pos * inputWeight;
                    totalWeight += inputWeight;
                }

                activeInputCount++;
            }

            if (activeInputCount > 0 && totalWeight > 0f)
            {
                target.position = blendedPosition / totalWeight;
            }
        }
    }
}
