using System;
using UnityEngine;
using TweenPlayables;

namespace Day06.Timeline
{
    [Serializable]
    public sealed class Day06VisualsBehaviour : TweenAnimationBehaviour<Day06Visuals>
    {
        // 1. Define the parameters you want to tween using the library's types
        [SerializeField] ColorTweenParameter glowColor;
        [SerializeField] FloatTweenParameter glowIntensity;
        [SerializeField] Vector3TweenParameter localOffset;
        [SerializeField] FloatTweenParameter spinSpeed;

        // 2. Expose ReadOnly properties for the Mixer to access
        public ReadOnlyTweenParameter<Color> GlowColor => glowColor;
        public ReadOnlyTweenParameter<float> GlowIntensity => glowIntensity;
        public ReadOnlyTweenParameter<Vector3> LocalOffset => localOffset;
        public ReadOnlyTweenParameter<float> SpinSpeed => spinSpeed;

        // 3. Capture the starting values when the timeline begins
        public override void OnTweenInitialize(Day06Visuals playerData)
        {
            glowColor.SetInitialValue(playerData, playerData.glowColor);
            glowIntensity.SetInitialValue(playerData, playerData.glowIntensity);
            localOffset.SetInitialValue(playerData, playerData.localOffset);
            spinSpeed.SetInitialValue(playerData, playerData.spinSpeed);
        }
    }
}