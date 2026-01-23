using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day02.Runtime
{
    [TrackColor(0.2f, 0.8f, 0.4f)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(Day02SystemClip))]
    public class Day02SystemTrack : TrackAsset
    {
    }
}