using System;
using Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day03.Runtime
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(Day03LoggerPlayableAsset))]
    public class Day03SystemTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerPlayable = ScriptPlayable<Day03MixerBehaviour>.Create(graph, inputCount);
            this.LogTrackMixerCreated(mixerPlayable, "magenta");
            return mixerPlayable;
        }

        protected override void OnBeforeTrackSerialize()
        {
            this.LogBeforeSerialize("cyan");
            base.OnBeforeTrackSerialize();
        }

        protected override void OnAfterTrackDeserialize()
        {
            base.OnAfterTrackDeserialize();
            this.LogAfterDeserialize("lime");
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            this.LogOnCreateClip(clip, "blue");
            base.OnCreateClip(clip);
        }
    }
}
