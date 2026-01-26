using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day02.Runtime
{
    [Serializable]
    public class Day02PlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private SimpleState _state;

        public ClipCaps clipCaps => ClipCaps.All;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<Day02SimplePlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            ((ISimpleSystem)behaviour).Initialize(_state);
            return playable;
        }
    }
}