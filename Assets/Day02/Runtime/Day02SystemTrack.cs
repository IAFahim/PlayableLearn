using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day02.Runtime
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(Day02LoggerPlayableAsset))]
    public class Day02SystemTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<Day02MixerBehaviour>.Create(graph, inputCount);
        }
    }
}
