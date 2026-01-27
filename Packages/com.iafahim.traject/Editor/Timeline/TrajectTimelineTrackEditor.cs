using AV.Traject.Runtime.Timeline;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Editor.Timeline
{
    [CustomTimelineEditor(typeof(TrajectTimelineTrack))]
    public class TrajectTimelineTrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            var options = base.GetTrackOptions(track, binding);
            options.errorText = IsValidTrack(track, binding) ? string.Empty : "Requires a Transform binding";
            return options;
        }

        private static bool IsValidTrack(TrackAsset track, Object binding)
        {
            return binding != null && binding is Transform;
        }
    }
}
