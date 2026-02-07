using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day06.Runtime
{
    [Serializable]
    public class Day06ShakePlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [Range(0f, 10f)]
        public float intensity = 1.0f;

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<Day06ShakePlayableBehaviour>.Create(graph);

            // TEACHER NOTE: This is "Data Injection"
            var behaviour = playable.GetBehaviour();
            behaviour.intensity = intensity;

            return playable;
        }
    }
}
