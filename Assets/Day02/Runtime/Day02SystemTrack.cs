using UnityEngine.Timeline;

namespace AV.Day02.Runtime
{
    [TrackBindingType(typeof(Day02Component))]
    [TrackClipType(typeof(Day02PlayableAsset))]
    public class Day02SystemTrack : TrackAsset
    {
    }
}
