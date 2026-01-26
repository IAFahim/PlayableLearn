using TweenPlayables;

namespace AV.Day07.Runtime.Day07Timeline
{
    public sealed class Day07GhostMixerBehaviour : TweenAnimationMixerBehaviour<Day07Ghost, Day07GhostBehaviour>
    {
        readonly FloatValueMixer opacityMixer = new FloatValueMixer();

        public override void Blend(Day07Ghost binding, Day07GhostBehaviour behaviour, float weight, float progress)
        {
            // Reading the behaviour data (Start/End values from Timeline)
            opacityMixer.TryBlend(behaviour.Opacity, binding, progress, weight);
        }

        public override void Apply(Day07Ghost binding)
        {
            // ? WRITING TO THE PUBLIC PROPERTY
            // The mixer passes the value to 'Opacity', which sets '_secretOpacity'
            opacityMixer.TryApplyAndClear(binding, (val, target) => target.Opacity = val);
        }
    }
}
