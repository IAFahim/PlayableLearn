using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using AV.Traject.Runtime.Integration;

namespace AV.Traject.Runtime.Timeline
{
    [Serializable]
    public class TrajectTimelineClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private TrajectTimelineBehaviour behaviour = new TrajectTimelineBehaviour();

        public TrajectAsset Asset => behaviour.Asset;
        public float Range => behaviour.Range;
        public bool UseCachedOrigin => behaviour.UseCachedOrigin;

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TrajectTimelineBehaviour>.Create(graph, behaviour);
            return playable;
        }
    }
}
