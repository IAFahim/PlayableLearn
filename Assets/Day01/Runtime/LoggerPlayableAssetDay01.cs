using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day01.Runtime
{
    [System.Serializable]
    public class LoggerPlayableAssetDay01 : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LoggerPlayableBehaviour01>.Create(graph);
            return playable;
        }
    }
}