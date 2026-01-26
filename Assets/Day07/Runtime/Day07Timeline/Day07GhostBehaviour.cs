using System;
using TweenPlayables;
using UnityEngine;

namespace AV.Day07.Runtime.Day07Timeline
{
    [Serializable]
    public sealed class Day07GhostBehaviour : TweenAnimationBehaviour<Day07Ghost>
    {
        [SerializeField] FloatTweenParameter opacity;

        public ReadOnlyTweenParameter<float> Opacity => opacity;

        public override void OnTweenInitialize(Day07Ghost playerData)
        {
            // ? READING FROM THE PUBLIC PROPERTY
            opacity.SetInitialValue(playerData, playerData.Opacity);
        }
    }
}
