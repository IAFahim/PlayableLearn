using AV.Day03.Runtime;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace AV.Day03.Editor
{
    [MenuEntry("Day 03/Select All Clips on Track",  200)]
    public class Day03SelectClipsOnTrack : TrackAction
    {
        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            List<TimelineClip> clipsToSelect = new List<TimelineClip>();

            foreach (var track in tracks)
            {
                clipsToSelect.AddRange(track.GetClips());
            }

            UnityEditor.Timeline.TimelineEditor.selectedClips = clipsToSelect.ToArray();

            Debug.Log($"<color=orange>[Day03]</color> Selected {clipsToSelect.Count} clips.");
            return true;
        }
    }
}
