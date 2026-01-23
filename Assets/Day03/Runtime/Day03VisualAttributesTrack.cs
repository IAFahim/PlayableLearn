using AV.Day02.Runtime;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day03.Runtime
{
    // [TrackColor]
    // Defines the color of the track header and the side accent.
    // Use standard RGB float values (0f to 1f).
    [TrackColor(0.5f, 0f, 0.5f)] // Purple

    // [TrackBindingType]
    // Defines what kind of object can be bound to this track (GameObject, Animator, Custom Component).
    [TrackBindingType(typeof(GameObject))]

    // [TrackClipType]
    // Limits which PlayableAssets can be dropped onto this track.
    // You can have multiple of these.
    [TrackClipType(typeof(Day03DisplayNamePlayableAsset))]
    [TrackClipType(typeof(Day02PlayableAsset))]
    [TrackClipType(typeof(AnimationPlayableAsset))]

    // [DisplayName]
    // Overrides the default class name shown in the "Add Track" menu.
    // Useful for grouping tracks (e.g., "Day 03/Visual Track").
    [System.ComponentModel.DisplayName("Day 03/Visual Attributes Track")]
    public class Day03VisualAttributesTrack : TrackAsset
    {
    }
}
