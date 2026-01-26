#if UNITY_EDITOR
using System.ComponentModel;
#endif
using TweenPlayables;
using UnityEngine.Timeline;

namespace AV.Day05.Runtime
{
    [TrackBindingType(typeof(HealthComponent))]
    [TrackClipType(typeof(TweenCanvasGroupClip))]
#if UNITY_EDITOR
    [DisplayName("AV/Health")]
#endif
    public sealed class HealthTrack : TweenAnimationTrack<HealthComponent, HealthMixerBehaviour, TweenHealthBehaviour> { }
}