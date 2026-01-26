using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day04.Runtime
{
    [TrackColor(1f, 0.5f, 0f)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(Day04AdvancedPlayableAsset))]
    public class Day04AdvancedTrack : TrackAsset
    {
    }
}