using Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day03.Runtime
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(Day03LoggerPlayableAsset))]
    public class Day03SystemTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<Day03MixerBehaviour>.Create(graph, inputCount);
            return mixer;
        }
        
        
    }
}
