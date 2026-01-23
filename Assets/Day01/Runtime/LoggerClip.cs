using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day01.Runtime
{
    [System.Serializable]
    public class LoggerClip : PlayableAsset, ITimelineClipAsset
    {
        public string message = "My Event Name";

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LoggerBehaviour>.Create(graph);

            var behaviour = playable.GetBehaviour();
            behaviour.messageContent = message;

            return playable;
        }
    }
}