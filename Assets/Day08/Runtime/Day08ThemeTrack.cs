using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day08.Runtime
{
    [TrackColor(0.5f, 0f, 0.5f)]
    [TrackClipType(typeof(Day08ThemePlayableAsset))]
    [TrackBindingType(typeof(Renderer))]
    public class Day08ThemeTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<Day08ThemeMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
