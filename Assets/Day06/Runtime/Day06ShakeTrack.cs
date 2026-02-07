using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day06.Runtime
{
    [TrackColor(1f, 0.2f, 0.2f)]
    [TrackClipType(typeof(Day06ShakePlayableAsset))]
    public class Day06ShakeTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<Day06ShakeMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
