using System;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;

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
            // Format: concise "Track Name" as T01
            line = $"concise \"{groupName}\" as T{index:D2}"; // Atomic format with padding
        }
    }

    // LAYER C: EXTENSIONS
    public static class GroupLogExtensions
    {
        public static bool TryGeneratePuml(ref this GroupLogState state, TimelineAsset timeline, out string result)
        {
            result = string.Empty; // Default

            if (timeline == null) return false; // Guard: No asset

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

            return groupCount > 0; // Success if groups found
        }
    }

    // LAYER D: INTERFACE & BRIDGE
    public interface ITimelineLogTool
    {
        bool TryAnalyze(TimelineAsset asset);
        void DrawUI();
    }

    public class TimelineGroupLogWindow : EditorWindow, ITimelineLogTool
    {
        [SerializeField] private TimelineAsset _targetTimeline;
        [SerializeField] private GroupLogState _state;
        private Vector2 _scrollPos;

        [MenuItem("Tools/Tween Playables/Timeline Group Logger")]
        public static void ShowWindow()
        {
            GetWindow<TimelineGroupLogWindow>("Group Logger");
        }

        private void OnGUI()
        {
            ((ITimelineLogTool)this).DrawUI();
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

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Generate PlantUML"))
            {
                var success = ((ITimelineLogTool)this).TryAnalyze(_targetTimeline);
                Debug.Log(success ? $"<color=green>GENERATED:</color> {_state}" : "FAILED: No groups or asset missing");
            }

            EditorGUILayout.EndVertical();

            if (_state.IsGenerated)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Output:", EditorStyles.boldLabel);
                
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(200));
                EditorGUILayout.TextArea(_state.LastOutput);
                EditorGUILayout.EndScrollView();
            }
        }
    }
}