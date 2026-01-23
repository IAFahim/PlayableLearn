using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace AV.Day03.Editor
{
    public abstract class Day03ToggleSnapAction : TimelineAction
    {
        [TimelineShortcut("Day03/ToggleSnap", KeyCode.S, ShortcutModifiers.Alt)]
        public static void HandleShortcut(ShortcutArguments args)
        {
            Invoker.InvokeWithSelected<Day03ToggleSnapAction>();
        }

        public override ActionValidity Validate(ActionContext context)
        {
            return ActionValidity.Valid;
        }

        public override bool Execute(ActionContext context)
        {
            bool currentSnap = TimelinePreferences.instance.snapToFrame;
            TimelinePreferences.instance.snapToFrame = !currentSnap;

            Debug.Log($"<color=magenta>[Day03]</color> Snap to Frame is now: {(!currentSnap)}");

            return true;
        }
    }
}
