using System.Collections.Generic;
using AV.Day07.Runtime.Day07Timeline;
using TweenPlayables.Editor;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

namespace AV.Day07.Editor.Day07Timeline
{
    [CustomTimelineEditor(typeof(Day07GhostTrack))]
    public sealed class Day07GhostTrackEditor : TweenAnimationTrackEditor
    {
        public override string DefaultTrackName => "Day 07 Ghost Track";
        public override Color TrackColor => new Color(0.9f, 0.9f, 0.9f); // Ghostly White
        public override Texture2D TrackIcon => Styles.CsScriptIcon;
    }

    [CustomTimelineEditor(typeof(Day07GhostClip))]
    public sealed class Day07GhostClipEditor : TweenAnimationClipEditor
    {
        public override string DefaultClipName => "Tween Ghost";
        public override Color ClipColor => new Color(0.9f, 0.9f, 0.9f);
        public override Texture2D ClipIcon => Styles.CsScriptIcon;
    }

    [CustomPropertyDrawer(typeof(Day07GhostBehaviour))]
    public sealed class Day07GhostBehaviourDrawer : TweenAnimationBehaviourDrawer
    {
        static readonly string[] parameters = new string[]
        {
            "opacity"
        };

        protected override IEnumerable<string> GetPropertyNames() => parameters;
    }
}
