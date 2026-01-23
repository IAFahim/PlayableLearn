using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day01.Runtime
{
    [System.Serializable]
    public class Day01LoggerPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.All;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<Day01LoggerPlayableBehaviour>.Create(graph);
            return playable;
        }
    }
}