using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day01.Runtime
{
    [System.Serializable]
    public class LoggerPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LoggerPlayableBehaviour>.Create(graph);
            return playable;
        }
    }
}