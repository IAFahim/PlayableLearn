using TweenPlayables;
using UnityEngine.Timeline;

namespace AV.Day07.Runtime.Day07Timeline
{
    [TrackBindingType(typeof(Day07Ghost))]
    [TrackClipType(typeof(Day07GhostClip))]
    public sealed class Day07GhostTrack : TweenAnimationTrack<Day07Ghost, Day07GhostMixerBehaviour, Day07GhostBehaviour> { }
}
