using System;
using TweenPlayables;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day05.Runtime
{
    [Serializable]
    public sealed class Day05TweenHealthBehaviour : TweenAnimationBehaviour<Day05HealthComponent>
    {
        [SerializeField] private FloatTweenParameter health;

        public ReadOnlyTweenParameter<float> Health => health;

        public override void OnTweenInitialize(Day05HealthComponent playerData)
        {
            health.SetInitialValue(playerData, playerData.health);
        }
    }

    public sealed class Day05TweenHealthMixerBehaviour : 
        TweenAnimationMixerBehaviour<Day05HealthComponent, Day05TweenHealthBehaviour>
    {
        private readonly FloatValueMixer healthMixer = new();

        public override void Blend(Day05HealthComponent binding, Day05TweenHealthBehaviour behaviour, float weight,
            float progress)
        {
            healthMixer.TryBlend(behaviour.Health, binding, progress, weight);
        }

        public override void Apply(Day05HealthComponent binding)
        {
            healthMixer.TryApplyAndClear(binding, (val, target) => target.health = val);
        }
    }

    [TrackBindingType(typeof(Day05HealthComponent))]
    [TrackClipType(typeof(Day05TweenHealthClip))]
    public sealed class
        Day05TweenHealthTrack : TweenAnimationTrack<Day05HealthComponent, Day05TweenHealthMixerBehaviour, Day05TweenHealthBehaviour>
    {
    }
}