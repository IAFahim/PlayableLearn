using AV.Traject.Runtime.Timeline;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Editor.Timeline
{
    [CustomTimelineEditor(typeof(TrajectTimelineClip))]
    public class TrajectTimelineClipEditor : ClipEditor
    {
        private static readonly Color ClipColor = new Color(0.3f, 0.8f, 1f);
        private static readonly Texture2D ClipIcon = EditorGUIUtility.IconContent("Animation Icon").image as Texture2D;

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var options = base.GetClipOptions(clip);
            options.icons = new Texture2D[] { ClipIcon };
            options.highlightColor = ClipColor;
            return options;
        }

        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var trajectClip = clip.asset as TrajectTimelineClip;
            clip.displayName = trajectClip?.Asset != null ? trajectClip.Asset.name : "Traject";
        }
    }
}
