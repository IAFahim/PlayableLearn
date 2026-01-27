using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEditor.Timeline.Actions;

namespace TweenPlayables.Editor.Tools
{
    // LAYER A: DATA
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct GroupLogState
    {
        public string LastOutput;
        public int GroupCount;
        public bool IsGenerated;

        public override string ToString() => $"[LogState] Groups: {GroupCount} ({(IsGenerated ? "READY" : "EMPTY")})"; // Debug view
    }

    // LAYER B: LOGIC
    public static class GroupLogLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatHeader(out string header)
        {
            header = "@startuml"; // Atomic string
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatFooter(out string footer)
        {
            footer = "@enduml"; // Atomic string
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatGroupLine(string groupName, int index, out string line)
        {
            line = $"concise \"{groupName}\" as T{index:D2}"; // Atomic format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateSelection(IEnumerable<TrackAsset> tracks, out bool isValid)
        {
            isValid = tracks != null && tracks.Any(); // Atomic validation
        }
    }

    // LAYER C: EXTENSIONS
    public static class GroupLogExtensions
    {
        public static bool TryGeneratePuml(ref this GroupLogState state, TimelineAsset timeline, out string result)
        {
            result = string.Empty; // Default

            if (timeline == null) return false; // Guard

            var sb = new StringBuilder();
            
            GroupLogLogic.FormatHeader(out var header); // Get Header
            sb.AppendLine(header); // Append

            int groupCount = 0;
            int rootCount = timeline.rootTrackCount; // Cache count

            for (int i = 0; i < rootCount; i++)
            {
                var track = timeline.GetRootTrack(i); // Access by index
                
                if (track is GroupTrack)
                {
                    groupCount++;
                    GroupLogLogic.FormatGroupLine(track.name, groupCount, out var line); // Format
                    sb.AppendLine(line); // Append
                }
            }

            GroupLogLogic.FormatFooter(out var footer); // Get Footer
            sb.AppendLine(footer); // Append

            state.GroupCount = groupCount; // State mutation
            state.LastOutput = sb.ToString(); // State mutation
            state.IsGenerated = true; // State mutation
            
            result = state.LastOutput; // Return data

            return groupCount > 0; // Success
        }
    }

    // LAYER D: WINDOW BRIDGE
    public interface ITimelineLogTool
    {
        bool TryAnalyze(TimelineAsset asset);
        void DrawUI();
    }

    public class TimelineGroupLogWindow : EditorWindow, ITimelineLogTool
    {
        [SerializeField] private TimelineAsset _targetTimeline;
        [SerializeField] private GroupLogState _state;

        [MenuItem("Tools/Tween Playables/Timeline Group Logger Window")]
        public static void ShowWindow()
        {
            GetWindow<TimelineGroupLogWindow>("Group Logger");
        }

        private void OnGUI()
        {
            ((ITimelineLogTool)this).DrawUI(); // Explicit call
        }

        bool ITimelineLogTool.TryAnalyze(TimelineAsset asset)
        {
            return _state.TryGeneratePuml(asset, out _); // Forward to extension
        }

        void ITimelineLogTool.DrawUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Target Timeline", EditorStyles.boldLabel);
            _targetTimeline = (TimelineAsset)EditorGUILayout.ObjectField(_targetTimeline, typeof(TimelineAsset), false);

            if (GUILayout.Button("Generate PlantUML"))
            {
                var success = ((ITimelineLogTool)this).TryAnalyze(_targetTimeline);
                Debug.Log(success ? $"<color=green>GENERATED:</color> {_state}" : "FAILED"); // Log
            }
            EditorGUILayout.EndVertical();

            if (_state.IsGenerated)
            {
                EditorGUILayout.TextArea(_state.LastOutput);
            }
        }
    }

    // LAYER E: ACTION BRIDGE
    [MenuEntry("Copy PlantUML to Clipboard", priority: 1000)]
    public class CopyPlantUMLAction : TrackAction
    {
        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            GroupLogLogic.ValidateSelection(tracks, out var isValid); // Logic call
            return isValid ? ActionValidity.Valid : ActionValidity.NotApplicable;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            var firstTrack = tracks.FirstOrDefault(); // Get context
            if (firstTrack == null) return false; // Guard
            
            var timeline = firstTrack.timelineAsset; // Extract asset
            var state = new GroupLogState(); // Local state

            if (state.TryGeneratePuml(timeline, out var result)) // Extension call
            {
                EditorGUIUtility.systemCopyBuffer = result; // Unity API interaction
                Debug.Log($"<color=cyan>[TweenPlayables]</color> PlantUML copied! ({state.GroupCount} groups)"); // Log
                return true;
            }

            return false; // Fail
        }
    }
}