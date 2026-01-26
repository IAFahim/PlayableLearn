using UnityEngine;
using UnityEngine.Timeline;
using TweenPlayables;

namespace Day06.Timeline
{
    [TrackBindingType(typeof(Day06Visuals))]
    [TrackClipType(typeof(Day06VisualsClip))]
    public sealed class Day06VisualsTrack : TweenAnimationTrack<Day06Visuals, Day06VisualsMixerBehaviour, Day06VisualsBehaviour> { }
}