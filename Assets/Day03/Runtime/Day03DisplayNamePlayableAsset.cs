using AV.Day02.Runtime;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day03.Runtime
{
    [System.ComponentModel.DisplayName("Day 03 Action Clip")]
    [NotKeyable]
    [Serializable]
    public class Day03DisplayNamePlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private VisualState _state;

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<Day03VisualPlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            ((IVisualSystem)behaviour).Initialize(_state);
            return playable;
        }
    }
}
