using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day03.Runtime
{
    // [DisplayName]
    // Sets the default name of the clip when created.
    [System.ComponentModel.DisplayName("Day 03 Action Clip")]

    // [NotKeyable]
    // Prevents the "Record" button from appearing on the track for this clip properties,
    // and prevents Curves from being added to this specific asset.
    [NotKeyable]
    public class Day03DisplayNamePlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }
}
