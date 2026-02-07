using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day08.Runtime
{
    [Serializable]
    public class Day08ThemePlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public Color color = Color.white;

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<Day08ThemePlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.color = color;
            return playable;
        }
    }
}
