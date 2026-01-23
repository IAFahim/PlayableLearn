using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day02.Runtime
{
    public class Day02SystemClip : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
        
        
    }
}