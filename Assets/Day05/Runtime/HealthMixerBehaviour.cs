using TweenPlayables;
using UnityEngine;

namespace AV.Day05.Runtime
{
    public class HealthMixerBehaviour: TweenAnimationMixerBehaviour<HealthComponent, TweenHealthBehaviour>
    {
        public override void Blend(HealthComponent binding, TweenHealthBehaviour behaviour, float weight, float progress)
        {
            Debug.Log(weight, binding);
        }

        public override void Apply(HealthComponent binding)
        {
            Debug.Log(binding.ToString(), binding);
        }
    }
}