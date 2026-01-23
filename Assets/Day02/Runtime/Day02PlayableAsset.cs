using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day02.Runtime
{
    public class Day02PlayableAsset : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }
}