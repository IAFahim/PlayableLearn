using System.Collections.Generic;
using AV.Day03.Runtime;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

namespace AV.Day03.Editor
{
    [MenuEntry("Day 03/Quick Log Clip Info", 100)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.Active)]
    public class Day03LogClipAction : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            foreach (var clip in clips)
            {
                if (clip.asset is Day03DisplayNamePlayableAsset)
                    return ActionValidity.Valid;
            }

            return ActionValidity.NotApplicable;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            foreach (var clip in clips)
            {
                Debug.Log($"<color=cyan>[Day03 Action]</color> Clip: {clip.displayName} | Start: {clip.start:F2}");
            }

            return true;
        }
    }

    public class Day03DoubleSpeedAction : ClipAction
    {
        [TimelineShortcut("Day03/DoubleClipSpeed", KeyCode.H, ShortcutModifiers.Shift)]
        public static void HandleShortCut(ShortcutArguments args)
        {
            Invoker.InvokeWithSelectedClips<Day03DoubleSpeedAction>();
        }

        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            Undo.RegisterCompleteObjectUndo(TimelineEditor.inspectedAsset, "Double Speed");
            foreach (var clip in clips)
            {
                clip.timeScale *= 2.0;
                Debug.Log($"<color=green>[Day03]</color> Speed doubled for {clip.displayName}");
            }

            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
            return true;
        }
    }
}