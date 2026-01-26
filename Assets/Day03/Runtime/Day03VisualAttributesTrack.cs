using System.ComponentModel;
using AV.Day02.Runtime;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day03.Runtime
{
    [TrackColor(0.5f, 0f, 0.5f)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(Day03DisplayNamePlayableAsset))]
    [TrackClipType(typeof(Day02PlayableAsset))]
    [TrackClipType(typeof(AnimationPlayableAsset))]
    [DisplayName("Day 03/Visual Attributes Track")]
    public class Day03VisualAttributesTrack : TrackAsset
    {
    }
}