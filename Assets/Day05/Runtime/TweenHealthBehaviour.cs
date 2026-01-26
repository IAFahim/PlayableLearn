using System;
using TweenPlayables;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day05.Runtime
{
    [Serializable]
    public sealed class TweenHealthBehaviour : TweenAnimationBehaviour<HealthComponent>
    {
        [SerializeField] private FloatTweenParameter health;

        public ReadOnlyTweenParameter<float> Health => health;

        public override void OnTweenInitialize(HealthComponent playerData)
        {
            health.SetInitialValue(playerData, playerData.health);
        }
    }

    public sealed class
        TweenHealthMixerBehaviour : TweenAnimationMixerBehaviour<HealthComponent, TweenHealthBehaviour>
    {
        private readonly FloatValueMixer healthMixer = new();

        public override void Blend(HealthComponent binding, TweenHealthBehaviour behaviour, float weight,
            float progress)
        {
            healthMixer.TryBlend(behaviour.Health, binding, progress, weight);
        }

        public override void Apply(HealthComponent binding)
        {
            healthMixer.TryApplyAndClear(binding, (val, target) => target.health = val);
        }
    }

    [TrackBindingType(typeof(HealthComponent))]
    [TrackClipType(typeof(TweenHealthClip))]
    public sealed class
        TweenHealthTrack : TweenAnimationTrack<HealthComponent, TweenHealthMixerBehaviour, TweenHealthBehaviour>
    {
    }
}