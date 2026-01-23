using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day01.Runtime
{
    [System.Serializable]
    public class Day01LoggerPlayableAsset : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<Day01LoggerPlayableBehaviour>.Create(graph);
        }
    }
}