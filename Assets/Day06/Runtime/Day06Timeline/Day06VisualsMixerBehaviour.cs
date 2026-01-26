using UnityEngine;
using TweenPlayables;

namespace Day06.Timeline
{
    public sealed class Day06VisualsMixerBehaviour : TweenAnimationMixerBehaviour<Day06Visuals, Day06VisualsBehaviour>
    {
        // 1. Create Mixers for each data type
        readonly ColorValueMixer colorMixer = new ColorValueMixer();
        readonly FloatValueMixer intensityMixer = new FloatValueMixer();
        readonly Vector3ValueMixer offsetMixer = new Vector3ValueMixer();
        readonly FloatValueMixer spinMixer = new FloatValueMixer();

        // 2. BLEND STEP: Accumulate values from all active clips
        public override void Blend(Day06Visuals binding, Day06VisualsBehaviour behaviour, float weight, float progress)
        {
            // The library's TryBlend handles "Active" checks and easing automatically
            colorMixer.TryBlend(behaviour.GlowColor, binding, progress, weight);
            intensityMixer.TryBlend(behaviour.GlowIntensity, binding, progress, weight);
            offsetMixer.TryBlend(behaviour.LocalOffset, binding, progress, weight);
            spinMixer.TryBlend(behaviour.SpinSpeed, binding, progress, weight);
        }

        // 3. APPLY STEP: Write the final calculated values to the component
        public override void Apply(Day06Visuals binding)
        {
            // ApplyAndClear resets the mixer for the next frame
            colorMixer.TryApplyAndClear(binding, (val, target) => target.glowColor = val);
            intensityMixer.TryApplyAndClear(binding, (val, target) => target.glowIntensity = val);
            offsetMixer.TryApplyAndClear(binding, (val, target) => target.localOffset = val);
            spinMixer.TryApplyAndClear(binding, (val, target) => target.spinSpeed = val);
        }
    }
}