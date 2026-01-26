using System.Collections.Generic;
using TweenPlayables.Editor;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

namespace AV.Day05.Runtime
{
    [CustomTimelineEditor(typeof(TweenHealthTrack))]
    public sealed class TweenHealthTrackEditor : TweenAnimationTrackEditor
    {
        public override string DefaultTrackName => "Tween Health Track";
        public override Color TrackColor => new(0.9f, 0.2f, 0.2f);
        public override Texture2D TrackIcon => Styles.CsScriptIcon;
    }

    [CustomTimelineEditor(typeof(TweenHealthClip))]
    public sealed class TweenHealthClipEditor : TweenAnimationClipEditor
    {
        public override string DefaultClipName => "Tween Health";
        public override Color ClipColor => new(0.9f, 0.2f, 0.2f);
        public override Texture2D ClipIcon => Styles.CsScriptIcon;
    }

    [CustomPropertyDrawer(typeof(TweenHealthBehaviour))]
    public sealed class TweenHealthBehaviourDrawer : TweenAnimationBehaviourDrawer
    {
        private static readonly string[] Parameters =
        {
            nameof(HealthComponent.health)
        };

        protected override IEnumerable<string> GetPropertyNames()
        {
            return Parameters;
        }
    }
}