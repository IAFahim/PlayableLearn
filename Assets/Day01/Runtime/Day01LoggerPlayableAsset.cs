using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day01.Runtime
{
    [Serializable]
    public class Day01LoggerPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<Day01LoggerPlayableBehaviour>.Create(graph);
        }
    }
}