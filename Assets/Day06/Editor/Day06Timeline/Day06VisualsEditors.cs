using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using TweenPlayables.Editor;
using Day06.Timeline;

namespace Day06.Editor
{
    // 1. TRACK EDITOR: Defines how the track looks in the Timeline window
    [CustomTimelineEditor(typeof(Day06VisualsTrack))]
    public sealed class Day06VisualsTrackEditor : TweenAnimationTrackEditor
    {
        public override string DefaultTrackName => "Day 06 Visuals Track";
        public override Color TrackColor => new Color(0.0f, 0.8f, 0.9f); // Cyan
        public override Texture2D TrackIcon => Styles.CsScriptIcon; // Or load a custom icon
    }

    // 2. CLIP EDITOR: Defines how the clip looks on the track
    [CustomTimelineEditor(typeof(Day06VisualsClip))]
    public sealed class Day06VisualsClipEditor : TweenAnimationClipEditor
    {
        public override string DefaultClipName => "Tween Visuals";
        public override Color ClipColor => new Color(0.0f, 0.8f, 0.9f);
        public override Texture2D ClipIcon => Styles.CsScriptIcon;
    }

    // 3. PROPERTY DRAWER: The Magic. 
    // This connects the internal field names to the "Active/Start/End/Ease" drawer logic.
    [CustomPropertyDrawer(typeof(Day06VisualsBehaviour))]
    public sealed class Day06VisualsBehaviourDrawer : TweenAnimationBehaviourDrawer
    {
        // MUST match the field names in Day06VisualsBehaviour.cs exactly
        static readonly string[] parameters = new string[]
        {
            "glowColor",
            "glowIntensity",
            "localOffset",
            "spinSpeed"
        };

        protected override IEnumerable<string> GetPropertyNames() => parameters;
    }
}